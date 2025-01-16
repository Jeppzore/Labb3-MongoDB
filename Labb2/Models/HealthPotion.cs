using MongoDB.Bson.Serialization.Attributes;

namespace Labb3_MongoDB.Models;

[BsonDiscriminator("HealthPotion")]
public class HealthPotion(Position position) : 
    LevelElement(position, '%', ConsoleColor.DarkRed, ElementType.HealthPotion)
{
}