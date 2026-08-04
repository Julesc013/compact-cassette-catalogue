using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace C3.Catalogue.Tapes
{
    public sealed class TapeService
    {
        private readonly ITapeRepository repository;

        public TapeService(ITapeRepository repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            this.repository = repository;
        }

        public IList<Tape> GetAll()
        {
            return repository.GetAll()
                .OrderBy(value => value.ShortIdentifier, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public Tape Find(string shortIdentifier)
        {
            return repository.FindByShortIdentifier(Normalize(shortIdentifier));
        }

        public TapeOperationResult CreateMany(TapeDraft draft, int count, DateTime addedAt)
        {
            var validation = Validate(draft, count);
            if (!validation.IsSuccess)
            {
                return validation;
            }

            var modelIdentifier = Normalize(draft.ModelIdentifier);
            if (!repository.ModelExists(modelIdentifier))
            {
                return TapeOperationResult.Failed(
                    TapeFailure.ModelNotFound,
                    "The selected cassette model no longer exists.");
            }

            var firstNumber = repository.NextNumberForModel(modelIdentifier);
            if (firstNumber < 0 || firstNumber + count - 1 > 999)
            {
                return TapeOperationResult.Failed(
                    TapeFailure.IdentifierCapacityExceeded,
                    "This model has exhausted the three-digit tape identifier range.");
            }

            var values = new List<Tape>();
            for (var offset = 0; offset <= count - 1; offset++)
            {
                var number = firstNumber + offset;
                var numberCode = number.ToString("000", CultureInfo.InvariantCulture);
                var shortIdentifier = modelIdentifier + numberCode;
                var identifier = BuildIdentifier(
                    modelIdentifier,
                    draft.Year,
                    draft.LengthMinutes,
                    numberCode);
                if (repository.IdentifierExists(identifier, shortIdentifier))
                {
                    return TapeOperationResult.Failed(
                        TapeFailure.DuplicateIdentifier,
                        "Tape identifier '" + shortIdentifier + "' is already in use.");
                }

                values.Add(new Tape(
                    modelIdentifier,
                    draft.Year,
                    draft.LengthMinutes,
                    Normalize(draft.Region),
                    number,
                    identifier,
                    shortIdentifier,
                    draft.Condition,
                    draft.Packaged,
                    NormalizeSide(draft.SideA, draft.Packaged),
                    NormalizeSide(draft.SideB, draft.Packaged),
                    addedAt,
                    Normalize(draft.Notes)));
            }

            try
            {
                repository.AddRange(values);
                return TapeOperationResult.Success(values);
            }
            catch (Exception exception)
            {
                return TapeOperationResult.Failed(TapeFailure.StorageFailure, exception.Message);
            }
        }

        public TapeOperationResult Update(string shortIdentifier, TapeDraft draft)
        {
            var existing = repository.FindByShortIdentifier(Normalize(shortIdentifier));
            if (existing == null)
            {
                return TapeOperationResult.Failed(
                    TapeFailure.NotFound,
                    "The selected tape no longer exists.");
            }

            var validation = Validate(draft, 1);
            if (!validation.IsSuccess)
            {
                return validation;
            }

            var value = new Tape(
                existing.ModelIdentifier,
                draft.Year,
                draft.LengthMinutes,
                Normalize(draft.Region),
                existing.Number,
                BuildIdentifier(
                    existing.ModelIdentifier,
                    draft.Year,
                    draft.LengthMinutes,
                    existing.Number.ToString("000", CultureInfo.InvariantCulture)),
                existing.ShortIdentifier,
                draft.Condition,
                draft.Packaged,
                NormalizeSide(draft.SideA, draft.Packaged),
                NormalizeSide(draft.SideB, draft.Packaged),
                existing.AddedAt,
                Normalize(draft.Notes));
            try
            {
                repository.Update(value);
                return TapeOperationResult.Success(new List<Tape> { value });
            }
            catch (Exception exception)
            {
                return TapeOperationResult.Failed(TapeFailure.StorageFailure, exception.Message);
            }
        }

        public TapeOperationResult Delete(string shortIdentifier)
        {
            var existing = repository.FindByShortIdentifier(Normalize(shortIdentifier));
            if (existing == null)
            {
                return TapeOperationResult.Failed(
                    TapeFailure.NotFound,
                    "The selected tape no longer exists.");
            }

            try
            {
                repository.Delete(existing.ShortIdentifier);
                return TapeOperationResult.Success(new List<Tape> { existing });
            }
            catch (Exception exception)
            {
                return TapeOperationResult.Failed(TapeFailure.StorageFailure, exception.Message);
            }
        }

        private static TapeOperationResult Validate(TapeDraft draft, int count)
        {
            if (draft == null || string.IsNullOrWhiteSpace(draft.ModelIdentifier))
            {
                return TapeOperationResult.Failed(
                    TapeFailure.ModelRequired,
                    "A cassette model is required.");
            }

            if (count < 1)
            {
                return TapeOperationResult.Failed(
                    TapeFailure.InvalidBulkCount,
                    "At least one tape must be added.");
            }

            if (!draft.Packaged)
            {
                if (draft.SideA != null && draft.SideA.IsRecorded &&
                    string.IsNullOrWhiteSpace(draft.SideA.Name))
                {
                    return TapeOperationResult.Failed(
                        TapeFailure.SideNameRequired,
                        "A name is required for side A.");
                }

                if (draft.SideB != null && draft.SideB.IsRecorded &&
                    string.IsNullOrWhiteSpace(draft.SideB.Name))
                {
                    return TapeOperationResult.Failed(
                        TapeFailure.SideNameRequired,
                        "A name is required for side B.");
                }
            }

            return TapeOperationResult.Success(new List<Tape>());
        }

        private static TapeSide NormalizeSide(TapeSide value, bool packaged)
        {
            if (packaged || value == null || !value.IsRecorded)
            {
                return TapeSide.Empty();
            }

            return new TapeSide(
                true,
                Normalize(value.Name),
                value.RecordedAt,
                Normalize(value.DeckName),
                Normalize(value.InputName),
                value.PeakLevel,
                Normalize(value.NoiseReduction),
                value.Hx,
                value.Mpx,
                value.Dubbed,
                Normalize(value.Speed),
                value.Bias,
                value.BiasCalibration,
                Normalize(value.Equalization),
                value.Level,
                value.LevelCalibration,
                Normalize(value.Contents),
                Normalize(value.Artist),
                Normalize(value.Title));
        }

        private static string EncodeLength(decimal value)
        {
            var rounded = Convert.ToInt32(value);
            var digits = Math.Abs(rounded).ToString(CultureInfo.InvariantCulture);
            if (digits.Length > 2)
            {
                return "X" + digits.Substring(1, 1);
            }

            return rounded.ToString("00", CultureInfo.InvariantCulture);
        }

        private static string BuildIdentifier(
            string modelIdentifier,
            int year,
            decimal lengthMinutes,
            string numberCode)
        {
            return modelIdentifier +
                (year % 100).ToString("00", CultureInfo.InvariantCulture) +
                EncodeLength(lengthMinutes) +
                numberCode;
        }

        private static string Normalize(string value)
        {
            return (value ?? string.Empty).Trim();
        }
    }
}
