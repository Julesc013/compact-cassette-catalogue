using System;
using System.Globalization;
using System.Text;

namespace C3.Infrastructure.Migrations.V2_0ToV1_1
{
    public sealed class LegacyExportReportWriter
    {
        private static readonly Encoding Utf8WithoutBom = new UTF8Encoding(false, true);

        public byte[] WriteJson(LegacyExportReport report)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));
            var text = new StringBuilder();
            text.Append("{\n  \"schemaVersion\": 1,\n");
            text.Append("  \"profile\": \"").Append(Escape(report.Profile)).Append("\",\n");
            text.Append("  \"status\": \"").Append(report.HasBlockingIssues ? "blocked" : "completed").Append("\",\n");
            text.Append("  \"destinationPath\": \"").Append(Escape(report.DestinationPath ?? string.Empty)).Append("\",\n");
            text.Append("  \"destinationRevision\": \"").Append(Escape(report.DestinationRevision ?? string.Empty)).Append("\",\n");
            text.Append("  \"issues\": [");
            for (var index = 0; index < report.Issues.Count; index++)
            {
                if (index != 0) text.Append(',');
                var issue = report.Issues[index];
                text.Append("\n    {\"severity\": \"").Append(issue.Severity.ToString().ToLowerInvariant());
                text.Append("\", \"code\": \"").Append(Escape(issue.Code));
                text.Append("\", \"path\": \"").Append(Escape(issue.Path));
                text.Append("\", \"message\": \"").Append(Escape(issue.Message)).Append("\"}");
            }
            if (report.Issues.Count != 0) text.Append('\n').Append("  ");
            text.Append("]\n}\n");
            return Utf8WithoutBom.GetBytes(text.ToString());
        }

        private static string Escape(string value)
        {
            var result = new StringBuilder(value.Length + 8);
            foreach (var character in value)
            {
                switch (character)
                {
                    case '\\': result.Append("\\\\"); break;
                    case '"': result.Append("\\\""); break;
                    case '\n': result.Append("\\n"); break;
                    case '\r': result.Append("\\r"); break;
                    case '\t': result.Append("\\t"); break;
                    default:
                        if (character < 32) result.Append("\\u").Append(((int)character).ToString("x4", CultureInfo.InvariantCulture));
                        else result.Append(character);
                        break;
                }
            }
            return result.ToString();
        }
    }
}
