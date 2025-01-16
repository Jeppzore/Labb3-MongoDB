using Labb3_MongoDB.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Diagnostics;

namespace Labb3_MongoDB.MongoDB
{
    public class MongoDbService
    {
        private readonly IMongoCollection<LevelElement> _elementCollection;
        private readonly IMongoDatabase? _database;

        public MongoDbService()
        {

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddUserSecrets<MongoDbService>();

            var configuration = builder.Build();

            string connectionString = configuration["ConnectionString"];
            string databaseName = configuration["DatabaseName"];

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _elementCollection = database.GetCollection<LevelElement>("Elements");
            _database = client.GetDatabase(databaseName);
        }

        public void SaveElements(List<LevelElement> elements)
        {
            try
            {
                foreach (var element in elements)
                {
                    // Ensure each element has a unique Id
                    if (string.IsNullOrEmpty(element.Id))
                    {
                        element.Id = Guid.NewGuid().ToString();
                    }

                    // Update if current player exists, otherwise insert
                    _elementCollection.ReplaceOne(
                    e => e.Id == element.Id,
                    element,
                    new ReplaceOptions { IsUpsert = true }
                    );
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving player: {ex.Message}");
                throw;

            }
        }
        public List<LevelElement> GetElements()
        {
            try
            {
                var result = _elementCollection.Find(_ => true).ToList();
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving elements: {ex.Message}");
                throw;
            }
        }

        public void DeleteElements(string elementId)
        {
            try
            {
                var deleteResult = _elementCollection.DeleteOne(e => e.Id == elementId);

                if (deleteResult.DeletedCount > 0)
                {
                    Debug.WriteLine($"Successfully deleted element with Id: {elementId}");
                }
                else
                {
                    Debug.WriteLine($"No element found with Id: {elementId}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting element: {ex.Message}");
                throw;
            }
        }

        public void DeleteCollection()
        {
            _database!.DropCollection("Elements");
            Debug.WriteLine("Successfully deleted the Elements collection.");
        }
    }
}
