using Labb3_MongoDB.Models;

namespace Labb3_MongoDB;

public static class GameLoopHelper
{
    public static void DisplayControls()
    {
        // Display the user control
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.SetCursorPosition(70, 7);
        Console.WriteLine("Controls:");

        Console.SetCursorPosition(70, 8);
        Console.WriteLine("W: Up");

        Console.SetCursorPosition(70, 9);
        Console.WriteLine("A: Left");

        Console.SetCursorPosition(70, 10);
        Console.WriteLine("S: Down");

        Console.SetCursorPosition(70, 11);
        Console.WriteLine("D: Right");

        Console.SetCursorPosition(70, 13);
        Console.WriteLine("ESC: Restart game without saving");

        Console.SetCursorPosition(70, 14);
        Console.WriteLine("ENTER: Save & Exit");

        Console.ResetColor();
    }

    public static bool IsWinCondition(bool isAllEnemiesDead)
    {
        if (isAllEnemiesDead)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
        
            Console.WriteLine("Y O U  W I N!");
            Thread.Sleep(2000);

            Console.WriteLine("Restarting game...");
            Thread.Sleep(3000);

            Console.ResetColor();
            Console.Clear();

            return true;
        }

        return false;
    }
}