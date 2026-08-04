using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace C3.Presentation.WinForms.Interaction
{
    public sealed class ValidationMessage
    {
        public ValidationMessage(string fieldKey, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("A validation message is required.", nameof(message));
            }

            FieldKey = (fieldKey ?? string.Empty).Trim();
            Message = message.Trim();
        }

        public string FieldKey { get; }

        public string Message { get; }
    }

    public sealed class ValidationPresentation
    {
        private ReadOnlyCollection<ValidationMessage> messages =
            new List<ValidationMessage>().AsReadOnly();

        public ReadOnlyCollection<ValidationMessage> Messages => messages;

        public bool HasErrors => messages.Count > 0;

        public string Summary => string.Join(
            Environment.NewLine,
            messages.Select(value => value.Message).Distinct().ToArray());

        public string ForField(string fieldKey)
        {
            var normalized = (fieldKey ?? string.Empty).Trim();
            var match = messages.FirstOrDefault(value =>
                string.Equals(value.FieldKey, normalized, StringComparison.Ordinal));
            return match == null ? string.Empty : match.Message;
        }

        public void Show(params ValidationMessage[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            messages = values.ToList().AsReadOnly();
        }

        public void Clear()
        {
            messages = new List<ValidationMessage>().AsReadOnly();
        }
    }
}
