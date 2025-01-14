using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Labb3_MongoDB.Models;

namespace Labb3_MongoDB.MongoDB.Entities
{
    [BsonIgnoreExtraElements]
    public class Rat
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)] // Stores Guid as a string in MongoDB
        public string? Id { get; set; }
        public int Health { get; set; }
        public string? Name { get; set; }
        public int AttackPower { get; set; }
        public int DefenseStrength { get; set; }
        public bool IsMongo { get; set; } = true;
        public Position? CurrentLocation { get; set; }
    }
}
