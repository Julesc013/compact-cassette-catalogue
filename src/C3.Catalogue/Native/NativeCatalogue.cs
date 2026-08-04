using C3.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace C3.Catalogue.Native
{
    public sealed class NativeCatalogue
    {
        public NativeCatalogue(
            EntityId<NativeCatalogue> id,
            NativeCatalogueMetadata metadata,
            IEnumerable<NativeBrand> brands,
            IEnumerable<NativeCassetteModel> cassetteModels,
            IEnumerable<NativeDeckModel> deckModels,
            IEnumerable<NativeDeckUnit> deckUnits,
            IEnumerable<NativeTape> tapes)
        {
            Id = id;
            Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            Brands = CopyAndSort(brands, item => item.Id.ToString(), nameof(brands));
            CassetteModels = CopyAndSort(cassetteModels, item => item.Id.ToString(), nameof(cassetteModels));
            DeckModels = CopyAndSort(deckModels, item => item.Id.ToString(), nameof(deckModels));
            DeckUnits = CopyAndSort(deckUnits, item => item.Id.ToString(), nameof(deckUnits));
            Tapes = CopyAndSort(tapes, item => item.Id.ToString(), nameof(tapes));
            ValidateGraph();
        }

        public EntityId<NativeCatalogue> Id { get; }
        public NativeCatalogueMetadata Metadata { get; }
        public ReadOnlyCollection<NativeBrand> Brands { get; }
        public ReadOnlyCollection<NativeCassetteModel> CassetteModels { get; }
        public ReadOnlyCollection<NativeDeckModel> DeckModels { get; }
        public ReadOnlyCollection<NativeDeckUnit> DeckUnits { get; }
        public ReadOnlyCollection<NativeTape> Tapes { get; }

        private static ReadOnlyCollection<T> CopyAndSort<T>(
            IEnumerable<T> source,
            Func<T, string> key,
            string parameterName)
        {
            if (source == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            var values = new List<T>(source);
            if (values.Exists(item => ReferenceEquals(item, null)))
            {
                throw new ArgumentException("Entity collections cannot contain null.", parameterName);
            }

            values.Sort((left, right) => StringComparer.Ordinal.Compare(key(left), key(right)));
            var previous = string.Empty;
            foreach (var value in values)
            {
                var current = key(value);
                if (string.Equals(previous, current, StringComparison.Ordinal))
                {
                    throw new ArgumentException("Entity identifiers must be unique.", parameterName);
                }

                previous = current;
            }

            return values.AsReadOnly();
        }

        private void ValidateGraph()
        {
            var brandIds = IdSet(Brands, item => item.Id.ToString());
            var modelIds = IdSet(CassetteModels, item => item.Id.ToString());
            var deckModelIds = IdSet(DeckModels, item => item.Id.ToString());
            var deckUnitIds = IdSet(DeckUnits, item => item.Id.ToString());
            var recordingIds = new HashSet<string>(StringComparer.Ordinal);

            foreach (var model in CassetteModels)
            {
                RequireReference(brandIds, model.BrandId.ToString(), "Cassette model brand");
            }
            foreach (var deck in DeckUnits)
            {
                RequireReference(deckModelIds, deck.DeckModelId.ToString(), "Deck unit model");
            }
            foreach (var tape in Tapes)
            {
                RequireReference(modelIds, tape.CassetteModelId.ToString(), "Tape cassette model");
                ValidateRecording(tape.SideA, deckUnitIds, recordingIds);
                ValidateRecording(tape.SideB, deckUnitIds, recordingIds);
            }
        }

        private static HashSet<string> IdSet<T>(IEnumerable<T> values, Func<T, string> key)
        {
            var result = new HashSet<string>(StringComparer.Ordinal);
            foreach (var value in values)
            {
                result.Add(key(value));
            }

            return result;
        }

        private static void ValidateRecording(
            NativeTapeSide side,
            HashSet<string> deckUnitIds,
            HashSet<string> recordingIds)
        {
            if (!side.Recording.HasValue)
            {
                return;
            }

            var recording = side.Recording.Value;
            if (!recordingIds.Add(recording.Id.ToString()))
            {
                throw new ArgumentException("Recording identifiers must be unique within the catalogue.");
            }

            if (recording.DeckUnitId.HasValue)
            {
                RequireReference(deckUnitIds, recording.DeckUnitId.Value.ToString(), "Recording deck unit");
            }
        }

        private static void RequireReference(HashSet<string> ids, string id, string relationship)
        {
            if (!ids.Contains(id))
            {
                throw new ArgumentException(relationship + " reference does not resolve: " + id + ".");
            }
        }
    }
}
