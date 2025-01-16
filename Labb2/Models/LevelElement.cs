using MongoDB.Bson.Serialization.Attributes;

namespace Labb3_MongoDB.Models;
[BsonIgnoreExtraElements]
[BsonDiscriminator(Required = true)]
[BsonKnownTypes(typeof(Player), typeof(Rat), typeof(Snake), typeof(Wall), typeof(HealthPotion))]

public abstract class LevelElement(Position position, char icon, ConsoleColor consoleColor, ElementType type)
{
    public string? Id { get; set; } = Guid.NewGuid().ToString();

    public bool IsVisible { get; set; } = false;

    public bool IsDiscovered { get; set; } = false;

    public Position Position { get; set; } = position;

    public ElementType Type { get; set; } = type;

    public char Icon { get; set; } = icon;

    public ConsoleColor CharacterColor { get; set; } = consoleColor;

    public void Draw()
    {
        Console.SetCursorPosition(Position.X, Position.Y + 5);
        Console.ForegroundColor = CharacterColor;
        Console.WriteLine(Icon);
        Console.ResetColor();
    }

    public void Clear()
    {
        Console.SetCursorPosition(Position.X, Position.Y + 5);
        Console.WriteLine(' ');
    }

    protected static bool IsMoveAllowed(int newX, int newY)
    {
        foreach (var element in LevelData.Elements)
        {
            if (element.Position.X == newX && element.Position.Y == newY)
            {
                return false; // Collides with another element
            }
        }

        return true; // No Collision with another element
    }
}

