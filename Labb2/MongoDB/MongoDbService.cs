using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly IMongoCollection<Player> ?_playerCollection;      

        // TODO: Replace hardcoded connectionString with hidden code ex. appsettings.json
        public MongoDbService(string connectionString = "mongodb://localhost:27017/", string databaseName = "JesperJohansson")
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _playerCollection = database.GetCollection<Player>("Players");
        }

        public void SavePlayer(Player player)
        {
            try
            {
                // Update if current player exists, otherwise insert
                _playerCollection.ReplaceOne(
                    p => p.Id == player.Id,
                    player,
                    new ReplaceOptions { IsUpsert = true }
                    );
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving player: {ex.Message}");
                throw;

            }
        }

        // Can be used to check for a specific playerId for multiple players
        public async Task<Player> LoadPlayer(string playerId)
        {
            return await _playerCollection.Find(p => p.Id == playerId).FirstOrDefaultAsync();
        }

        public Player? GetExistingPlayer()
        {
            try
            {
                if (_playerCollection == null)
                {
                    Debug.WriteLine("Error: _playerCollection is not initialized.");
                    throw new InvalidOperationException("The _playerCollection-collection is not initialized.");
                }
                Debug.WriteLine("_playerCollection is not null. Starting query.");
                var result = _playerCollection.Find(_ => true).FirstOrDefault();
                Debug.WriteLine(result != null
                    ? $"Player found: {result.Name}"
                    : "No player found in the database.");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error querying MongoDB: {ex.Message}");
                throw; // Re-throw the exception to identify it
            }
        }

        public async Task DeletePlayer(string playerId)
        {
            await _playerCollection.DeleteOneAsync(players => players.Id == playerId);
        }
    }
}
