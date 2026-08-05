using System;

namespace C3.Catalogue.Canonical
{
    public sealed class CatalogueEntityKey :
        IEquatable<CatalogueEntityKey>,
        IComparable<CatalogueEntityKey>
    {
        public CatalogueEntityKey(CatalogueEntityKind kind, string entityId)
        {
            if (!Enum.IsDefined(typeof(CatalogueEntityKind), kind))
            {
                throw new ArgumentOutOfRangeException(nameof(kind));
            }
            if (!IsCanonicalEntityId(entityId))
            {
                throw new ArgumentException(
                    "A canonical entity key requires 32 lowercase hexadecimal characters.",
                    nameof(entityId));
            }

            Kind = kind;
            EntityId = entityId;
        }

        public CatalogueEntityKind Kind { get; }
        public string EntityId { get; }

        public int CompareTo(CatalogueEntityKey other)
        {
            if (other == null)
            {
                return 1;
            }

            var kindResult = Kind.CompareTo(other.Kind);
            return kindResult != 0
                ? kindResult
                : StringComparer.Ordinal.Compare(EntityId, other.EntityId);
        }

        public bool Equals(CatalogueEntityKey other)
        {
            return other != null && Kind == other.Kind &&
                string.Equals(EntityId, other.EntityId, StringComparison.Ordinal);
        }

        public override bool Equals(object other)
        {
            return Equals(other as CatalogueEntityKey);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)Kind * 397) ^
                    StringComparer.Ordinal.GetHashCode(EntityId);
            }
        }

        public override string ToString()
        {
            return KindCode(Kind) + ":" + EntityId;
        }

        public static string KindCode(CatalogueEntityKind kind)
        {
            switch (kind)
            {
                case CatalogueEntityKind.Brand: return "brand";
                case CatalogueEntityKind.CassetteModel: return "cassette-model";
                case CatalogueEntityKind.DeckModel: return "deck-model";
                case CatalogueEntityKind.DeckUnit: return "deck-unit";
                case CatalogueEntityKind.Tape: return "tape";
                case CatalogueEntityKind.Recording: return "recording";
                case CatalogueEntityKind.CatalogueMetadata: return "catalogue-metadata";
                default: throw new ArgumentOutOfRangeException(nameof(kind));
            }
        }

        private static bool IsCanonicalEntityId(string value)
        {
            if (value == null || value.Length != 32)
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
