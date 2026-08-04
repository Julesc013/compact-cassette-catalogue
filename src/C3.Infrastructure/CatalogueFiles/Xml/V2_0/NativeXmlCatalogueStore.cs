using C3.Catalogue.Catalogues;
using C3.Catalogue.Native;
using C3.Infrastructure.FileOperations;
using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace C3.Infrastructure.CatalogueFiles.Xml.V2_0
{
    public sealed class NativeXmlCatalogueStore
    {
        private readonly NativeXmlCatalogueReader reader;
        private readonly NativeXmlCatalogueWriter writer;

        public NativeXmlCatalogueStore()
            : this(new NativeXmlCatalogueReader(), new NativeXmlCatalogueWriter())
        {
        }

        public NativeXmlCatalogueStore(
            NativeXmlCatalogueReader reader,
            NativeXmlCatalogueWriter writer)
        {
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
            this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        public NativeCatalogueLoadResult Load(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return NativeCatalogueLoadResult.Failed(
                    NativeCatalogueFileFailure.FileNotFound,
                    "The selected native catalogue file does not exist.");
            }

            try
            {
                var info = new FileInfo(path);
                if (info.Length > NativeXmlCatalogueReader.MaximumCatalogueBytes)
                {
                    return NativeCatalogueLoadResult.Failed(
                        NativeCatalogueFileFailure.FileTooLarge,
                        "The catalogue exceeds the 64 MiB safety limit.");
                }
                var bytes = File.ReadAllBytes(path);
                var document = reader.Read(bytes);
                return NativeCatalogueLoadResult.Success(document, Revision(bytes));
            }
            catch (NativeXmlCatalogueException exception)
            {
                return NativeCatalogueLoadResult.Failed(exception.Failure, exception.Message);
            }
            catch (UnauthorizedAccessException exception)
            {
                return NativeCatalogueLoadResult.Failed(
                    NativeCatalogueFileFailure.AccessDenied,
                    exception.Message);
            }
            catch (IOException exception)
            {
                return NativeCatalogueLoadResult.Failed(
                    NativeCatalogueFileFailure.IoFailure,
                    exception.Message);
            }
            catch (Exception exception)
            {
                return NativeCatalogueLoadResult.Failed(
                    NativeCatalogueFileFailure.InvalidStructure,
                    exception.Message);
            }
        }

        public NativeCatalogueSaveResult Save(
            string path,
            NativeCatalogue document,
            CatalogueRevision expectedRevision)
        {
            return SaveCore(path, document, expectedRevision, false);
        }

        public NativeCatalogueSaveResult SaveNew(
            string path,
            NativeCatalogue document)
        {
            return SaveCore(path, document, null, true);
        }

        private NativeCatalogueSaveResult SaveCore(
            string path,
            NativeCatalogue document,
            CatalogueRevision expectedRevision,
            bool requireAbsent)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return NativeCatalogueSaveResult.Failed(
                    NativeCatalogueFileFailure.IoFailure,
                    "A destination path is required.");
            }
            if (document == null)
            {
                return NativeCatalogueSaveResult.Failed(
                    NativeCatalogueFileFailure.InvalidStructure,
                    "There is no native catalogue document to save.");
            }

            var fullPath = Path.GetFullPath(path);
            var directory = Path.GetDirectoryName(fullPath);
            if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
            {
                return NativeCatalogueSaveResult.Failed(
                    NativeCatalogueFileFailure.IoFailure,
                    "The destination directory does not exist.");
            }

            var backupPath = fullPath + ".bak";
            try
            {
                if (requireAbsent && File.Exists(fullPath))
                {
                    return NativeCatalogueSaveResult.Failed(
                        NativeCatalogueFileFailure.ExternalModification,
                        "Convert-copy refuses to overwrite an existing destination.");
                }
                if (expectedRevision != null)
                {
                    if (!File.Exists(fullPath) ||
                        !expectedRevision.Equals(Revision(File.ReadAllBytes(fullPath))))
                    {
                        return NativeCatalogueSaveResult.Failed(
                            NativeCatalogueFileFailure.ExternalModification,
                            "The native catalogue changed on disk after it was opened.");
                    }
                }

                using (var temporary = OwnedSiblingTemporaryFile.Create(fullPath))
                {
                    writer.Write(temporary.Stream, document);
                    temporary.Stream.Flush(true);
                    temporary.Stream.Dispose();

                    var verification = Load(temporary.Path);
                    if (!verification.IsSuccess)
                    {
                        return NativeCatalogueSaveResult.Failed(
                            NativeCatalogueFileFailure.VerificationFailure,
                            "The temporary native output did not reopen: " + verification.Message);
                    }

                    var intendedBytes = writer.Write(verification.Document);
                    var actualBytes = File.ReadAllBytes(temporary.Path);
                    if (!BytesEqual(intendedBytes, actualBytes))
                    {
                        return NativeCatalogueSaveResult.Failed(
                            NativeCatalogueFileFailure.VerificationFailure,
                            "The temporary native output did not round-trip to canonical bytes.");
                    }

                    if (requireAbsent)
                    {
                        if (File.Exists(fullPath))
                        {
                            return NativeCatalogueSaveResult.Failed(
                                NativeCatalogueFileFailure.ExternalModification,
                                "The new destination appeared while the verified output was being prepared.");
                        }
                        File.Move(temporary.Path, fullPath);
                        backupPath = null;
                    }
                    else if (File.Exists(fullPath))
                    {
                        File.Replace(temporary.Path, fullPath, backupPath, true);
                    }
                    else
                    {
                        File.Move(temporary.Path, fullPath);
                        backupPath = null;
                    }

                    return NativeCatalogueSaveResult.Success(
                        Revision(File.ReadAllBytes(fullPath)),
                        backupPath);
                }
            }
            catch (UnauthorizedAccessException exception)
            {
                return NativeCatalogueSaveResult.Failed(
                    NativeCatalogueFileFailure.AccessDenied,
                    exception.Message);
            }
            catch (IOException exception)
            {
                if (requireAbsent && File.Exists(fullPath))
                {
                    return NativeCatalogueSaveResult.Failed(
                        NativeCatalogueFileFailure.ExternalModification,
                        "The new destination appeared while the verified output was being committed.");
                }
                return NativeCatalogueSaveResult.Failed(
                    NativeCatalogueFileFailure.IoFailure,
                    exception.Message);
            }
            catch (NativeXmlCatalogueException exception)
            {
                return NativeCatalogueSaveResult.Failed(exception.Failure, exception.Message);
            }
            catch (Exception exception)
            {
                return NativeCatalogueSaveResult.Failed(
                    NativeCatalogueFileFailure.VerificationFailure,
                    exception.Message);
            }
        }

        private static CatalogueRevision Revision(byte[] bytes)
        {
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(bytes);
                var text = new StringBuilder(hash.Length * 2);
                foreach (var value in hash)
                {
                    text.Append(value.ToString("x2", CultureInfo.InvariantCulture));
                }
                return new CatalogueRevision(text.ToString());
            }
        }

        private static bool BytesEqual(byte[] left, byte[] right)
        {
            if (left.Length != right.Length)
            {
                return false;
            }
            for (var index = 0; index < left.Length; index++)
            {
                if (left[index] != right[index])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
