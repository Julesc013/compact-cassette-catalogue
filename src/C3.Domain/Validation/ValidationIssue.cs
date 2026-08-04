using System;

namespace C3.Domain.Validation
{
    public sealed class ValidationIssue
    {
        public ValidationIssue(string code, string path, string message)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentException("A stable validation code is required.", nameof(code));
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("A validation message is required.", nameof(message));
            }

            Code = code.Trim();
            Path = (path ?? string.Empty).Trim();
            Message = message.Trim();
        }

        public string Code { get; }

        public string Path { get; }

        public string Message { get; }
    }
}
