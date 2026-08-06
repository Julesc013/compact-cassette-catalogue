using C3.Infrastructure.CatalogueFiles.Canonical;
using C3.Infrastructure.CatalogueFiles.Xml.V1_1;
using System;
using System.IO;

namespace C3.Infrastructure.Migrations.V1_1ToV2_0
{
    /// <summary>
    /// Loads a validated legacy catalogue, projects it through the one canonical
    /// legacy mapper, and adapts the accepted graph to the frozen native-v2
    /// persistence profile. It owns orchestration, not another field mapping.
    /// </summary>
    public sealed class LegacyToNativeMigrator
    {
        private readonly LegacyXmlCatalogueStore legacyStore;

        public LegacyToNativeMigrator()
            : this(new LegacyXmlCatalogueStore())
        {
        }

        public LegacyToNativeMigrator(LegacyXmlCatalogueStore legacyStore)
        {
            this.legacyStore = legacyStore ??
                throw new ArgumentNullException(nameof(legacyStore));
        }

        public LegacyToNativeMigrationResult DryRun(string sourcePath)
        {
            var fullPath = string.IsNullOrWhiteSpace(sourcePath)
                ? string.Empty
                : Path.GetFullPath(sourcePath);
            var initialReport = new MigrationReport(fullPath, string.Empty);
            if (string.IsNullOrWhiteSpace(fullPath))
            {
                Block(
                    initialReport,
                    "source.path-required",
                    "source",
                    "A legacy source path is required.");
                return new LegacyToNativeMigrationResult(
                    null, null, initialReport);
            }

            var schema = LegacyCatalogueSchema.Create(new LegacyCatalogueMetadata
            {
                FileVersion = "1.1.0",
                ProductVersion = "2.0.0",
                ProductStage = "Migration",
                ProductDate = new DateTime(2026, 8, 4),
                CreatedAt = new DateTime(2026, 8, 4)
            });
            var loaded = legacyStore.Load(fullPath, schema, new[] { "1.1.0" });
            if (!loaded.IsSuccess)
            {
                Block(
                    initialReport,
                    "source." + loaded.Failure.ToString().ToLowerInvariant(),
                    "source",
                    loaded.Message);
                return new LegacyToNativeMigrationResult(
                    null, null, initialReport);
            }

            var report = new MigrationReport(fullPath, loaded.Revision.Token);
            try
            {
                var state = new LegacyV1CanonicalMapper().Map(
                    loaded.Document,
                    LegacyTimestampLexemes.Load(fullPath),
                    report);
                var document = state == null
                    ? null
                    : new CanonicalToNativeV2Adapter().AdaptLegacyMigration(state);
                return new LegacyToNativeMigrationResult(
                    report.HasBlockingIssues ? null : document,
                    report.HasBlockingIssues ? null : state,
                    report);
            }
            catch (Exception exception)
            {
                Block(
                    report,
                    "migration.unexpected",
                    "source",
                    exception.Message);
                return new LegacyToNativeMigrationResult(null, null, report);
            }
        }

        private static void Block(
            MigrationReport report,
            string code,
            string path,
            string message)
        {
            report.AddIssue(new MigrationIssue(
                MigrationIssueSeverity.Blocking, code, path, message));
        }
    }
}
