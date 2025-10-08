using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.Entities
{
    public class ProductBrand : BaseEntity
    {
        [BsonElement("name")]
        public string Name { get; set; }
    }
}
