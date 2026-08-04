using System;

namespace C3.Catalogue.Catalogues
{
    public sealed class CatalogueSession
    {
        private readonly C3.Domain.Catalogues.CatalogueSession value;
        private CatalogueRevision revision;
        private bool coreRaisedChanged;

        public CatalogueSession(string newCatalogueDisplayName)
        {
            value = new C3.Domain.Catalogues.CatalogueSession(newCatalogueDisplayName);
            value.SessionChanged += OnCoreSessionChanged;
        }

        public event EventHandler SessionChanged;

        public string FilePath => value.FilePath;

        public string DisplayName => value.DisplayName;

        public CatalogueRevision Revision => revision;

        public bool IsDirty => value.IsDirty;

        public long ChangeSequence => value.ChangeSequence;

        public void BeginNew(string displayName)
        {
            BeginCoreChange();
            value.BeginNew(displayName);
            revision = null;
            CompleteCoreChange();
        }

        public void SetDocumentLocation(string path, string displayName)
        {
            BeginCoreChange();
            value.SetDocumentLocation(path, displayName);
            CompleteCoreChange();
        }

        public void MarkChanged()
        {
            BeginCoreChange();
            value.MarkChanged();
            CompleteCoreChange();
        }

        public void SetDirtyForMigration(bool isDirty)
        {
            BeginCoreChange();
            value.SetDirtyForMigration(isDirty);
            CompleteCoreChange();
        }

        public void MarkLoaded(
            string path,
            string displayName,
            CatalogueRevision revision)
        {
            BeginCoreChange();
            value.MarkLoaded(path, displayName, NativeRevision(revision));
            this.revision = revision;
            CompleteCoreChange();
        }

        public void MarkSaved(
            string path,
            string displayName,
            CatalogueRevision revision)
        {
            BeginCoreChange();
            value.MarkSaved(path, displayName, NativeRevision(revision));
            this.revision = revision;
            CompleteCoreChange();
        }

        private static C3.Domain.Catalogues.CatalogueRevision NativeRevision(
            CatalogueRevision source)
        {
            return source == null ? null : source.Value;
        }

        private void BeginCoreChange()
        {
            coreRaisedChanged = false;
        }

        private void OnCoreSessionChanged(object sender, EventArgs arguments)
        {
            coreRaisedChanged = true;
        }

        private void CompleteCoreChange()
        {
            if (!coreRaisedChanged)
            {
                return;
            }

            var handler = SessionChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
