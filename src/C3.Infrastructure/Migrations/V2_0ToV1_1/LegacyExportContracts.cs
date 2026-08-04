using C3.Catalogue.Catalogues;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace C3.Infrastructure.Migrations.V2_0ToV1_1
{
    public enum LegacyExportIssueSeverity
    {
        Information = 0,
        Warning = 1,
        Blocking = 2
    }

    public sealed class LegacyExportIssue
    {
        public LegacyExportIssue(LegacyExportIssueSeverity severity, string code, string path, string message)
        {
            Severity = severity;
            Code = code ?? string.Empty;
            Path = path ?? string.Empty;
            Message = message ?? string.Empty;
        }

        public LegacyExportIssueSeverity Severity { get; }
        public string Code { get; }
        public string Path { get; }
        public string Message { get; }
    }

    public sealed class LegacyExportReport
    {
        private readonly List<LegacyExportIssue> issues = new List<LegacyExportIssue>();

        public string Profile => "v2.0-to-v1.1/1";
        public string DestinationPath { get; internal set; }
        public string DestinationRevision { get; internal set; }
        public ReadOnlyCollection<LegacyExportIssue> Issues => issues.AsReadOnly();
        public bool HasBlockingIssues => issues.Exists(item => item.Severity == LegacyExportIssueSeverity.Blocking);

        internal void Add(LegacyExportIssueSeverity severity, string code, string path, string message)
        {
            issues.Add(new LegacyExportIssue(severity, code, path, message));
        }
    }

    public sealed class LegacyExportPreview
    {
        internal LegacyExportPreview(DataSet document, LegacyExportReport report)
        {
            Document = document;
            Report = report;
        }

        public bool IsExportable => Document != null && !Report.HasBlockingIssues;
        public LegacyExportReport Report { get; }
        internal DataSet Document { get; }
    }

    public sealed class LegacyExportResult
    {
        internal LegacyExportResult(bool success, LegacyExportReport report, CatalogueRevision revision, string reportPath, string message)
        {
            IsSuccess = success;
            Report = report;
            Revision = revision;
            ReportPath = reportPath ?? string.Empty;
            Message = message ?? string.Empty;
        }

        public bool IsSuccess { get; }
        public LegacyExportReport Report { get; }
        public CatalogueRevision Revision { get; }
        public string ReportPath { get; }
        public string Message { get; }
    }
}
