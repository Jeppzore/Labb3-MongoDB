using Labb3_MongoDB.Models;
using System.Diagnostics;

namespace Labb3_MongoDB.MongoDB.Entities
{

    public class GameManager
    {
        LevelElement? levelElement;
        private readonly MongoDbService? _mongoDbService;

        public GameManager(MongoDbService? mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        public void SaveProgress(List<LevelElement> elements)
        {
            _mongoDbService!.SaveElements(elements);
            SaveProgressText();
        }

        public void LoadElements(List<LevelElement> element)
        {
            try
            {
                // Clear the current in-memory elements and populate with saved data
                LevelData._elements.Clear();
                LevelData._elements.AddRange(element);

                // Log or debug to verify loading
                Debug.WriteLine($"Loaded {element.Count} elements from the database.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading elements: {ex.Message}");
                throw;
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Loading save file...");
            Thread.Sleep(1000);


            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Finalizing...");
            Thread.Sleep(2000);
            Console.ResetColor();
            Console.Clear();
        }

        private static void SaveProgressText()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(0, 27);
            Console.WriteLine($"Saving game...".PadRight(Console.BufferWidth));
            Console.ResetColor();
        }
    }
}
