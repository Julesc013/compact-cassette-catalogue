using C3.Domain.Catalogues;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace C3.Catalogue.Canonical
{
    public sealed class CatalogueFingerprintEngine
    {
        private const string Header = "c3-fingerprint-index-v1\n";

        public CatalogueFingerprintIndex ComputeFull(
            IEnumerable<CatalogueFingerprintEntry> entries)
        {
            if (entries == null)
            {
                throw new ArgumentNullException(nameof(entries));
            }

            var copy = entries.ToList();
            var provisional = new CatalogueFingerprintIndex(
                copy,
                StateFingerprint.Sha256V1(new string('0', 64)));
            return new CatalogueFingerprintIndex(
                provisional.Entries,
                StateFingerprint.Sha256V1(ComputeRoot(provisional.Entries)));
        }

        public CatalogueFingerprintIndex ApplyDelta(
            CatalogueFingerprintIndex current,
            CatalogueFingerprintDelta delta)
        {
            if (current == null)
            {
                throw new ArgumentNullException(nameof(current));
            }
            if (delta == null)
            {
                throw new ArgumentNullException(nameof(delta));
            }

            var entries = current.Entries.ToDictionary(item => item.Key);
            foreach (var key in delta.Removals)
            {
                if (!entries.Remove(key))
                {
                    throw new InvalidOperationException(
                        "A fingerprint delta cannot remove an absent entity: " + key + ".");
                }
            }
            foreach (var entry in delta.Upserts)
            {
                entries[entry.Key] = entry;
            }

            return ComputeFull(entries.Values);
        }

        public bool Verify(
            CatalogueFingerprintIndex expected,
            IEnumerable<CatalogueFingerprintEntry> currentEntries)
        {
            if (expected == null)
            {
                throw new ArgumentNullException(nameof(expected));
            }

            var actual = ComputeFull(currentEntries);
            if (!expected.Root.Equals(actual.Root) ||
                expected.Entries.Count != actual.Entries.Count)
            {
                return false;
            }

            for (var index = 0; index < expected.Entries.Count; index++)
            {
                var left = expected.Entries[index];
                var right = actual.Entries[index];
                if (!left.Key.Equals(right.Key) ||
                    !string.Equals(left.Digest, right.Digest, StringComparison.Ordinal))
                {
                    return false;
                }
            }

            return true;
        }

        private static string ComputeRoot(
            IEnumerable<CatalogueFingerprintEntry> entries)
        {
            var text = new StringBuilder(Header);
            foreach (var entry in entries)
            {
                text.Append(CatalogueEntityKey.KindCode(entry.Key.Kind))
                    .Append('|')
                    .Append(entry.Key.EntityId)
                    .Append('|')
                    .Append(entry.Digest)
                    .Append('\n');
            }

            using (var algorithm = SHA256.Create())
            {
                var digest = algorithm.ComputeHash(
                    new UTF8Encoding(false, true).GetBytes(text.ToString()));
                var result = new StringBuilder(digest.Length * 2);
                foreach (var value in digest)
                {
                    result.Append(value.ToString("x2", CultureInfo.InvariantCulture));
                }

                return result.ToString();
            }
        }
    }
}
