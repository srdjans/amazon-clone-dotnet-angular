using Catalog.Entities;
using MongoDB.Driver;

namespace Catalog.Repositories
{
    public class TypeRepository : ITypeRepository
    {
        private readonly IMongoCollection<ProductType> _types;

        public TypeRepository(IConfiguration config)
        {
            var client = new MongoClient(config.GetValue<string>("DatabaseSettings:ConnectionString"));
            var db = client.GetDatabase(config.GetValue<string>("DatabaseSettings:DatabaseName"));
            _types = db.GetCollection<ProductType>(config.GetValue<string>("DatabaseSettings:TypesCollectionName"));
        }

        public async Task<IEnumerable<ProductType>> GetTypesAsync()
        {
            return await _types.Find(t => true).ToListAsync();
        }

        public async Task<ProductType> GetTypeByIdAsync(string id)
        {
            return await _types.Find(t => t.Id == id).FirstOrDefaultAsync();
        }
    }
}
