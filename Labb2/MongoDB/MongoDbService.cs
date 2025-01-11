using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Labb3_MongoDB.MongoDB
{
    public class MongoDbService
    {
        private readonly IMongoCollection<Players> ?_players;      

        // TODO: Replace hardcoded connectionString with hidden code ex. appsettings.json
        public MongoDbService(string connectionString = "mongodb://localhost:27017/", string databaseName = "JesperJohansson")
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _players = database.GetCollection<Players>("Players");
        }

        public async Task SavePlayers(Players players)
        {
            try
            {
                // Update if current player exists, otherwise insert
                await _players.ReplaceOneAsync(
                    p => p.Id == players.Id,
                    players,
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

        public async Task<Players> LoadPlayers(string playersId)
        {
            return await _players.Find(p => p.Id == playersId).FirstOrDefaultAsync();
        }

        public async Task DeletePlayers(string playersId)
        {
            await _players.DeleteOneAsync(players => players.Id == playersId);
        }
    }
}
