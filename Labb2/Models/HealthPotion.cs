namespace Labb3_MongoDB.Models;

public class HealthPotion(Position position) : 
    LevelElement(position, '%', ConsoleColor.DarkRed, ElementType.HealthPotion)
{
}