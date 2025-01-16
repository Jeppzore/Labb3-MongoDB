using Labb3_MongoDB.MongoDB;
using MongoDB.Bson.Serialization.Attributes;
using System.Xml.Linq;

namespace Labb3_MongoDB.Models;

[BsonIgnoreExtraElements]
[BsonDiscriminator("Player")]
public class Player : LevelElement
{
    private readonly MongoDbService? _mongoDbService;

    public Dice? DefenceDice { get; set; }
    public int VisionRange { get; set; }
    public int MaxHealth { get; set; }
    public int Health { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public string Name { get; set; }
    public bool IsAlive { get; set; } = true;
    public bool IsMongo { get; set; } = false;
    public int AttackPower { get; set; } = 0;
    public int DefenseStrength { get; set; } = 0;
    public int Turns { get; set; } = 0;

    public Player(Position position) : 
        base(position, '@', ConsoleColor.Yellow, ElementType.Player)
    {
        Health = 100;
        MaxHealth = Health;
        Name = "Player";
        Level = 1;
        Experience = 0;
        VisionRange = 5;
        AttackPower = 6;
        DefenseStrength = 6;
    }

    public void SetName(string name)
    {
        Name = name;
    }

    public bool IsWithinVisionRange(LevelElement element)
    {
        int distanceX = Math.Abs(Position.X - element.Position.X);
        int distanceY = Math.Abs(Position.Y - element.Position.Y);
        return distanceX <= VisionRange && distanceY <= VisionRange;
    }

    public void TakeDamage(int damage)
    {
        if (damage > 0)
        {
            Health -= damage;
        }
        else
        {
            return;
        }

        if (Health <= 0)
        {
            Health = 0;
            IsAlive = false;
            _mongoDbService!.DeleteElements(Id!);

        }
    }

    public void AddExperience(int experience)
    {
        Experience += experience;
    }

    public void PlayerLevelCheck()
    {
        if ((Level == 1 && Experience >= 20) ||
            (Level == 2 && Experience >= 60))
        {
            LevelUp();
        }
    }

    public void MoveToPosition(Position position)
    {
        LevelElement? element = LevelData.Elements.FirstOrDefault(elem => elem.Position.X == position.X && elem.Position.Y == position.Y);

        if (element is null)
        {
            Position = new Position(position.X, position.Y);
            return;
        }
        else if (element is Enemy enemy)
        {
            AttackEnemy(enemy);
        }
        else if (element is HealthPotion potion)
        {
            var mongoDbService = new MongoDbService();
            Position = new Position(position.X, position.Y);

            RestoreHealth();

            element.Clear();
            mongoDbService!.DeleteElements(element.Id!);
            LevelData.Elements.Remove(element);

            return;
        }
    }

    public void AttackEnemy(Enemy enemy)
    {
        Dice playerAttackDice = new(1, AttackPower, 2 * Level);
        Dice enemyDefenseDice = enemy.Type switch
        {
            ElementType.Rat => new(1, enemy.DefenseStrength, 0),
            ElementType.Snake => new(1, enemy.DefenseStrength, 1),
            _ => new(0, 0, 0),
        };

        DealDamageToEnemy(enemy, playerAttackDice, enemyDefenseDice);
    }

    private void LevelUp()
    {
        Level++;
        Health = MaxHealth * Level;
        MaxHealth = Health;

        Console.SetCursorPosition(0, 26);
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"Congratulations! You advanced to level: {Level}. You gain full health ({Health}). Attack modifier increased".PadRight(Console.BufferWidth));
        Console.ResetColor();
    }

    private void RestoreHealth()
    {
        Health = MaxHealth;

        Console.ForegroundColor = ConsoleColor.Green;
        Console.SetCursorPosition(0, 26);
        Console.WriteLine($"Your health was fully restored to {MaxHealth} health.".PadRight(Console.BufferWidth));
        Console.ResetColor();
        Clear();
    }

    private void DealDamageToEnemy(Enemy enemy, Dice playerAttackDice, Dice enemyDefenceDice)
    {
        var playerDamage = playerAttackDice.ThrowDice();
        int enemyDefence = enemyDefenceDice.ThrowDice();

        enemy.TakeDamage(playerDamage - enemyDefence, this);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.SetCursorPosition(0, 2);
        Console.WriteLine($"{Name} attacked {enemy.Name} with: {playerDamage} ({playerAttackDice}) damage. {enemy.Name} defence: {enemyDefence} ({enemyDefenceDice}) {enemy.Name} took {playerDamage - enemyDefence} damage ({enemy.Health} health left).".PadRight(Console.BufferWidth));
    }
}