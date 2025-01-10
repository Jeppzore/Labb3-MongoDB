namespace Labb2.Models;

public class Wall(Position position) : 
    LevelElement(position, '#', ConsoleColor.White, ElementType.Wall)
{
}
