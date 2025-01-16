using Labb3_MongoDB.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Labb3_MongoDB.MongoDB.Entities
{
    [BsonIgnoreExtraElements]
    public class Player
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)] // Stores Guid as a string in MongoDB
        public string? Id { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public string? Name { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public int VisionRange { get; set; }
        public int AttackPower { get; set; }
        public int DefenseStrength { get; set; }
        public int Turns { get; set; }
        public bool IsMongo { get; set; } = true;
        public Position? CurrentLocation { get; set; }
        public DateTime LastSaveTime { get; set; }

    }
}
