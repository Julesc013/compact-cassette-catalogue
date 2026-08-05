using System;

namespace C3.Catalogue.Canonical
{
    public sealed class CatalogueFingerprintEntry
    {
        public CatalogueFingerprintEntry(CatalogueEntityKey key, string digest)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            if (!IsSha256(digest))
            {
                throw new ArgumentException(
                    "An entity digest requires 64 lowercase hexadecimal characters.",
                    nameof(digest));
            }

            Digest = digest;
        }

        public CatalogueEntityKey Key { get; }
        public string Digest { get; }

        private static bool IsSha256(string value)
        {
            if (value == null || value.Length != 64)
            {
                return false;
            }

            for (var index = 0; index < value.Length; index++)
            {
                var character = value[index];
                if (!((character >= '0' && character <= '9') ||
                    (character >= 'a' && character <= 'f')))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
