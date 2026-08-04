using System;

namespace C3.Presentation.WinForms.Interaction
{
    public sealed class ProgressPresentation
    {
        public bool IsActive { get; private set; }

        public string Operation { get; private set; } = string.Empty;

        public int Completed { get; private set; }

        public int Total { get; private set; }

        public bool CanCancel { get; private set; }

        public void Start(string operation, int total, bool canCancel)
        {
            if (string.IsNullOrWhiteSpace(operation))
            {
                throw new ArgumentException("An operation name is required.", nameof(operation));
            }

            if (total < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(total));
            }

            IsActive = true;
            Operation = operation.Trim();
            Completed = 0;
            Total = total;
            CanCancel = canCancel;
        }

        public void Report(int completed)
        {
            if (!IsActive)
            {
                throw new InvalidOperationException("No operation is active.");
            }

            if (completed < 0 || (Total > 0 && completed > Total))
            {
                throw new ArgumentOutOfRangeException(nameof(completed));
            }

            Completed = completed;
        }

        public void Complete()
        {
            IsActive = false;
            Operation = string.Empty;
            Completed = 0;
            Total = 0;
            CanCancel = false;
        }
    }
}
