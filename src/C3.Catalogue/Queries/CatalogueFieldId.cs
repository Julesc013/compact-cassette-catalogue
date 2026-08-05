using System;

namespace C3.Catalogue.Queries
{
    public sealed class CatalogueFieldId : IEquatable<CatalogueFieldId>
    {
        public CatalogueFieldId(string code)
        {
            if (string.IsNullOrEmpty(code) || code.Length > 96)
            {
                throw new ArgumentException(
                    "A field code must contain 1 through 96 characters.",
                    nameof(code));
            }
            for (var index = 0; index < code.Length; index++)
            {
                var character = code[index];
                if (!((character >= 'a' && character <= 'z') ||
                    (character >= '0' && character <= '9') ||
                    character == '.' || character == '-'))
                {
                    throw new ArgumentException(
                        "A field code must use lowercase ASCII letters, digits, dots, and hyphens.",
                        nameof(code));
                }
            }

            Code = code;
        }

        public string Code { get; }

        public bool Equals(CatalogueFieldId other)
        {
            return other != null &&
                string.Equals(Code, other.Code, StringComparison.Ordinal);
        }

        public override bool Equals(object other)
        {
            return Equals(other as CatalogueFieldId);
        }

        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(Code);
        }

        public override string ToString()
        {
            return Code;
        }
    }
}
