using System.Collections.Generic;

namespace C3.Catalogue.Brands
{
    public interface IBrandRepository
    {
        IList<Brand> GetAll();

        Brand FindByCode(string code);

        bool IsCodeInUse(string code);

        bool IsReferencedByModel(string code);

        void Add(Brand value);

        void Update(Brand value);

        void Delete(string code);
    }
}
