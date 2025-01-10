using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Labb3_MongoDB.MongoDB
{
    public class MongoDbService
    {
        private readonly IMongoCollection<Players> ?_players;      

        public MongoDbService(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _players = database.GetCollection<Players>("Players");
        }

        public MongoDbService(IMongoDatabase database)
        {
            _players = database.GetCollection<Players>("Players");
        }

        public async Task SavePlayers(Players players)
        {
            // Update if current player exists, otherwise insert
            await _players.ReplaceOneAsync(
                p => p.Id == players.Id,
                players,
                new ReplaceOptions { IsUpsert = true });
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
