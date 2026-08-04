namespace C3.Catalogue.CassetteModels
{
    public enum CassetteModelFailure
    {
        None = 0,
        BrandRequired,
        BrandNotFound,
        InvalidType,
        ModelNameRequired,
        InvalidCode,
        DuplicateIdentifier,
        NotFound,
        ReferencedByTape,
        StorageFailure
    }

    public sealed class CassetteModelOperationResult
    {
        private CassetteModelOperationResult()
        {
        }

        public bool IsSuccess { get; set; }

        public CassetteModel Model { get; set; }

        public CassetteModelFailure Failure { get; set; }

        public string Message { get; set; }

        public static CassetteModelOperationResult Success(CassetteModel value)
        {
            return new CassetteModelOperationResult
            {
                IsSuccess = true,
                Model = value,
                Failure = CassetteModelFailure.None,
                Message = string.Empty
            };
        }

        public static CassetteModelOperationResult Failed(
            CassetteModelFailure failure,
            string message)
        {
            return new CassetteModelOperationResult
            {
                IsSuccess = false,
                Failure = failure,
                Message = message
            };
        }
    }
}
