using C3.Infrastructure.CatalogueFiles.Xml.V2_0;
using System;
using System.IO;

namespace C3.Infrastructure.Migrations.V1_1ToV2_0
{
    public sealed class LegacyToNativeConversionService
    {
        private readonly LegacyToNativeMigrator migrator;
        private readonly NativeXmlCatalogueStore nativeStore;
        private readonly MigrationReportWriter reportWriter;

        public LegacyToNativeConversionService()
            : this(new LegacyToNativeMigrator(), new NativeXmlCatalogueStore(), new MigrationReportWriter())
        {
        }

        public LegacyToNativeConversionService(
            LegacyToNativeMigrator migrator,
            NativeXmlCatalogueStore nativeStore,
            MigrationReportWriter reportWriter)
        {
            this.migrator = migrator ?? throw new ArgumentNullException(nameof(migrator));
            this.nativeStore = nativeStore ?? throw new ArgumentNullException(nameof(nativeStore));
            this.reportWriter = reportWriter ?? throw new ArgumentNullException(nameof(reportWriter));
        }

        public MigrationConversionResult ConvertCopy(string sourcePath, string destinationPath)
        {
            return ConvertCopy(sourcePath, destinationPath, null);
        }

        public MigrationConversionResult ConvertCopy(
            string sourcePath,
            string destinationPath,
            IMigrationProgress progress)
        {
            string source;
            string destination;
            try
            {
                source = Path.GetFullPath(sourcePath ?? string.Empty);
                destination = Path.GetFullPath(destinationPath ?? string.Empty);
            }
            catch (Exception exception)
            {
                return Result(MigrationConversionStatus.Blocked, null, string.Empty, exception.Message);
            }
            if (string.Equals(source, destination, StringComparison.OrdinalIgnoreCase))
                return Result(MigrationConversionStatus.Blocked, null, string.Empty, "Migration requires a distinct copy destination.");

            var jsonPath = destination + ".migration.json";
            var textPath = destination + ".migration.txt";
            var recoveryPath = destination + ".migration.recovery.xml";
            if (Exists(destination, jsonPath, textPath, recoveryPath))
                return Result(MigrationConversionStatus.Blocked, null, recoveryPath, "Migration refuses to replace a destination, report, or recovery journal.");

            var dryRun = migrator.DryRun(source);
            if (!dryRun.IsSuccess)
                return Result(MigrationConversionStatus.Blocked, dryRun.Report, recoveryPath, "The legacy source cannot be migrated safely.");
            dryRun.Report.DestinationPath = destination;
            var journal = new MigrationRecoveryJournal
            {
                State = "planned",
                SourcePath = source,
                SourceRevision = dryRun.Report.SourceRevision,
                DestinationPath = destination,
                JsonReportPath = jsonPath,
                TextReportPath = textPath
            };
            try
            {
                journal.Create(recoveryPath);
                if (!Continue(progress, MigrationCheckpoint.Planned))
                {
                    File.Delete(recoveryPath);
                    return Result(MigrationConversionStatus.Interrupted, dryRun.Report, string.Empty, "Migration stopped before writing output.");
                }

                var saved = nativeStore.SaveNew(destination, dryRun.Document);
                if (!saved.IsSuccess)
                    return Result(MigrationConversionStatus.Failed, dryRun.Report, recoveryPath, saved.Message);
                dryRun.Report.DestinationRevision = saved.Revision.Token;
                journal.DestinationRevision = saved.Revision.Token;
                journal.State = "native-written";
                journal.Update(recoveryPath);
                if (!Continue(progress, MigrationCheckpoint.NativeWritten))
                    return Result(MigrationConversionStatus.Interrupted, dryRun.Report, recoveryPath, "Migration stopped after the native copy was verified.");

                WriteReportsNew(dryRun.Report, jsonPath, textPath);
                journal.State = "reports-written";
                journal.Update(recoveryPath);
                if (!Continue(progress, MigrationCheckpoint.ReportsWritten))
                    return Result(MigrationConversionStatus.Interrupted, dryRun.Report, recoveryPath, "Migration stopped after reports were verified.");
                File.Delete(recoveryPath);
                return Result(MigrationConversionStatus.Completed, dryRun.Report, string.Empty, "Migration copy and reports were verified.");
            }
            catch (Exception exception)
            {
                return Result(MigrationConversionStatus.Failed, dryRun.Report, recoveryPath, exception.Message);
            }
        }

        public MigrationConversionResult Recover(string recoveryPath)
        {
            return Recover(recoveryPath, null);
        }

        public MigrationConversionResult Recover(string recoveryPath, IMigrationProgress progress)
        {
            try
            {
                var fullRecoveryPath = Path.GetFullPath(recoveryPath ?? string.Empty);
                var journal = MigrationRecoveryJournal.Read(fullRecoveryPath);
                if (journal.State != "native-written" && journal.State != "reports-written")
                    return Result(MigrationConversionStatus.Blocked, null, fullRecoveryPath, "The recovery journal is not at a resumable checkpoint.");
                var dryRun = migrator.DryRun(journal.SourcePath);
                if (!dryRun.IsSuccess || dryRun.Report.SourceRevision != journal.SourceRevision)
                    return Result(MigrationConversionStatus.Blocked, dryRun.Report, fullRecoveryPath, "The legacy source no longer matches the recovery journal.");
                var loaded = nativeStore.Load(journal.DestinationPath);
                if (!loaded.IsSuccess || loaded.Revision.Token != journal.DestinationRevision)
                    return Result(MigrationConversionStatus.Blocked, dryRun.Report, fullRecoveryPath, "The native copy no longer matches the recovery journal.");
                dryRun.Report.DestinationPath = journal.DestinationPath;
                dryRun.Report.DestinationRevision = journal.DestinationRevision;

                if (journal.State == "native-written")
                {
                    if (File.Exists(journal.JsonReportPath) || File.Exists(journal.TextReportPath))
                        return Result(MigrationConversionStatus.Blocked, dryRun.Report, fullRecoveryPath, "Unexpected report output prevents safe recovery.");
                    WriteReportsNew(dryRun.Report, journal.JsonReportPath, journal.TextReportPath);
                    journal.State = "reports-written";
                    journal.Update(fullRecoveryPath);
                }
                else
                {
                    VerifyReport(reportWriter.WriteJson(dryRun.Report, "completed"), journal.JsonReportPath);
                    VerifyReport(reportWriter.WriteText(dryRun.Report, "completed"), journal.TextReportPath);
                }
                if (!Continue(progress, MigrationCheckpoint.ReportsWritten))
                    return Result(MigrationConversionStatus.Interrupted, dryRun.Report, fullRecoveryPath, "Recovery stopped after reports were verified.");
                File.Delete(fullRecoveryPath);
                return Result(MigrationConversionStatus.Completed, dryRun.Report, string.Empty, "Migration recovery completed.");
            }
            catch (Exception exception)
            {
                return Result(MigrationConversionStatus.Failed, null, recoveryPath, exception.Message);
            }
        }

        private void WriteReportsNew(MigrationReport report, string jsonPath, string textPath)
        {
            var json = reportWriter.WriteJson(report, "completed");
            var text = reportWriter.WriteText(report, "completed");
            WriteNew(jsonPath, json);
            try
            {
                WriteNew(textPath, text);
            }
            catch
            {
                File.Delete(jsonPath);
                throw;
            }
            VerifyReport(json, jsonPath);
            VerifyReport(text, textPath);
        }

        private static void WriteNew(string path, byte[] payload)
        {
            using (var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                stream.Write(payload, 0, payload.Length);
                stream.Flush(true);
            }
        }

        private static void VerifyReport(byte[] expected, string path)
        {
            var actual = File.ReadAllBytes(path);
            if (expected.Length != actual.Length) throw new InvalidDataException("A migration report failed byte verification.");
            for (var index = 0; index < expected.Length; index++)
                if (expected[index] != actual[index]) throw new InvalidDataException("A migration report failed byte verification.");
        }

        private static bool Continue(IMigrationProgress progress, MigrationCheckpoint checkpoint)
        {
            return progress == null || progress.ShouldContinue(checkpoint);
        }

        private static bool Exists(params string[] paths)
        {
            foreach (var path in paths) if (File.Exists(path) || Directory.Exists(path)) return true;
            return false;
        }

        private static MigrationConversionResult Result(
            MigrationConversionStatus status,
            MigrationReport report,
            string recoveryPath,
            string message)
        {
            return new MigrationConversionResult(status, report, recoveryPath, message);
        }
    }
}
