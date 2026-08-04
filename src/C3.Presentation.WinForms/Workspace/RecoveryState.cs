using System;

namespace C3.Presentation.WinForms.Workspace
{
    public enum RecoveryStatus
    {
        None = 0,
        Available = 1,
        InProgress = 2,
        Failed = 3
    }

    public sealed class RecoveryState : WorkspaceStateComponent
    {
        public RecoveryStatus Status { get; private set; }

        public string Message { get; private set; } = string.Empty;

        public void Report(RecoveryStatus status, string message)
        {
            if (!Enum.IsDefined(typeof(RecoveryStatus), status))
            {
                throw new ArgumentOutOfRangeException(nameof(status));
            }

            if (status == RecoveryStatus.None)
            {
                Reset();
                return;
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException(
                    "A recovery status requires a user-facing message.",
                    nameof(message));
            }

            Status = status;
            Message = message.Trim();
            RaiseChanged();
        }

        public void Reset()
        {
            if (Status == RecoveryStatus.None && Message.Length == 0)
            {
                return;
            }

            Status = RecoveryStatus.None;
            Message = string.Empty;
            RaiseChanged();
        }
    }
}
