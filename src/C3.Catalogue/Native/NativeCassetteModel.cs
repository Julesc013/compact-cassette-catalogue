using C3.Domain.Identity;
using C3.Domain.Time;

namespace C3.Catalogue.Native
{
    public sealed class NativeCassetteModel
    {
        public NativeCassetteModel(
            EntityId<NativeCassetteModel> id,
            EntityId<NativeBrand> brandId,
            int typeNumber,
            string modelName,
            string legacyCode,
            string legacyIdentifier,
            string displayName,
            UtcTimestamp addedAt,
            string notes)
        {
            Id = id;
            BrandId = brandId;
            TypeNumber = typeNumber;
            ModelName = NativeText.Required(modelName, nameof(modelName));
            LegacyCode = NativeText.Required(legacyCode, nameof(legacyCode));
            LegacyIdentifier = NativeText.Required(legacyIdentifier, nameof(legacyIdentifier));
            DisplayName = NativeText.Required(displayName, nameof(displayName));
            AddedAt = addedAt;
            Notes = NativeText.Optional(notes);
        }

        public EntityId<NativeCassetteModel> Id { get; }
        public EntityId<NativeBrand> BrandId { get; }
        public int TypeNumber { get; }
        public string ModelName { get; }
        public string LegacyCode { get; }
        public string LegacyIdentifier { get; }
        public string DisplayName { get; }
        public UtcTimestamp AddedAt { get; }
        public string Notes { get; }
    }
}
