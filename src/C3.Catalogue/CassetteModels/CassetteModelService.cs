using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace C3.Catalogue.CassetteModels
{
    public sealed class CassetteModelService
    {
        private readonly ICassetteModelRepository repository;

        public CassetteModelService(ICassetteModelRepository repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            this.repository = repository;
        }

        public IList<CassetteModel> GetAll()
        {
            return repository.GetAll()
                .OrderBy(value => value.Identifier, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public CassetteModel Find(string identifier)
        {
            return repository.FindByIdentifier(Normalize(identifier));
        }

        public CassetteModelOperationResult Create(
            CassetteModelDraft draft,
            DateTime addedAt)
        {
            var validation = ValidateDraft(draft);
            if (!validation.IsSuccess)
            {
                return validation;
            }

            var brandCode = Normalize(draft.BrandCode).ToUpperInvariant();
            if (!repository.BrandExists(brandCode))
            {
                return CassetteModelOperationResult.Failed(
                    CassetteModelFailure.BrandNotFound,
                    "The selected brand no longer exists.");
            }

            var code = Normalize(draft.Code).ToUpperInvariant();
            var identifier = brandCode +
                draft.TypeNumber.ToString(CultureInfo.InvariantCulture) + code;
            if (repository.IdentifierExists(identifier))
            {
                return CassetteModelOperationResult.Failed(
                    CassetteModelFailure.DuplicateIdentifier,
                    "Cassette model identifier '" + identifier + "' is already in use.");
            }

            var value = new CassetteModel(
                brandCode,
                draft.TypeNumber,
                draft.ModelName.Trim(),
                code,
                identifier,
                Normalize(draft.DisplayName),
                0,
                addedAt,
                Normalize(draft.Notes));
            try
            {
                repository.Add(value);
                return CassetteModelOperationResult.Success(value);
            }
            catch (Exception exception)
            {
                return CassetteModelOperationResult.Failed(
                    CassetteModelFailure.StorageFailure,
                    exception.Message);
            }
        }

        public CassetteModelOperationResult Update(
            string identifier,
            CassetteModelDraft draft)
        {
            var normalizedIdentifier = Normalize(identifier);
            var existing = repository.FindByIdentifier(normalizedIdentifier);
            if (existing == null)
            {
                return CassetteModelOperationResult.Failed(
                    CassetteModelFailure.NotFound,
                    "The selected cassette model no longer exists.");
            }

            if (draft == null || string.IsNullOrWhiteSpace(draft.ModelName))
            {
                return CassetteModelOperationResult.Failed(
                    CassetteModelFailure.ModelNameRequired,
                    "Model name is required.");
            }

            var updated = new CassetteModel(
                existing.BrandCode,
                existing.TypeNumber,
                draft.ModelName.Trim(),
                existing.Code,
                existing.Identifier,
                Normalize(draft.DisplayName),
                existing.TapeCount,
                existing.AddedAt,
                Normalize(draft.Notes));
            try
            {
                repository.Update(updated);
                return CassetteModelOperationResult.Success(updated);
            }
            catch (Exception exception)
            {
                return CassetteModelOperationResult.Failed(
                    CassetteModelFailure.StorageFailure,
                    exception.Message);
            }
        }

        public CassetteModelOperationResult Delete(string identifier)
        {
            var normalizedIdentifier = Normalize(identifier);
            var existing = repository.FindByIdentifier(normalizedIdentifier);
            if (existing == null)
            {
                return CassetteModelOperationResult.Failed(
                    CassetteModelFailure.NotFound,
                    "The selected cassette model no longer exists.");
            }

            if (repository.IsReferencedByTape(normalizedIdentifier))
            {
                return CassetteModelOperationResult.Failed(
                    CassetteModelFailure.ReferencedByTape,
                    "Cassette model '" + normalizedIdentifier +
                    "' cannot be deleted while tapes use it.");
            }

            try
            {
                repository.Delete(normalizedIdentifier);
                return CassetteModelOperationResult.Success(existing);
            }
            catch (Exception exception)
            {
                return CassetteModelOperationResult.Failed(
                    CassetteModelFailure.StorageFailure,
                    exception.Message);
            }
        }

        private static CassetteModelOperationResult ValidateDraft(CassetteModelDraft draft)
        {
            if (draft == null || string.IsNullOrWhiteSpace(draft.BrandCode))
            {
                return CassetteModelOperationResult.Failed(
                    CassetteModelFailure.BrandRequired,
                    "A brand is required.");
            }

            if (draft.TypeNumber < 1 || draft.TypeNumber > 4)
            {
                return CassetteModelOperationResult.Failed(
                    CassetteModelFailure.InvalidType,
                    "Cassette type must be between I and IV.");
            }

            if (string.IsNullOrWhiteSpace(draft.ModelName))
            {
                return CassetteModelOperationResult.Failed(
                    CassetteModelFailure.ModelNameRequired,
                    "Model name is required.");
            }

            var code = Normalize(draft.Code).ToUpperInvariant();
            if (code.Length != 2 || code.Any(character => character < 'A' || character > 'Z'))
            {
                return CassetteModelOperationResult.Failed(
                    CassetteModelFailure.InvalidCode,
                    "Model code must contain exactly two letters (A-Z).");
            }

            return CassetteModelOperationResult.Success(null);
        }

        private static string Normalize(string value)
        {
            return (value ?? string.Empty).Trim();
        }
    }
}
