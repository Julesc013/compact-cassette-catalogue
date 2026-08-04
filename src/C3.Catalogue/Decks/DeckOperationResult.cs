namespace C3.Catalogue.Decks
{
    public enum DeckFailure
    {
        None = 0,
        ManufacturerRequired,
        ModelRequired,
        TapeTypeRequired,
        SpeedRequired,
        DuplicateName,
        NotFound,
        ReferencedByTape,
        StorageFailure
    }

    public sealed class DeckOperationResult
    {
        private DeckOperationResult()
        {
        }

        public bool IsSuccess { get; set; }

        public Deck Deck { get; set; }

        public DeckFailure Failure { get; set; }

        public string Message { get; set; }

        public static DeckOperationResult Success(Deck value)
        {
            return new DeckOperationResult
            {
                IsSuccess = true,
                Deck = value,
                Failure = DeckFailure.None,
                Message = string.Empty
            };
        }

        public static DeckOperationResult Failed(DeckFailure failure, string message)
        {
            return new DeckOperationResult
            {
                IsSuccess = false,
                Failure = failure,
                Message = message
            };
        }
    }
}
