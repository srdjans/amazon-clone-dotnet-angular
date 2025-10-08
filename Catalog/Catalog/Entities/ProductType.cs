using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.Entities
{
    public class ProductType : BaseEntity
    {
        [BsonElement("name")]
        public string Name { get; set; }
    }
}