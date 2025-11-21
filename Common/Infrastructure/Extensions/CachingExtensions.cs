using Common.Application.Interfaces;
using Common.Domain.Entities;
using Common.Infrastructure.Caching;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure.Extensions
{
    public static class CachingExtensions
    {
        public static IServiceCollection AddCachedRepository<TEntity, TRepository>(
            this IServiceCollection services,
            string servicePrefix,
            TimeSpan? cacheExpiration = null)
            where TEntity : BaseEntity
            where TRepository : class, IRepository<TEntity>
        {
            if (string.IsNullOrWhiteSpace(servicePrefix))
                throw new ArgumentException("Service prefix cannot be empty", nameof(servicePrefix));

            services.AddScoped<TRepository>();

            services.AddScoped<IRepository<TEntity>>(provider =>
            {
                var innerRepository = provider.GetRequiredService<TRepository>();
                var cache = provider.GetRequiredService<IDistributedCacheService>();
                return new CachedRepository<TEntity>(innerRepository, cache, servicePrefix, cacheExpiration);
            });

            return services;
        }
    }
}
