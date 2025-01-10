namespace Labb2.Models;

public class HealthPotion(Position position) : 
    LevelElement(position, '%', ConsoleColor.DarkRed, ElementType.HealthPotion)
{
}