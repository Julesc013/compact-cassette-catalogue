using System;

namespace C3.Catalogue.Decks
{
    public sealed class Deck
    {
        public Deck(string name, DateTime addedAt, DeckDetails details)
        {
            if (details == null)
            {
                throw new ArgumentNullException(nameof(details));
            }

            Name = name;
            AddedAt = addedAt;
            Details = details;
        }

        public string Name { get; }

        public DateTime AddedAt { get; }

        public DeckDetails Details { get; }
    }
}
