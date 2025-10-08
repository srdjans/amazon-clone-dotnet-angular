using Catalog.Entities;
using MongoDB.Driver;

namespace Catalog.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly IMongoCollection<ProductBrand> _brands;
        public BrandRepository(IConfiguration config)
        {
            var client = new MongoClient(config.GetValue<string>("DatabaseSettings:ConnectionString"));
            var db = client.GetDatabase(config.GetValue<string>("DatabaseSettings:DatabaseName"));
            _brands = db.GetCollection<ProductBrand>(config.GetValue<string>("DatabaseSettings:BrandsCollectionName"));
        }

        public async Task<ProductBrand> GetBrandByIdAsync(string id)
        {
            return await _brands.Find(b => b.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ProductBrand>> GetBrandsAsync()
        {
            return await _brands.Find(b => true).ToListAsync();
        }
    }
}
