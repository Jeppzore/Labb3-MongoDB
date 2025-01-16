using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Labb3_MongoDB.MongoDB.Entities
{
    [BsonIgnoreExtraElements]
    public class HealthPotion
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)] // Stores Guid as a string in MongoDB
        public string? Id { get; set; }
        public bool IsUsed { get; set; }
    }
}
