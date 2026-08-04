using C3.Domain.Time;
using C3.Domain.Values;
using System;

namespace C3.Catalogue.Native
{
    public sealed class NativeCatalogueMetadata
    {
        public NativeCatalogueMetadata(
            string producer,
            UtcTimestamp createdAt,
            UtcTimestamp modifiedAt,
            Optional<NativeCatalogueProvenance> provenance)
        {
            Producer = NativeText.Required(producer, nameof(producer));
            if (modifiedAt.CompareTo(createdAt) < 0)
            {
                throw new ArgumentException("Modified time cannot precede created time.", nameof(modifiedAt));
            }

            CreatedAt = createdAt;
            ModifiedAt = modifiedAt;
            Provenance = provenance;
        }

        public string Producer { get; }

        public UtcTimestamp CreatedAt { get; }

        public UtcTimestamp ModifiedAt { get; }

        public Optional<NativeCatalogueProvenance> Provenance { get; }
    }

    public sealed class NativeCatalogueProvenance
    {
        public NativeCatalogueProvenance(
            string sourceFormat,
            string sourceRevision,
            string migrationProfile)
        {
            SourceFormat = NativeText.Required(sourceFormat, nameof(sourceFormat));
            if (sourceRevision == null || sourceRevision.Length != 64)
            {
                throw new ArgumentException("Source revisions must be a 64-character SHA-256 value.", nameof(sourceRevision));
            }

            for (var index = 0; index < sourceRevision.Length; index++)
            {
                var character = sourceRevision[index];
                if (!((character >= '0' && character <= '9') ||
                    (character >= 'a' && character <= 'f')))
                {
                    throw new ArgumentException("Source revisions must use lowercase hexadecimal SHA-256 text.", nameof(sourceRevision));
                }
            }

            SourceRevision = sourceRevision;
            MigrationProfile = NativeText.Required(migrationProfile, nameof(migrationProfile));
        }

        public string SourceFormat { get; }

        public string SourceRevision { get; }

        public string MigrationProfile { get; }
    }
}
