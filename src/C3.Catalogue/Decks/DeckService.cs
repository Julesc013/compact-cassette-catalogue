using System;
using System.Collections.Generic;
using System.Linq;

namespace C3.Catalogue.Decks
{
    public sealed class DeckService
    {
        private readonly IDeckRepository repository;

        public DeckService(IDeckRepository repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            this.repository = repository;
        }

        public IList<Deck> GetAll()
        {
            return repository.GetAll()
                .OrderBy(value => value.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public Deck Find(string name)
        {
            return repository.FindByName(Normalize(name));
        }

        public DeckOperationResult Create(DeckDetails details, DateTime addedAt)
        {
            var validation = Validate(details);
            if (!validation.IsSuccess)
            {
                return validation;
            }

            var normalizedDetails = Normalize(details);
            var name = normalizedDetails.Manufacturer + " " + normalizedDetails.Model;
            if (repository.NameExists(name))
            {
                return DeckOperationResult.Failed(
                    DeckFailure.DuplicateName,
                    "Deck name '" + name + "' is already in use.");
            }

            var value = new Deck(name, addedAt, normalizedDetails);
            try
            {
                repository.Add(value);
                return DeckOperationResult.Success(value);
            }
            catch (Exception exception)
            {
                return DeckOperationResult.Failed(DeckFailure.StorageFailure, exception.Message);
            }
        }

        public DeckOperationResult Update(string name, DeckDetails details)
        {
            var normalizedName = Normalize(name);
            var existing = repository.FindByName(normalizedName);
            if (existing == null)
            {
                return DeckOperationResult.Failed(
                    DeckFailure.NotFound,
                    "The selected deck no longer exists.");
            }

            var validation = Validate(details);
            if (!validation.IsSuccess)
            {
                return validation;
            }

            var value = new Deck(existing.Name, existing.AddedAt, Normalize(details));
            try
            {
                repository.Update(value);
                return DeckOperationResult.Success(value);
            }
            catch (Exception exception)
            {
                return DeckOperationResult.Failed(DeckFailure.StorageFailure, exception.Message);
            }
        }

        public DeckOperationResult Delete(string name)
        {
            var normalizedName = Normalize(name);
            var existing = repository.FindByName(normalizedName);
            if (existing == null)
            {
                return DeckOperationResult.Failed(
                    DeckFailure.NotFound,
                    "The selected deck no longer exists.");
            }

            if (repository.IsReferencedByTape(normalizedName))
            {
                return DeckOperationResult.Failed(
                    DeckFailure.ReferencedByTape,
                    "Deck '" + normalizedName + "' cannot be deleted while recordings use it.");
            }

            try
            {
                repository.Delete(normalizedName);
                return DeckOperationResult.Success(existing);
            }
            catch (Exception exception)
            {
                return DeckOperationResult.Failed(DeckFailure.StorageFailure, exception.Message);
            }
        }

        private static DeckOperationResult Validate(DeckDetails details)
        {
            if (details == null || string.IsNullOrWhiteSpace(details.Manufacturer))
            {
                return DeckOperationResult.Failed(
                    DeckFailure.ManufacturerRequired,
                    "Manufacturer name is required.");
            }

            if (string.IsNullOrWhiteSpace(details.Model))
            {
                return DeckOperationResult.Failed(DeckFailure.ModelRequired, "Model name is required.");
            }

            if (!(details.Type1 || details.Type2 || details.Type3 || details.Type4))
            {
                return DeckOperationResult.Failed(
                    DeckFailure.TapeTypeRequired,
                    "At least one supported cassette type is required.");
            }

            if (!(details.SpeedSlow || details.SpeedNormal || details.SpeedFast))
            {
                return DeckOperationResult.Failed(
                    DeckFailure.SpeedRequired,
                    "At least one supported tape speed is required.");
            }

            return DeckOperationResult.Success(null);
        }

        private static DeckDetails Normalize(DeckDetails details)
        {
            return new DeckDetails(
                Normalize(details.Manufacturer),
                Normalize(details.Model),
                details.Year,
                details.Condition,
                details.Type1,
                details.Type2,
                details.Type3,
                details.Type4,
                details.Hx,
                details.Mpx,
                details.DolbyB,
                details.DolbyC,
                details.DolbyS,
                details.Dbx1,
                details.Dbx2,
                details.Stereo,
                details.ProgramSearch,
                details.Reverse,
                details.Calibration,
                details.Azimuth,
                details.DubbingSlow,
                details.DubbingFast,
                details.FrequencyLow,
                details.FrequencyHigh,
                details.SignalRatio,
                Normalize(details.SignalRatioNoiseReduction),
                details.WowFlutter,
                details.Distortion,
                details.Heads,
                details.Wells,
                details.SpeedSlow,
                details.SpeedNormal,
                details.SpeedFast,
                Normalize(details.Notes));
        }

        private static string Normalize(string value)
        {
            return (value ?? string.Empty).Trim();
        }
    }
}
