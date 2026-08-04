using System;
using System.Collections.Generic;
using System.Linq;

namespace C3.Catalogue.Brands
{
    public sealed class BrandService
    {
        private readonly IBrandRepository repository;

        public BrandService(IBrandRepository repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            this.repository = repository;
        }

        public IList<Brand> GetAll(string notesFilter)
        {
            IEnumerable<Brand> values = repository.GetAll();
            if (!string.IsNullOrWhiteSpace(notesFilter))
            {
                var filter = notesFilter.Trim();
                values = values.Where(value => (value.Notes ?? string.Empty).IndexOf(
                    filter,
                    StringComparison.CurrentCultureIgnoreCase) >= 0);
            }

            return values
                .OrderBy(value => value.Code, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public Brand Find(string code)
        {
            return repository.FindByCode(NormalizeCode(code));
        }

        public BrandOperationResult Create(BrandDraft draft, DateTime addedAt)
        {
            var validation = ValidateDraft(draft);
            if (!validation.IsSuccess)
            {
                return validation;
            }

            var code = NormalizeCode(draft.Code);
            if (repository.IsCodeInUse(code))
            {
                return BrandOperationResult.Failed(
                    BrandFailure.DuplicateCode,
                    "Brand code '" + code + "' is already in use.");
            }

            var value = new Brand(
                draft.Name.Trim(),
                code,
                addedAt,
                NormalizeNotes(draft.Notes));
            try
            {
                repository.Add(value);
                return BrandOperationResult.Success(value);
            }
            catch (Exception exception)
            {
                return BrandOperationResult.Failed(
                    BrandFailure.StorageFailure,
                    exception.Message);
            }
        }

        public BrandOperationResult Update(string code, BrandDraft draft)
        {
            var normalizedCode = NormalizeCode(code);
            var existing = repository.FindByCode(normalizedCode);
            if (existing == null)
            {
                return BrandOperationResult.Failed(
                    BrandFailure.NotFound,
                    "The selected brand no longer exists.");
            }

            var validation = ValidateName(draft);
            if (!validation.IsSuccess)
            {
                return validation;
            }

            var updated = new Brand(
                draft.Name.Trim(),
                normalizedCode,
                existing.AddedAt,
                NormalizeNotes(draft.Notes));
            try
            {
                repository.Update(updated);
                return BrandOperationResult.Success(updated);
            }
            catch (Exception exception)
            {
                return BrandOperationResult.Failed(
                    BrandFailure.StorageFailure,
                    exception.Message);
            }
        }

        public BrandOperationResult Delete(string code)
        {
            var normalizedCode = NormalizeCode(code);
            var existing = repository.FindByCode(normalizedCode);
            if (existing == null)
            {
                return BrandOperationResult.Failed(
                    BrandFailure.NotFound,
                    "The selected brand no longer exists.");
            }

            if (repository.IsReferencedByModel(normalizedCode))
            {
                return BrandOperationResult.Failed(
                    BrandFailure.ReferencedByModel,
                    "Brand '" + normalizedCode +
                    "' cannot be deleted while cassette models use it.");
            }

            try
            {
                repository.Delete(normalizedCode);
                return BrandOperationResult.Success(existing);
            }
            catch (Exception exception)
            {
                return BrandOperationResult.Failed(
                    BrandFailure.StorageFailure,
                    exception.Message);
            }
        }

        private static BrandOperationResult ValidateDraft(BrandDraft draft)
        {
            var nameValidation = ValidateName(draft);
            if (!nameValidation.IsSuccess)
            {
                return nameValidation;
            }

            var code = NormalizeCode(draft.Code);
            if (code.Length != 2 || code.Any(character => character < 'A' || character > 'Z'))
            {
                return BrandOperationResult.Failed(
                    BrandFailure.InvalidCode,
                    "Brand code must contain exactly two letters (A-Z).");
            }

            return BrandOperationResult.Success(null);
        }

        private static BrandOperationResult ValidateName(BrandDraft draft)
        {
            if (draft == null || string.IsNullOrWhiteSpace(draft.Name))
            {
                return BrandOperationResult.Failed(
                    BrandFailure.NameRequired,
                    "Brand name is required.");
            }

            return BrandOperationResult.Success(null);
        }

        private static string NormalizeCode(string value)
        {
            return (value ?? string.Empty).Trim().ToUpperInvariant();
        }

        private static string NormalizeNotes(string value)
        {
            return value ?? string.Empty;
        }
    }
}
