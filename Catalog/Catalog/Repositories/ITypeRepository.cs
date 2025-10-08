using Catalog.Entities;

namespace Catalog.Repositories
{
    public interface ITypeRepository
    {
        Task<IEnumerable<ProductType>> GetTypesAsync();
        Task<ProductType> GetTypeByIdAsync(string id);
    }
}
