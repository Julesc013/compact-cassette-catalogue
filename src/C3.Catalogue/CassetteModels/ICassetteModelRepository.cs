using System.Collections.Generic;

namespace C3.Catalogue.CassetteModels
{
    public interface ICassetteModelRepository
    {
        IList<CassetteModel> GetAll();

        CassetteModel FindByIdentifier(string identifier);

        bool BrandExists(string code);

        bool IdentifierExists(string identifier);

        bool IsReferencedByTape(string identifier);

        void Add(CassetteModel value);

        void Update(CassetteModel value);

        void Delete(string identifier);
    }
}
