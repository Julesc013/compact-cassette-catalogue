namespace C3.Catalogue.Brands
{
    public enum BrandFailure
    {
        None = 0,
        NameRequired,
        InvalidCode,
        DuplicateCode,
        NotFound,
        ReferencedByModel,
        StorageFailure
    }

    public sealed class BrandOperationResult
    {
        private BrandOperationResult()
        {
        }

        public bool IsSuccess { get; set; }

        public Brand Brand { get; set; }

        public BrandFailure Failure { get; set; }

        public string Message { get; set; }

        public static BrandOperationResult Success(Brand value)
        {
            return new BrandOperationResult
            {
                IsSuccess = true,
                Brand = value,
                Failure = BrandFailure.None,
                Message = string.Empty
            };
        }

        public static BrandOperationResult Failed(BrandFailure failure, string message)
        {
            return new BrandOperationResult
            {
                IsSuccess = false,
                Failure = failure,
                Message = message
            };
        }
    }
}
