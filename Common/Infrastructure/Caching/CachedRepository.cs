using Common.Application.Interfaces;
using Common.Domain.Entities;

namespace Common.Infrastructure.Caching
{
    public class CachedRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly IRepository<T> _innerRepository;
        private readonly IDistributedCacheService _cache;
        private readonly TimeSpan _defaultExpiration;
        private readonly string _servicePrefix;
        private readonly string _entityName;

        private string GetByIdKey(Guid id) => $"{_servicePrefix}:{_entityName}:{id}";
        private string GetAllKey() => $"{_servicePrefix}:{_entityName}:all";

        public CachedRepository(
            IRepository<T> innerRepository,
            IDistributedCacheService cache,
            string servicePrefix,
            TimeSpan? defaultExpiration = null)
        {
            _innerRepository = innerRepository;
            _cache = cache;
            _servicePrefix = servicePrefix ?? throw new ArgumentNullException(nameof(servicePrefix));
            _defaultExpiration = defaultExpiration ?? TimeSpan.FromMinutes(10);
            _entityName = typeof(T).Name.ToLower();
        }

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var cacheKey = GetByIdKey(id);
            
            var cached = await _cache.GetAsync<T>(cacheKey, cancellationToken);
            if (cached != null)
                return cached;

            var entity = await _innerRepository.GetByIdAsync(id, cancellationToken);
            if (entity != null)
            {
                await _cache.SetAsync(cacheKey, entity, _defaultExpiration, cancellationToken);
            }

            return entity;
        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var cacheKey = GetAllKey();
            
            var cached = await _cache.GetAsync<IEnumerable<T>>(cacheKey, cancellationToken);
            if (cached != null)
                return cached;

            var entities = await _innerRepository.GetAllAsync(cancellationToken);
            await _cache.SetAsync(cacheKey, entities, TimeSpan.FromMinutes(5), cancellationToken);

            return entities;
        }

        public async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
        {
            var created = await _innerRepository.CreateAsync(entity, cancellationToken);
            
            await _cache.RemoveAsync(GetAllKey(), cancellationToken);
            await _cache.SetAsync(GetByIdKey(created.Id), created, _defaultExpiration, cancellationToken);

            return created;
        }

        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            var created = await _innerRepository.AddRangeAsync(entities, cancellationToken);
            await _cache.RemoveAsync(GetAllKey(), cancellationToken);

            return created;
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _innerRepository.UpdateAsync(entity, cancellationToken);
            await _cache.RemoveAsync(GetByIdKey(entity.Id), cancellationToken);
            await _cache.RemoveAsync(GetAllKey(), cancellationToken);
        }

        public async Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await _innerRepository.UpdateRangeAsync(entities, cancellationToken);
            await _cache.RemoveAsync(GetAllKey(), cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await _innerRepository.DeleteAsync(id, cancellationToken);
            await _cache.RemoveAsync(GetByIdKey(id), cancellationToken);
            await _cache.RemoveAsync(GetAllKey(), cancellationToken);
        }

        public async Task DeleteRangeAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        {
            await _innerRepository.DeleteRangeAsync(ids, cancellationToken);
            await _cache.RemoveAsync(GetAllKey(), cancellationToken);
        }

        public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _innerRepository.ExistsAsync(id, cancellationToken);
        }
    }
}
