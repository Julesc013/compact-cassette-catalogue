using C3.Catalogue.Catalogues;
using C3.Catalogue.Native;

namespace C3.Infrastructure.CatalogueFiles.Xml.V2_0
{
    public sealed class NativeCatalogueLoadResult
    {
        private NativeCatalogueLoadResult()
        {
        }

        public bool IsSuccess { get; private set; }
        public NativeCatalogue Document { get; private set; }
        public CatalogueRevision Revision { get; private set; }
        public NativeCatalogueFileFailure Failure { get; private set; }
        public string Message { get; private set; }

        public static NativeCatalogueLoadResult Success(
            NativeCatalogue document,
            CatalogueRevision revision)
        {
            return new NativeCatalogueLoadResult
            {
                IsSuccess = true,
                Document = document,
                Revision = revision,
                Failure = NativeCatalogueFileFailure.None,
                Message = string.Empty
            };
        }

        public static NativeCatalogueLoadResult Failed(
            NativeCatalogueFileFailure failure,
            string message)
        {
            return new NativeCatalogueLoadResult
            {
                IsSuccess = false,
                Failure = failure,
                Message = message ?? string.Empty
            };
        }
    }

    public sealed class NativeCatalogueSaveResult
    {
        private NativeCatalogueSaveResult()
        {
        }

        public bool IsSuccess { get; private set; }
        public CatalogueRevision Revision { get; private set; }
        public string BackupPath { get; private set; }
        public NativeCatalogueFileFailure Failure { get; private set; }
        public string Message { get; private set; }

        public static NativeCatalogueSaveResult Success(
            CatalogueRevision revision,
            string backupPath)
        {
            return new NativeCatalogueSaveResult
            {
                IsSuccess = true,
                Revision = revision,
                BackupPath = backupPath,
                Failure = NativeCatalogueFileFailure.None,
                Message = string.Empty
            };
        }

        public static NativeCatalogueSaveResult Failed(
            NativeCatalogueFileFailure failure,
            string message)
        {
            return new NativeCatalogueSaveResult
            {
                IsSuccess = false,
                Failure = failure,
                Message = message ?? string.Empty
            };
        }
    }
}
