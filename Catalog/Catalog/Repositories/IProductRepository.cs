using Catalog.Entities;
using Catalog.Specifications;

namespace Catalog.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<Pagination<Product>> GetProductsAsync(CatalogSpecParams specParams);
        Task<IEnumerable<Product>> GetProductsByNameAsync(string name);
        Task<IEnumerable<Product>> GetProductsByBrandAsync(string name);
        Task<Product> GetProductByIdAsync(string id);
        Task<Product> CreateProductAsync(Product product);
        Task<bool> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(string id);
        Task<ProductBrand> GetBrandByBrandIdAsync(string brandId);
        Task<ProductType> GetTypeByTypeIdAsync(string typeId);
    }
}
