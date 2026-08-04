using System;

namespace C3.Presentation.WinForms.Workspace
{
    public sealed class BackgroundOperationState : WorkspaceStateComponent
    {
        public bool IsActive { get; private set; }

        public string Description { get; private set; } = string.Empty;

        public bool CanCancel { get; private set; }

        public bool CancellationRequested { get; private set; }

        public void Start(string description, bool canCancel)
        {
            if (IsActive)
            {
                throw new InvalidOperationException("A background operation is already active.");
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException(
                    "A background operation description is required.",
                    nameof(description));
            }

            IsActive = true;
            Description = description.Trim();
            CanCancel = canCancel;
            CancellationRequested = false;
            RaiseChanged();
        }

        public bool RequestCancellation()
        {
            if (!IsActive || !CanCancel || CancellationRequested)
            {
                return false;
            }

            CancellationRequested = true;
            RaiseChanged();
            return true;
        }

        public void Complete()
        {
            Reset();
        }

        internal void Reset()
        {
            if (!IsActive && Description.Length == 0 &&
                !CanCancel && !CancellationRequested)
            {
                return;
            }

            IsActive = false;
            Description = string.Empty;
            CanCancel = false;
            CancellationRequested = false;
            RaiseChanged();
        }
    }
}
