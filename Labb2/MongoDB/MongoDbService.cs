using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Labb3_MongoDB.MongoDB.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Labb3_MongoDB.MongoDB
{
    public class MongoDbService
    {
        private readonly IMongoCollection<Player> ?_player;      

        // TODO: Replace hardcoded connectionString with hidden code ex. appsettings.json
        public MongoDbService(string connectionString = "mongodb://localhost:27017/", string databaseName = "JesperJohansson")
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _player = database.GetCollection<Player>("Players");
        }

        public async Task SavePlayer(Player player)
        {
            try
            {
                // Update if current player exists, otherwise insert
                await _player.ReplaceOneAsync(
                    p => p.Id == player.Id,
                    player,
                    new ReplaceOptions { IsUpsert = true });
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error saving player: {ex.Message}");
                Console.ResetColor();
                //Thread.Sleep(10000);
                throw;

            }
        }

        public async Task<Player> LoadPlayer(string playerId)
        {
            return await _player.Find(p => p.Id == playerId).FirstOrDefaultAsync();
        }

        public async Task DeletePlayer(string playerId)
        {
            await _player.DeleteOneAsync(players => players.Id == playerId);
        }
    }
}
