using System.Collections.Generic;

namespace C3.Catalogue.Tapes
{
    public enum TapeFailure
    {
        None = 0,
        ModelRequired,
        ModelNotFound,
        InvalidBulkCount,
        IdentifierCapacityExceeded,
        SideNameRequired,
        DuplicateIdentifier,
        NotFound,
        StorageFailure
    }

    public sealed class TapeOperationResult
    {
        private TapeOperationResult()
        {
            Tapes = new List<Tape>();
        }

        public bool IsSuccess { get; set; }

        public IList<Tape> Tapes { get; set; }

        public TapeFailure Failure { get; set; }

        public string Message { get; set; }

        public static TapeOperationResult Success(IList<Tape> values)
        {
            return new TapeOperationResult
            {
                IsSuccess = true,
                Tapes = values,
                Failure = TapeFailure.None,
                Message = string.Empty
            };
        }

        public static TapeOperationResult Failed(TapeFailure failure, string message)
        {
            return new TapeOperationResult
            {
                IsSuccess = false,
                Failure = failure,
                Message = message
            };
        }
    }
}
