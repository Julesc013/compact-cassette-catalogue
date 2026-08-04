using System.Collections.Generic;

namespace C3.Catalogue.Tapes
{
    public interface ITapeRepository
    {
        IList<Tape> GetAll();

        Tape FindByShortIdentifier(string identifier);

        bool ModelExists(string identifier);

        int NextNumberForModel(string identifier);

        bool IdentifierExists(string identifier, string shortIdentifier);

        void AddRange(IList<Tape> values);

        void Update(Tape value);

        void Delete(string shortIdentifier);
    }
}
