using C3.Catalogue.Native;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace C3.Infrastructure.Migrations.V1_1ToV2_0
{
    public enum MigrationIssueSeverity
    {
        Information = 0,
        Warning = 1,
        Blocking = 2
    }

    public sealed class MigrationIssue
    {
        public MigrationIssue(
            MigrationIssueSeverity severity,
            string code,
            string path,
            string message)
        {
            Severity = severity;
            Code = code ?? string.Empty;
            Path = path ?? string.Empty;
            Message = message ?? string.Empty;
        }

        public MigrationIssueSeverity Severity { get; }
        public string Code { get; }
        public string Path { get; }
        public string Message { get; }
    }

    public sealed class MigrationMapping
    {
        public MigrationMapping(string entityKind, string legacyKey, string nativeId)
        {
            EntityKind = entityKind ?? string.Empty;
            LegacyKey = legacyKey ?? string.Empty;
            NativeId = nativeId ?? string.Empty;
        }

        public string EntityKind { get; }
        public string LegacyKey { get; }
        public string NativeId { get; }
    }

    public sealed class MigrationNormalization
    {
        public MigrationNormalization(string code, string path, string input, string output)
        {
            Code = code ?? string.Empty;
            Path = path ?? string.Empty;
            Input = input ?? string.Empty;
            Output = output ?? string.Empty;
        }

        public string Code { get; }
        public string Path { get; }
        public string Input { get; }
        public string Output { get; }
    }

    public sealed class MigrationEntityCounts
    {
        public int Brands { get; internal set; }
        public int CassetteModels { get; internal set; }
        public int DeckModels { get; internal set; }
        public int DeckUnits { get; internal set; }
        public int Tapes { get; internal set; }
        public int Recordings { get; internal set; }
    }

    public sealed class MigrationReport
    {
        private readonly List<MigrationIssue> issues = new List<MigrationIssue>();
        private readonly List<MigrationMapping> mappings = new List<MigrationMapping>();
        private readonly List<MigrationNormalization> normalizations =
            new List<MigrationNormalization>();

        internal MigrationReport(string sourcePath, string sourceRevision)
        {
            SourcePath = sourcePath ?? string.Empty;
            SourceRevision = sourceRevision ?? string.Empty;
            Profile = "v1.1-to-v2.0/1";
            Counts = new MigrationEntityCounts();
        }

        public string Profile { get; }
        public string SourcePath { get; }
        public string SourceRevision { get; }
        public string DestinationPath { get; internal set; }
        public string DestinationRevision { get; internal set; }
        public MigrationEntityCounts Counts { get; }
        public ReadOnlyCollection<MigrationIssue> Issues => issues.AsReadOnly();
        public ReadOnlyCollection<MigrationMapping> Mappings => mappings.AsReadOnly();
        public ReadOnlyCollection<MigrationNormalization> Normalizations =>
            normalizations.AsReadOnly();

        public bool HasBlockingIssues
        {
            get
            {
                return issues.Exists(issue => issue.Severity == MigrationIssueSeverity.Blocking);
            }
        }

        internal void AddIssue(MigrationIssue issue)
        {
            issues.Add(issue);
        }

        internal void AddMapping(MigrationMapping mapping)
        {
            mappings.Add(mapping);
        }

        internal void AddNormalization(MigrationNormalization normalization)
        {
            normalizations.Add(normalization);
        }
    }

    public sealed class LegacyToNativeMigrationResult
    {
        internal LegacyToNativeMigrationResult(NativeCatalogue document, MigrationReport report)
        {
            Document = document;
            Report = report;
        }

        public bool IsSuccess => Document != null && !Report.HasBlockingIssues;
        public NativeCatalogue Document { get; }
        public MigrationReport Report { get; }
    }

    public enum MigrationConversionStatus
    {
        Completed = 0,
        Blocked = 1,
        Interrupted = 2,
        Failed = 3
    }

    public enum MigrationCheckpoint
    {
        Planned = 0,
        NativeWritten = 1,
        ReportsWritten = 2
    }

    public interface IMigrationProgress
    {
        bool ShouldContinue(MigrationCheckpoint checkpoint);
    }

    public sealed class MigrationConversionResult
    {
        internal MigrationConversionResult(
            MigrationConversionStatus status,
            MigrationReport report,
            string recoveryPath,
            string message)
        {
            Status = status;
            Report = report;
            RecoveryPath = recoveryPath ?? string.Empty;
            Message = message ?? string.Empty;
        }

        public bool IsSuccess => Status == MigrationConversionStatus.Completed;
        public MigrationConversionStatus Status { get; }
        public MigrationReport Report { get; }
        public string RecoveryPath { get; }
        public string Message { get; }
    }
}
