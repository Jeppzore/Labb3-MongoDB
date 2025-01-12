using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
