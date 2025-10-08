using Catalog.Entities;

namespace Catalog.Repositories
{
    public interface IBrandRepository
    {
        Task<IEnumerable<ProductBrand>> GetBrandsAsync();
        Task<ProductBrand> GetBrandByIdAsync(string id);
    }
}
