using MongoDB.Bson.Serialization.Attributes;

namespace Labb3_MongoDB.Models;

[BsonIgnoreExtraElements]
[BsonDiscriminator("Wall")]
public class Wall(Position position) : 
    LevelElement(position, '#', ConsoleColor.White, ElementType.Wall)
{
}
