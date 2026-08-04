using System;

namespace C3.Presentation.WinForms.Interaction
{
    public enum FeedbackKind
    {
        None = 0,
        Information = 1,
        Warning = 2,
        Error = 3
    }

    public sealed class FeedbackPresentation
    {
        public FeedbackKind Kind { get; private set; }

        public string Message { get; private set; } = string.Empty;

        public bool IsVisible => Kind != FeedbackKind.None;

        public void Show(FeedbackKind kind, string message)
        {
            if (kind == FeedbackKind.None)
            {
                throw new ArgumentOutOfRangeException(nameof(kind));
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("A feedback message is required.", nameof(message));
            }

            Kind = kind;
            Message = message.Trim();
        }

        public void Clear()
        {
            Kind = FeedbackKind.None;
            Message = string.Empty;
        }
    }
}
