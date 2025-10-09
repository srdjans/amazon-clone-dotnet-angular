using Catalog.Entities;
using MongoDB.Driver;
using System.Text.Json;

namespace Catalog.Data
{
    public class DatabaseSeeder
    {
        public static async Task SeedAsync(IConfiguration config)
        {
            var client = new MongoClient(config.GetValue<string>("DatabaseSettings:ConnectionString"));
            var db = client.GetDatabase(config.GetValue<string>("DatabaseSettings:DatabaseName"));
            var products = db.GetCollection<Product>(config.GetValue<string>("DatabaseSettings:ProductsCollectionName"));
            var brands = db.GetCollection<ProductBrand>(config.GetValue<string>("DatabaseSettings:BrandsCollectionName"));
            var types = db.GetCollection<ProductType>(config.GetValue<string>("DatabaseSettings:TypesCollectionName"));

            var seedBasePath = Path.Combine("Data", "SeedData");

            // Seed brands
            var brandList = new List<ProductBrand>();
            if (await brands.CountDocumentsAsync(_ => true) == 0)
            {
                var brandData = await File.ReadAllTextAsync(Path.Combine(seedBasePath, "brands.json"));
                brandList = JsonSerializer.Deserialize<List<ProductBrand>>(brandData);
                await brands.InsertManyAsync(brandList);
            } 
            else
            {
                brandList = await brands.Find(_ => true).ToListAsync();
            }

            // Seed types
            var typeList = new List<ProductType>();
            if (await types.CountDocumentsAsync(_ => true) == 0)
            {
                var typeData = await File.ReadAllTextAsync(Path.Combine(seedBasePath, "types.json"));
                typeList = JsonSerializer.Deserialize<List<ProductType>>(typeData);
                await types.InsertManyAsync(typeList);
            }
            else
            {
                typeList = await types.Find(_ => true).ToListAsync();
            }

            // Seed products
            if (await products.CountDocumentsAsync(_ => true) == 0)
            {
                var productData = await File.ReadAllTextAsync(Path.Combine(seedBasePath, "products.json"));
                var productList = JsonSerializer.Deserialize<List<Product>>(productData);

                foreach (var product in productList)
                {
                    // Reset id to let Mongo generate one
                    product.Id = null;

                    // Default CreatedAt if not set
                    if (product.CreatedAt == default)
                    {
                        product.CreatedAt = DateTime.UtcNow;
                    }

                    await products.InsertManyAsync(productList);
                }
            }
        }
    }
}
