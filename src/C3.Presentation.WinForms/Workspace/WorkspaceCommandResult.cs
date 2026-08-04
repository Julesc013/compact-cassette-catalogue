using System;

namespace C3.Presentation.WinForms.Workspace
{
    public sealed class WorkspaceCommandResult
    {
        private WorkspaceCommandResult(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public bool IsSuccess { get; }

        public string Message { get; }

        public static WorkspaceCommandResult Success()
        {
            return new WorkspaceCommandResult(true, string.Empty);
        }

        public static WorkspaceCommandResult Failed(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException(
                    "A failed workspace command requires a message.",
                    nameof(message));
            }

            return new WorkspaceCommandResult(false, message.Trim());
        }
    }
}
