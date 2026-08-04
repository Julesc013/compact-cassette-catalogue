using System;

namespace C3.Domain.Catalogues
{
    /// <summary>
    /// Owns the lifecycle state of one active catalogue document independently
    /// from presentation and persistence mechanisms.
    /// </summary>
    public sealed class CatalogueSession
    {
        private string filePath;
        private string displayName;
        private CatalogueRevision revision;
        private bool isDirty;
        private long changeSequence;

        public CatalogueSession(string newCatalogueDisplayName)
        {
            if (string.IsNullOrWhiteSpace(newCatalogueDisplayName))
            {
                throw new ArgumentException(
                    "A display name is required.",
                    nameof(newCatalogueDisplayName));
            }

            displayName = newCatalogueDisplayName;
        }

        public event EventHandler SessionChanged;

        public string FilePath => filePath;

        public string DisplayName => displayName;

        public CatalogueRevision Revision => revision;

        public bool IsDirty => isDirty;

        public long ChangeSequence => changeSequence;

        public void BeginNew(string newDisplayName)
        {
            filePath = null;
            displayName = RequireDisplayName(newDisplayName);
            revision = null;
            isDirty = false;
            RaiseChanged();
        }

        public void SetDocumentLocation(string path, string newDisplayName)
        {
            filePath = path;
            displayName = RequireDisplayName(newDisplayName);
            RaiseChanged();
        }

        public void MarkChanged()
        {
            changeSequence += 1;
            isDirty = true;
            RaiseChanged();
        }

        public void SetDirtyForMigration(bool dirty)
        {
            if (dirty)
            {
                MarkChanged();
                return;
            }

            if (isDirty)
            {
                isDirty = false;
                RaiseChanged();
            }
        }

        public void MarkLoaded(
            string path,
            string newDisplayName,
            CatalogueRevision newRevision)
        {
            filePath = path;
            displayName = RequireDisplayName(newDisplayName);
            revision = newRevision;
            isDirty = false;
            RaiseChanged();
        }

        public void MarkSaved(
            string path,
            string newDisplayName,
            CatalogueRevision newRevision)
        {
            filePath = path;
            displayName = RequireDisplayName(newDisplayName);
            revision = newRevision;
            isDirty = false;
            RaiseChanged();
        }

        private static string RequireDisplayName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("A display name is required.", nameof(value));
            }

            return value;
        }

        private void RaiseChanged()
        {
            var handler = SessionChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
