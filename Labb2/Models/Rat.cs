namespace Labb3_MongoDB.Models;

public class Rat : Enemy
{
    public Rat(Position position) : 
        base(position, icon: 'r', ConsoleColor.Red, ElementType.Rat, experience: 5)
    {
        Health = 10;
        Name = "rat";
        AttackPower = 6;
        DefenseStrength = 3;
    }

    public override void Update(Player player)
    {
        ClearOldPosition();

        MoveRat(player);

        if (player.IsWithinVisionRange(this))
        {      
            DrawNewPosition();
        }
    }

    public void MoveRat(Player player)
    {
        Random random = new();
        int ratMove = random.Next(4);

        Position newRatPosition = new(Position);

        switch (ratMove)
        {
            case 0: // Left
                newRatPosition.X--;
                break;

            case 1: // Right
                newRatPosition.X++;
                break;

            case 2: // Up
                newRatPosition.Y--;
                break;

            case 3: // Down
                newRatPosition.Y++;
                break;
        }

        var isPlayerEncounter = LevelData.Elements.FirstOrDefault(player => player.Position.X == newRatPosition.X && player.Position.Y == newRatPosition.Y) is Player;
        if (isPlayerEncounter)
        {
            HandlePlayerEncounter(player);
        }

        if (IsMoveAllowed(newRatPosition.X, newRatPosition.Y))
        {
            Position = newRatPosition;
        }
    }

    private void HandlePlayerEncounter(Player player)
    {
        Dice ratAttackDice = new(1, AttackPower, 0);
        int ratDamage = ratAttackDice.ThrowDice();

        // Do damage to player and let player retaliate if still alive
        player.TakeDamage(ratDamage);

        if (player.IsAlive)
        {
            player.AttackEnemy(this);
        }
    }
}