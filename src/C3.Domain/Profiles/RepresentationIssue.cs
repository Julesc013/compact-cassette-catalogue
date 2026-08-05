using System;

namespace C3.Domain.Profiles
{
    public sealed class RepresentationIssue
    {
        public RepresentationIssue(
            string code,
            string path,
            RepresentationEffect effect,
            string message)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentException(
                    "A stable representation issue code is required.",
                    nameof(code));
            }
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException(
                    "A representation issue message is required.",
                    nameof(message));
            }

            Code = code.Trim();
            Path = (path ?? string.Empty).Trim();
            Effect = effect;
            Message = message.Trim();
        }

        public string Code { get; }

        public string Path { get; }

        public RepresentationEffect Effect { get; }

        public string Message { get; }
    }
}
