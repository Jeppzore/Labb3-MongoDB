using Elasticsearch.Net.Specification.AsyncSearchApi;
using Labb3_MongoDB.Models;
using Labb3_MongoDB.MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb3_MongoDB.MongoDB.Entities
{

    public class GameManager
    {
        LevelElement ?levelElement;
        private readonly MongoDbService ?_mongoDbService;

        public GameManager(MongoDbService? mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        public void SaveProgress(Player player)
        {
            _mongoDbService!.SavePlayer(player);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(0, 27);
            Console.WriteLine($"Saving game...".PadRight(Console.BufferWidth));
            Console.ResetColor();
        }

        public async Task<Player> LoadProgress(string playerId)
        {
            var player = await _mongoDbService!.LoadPlayer(playerId);
            if (player != null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.SetCursorPosition(0, 27);
                Console.WriteLine($"Player progress loaded!".PadRight(Console.BufferWidth));
                Console.ResetColor();

                Console.SetCursorPosition(0, 27);
                Console.WriteLine(" ".PadRight(Console.BufferWidth));
            }
            return player!;
        }

        // added recently
        public void LoadPlayerData(Player player)
        {
            player = new Player
            {
                Id = player.Id,
                Name = player.Name,
                Health = player.Health,
                MaxHealth = player.MaxHealth,
                Level = player.Level,
                Experience = player.Experience,
                VisionRange = player.VisionRange,
                AttackPower = player.AttackPower,
                DefenseStrength = player.DefenseStrength,
                CurrentLocation = player.CurrentLocation // Map position
            };

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Loaded player '{player.Name}'.");
            Console.ResetColor();
        }
    }
}
