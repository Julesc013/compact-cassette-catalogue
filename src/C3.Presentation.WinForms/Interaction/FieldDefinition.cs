using System;

namespace C3.Presentation.WinForms.Interaction
{
    /// <summary>
    /// Describes one user-editable field without owning a control or a domain
    /// validation rule. Shells use this metadata for consistent labels, help,
    /// required markers, and input limits.
    /// </summary>
    public sealed class FieldDefinition
    {
        public FieldDefinition(
            string key,
            string label,
            bool isRequired,
            int maximumLength,
            string helpText)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("A field key is required.", nameof(key));
            }

            if (string.IsNullOrWhiteSpace(label))
            {
                throw new ArgumentException("A field label is required.", nameof(label));
            }

            if (maximumLength < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumLength));
            }

            Key = key.Trim();
            Label = label.Trim();
            IsRequired = isRequired;
            MaximumLength = maximumLength;
            HelpText = (helpText ?? string.Empty).Trim();
        }

        public string Key { get; }

        public string Label { get; }

        public bool IsRequired { get; }

        /// <summary>Zero means that the presentation does not impose a limit.</summary>
        public int MaximumLength { get; }

        public string HelpText { get; }
    }
}
