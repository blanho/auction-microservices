using SearchService.Application.Interfaces;
using SearchService.Domain.Entities;
using SearchService.Infrastructure.Data;
using MongoDB.Driver;
using Common.Core.Interfaces;
using Common.Core.Constants;

namespace SearchService.Infrastructure.Repositories
{
    public class SearchItemRepository : ISearchItemRepository
    {
        private readonly IMongoCollection<SearchItem> _collection;
        private readonly IDateTimeProvider _dateTime;

        public SearchItemRepository(MongoSearchDbContext context, IDateTimeProvider dateTime)
        {
            _collection = context.SearchItems;
            _dateTime = dateTime;
        }

        public async Task<List<SearchItem>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var filter = Builders<SearchItem>.Filter.Eq(x => x.IsDeleted, false);
            var sort = Builders<SearchItem>.Sort.Ascending(x => x.Title);
            return await _collection.Find(filter).Sort(sort).ToListAsync(cancellationToken);
        }

        public async Task<SearchItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var filter = Builders<SearchItem>.Filter.And(
                Builders<SearchItem>.Filter.Eq(x => x.Id, id),
                Builders<SearchItem>.Filter.Eq(x => x.IsDeleted, false));
            return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<SearchItem?> GetBySourceIdAsync(string source, Guid sourceId, CancellationToken cancellationToken = default)
        {
            var filter = Builders<SearchItem>.Filter.And(
                Builders<SearchItem>.Filter.Eq(x => x.Source, source),
                Builders<SearchItem>.Filter.Eq(x => x.SourceId, sourceId),
                Builders<SearchItem>.Filter.Eq(x => x.IsDeleted, false));
            return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<SearchItem>> SearchAsync(string query, string category, decimal? minPrice, decimal? maxPrice, string status, string source, int skip, int take, CancellationToken cancellationToken = default)
        {
            var filters = new List<FilterDefinition<SearchItem>>
            {
                Builders<SearchItem>.Filter.Eq(x => x.IsDeleted, false)
            };

            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.ToLowerInvariant();
                filters.Add(
                    Builders<SearchItem>.Filter.Or(
                        Builders<SearchItem>.Filter.Regex(x => x.Title, new MongoDB.Bson.BsonRegularExpression(q, "i")),
                        Builders<SearchItem>.Filter.Regex(x => x.Description, new MongoDB.Bson.BsonRegularExpression(q, "i")),
                        Builders<SearchItem>.Filter.Regex(x => x.Tags, new MongoDB.Bson.BsonRegularExpression(q, "i")),
                        Builders<SearchItem>.Filter.Regex("Metadata.SearchVector", new MongoDB.Bson.BsonRegularExpression(q, "i"))
                    ));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                filters.Add(Builders<SearchItem>.Filter.Eq(x => x.Category, category));
            }

            if (minPrice.HasValue)
            {
                filters.Add(Builders<SearchItem>.Filter.Gte(x => x.Price, minPrice.Value));
            }

            if (maxPrice.HasValue)
            {
                filters.Add(Builders<SearchItem>.Filter.Lte(x => x.Price, maxPrice.Value));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                filters.Add(Builders<SearchItem>.Filter.Eq(x => x.Status, status));
            }

            if (!string.IsNullOrWhiteSpace(source))
            {
                filters.Add(Builders<SearchItem>.Filter.Eq(x => x.Source, source));
            }

            var filter = Builders<SearchItem>.Filter.And(filters);

            var sort = !string.IsNullOrWhiteSpace(query)
                ? Builders<SearchItem>.Sort.Descending("Metadata.Relevance").Descending(x => x.CreatedAt)
                : Builders<SearchItem>.Sort.Descending(x => x.CreatedAt);

            return await _collection.Find(filter)
                .Sort(sort)
                .Skip(skip)
                .Limit(take)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetSearchCountAsync(string query, string category, decimal? minPrice, decimal? maxPrice, string status, string source, CancellationToken cancellationToken = default)
        {
            var filters = new List<FilterDefinition<SearchItem>>
            {
                Builders<SearchItem>.Filter.Eq(x => x.IsDeleted, false)
            };

            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.ToLowerInvariant();
                filters.Add(
                    Builders<SearchItem>.Filter.Or(
                        Builders<SearchItem>.Filter.Regex(x => x.Title, new MongoDB.Bson.BsonRegularExpression(q, "i")),
                        Builders<SearchItem>.Filter.Regex(x => x.Description, new MongoDB.Bson.BsonRegularExpression(q, "i")),
                        Builders<SearchItem>.Filter.Regex(x => x.Tags, new MongoDB.Bson.BsonRegularExpression(q, "i")),
                        Builders<SearchItem>.Filter.Regex("Metadata.SearchVector", new MongoDB.Bson.BsonRegularExpression(q, "i"))
                    ));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                filters.Add(Builders<SearchItem>.Filter.Eq(x => x.Category, category));
            }

            if (minPrice.HasValue)
            {
                filters.Add(Builders<SearchItem>.Filter.Gte(x => x.Price, minPrice.Value));
            }

            if (maxPrice.HasValue)
            {
                filters.Add(Builders<SearchItem>.Filter.Lte(x => x.Price, maxPrice.Value));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                filters.Add(Builders<SearchItem>.Filter.Eq(x => x.Status, status));
            }

            if (!string.IsNullOrWhiteSpace(source))
            {
                filters.Add(Builders<SearchItem>.Filter.Eq(x => x.Source, source));
            }

            var filter = Builders<SearchItem>.Filter.And(filters);
            var count = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
            return (int)count;
        }

        public async Task<SearchItem> CreateAsync(SearchItem item, CancellationToken cancellationToken = default)
        {
            item.CreatedAt = _dateTime.UtcNow;
            item.CreatedBy = SystemGuids.System;
            await _collection.InsertOneAsync(item, cancellationToken: cancellationToken);
            return item;
        }

        public async Task<IEnumerable<SearchItem>> AddRangeAsync(IEnumerable<SearchItem> items, CancellationToken cancellationToken = default)
        {
            var utcNow = _dateTime.UtcNow;
            foreach (var item in items)
            {
                item.CreatedAt = utcNow;
                item.CreatedBy = SystemGuids.System;
            }
            await _collection.InsertManyAsync(items, cancellationToken: cancellationToken);
            return items;
        }

        public async Task UpdateAsync(SearchItem item, CancellationToken cancellationToken = default)
        {
            item.UpdatedAt = _dateTime.UtcNow;
            item.UpdatedBy = SystemGuids.System;
            var filter = Builders<SearchItem>.Filter.Eq(x => x.Id, item.Id);
            await _collection.ReplaceOneAsync(filter, item, cancellationToken: cancellationToken);
        }

        public async Task UpdateRangeAsync(IEnumerable<SearchItem> items, CancellationToken cancellationToken = default)
        {
            var models = new List<WriteModel<SearchItem>>();
            var utcNow = _dateTime.UtcNow;
            foreach (var item in items)
            {
                item.UpdatedAt = utcNow;
                item.UpdatedBy = SystemGuids.System;
                var filter = Builders<SearchItem>.Filter.Eq(x => x.Id, item.Id);
                models.Add(new ReplaceOneModel<SearchItem>(filter, item));
            }
            if (models.Count > 0)
            {
                await _collection.BulkWriteAsync(models, cancellationToken: cancellationToken);
            }
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var update = Builders<SearchItem>.Update
                .Set(x => x.IsDeleted, true)
                .Set(x => x.DeletedAt, _dateTime.UtcNow)
                .Set(x => x.DeletedBy, SystemGuids.System);
            await _collection.UpdateOneAsync(x => x.Id == id, update, cancellationToken: cancellationToken);
        }

        public async Task DeleteRangeAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        {
            var filter = Builders<SearchItem>.Filter.In(x => x.Id, ids);
            var update = Builders<SearchItem>.Update
                .Set(x => x.IsDeleted, true)
                .Set(x => x.DeletedAt, _dateTime.UtcNow)
                .Set(x => x.DeletedBy, SystemGuids.System)
                .Set(x => x.UpdatedAt, _dateTime.UtcNow)
                .Set(x => x.UpdatedBy, SystemGuids.System);
            await _collection.UpdateManyAsync(filter, update, cancellationToken: cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var count = await _collection.CountDocumentsAsync(x => x.Id == id && !x.IsDeleted, cancellationToken: cancellationToken);
            return count > 0;
        }
    }
}