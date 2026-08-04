using System;

namespace C3.Presentation.WinForms.Workspace
{
    public sealed class EditorDraftState : WorkspaceStateComponent
    {
        public string Feature { get; private set; }

        public string EntityId { get; private set; }

        public bool IsActive => Feature != null;

        public bool IsDirty { get; private set; }

        public void Begin(string feature, string entityId)
        {
            if (string.IsNullOrWhiteSpace(feature))
            {
                throw new ArgumentException("An editor feature is required.", nameof(feature));
            }

            Feature = feature.Trim();
            EntityId = string.IsNullOrWhiteSpace(entityId) ? null : entityId.Trim();
            IsDirty = false;
            RaiseChanged();
        }

        public void MarkChanged()
        {
            if (!IsActive)
            {
                throw new InvalidOperationException("No editor draft is active.");
            }

            if (!IsDirty)
            {
                IsDirty = true;
                RaiseChanged();
            }
        }

        public void MarkApplied()
        {
            if (IsActive && IsDirty)
            {
                IsDirty = false;
                RaiseChanged();
            }
        }

        public void Clear()
        {
            if (!IsActive && !IsDirty)
            {
                return;
            }

            Feature = null;
            EntityId = null;
            IsDirty = false;
            RaiseChanged();
        }
    }
}
