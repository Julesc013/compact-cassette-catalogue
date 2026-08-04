using C3.Domain.Identity;
using C3.Domain.Time;

namespace C3.Catalogue.Native
{
    public sealed class NativeBrand
    {
        public NativeBrand(
            EntityId<NativeBrand> id,
            string name,
            string legacyCode,
            UtcTimestamp addedAt,
            string notes)
        {
            Id = id;
            Name = NativeText.Required(name, nameof(name));
            LegacyCode = NativeText.Required(legacyCode, nameof(legacyCode));
            AddedAt = addedAt;
            Notes = NativeText.Optional(notes);
        }

        public EntityId<NativeBrand> Id { get; }

        public string Name { get; }

        public string LegacyCode { get; }

        public UtcTimestamp AddedAt { get; }

        public string Notes { get; }
    }
}
