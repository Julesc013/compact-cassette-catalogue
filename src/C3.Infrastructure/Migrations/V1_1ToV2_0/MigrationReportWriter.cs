using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace C3.Infrastructure.Migrations.V1_1ToV2_0
{
    public sealed class MigrationReportWriter
    {
        private static readonly Encoding Utf8WithoutBom = new UTF8Encoding(false, true);

        public byte[] WriteJson(MigrationReport report, string status)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));
            var text = new StringBuilder();
            text.Append("{\n  \"schemaVersion\": 1,\n");
            Property(text, "profile", report.Profile, true);
            Property(text, "status", status ?? string.Empty, true);
            Property(text, "sourcePath", report.SourcePath, true);
            Property(text, "sourceRevision", report.SourceRevision, true);
            Property(text, "destinationPath", report.DestinationPath, true);
            Property(text, "destinationRevision", report.DestinationRevision, true);
            text.Append("  \"counts\": {");
            text.Append("\"brands\": ").Append(Invariant(report.Counts.Brands));
            text.Append(", \"cassetteModels\": ").Append(Invariant(report.Counts.CassetteModels));
            text.Append(", \"deckModels\": ").Append(Invariant(report.Counts.DeckModels));
            text.Append(", \"deckUnits\": ").Append(Invariant(report.Counts.DeckUnits));
            text.Append(", \"tapes\": ").Append(Invariant(report.Counts.Tapes));
            text.Append(", \"recordings\": ").Append(Invariant(report.Counts.Recordings));
            text.Append("},\n");
            text.Append("  \"mappings\": [");
            for (var index = 0; index < report.Mappings.Count; index++)
            {
                if (index != 0) text.Append(',');
                var item = report.Mappings[index];
                text.Append("\n    {\"entityKind\": \"").Append(Escape(item.EntityKind));
                text.Append("\", \"legacyKey\": \"").Append(Escape(item.LegacyKey));
                text.Append("\", \"nativeId\": \"").Append(Escape(item.NativeId)).Append("\"}");
            }
            CloseArray(text, report.Mappings.Count, true);
            text.Append("  \"normalizations\": [");
            for (var index = 0; index < report.Normalizations.Count; index++)
            {
                if (index != 0) text.Append(',');
                var item = report.Normalizations[index];
                text.Append("\n    {\"code\": \"").Append(Escape(item.Code));
                text.Append("\", \"path\": \"").Append(Escape(item.Path));
                text.Append("\", \"input\": \"").Append(Escape(item.Input));
                text.Append("\", \"output\": \"").Append(Escape(item.Output)).Append("\"}");
            }
            CloseArray(text, report.Normalizations.Count, true);
            text.Append("  \"issues\": [");
            for (var index = 0; index < report.Issues.Count; index++)
            {
                if (index != 0) text.Append(',');
                var item = report.Issues[index];
                text.Append("\n    {\"severity\": \"").Append(Escape(item.Severity.ToString().ToLowerInvariant()));
                text.Append("\", \"code\": \"").Append(Escape(item.Code));
                text.Append("\", \"path\": \"").Append(Escape(item.Path));
                text.Append("\", \"message\": \"").Append(Escape(item.Message)).Append("\"}");
            }
            CloseArray(text, report.Issues.Count, false);
            text.Append("}\n");
            return Utf8WithoutBom.GetBytes(text.ToString());
        }

        public byte[] WriteText(MigrationReport report, string status)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));
            var text = new StringBuilder();
            text.Append("C3 catalogue migration report\n");
            text.Append("Status: ").Append(status ?? string.Empty).Append('\n');
            text.Append("Profile: ").Append(report.Profile).Append('\n');
            text.Append("Source: ").Append(report.SourcePath).Append('\n');
            text.Append("Source SHA-256: ").Append(report.SourceRevision).Append('\n');
            text.Append("Destination: ").Append(report.DestinationPath).Append('\n');
            text.Append("Destination SHA-256: ").Append(report.DestinationRevision).Append("\n\n");
            text.Append("Entities: ")
                .Append(Invariant(report.Counts.Brands)).Append(" brands; ")
                .Append(Invariant(report.Counts.CassetteModels)).Append(" cassette models; ")
                .Append(Invariant(report.Counts.DeckModels)).Append(" deck models; ")
                .Append(Invariant(report.Counts.DeckUnits)).Append(" deck units; ")
                .Append(Invariant(report.Counts.Tapes)).Append(" tapes; ")
                .Append(Invariant(report.Counts.Recordings)).Append(" recordings.\n");
            text.Append("Mappings: ").Append(Invariant(report.Mappings.Count)).Append('\n');
            text.Append("Normalizations: ").Append(Invariant(report.Normalizations.Count)).Append('\n');
            text.Append("Issues: ").Append(Invariant(report.Issues.Count)).Append('\n');
            foreach (var issue in report.Issues)
            {
                text.Append("- [").Append(issue.Severity.ToString().ToUpperInvariant()).Append("] ")
                    .Append(issue.Code).Append(" at ").Append(issue.Path).Append(": ")
                    .Append(issue.Message).Append('\n');
            }
            return Utf8WithoutBom.GetBytes(text.ToString());
        }

        private static void Property(StringBuilder text, string name, string value, bool comma)
        {
            text.Append("  \"").Append(name).Append("\": \"").Append(Escape(value ?? string.Empty));
            text.Append(comma ? "\",\n" : "\"\n");
        }

        private static void CloseArray(StringBuilder text, int count, bool comma)
        {
            if (count != 0) text.Append('\n').Append("  ");
            text.Append(comma ? "],\n" : "]\n");
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
                    case '\b': result.Append("\\b"); break;
                    case '\f': result.Append("\\f"); break;
                    case '\n': result.Append("\\n"); break;
                    case '\r': result.Append("\\r"); break;
                    case '\t': result.Append("\\t"); break;
                    default:
                        if (character < 32)
                        {
                            result.Append("\\u").Append(((int)character).ToString("x4", CultureInfo.InvariantCulture));
                        }
                        else result.Append(character);
                        break;
                }
            }
            return result.ToString();
        }

        private static string Invariant(int value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
