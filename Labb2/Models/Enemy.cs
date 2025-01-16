using Labb3_MongoDB.MongoDB;
using MongoDB.Bson.Serialization.Attributes;

namespace Labb3_MongoDB.Models;


[BsonDiscriminator("Enemy")]
public abstract class Enemy : LevelElement
{
    public string? Name { get; set; }

    public int Health { get; set; } = 0;

    public int AttackPower { get; set; } = 0;

    public int DefenseStrength { get; set; } = 0;

    public bool IsDead { get; set; } = false;

    public int Experience { get; set; } = 0;

    protected Enemy(Position position, char icon, ConsoleColor consoleColor, ElementType type, int experience) :
        base(position, icon, consoleColor, type)
    {
        IsVisible = false;
        Experience = experience;
    }

    public abstract void Update(Player player);

    public void TakeDamage(int damage, Player player)
    {
        if (damage > 0)
        {
            Health -= damage;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(0, 3);
            Console.WriteLine($"{player.Name} blocked the attack from {Name}! {player.Name} health: {player.Health}".PadRight(Console.BufferWidth));
            Console.ResetColor();
            return;
        }

        if (Health <= 0)
        {
            Health = 0;
            IsDead = true;

            var mongoDbService = new MongoDbService();
            mongoDbService!.DeleteElements(Id!);

            player.AddExperience(Experience);

            PrintEnemyDeathMessage(Experience, player.Experience);

            return;
        }
        else
        {
            RetaliationAttackOnPlayer(player);
        }
    }

    public void RetaliationAttackOnPlayer(Player player)
    {
        Dice playerDefenceDice = new(1, player.DefenseStrength, 0);
        int playerDefence = playerDefenceDice.ThrowDice();

        int enemyDamage;
        Dice enemyAttackDice = Type switch
        {
            ElementType.Rat => new(1, AttackPower, 1),
            ElementType.Snake => new(3, AttackPower, 1),
            _ => new(0, 0, 0),
        };

        enemyDamage = enemyAttackDice.ThrowDice();
        player.TakeDamage(enemyDamage - playerDefence);

        Console.ForegroundColor = ConsoleColor.Red;
        Console.SetCursorPosition(0, 3);
        Console.WriteLine($"{Name} attacked {player.Name} with: {enemyDamage} ({enemyAttackDice}) damage. {player.Name} defence: {playerDefence} ({playerDefenceDice}) {player.Name} took {enemyDamage - playerDefence} damage ({player.Health} health left).".PadRight(Console.BufferWidth));
        Console.ResetColor();
    }

    private static void PrintEnemyDeathMessage(int receivedExperience, int currentPlayerExperience)
    {
        Console.SetCursorPosition(0, 24);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"You received {receivedExperience} experience points".PadRight(Console.BufferWidth));
        Console.SetCursorPosition(0, 25);
        Console.WriteLine($"Current experience: {currentPlayerExperience}".PadRight(Console.BufferWidth));
        Console.ResetColor();
    }

    protected void ClearOldPosition()
    {
        Console.SetCursorPosition(Position.X, Position.Y + 5);
        Console.Write(' ');
    }

    protected void DrawNewPosition()
    {
        Console.SetCursorPosition(Position.X, Position.Y + 5);
        Console.ForegroundColor = CharacterColor;
        Console.Write(Icon);
        Console.ResetColor();
    }
}