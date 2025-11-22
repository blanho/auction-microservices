using MongoDB.Driver;
using SearchService.Domain.Entities;

namespace SearchService.Infrastructure.Data
{
    public sealed class MongoDbSettings
    {
        public string ConnectionString { get; set; } = "mongodb://localhost:27017";
        public string DatabaseName { get; set; } = "search_db";
        public string CollectionName { get; set; } = "SearchItems";
    }

    public class MongoSearchDbContext
    {
        private readonly IMongoDatabase _database;
        public IMongoCollection<SearchItem> SearchItems { get; }

        public MongoSearchDbContext(IMongoClient client, MongoDbSettings settings)
        {
            _database = client.GetDatabase(settings.DatabaseName);
            SearchItems = _database.GetCollection<SearchItem>(settings.CollectionName);

            EnsureIndexes(SearchItems).GetAwaiter().GetResult();
        }

        private static async Task EnsureIndexes(IMongoCollection<SearchItem> collection)
        {
            var indexKeys = Builders<SearchItem>.IndexKeys
                .Ascending(x => x.Source)
                .Ascending(x => x.SourceId);

            var uniqueSourceIndex = new CreateIndexModel<SearchItem>(indexKeys, new CreateIndexOptions
            {
                Name = "IX_Source_SourceId",
                Unique = true
            });

            var categoryIndex = new CreateIndexModel<SearchItem>(Builders<SearchItem>.IndexKeys.Ascending(x => x.Category));
            var statusIndex = new CreateIndexModel<SearchItem>(Builders<SearchItem>.IndexKeys.Ascending(x => x.Status));
            var priceIndex = new CreateIndexModel<SearchItem>(Builders<SearchItem>.IndexKeys.Ascending(x => x.Price));

            await collection.Indexes.CreateManyAsync(new[] { uniqueSourceIndex, categoryIndex, statusIndex, priceIndex });
        }
    }
}