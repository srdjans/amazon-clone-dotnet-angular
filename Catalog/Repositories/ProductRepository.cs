using Catalog.Entities;
using Catalog.Specifications;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace Catalog.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoCollection<Product> _products;
        private readonly IMongoCollection<ProductBrand> _brands;
        private readonly IMongoCollection<ProductType> _types;

        public ProductRepository(IConfiguration config)
        {
            var client = new MongoClient(config.GetValue<string>("DatabaseSettings:ConnectionString"));
            var db = client.GetDatabase(config.GetValue<string>("DatabaseSettings:DatabaseName"));
            _products = db.GetCollection<Product>(config.GetValue<string>("DatabaseSettings:ProductsCollectionName"));
            _brands = db.GetCollection<ProductBrand>(config.GetValue<string>("DatabaseSettings:BrandsCollectionName"));
            _types = db.GetCollection<ProductType>(config.GetValue<string>("DatabaseSettings:TypesCollectionName"));
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _products.Find(p => true).ToListAsync();
        }

        public async Task<Pagination<Product>> GetProductsAsync(CatalogSpecParams specParams)
        {
            var builder = Builders<Product>.Filter;
            var filter = builder.Empty;

            if (!string.IsNullOrEmpty(specParams.Search))
            {
                filter &= builder.Text(specParams.Search);
            }

            if (!string.IsNullOrEmpty(specParams.BrandId))
            {
                filter &= builder.Eq(p => p.Brand.Id, specParams.BrandId);
            }

            if (!string.IsNullOrEmpty(specParams.TypeId))
            {
                filter &= builder.Eq(p => p.Type.Id, specParams.TypeId);
            }

            var totalItems = await _products.CountDocumentsAsync(filter);
            var data = await ApplyDataFilters(specParams, filter);

            return new Pagination<Product>(specParams.PageIndex, specParams.PageSize, (int)totalItems, data);
        } 

        public async Task<Product> GetProductByIdAsync(string id)
        {
            return await _products.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            await _products.InsertOneAsync(product);
            return product;
        }

        public async Task<bool> DeleteProductAsync(string id)
        {
            var deletedProduct = await _products.DeleteOneAsync(p => p.Id == id);
            return deletedProduct.IsAcknowledged && deletedProduct.DeletedCount > 0;
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            var updatedProduct = await _products.ReplaceOneAsync(p => p.Id == product.Id, product);
            return updatedProduct.IsAcknowledged && updatedProduct.ModifiedCount > 0;
        }


        public async Task<ProductBrand> GetBrandByIdAsync(string brandId)
        {
            return await _brands.Find(b => b.Id == brandId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByBrandAsync(string brandName)
        {
            return await _products.Find(p => p.Brand.Name.Equals(brandName, StringComparison.OrdinalIgnoreCase)).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByNameAsync(string name)
        {
            var escapedName = Regex.Escape(name);
            var filter = Builders<Product>.Filter.Regex(p => p.Name, new BsonRegularExpression($".*{escapedName}*", "i"));

            return await _products.Find(filter).ToListAsync();
        }

        public async Task<ProductType> GetTypeByIdAsync(string typeId)
        {
            return await _types.Find(t => t.Id == typeId).FirstOrDefaultAsync();
        }

        private async Task<IReadOnlyCollection<Product>> ApplyDataFilters(CatalogSpecParams specParams, FilterDefinition<Product> filter)
        {
            var sortDefinition = specParams.Sort switch
            {
                "priceAsc" => Builders<Product>.Sort.Ascending(p => p.Price),
                "priceDesc" => Builders<Product>.Sort.Descending(p => p.Price),
                _ => Builders<Product>.Sort.Ascending(p => p.Name)
            };

            return await _products.Find(filter)
                .Sort(sortDefinition)
                .Skip(specParams.PageSize * (specParams.PageIndex - 1))
                .Limit(specParams.PageSize)
                .ToListAsync();
        }
    }
}
