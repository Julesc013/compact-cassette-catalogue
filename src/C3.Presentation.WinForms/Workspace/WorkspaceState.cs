using C3.Catalogue.Catalogues;

namespace C3.Presentation.WinForms.Workspace
{
    public sealed class WorkspaceState : WorkspaceStateComponent
    {
        internal WorkspaceState(
            CatalogueSession session,
            CatalogueCompatibilityMode compatibilityMode,
            bool isReadOnly)
        {
            Document = new DocumentState(session);
            Selection = new SelectionState();
            View = new ViewState();
            EditorDraft = new EditorDraftState();
            Compatibility = new CompatibilityState(compatibilityMode, isReadOnly);
            Recovery = new RecoveryState();
            BackgroundOperation = new BackgroundOperationState();

            Document.Changed += OnComponentChanged;
            Selection.Changed += OnComponentChanged;
            View.Changed += OnComponentChanged;
            EditorDraft.Changed += OnComponentChanged;
            Compatibility.Changed += OnComponentChanged;
            Recovery.Changed += OnComponentChanged;
            BackgroundOperation.Changed += OnComponentChanged;
        }

        public DocumentState Document { get; }

        public SelectionState Selection { get; }

        public ViewState View { get; }

        public EditorDraftState EditorDraft { get; }

        public CompatibilityState Compatibility { get; }

        public RecoveryState Recovery { get; }

        public BackgroundOperationState BackgroundOperation { get; }

        internal void ResetForDocument(
            CatalogueCompatibilityMode compatibilityMode,
            bool isReadOnly)
        {
            Selection.Clear();
            View.Clear();
            EditorDraft.Clear();
            Compatibility.Reset(compatibilityMode, isReadOnly);
            Recovery.Reset();
            BackgroundOperation.Reset();
        }

        private void OnComponentChanged(object sender, System.EventArgs arguments)
        {
            RaiseChanged();
        }
    }
}
