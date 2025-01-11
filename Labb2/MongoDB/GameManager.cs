using Elasticsearch.Net.Specification.AsyncSearchApi;
using Labb3_MongoDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb3_MongoDB.MongoDB
{

    public class GameManager
    {
        LevelElement ?levelElement;
        private readonly MongoDbService ?_mongoDbService;

        public GameManager(MongoDbService? mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        public async Task SaveProgress(Players players)
        {
            await _mongoDbService!.SavePlayers(players);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(0, 27);
            Console.WriteLine($"Game progress saved!...".PadRight(Console.BufferWidth));
            Console.ResetColor();

            await Task.Delay(3000);
            Console.SetCursorPosition(0, 27);
            Console.WriteLine(" ".PadRight(Console.BufferWidth));
        }

        public async Task<Players> LoadProgress(string playersId)
        {
            var players = await _mongoDbService!.LoadPlayers(playersId);
            if (players != null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.SetCursorPosition(0, 27);
                Console.WriteLine($"Player progress loaded!".PadRight(Console.BufferWidth));
                Console.ResetColor();

                Console.SetCursorPosition(0, 27);
                Console.WriteLine(" ".PadRight(Console.BufferWidth));
            }
            return players!;
        }
    }
}
