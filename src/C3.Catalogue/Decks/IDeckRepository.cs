using System.Collections.Generic;

namespace C3.Catalogue.Decks
{
    public interface IDeckRepository
    {
        IList<Deck> GetAll();

        Deck FindByName(string name);

        bool NameExists(string name);

        bool IsReferencedByTape(string name);

        void Add(Deck value);

        void Update(Deck value);

        void Delete(string name);
    }
}
