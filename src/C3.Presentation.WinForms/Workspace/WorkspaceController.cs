using C3.Catalogue.Catalogues;
using System;

namespace C3.Presentation.WinForms.Workspace
{
    /// <summary>
    /// Coordinates one active document and its transient interaction state. It
    /// delegates all persistence and catalogue mutations to their owning services.
    /// </summary>
    public sealed class WorkspaceController
    {
        private readonly CatalogueSession session;

        public WorkspaceController(
            CatalogueSession session,
            CatalogueCompatibilityMode compatibilityMode,
            bool isReadOnly,
            int historyCapacity)
        {
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            State = new WorkspaceState(session, compatibilityMode, isReadOnly);
            History = new CommandHistory(historyCapacity);
        }

        public WorkspaceState State { get; }

        public CommandHistory History { get; }

        public WorkspaceCommandResult Execute(IReversibleWorkspaceCommand command)
        {
            var result = History.Execute(command);
            SynchronizeDirtyState(result);
            return result;
        }

        public WorkspaceCommandResult Undo()
        {
            var result = History.Undo();
            SynchronizeDirtyState(result);
            return result;
        }

        public WorkspaceCommandResult Redo()
        {
            var result = History.Redo();
            SynchronizeDirtyState(result);
            return result;
        }

        public void BeginNew(
            string displayName,
            CatalogueCompatibilityMode compatibilityMode,
            bool isReadOnly)
        {
            session.BeginNew(displayName);
            ResetTransientState(compatibilityMode, isReadOnly);
        }

        public void MarkLoaded(
            string path,
            string displayName,
            CatalogueRevision revision,
            CatalogueCompatibilityMode compatibilityMode,
            bool isReadOnly)
        {
            session.MarkLoaded(path, displayName, revision);
            ResetTransientState(compatibilityMode, isReadOnly);
        }

        public void MarkSaved(
            string path,
            string displayName,
            CatalogueRevision revision)
        {
            session.MarkSaved(path, displayName, revision);
            History.MarkCheckpoint();
        }

        public void RecordUntrackedMutation()
        {
            // Legacy surfaces cannot supply a reversible semantic command. Their
            // successful mutation therefore invalidates history rather than
            // pretending that a partial undo chain is safe.
            History.Clear();
            History.InvalidateCheckpoint();
            session.MarkChanged();
        }

        private void ResetTransientState(
            CatalogueCompatibilityMode compatibilityMode,
            bool isReadOnly)
        {
            History.Clear();
            State.ResetForDocument(compatibilityMode, isReadOnly);
        }

        private void SynchronizeDirtyState(WorkspaceCommandResult result)
        {
            if (result == null || !result.IsSuccess)
            {
                return;
            }

            // Every execute/undo/redo is a real document mutation and advances the
            // session sequence. Returning exactly to a saved history checkpoint is
            // the only case where that mutation leaves the current bytes clean.
            session.MarkChanged();
            if (History.IsAtCheckpoint)
            {
                session.SetDirtyForMigration(false);
            }
        }
    }
}
