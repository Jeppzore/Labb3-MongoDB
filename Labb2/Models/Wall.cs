namespace Labb3_MongoDB.Models;

public class Wall(Position position) : 
    LevelElement(position, '#', ConsoleColor.White, ElementType.Wall)
{
}
