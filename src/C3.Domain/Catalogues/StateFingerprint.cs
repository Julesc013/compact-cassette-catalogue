using System;

namespace C3.Domain.Catalogues
{
    /// <summary>
    /// Identifies semantic catalogue content under an explicit canonicalization
    /// scheme. It is independent of file bytes, history position, and location.
    /// </summary>
    public sealed class StateFingerprint : IEquatable<StateFingerprint>
    {
        public const string Sha256V1Scheme = "c3-logical-state-sha256-v1";

        public StateFingerprint(string scheme, string digest)
        {
            Scheme = RequireScheme(scheme);
            Digest = RequireDigest(digest);
        }

        public string Scheme { get; }

        public string Digest { get; }

        public static StateFingerprint Sha256V1(string digest)
        {
            return new StateFingerprint(Sha256V1Scheme, digest);
        }

        public static StateFingerprint Parse(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var separator = value.IndexOf(':');
            if (separator <= 0 || separator != value.LastIndexOf(':'))
            {
                throw new ArgumentException(
                    "A state fingerprint must contain one scheme/digest separator.",
                    nameof(value));
            }

            return new StateFingerprint(
                value.Substring(0, separator),
                value.Substring(separator + 1));
        }

        public static bool TryParse(string value, out StateFingerprint result)
        {
            try
            {
                result = Parse(value);
                return true;
            }
            catch (ArgumentException)
            {
                result = null;
                return false;
            }
        }

        public bool Equals(StateFingerprint other)
        {
            return other != null &&
                string.Equals(Scheme, other.Scheme, StringComparison.Ordinal) &&
                string.Equals(Digest, other.Digest, StringComparison.Ordinal);
        }

        public override bool Equals(object other)
        {
            return Equals(other as StateFingerprint);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (StringComparer.Ordinal.GetHashCode(Scheme) * 397) ^
                    StringComparer.Ordinal.GetHashCode(Digest);
            }
        }

        public override string ToString()
        {
            return Scheme + ":" + Digest;
        }

        private static string RequireScheme(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length > 64)
            {
                throw new ArgumentException(
                    "A state-fingerprint scheme must contain 1 through 64 characters.",
                    nameof(value));
            }

            for (var index = 0; index < value.Length; index++)
            {
                var character = value[index];
                if (!((character >= 'a' && character <= 'z') ||
                    (character >= '0' && character <= '9') ||
                    character == '-'))
                {
                    throw new ArgumentException(
                        "A state-fingerprint scheme must use lowercase ASCII letters, digits, and hyphens.",
                        nameof(value));
                }
            }

            return value;
        }

        private static string RequireDigest(string value)
        {
            if (value == null || value.Length != 64)
            {
                throw new ArgumentException(
                    "A state-fingerprint digest must contain 64 lowercase hexadecimal characters.",
                    nameof(value));
            }

            for (var index = 0; index < value.Length; index++)
            {
                var character = value[index];
                if (!((character >= '0' && character <= '9') ||
                    (character >= 'a' && character <= 'f')))
                {
                    throw new ArgumentException(
                        "A state-fingerprint digest must contain 64 lowercase hexadecimal characters.",
                        nameof(value));
                }
            }

            return value;
        }
    }
}
