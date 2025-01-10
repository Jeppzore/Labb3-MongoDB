namespace Labb2.Models;

public class Dice(int numberOfDice, int sidesPerDice, int modifier)
{
    private readonly int numberOfDice = numberOfDice;
    private readonly int sidesPerDice = sidesPerDice;
    private readonly int modifier = modifier;
    private readonly Random random = new();

    public int ThrowDice()
    {
        int totalRoll = 0;

        if (numberOfDice > 0) 
        {
            for (int i = 0; i < numberOfDice; i++)
            {
                int diceRoll = random.Next(1, sidesPerDice + 1);
                totalRoll += diceRoll;
            }
        
            totalRoll += modifier;
        }

        return totalRoll;
    }

    public override string ToString()
    {
        return $"{numberOfDice}d{sidesPerDice}+{modifier}";
    }
}