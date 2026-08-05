using System;

namespace C3.Domain.Identity
{
    internal static class CanonicalGuidText
    {
        public static string Require(Guid value, string parameterName)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException(
                    "An empty GUID is not a valid identity.",
                    parameterName);
            }

            return value.ToString("D");
        }

        public static string Parse(string value, string parameterName)
        {
            Guid parsed;
            if (!TryParse(value, out parsed))
            {
                throw new ArgumentException(
                    "Identity text must be a non-empty lowercase GUID in canonical D format.",
                    parameterName);
            }

            return parsed.ToString("D");
        }

        public static bool TryParse(string value, out Guid parsed)
        {
            if (!Guid.TryParseExact(value, "D", out parsed) ||
                parsed == Guid.Empty ||
                !string.Equals(parsed.ToString("D"), value, StringComparison.Ordinal))
            {
                parsed = Guid.Empty;
                return false;
            }

            return true;
        }
    }
}
