using C3.Catalogue.Catalogues;
using System;

namespace C3.Presentation.WinForms.Workspace
{
    /// <summary>
    /// Read-only presentation projection over the catalogue session. The session
    /// remains the only owner of document identity, revision, and dirty state.
    /// </summary>
    public sealed class DocumentState : WorkspaceStateComponent
    {
        private readonly CatalogueSession session;

        internal DocumentState(CatalogueSession session)
        {
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            this.session.SessionChanged += OnSessionChanged;
        }

        public string FilePath => session.FilePath;

        public string DisplayName => session.DisplayName;

        public CatalogueRevision Revision => session.Revision;

        public bool IsDirty => session.IsDirty;

        public long ChangeSequence => session.ChangeSequence;

        private void OnSessionChanged(object sender, EventArgs arguments)
        {
            RaiseChanged();
        }
    }
}
