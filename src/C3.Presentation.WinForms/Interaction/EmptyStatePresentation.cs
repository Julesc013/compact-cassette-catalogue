using System;

namespace C3.Presentation.WinForms.Interaction
{
    public sealed class EmptyStatePresentation
    {
        private EmptyStatePresentation(bool isVisible, string title, string guidance)
        {
            IsVisible = isVisible;
            Title = title;
            Guidance = guidance;
        }

        public bool IsVisible { get; }

        public string Title { get; }

        public string Guidance { get; }

        public static EmptyStatePresentation Hidden()
        {
            return new EmptyStatePresentation(false, string.Empty, string.Empty);
        }

        public static EmptyStatePresentation Show(string title, string guidance)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("An empty-state title is required.", nameof(title));
            }

            return new EmptyStatePresentation(
                true,
                title.Trim(),
                (guidance ?? string.Empty).Trim());
        }
    }
}
