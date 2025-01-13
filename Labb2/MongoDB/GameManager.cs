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

        // TODO: Used only for multiple Player-saves. Delete later if not needed
        public async Task<Player> LoadProgress(string playerId)
        {
            var player = await _mongoDbService!.LoadPlayer(playerId);
            if (player != null)
            {
                
            }
            return player!;
        }

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

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Loaded player '{player.Name}'");
            Thread.Sleep(1000);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine("Stats:");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"MaxHealth: {player.MaxHealth}\nCurrent Health: {player.Health}\nLevel: {player.Level}\nExperience: {player.Experience}\nAttack Power: {player.AttackPower}\nDefense Strength: {player.DefenseStrength}");
            Thread.Sleep(2000);
            Console.ResetColor();
        }
    }
}
