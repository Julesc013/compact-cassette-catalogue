using C3.Infrastructure.CatalogueFiles.Xml.V2_0;
using C3.Infrastructure.Migrations.V1_1ToV2_0;
using C3.Infrastructure.Migrations.V2_0ToV1_1;
using System;
using System.IO;

namespace C3.Cli
{
    internal static class Program
    {
        private const int Success = 0;
        private const int Rejected = 2;
        private const int Failure = 3;
        private const int UsageError = 64;

        private static int Main(string[] arguments)
        {
            try
            {
                if (arguments.Length == 0) return Usage();
                switch (arguments[0].ToLowerInvariant())
                {
                    case "validate": return arguments.Length == 2 ? Validate(arguments[1]) : Usage();
                    case "migrate": return Migrate(arguments);
                    case "recover": return arguments.Length == 2 ? Recover(arguments[1]) : Usage();
                    case "export-legacy": return arguments.Length == 3 ? ExportLegacy(arguments[1], arguments[2]) : Usage();
                    default: return Usage();
                }
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine("C3_FAILURE|" + exception.Message);
                return Failure;
            }
        }

        private static int Validate(string path)
        {
            var fullPath = Path.GetFullPath(path);
            var native = new NativeXmlCatalogueStore().Load(fullPath);
            if (native.IsSuccess)
            {
                Console.WriteLine("VALID|native-v2|" + native.Revision.Token);
                return Success;
            }
            var legacy = new LegacyToNativeMigrator().DryRun(fullPath);
            if (legacy.IsSuccess)
            {
                Console.WriteLine("VALID|legacy-v1.1|" + legacy.Report.SourceRevision);
                return Success;
            }
            Console.Error.WriteLine("INVALID|native=" + native.Failure + "|legacy-blocking=" + legacy.Report.Issues.Count);
            return Rejected;
        }

        private static int Migrate(string[] arguments)
        {
            if (arguments.Length == 3 && arguments[1] == "--dry-run")
            {
                var result = new LegacyToNativeMigrator().DryRun(Path.GetFullPath(arguments[2]));
                Console.WriteLine("DRY_RUN|" + (result.IsSuccess ? "pass" : "blocked") + "|issues=" + result.Report.Issues.Count);
                return result.IsSuccess ? Success : Rejected;
            }
            if (arguments.Length != 3) return Usage();
            var converted = new LegacyToNativeConversionService().ConvertCopy(arguments[1], arguments[2]);
            Console.WriteLine("MIGRATE|" + converted.Status.ToString().ToLowerInvariant() + "|" + converted.Message);
            return converted.IsSuccess ? Success : Rejected;
        }

        private static int Recover(string path)
        {
            var result = new LegacyToNativeConversionService().Recover(path);
            Console.WriteLine("RECOVER|" + result.Status.ToString().ToLowerInvariant() + "|" + result.Message);
            return result.IsSuccess ? Success : Rejected;
        }

        private static int ExportLegacy(string nativePath, string destinationPath)
        {
            var loaded = new NativeXmlCatalogueStore().Load(Path.GetFullPath(nativePath));
            if (!loaded.IsSuccess)
            {
                Console.Error.WriteLine("EXPORT_LEGACY|blocked|" + loaded.Message);
                return Rejected;
            }
            var exported = new NativeToLegacyExporter().ExportCopy(loaded.Document, destinationPath);
            Console.WriteLine("EXPORT_LEGACY|" + (exported.IsSuccess ? "completed" : "blocked") + "|issues=" + exported.Report.Issues.Count);
            return exported.IsSuccess ? Success : Rejected;
        }

        private static int Usage()
        {
            Console.Error.WriteLine("Usage:");
            Console.Error.WriteLine("  c3 validate <catalogue>");
            Console.Error.WriteLine("  c3 migrate --dry-run <legacy-catalogue>");
            Console.Error.WriteLine("  c3 migrate <legacy-catalogue> <native-copy>");
            Console.Error.WriteLine("  c3 recover <migration-recovery-journal>");
            Console.Error.WriteLine("  c3 export-legacy <native-catalogue> <legacy-copy>");
            return UsageError;
        }
    }
}
