using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace C3.Catalogue.Canonical
{
    public sealed class CatalogueState
    {
        public CatalogueState(
            CatalogueMetadataState metadata,
            IEnumerable<CatalogueBrandState> brands,
            IEnumerable<CatalogueCassetteModelState> cassetteModels,
            IEnumerable<CatalogueDeckModelState> deckModels,
            IEnumerable<CatalogueDeckUnitState> deckUnits,
            IEnumerable<CatalogueTapeState> tapes,
            IEnumerable<CatalogueRecordingState> recordings)
        {
            Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            Brands = Copy(brands, item => item.Id, CatalogueEntityKind.Brand, nameof(brands));
            CassetteModels = Copy(
                cassetteModels,
                item => item.Id,
                CatalogueEntityKind.CassetteModel,
                nameof(cassetteModels));
            DeckModels = Copy(
                deckModels,
                item => item.Id,
                CatalogueEntityKind.DeckModel,
                nameof(deckModels));
            DeckUnits = Copy(
                deckUnits,
                item => item.Id,
                CatalogueEntityKind.DeckUnit,
                nameof(deckUnits));
            Tapes = Copy(tapes, item => item.Id, CatalogueEntityKind.Tape, nameof(tapes));
            Recordings = Copy(
                recordings,
                item => item.Id,
                CatalogueEntityKind.Recording,
                nameof(recordings));
            ValidateGraph();
        }

        public CatalogueMetadataState Metadata { get; }
        public ReadOnlyCollection<CatalogueBrandState> Brands { get; }
        public ReadOnlyCollection<CatalogueCassetteModelState> CassetteModels { get; }
        public ReadOnlyCollection<CatalogueDeckModelState> DeckModels { get; }
        public ReadOnlyCollection<CatalogueDeckUnitState> DeckUnits { get; }
        public ReadOnlyCollection<CatalogueTapeState> Tapes { get; }
        public ReadOnlyCollection<CatalogueRecordingState> Recordings { get; }

        public int TotalEntities => checked(
            Brands.Count + CassetteModels.Count + DeckModels.Count +
            DeckUnits.Count + Tapes.Count + Recordings.Count);

        private static ReadOnlyCollection<T> Copy<T>(
            IEnumerable<T> source,
            Func<T, CatalogueEntityKey> key,
            CatalogueEntityKind kind,
            string parameterName)
            where T : class
        {
            if (source == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            var values = new List<T>(source);
            if (values.Exists(item => item == null))
            {
                throw new ArgumentException(
                    "Entity collections cannot contain null.", parameterName);
            }
            values.Sort((left, right) => key(left).CompareTo(key(right)));
            CatalogueEntityKey previous = null;
            foreach (var value in values)
            {
                var current = CatalogueMetadataState.RequireKind(
                    key(value), kind, parameterName);
                if (previous != null && previous.Equals(current))
                {
                    throw new ArgumentException(
                        "Entity identifiers must be unique.", parameterName);
                }
                previous = current;
            }

            return values.AsReadOnly();
        }

        private void ValidateGraph()
        {
            var brands = Keys(Brands, item => item.Id);
            var models = Keys(CassetteModels, item => item.Id);
            var deckModels = Keys(DeckModels, item => item.Id);
            var deckUnits = Keys(DeckUnits, item => item.Id);
            var recordings = Keys(Recordings, item => item.Id);
            var usedRecordings = new HashSet<CatalogueEntityKey>();

            foreach (var model in CassetteModels)
            {
                Require(brands, model.BrandId, "Cassette model brand");
            }
            foreach (var deck in DeckUnits)
            {
                Require(deckModels, deck.DeckModelId, "Deck unit model");
            }
            foreach (var recording in Recordings)
            {
                if (recording.DeckUnitId != null)
                {
                    Require(deckUnits, recording.DeckUnitId, "Recording deck unit");
                }
            }
            foreach (var tape in Tapes)
            {
                Require(models, tape.CassetteModelId, "Tape cassette model");
                RequireRecording(tape.SideA, recordings, usedRecordings);
                RequireRecording(tape.SideB, recordings, usedRecordings);
            }
            if (usedRecordings.Count != Recordings.Count)
            {
                throw new ArgumentException(
                    "Every recording must belong to exactly one tape side.");
            }
        }

        private static HashSet<CatalogueEntityKey> Keys<T>(
            IEnumerable<T> values,
            Func<T, CatalogueEntityKey> key)
        {
            var result = new HashSet<CatalogueEntityKey>();
            foreach (var value in values)
            {
                result.Add(key(value));
            }
            return result;
        }

        private static void Require(
            HashSet<CatalogueEntityKey> values,
            CatalogueEntityKey key,
            string relationship)
        {
            if (!values.Contains(key))
            {
                throw new ArgumentException(
                    relationship + " reference does not resolve: " + key + ".");
            }
        }

        private static void RequireRecording(
            CatalogueTapeSideState side,
            HashSet<CatalogueEntityKey> recordings,
            HashSet<CatalogueEntityKey> used)
        {
            if (side.RecordingId == null)
            {
                return;
            }
            Require(recordings, side.RecordingId, "Tape-side recording");
            if (!used.Add(side.RecordingId))
            {
                throw new ArgumentException(
                    "A recording cannot belong to more than one tape side.");
            }
        }
    }
}
