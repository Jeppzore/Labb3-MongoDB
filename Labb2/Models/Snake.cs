using MongoDB.Bson.Serialization.Attributes;

namespace Labb3_MongoDB.Models;

[BsonIgnoreExtraElements]
[BsonDiscriminator("Snake")]
public class Snake : Enemy
{
    public bool IsMongo { get; set; } = false;
    //public string ?Id { get; set; }

    public Snake(Position position) : 
        base(position, icon: 's', ConsoleColor.Green, ElementType.Snake, experience: 10)
    {
        Health = 20;
        Name = "snake";
        AttackPower = 3;
        DefenseStrength = 8;
    }

    public override void Update(Player player)
    {
        ClearOldPosition();

        HandlePlayer(player);
    }

    private void HandlePlayer(Player player)
    {
        if (player is null)
        {
            return;
        }

        if (player.IsWithinVisionRange(this))
        {
            DrawNewPosition();
            ClearOldPosition();
        }

        int distanceToPlayerX = Math.Abs(player.Position.X - Position.X);
        int distanceToPlayerY = Math.Abs(player.Position.Y - Position.Y);
        if (distanceToPlayerX > 2 || distanceToPlayerY > 2)
        {
            return;
        }
        else
        {
            MoveSnakeFromPlayer(player);
        }
    }

    private void MoveSnakeFromPlayer(Player player)
    {
        Position newSnakePosition = new(Position);

        // Move Snake away from player X
        if (player.Position.X < Position.X)
        {
            newSnakePosition.X = Position.X + 1;
        }
        else if (player.Position.X > Position.X)
        {
            newSnakePosition.X = Position.X - 1;
        }

        // Move Snake away from player Y
        if (player.Position.Y < Position.Y)
        {
            newSnakePosition.Y = Position.Y + 1;
        }
        else if (player.Position.Y > Position.Y)
        {
            newSnakePosition.Y = Position.Y - 1;
        }

        if (IsMoveAllowed(newSnakePosition.X, newSnakePosition.Y))
        {
            Position = newSnakePosition;
        }
    }
}