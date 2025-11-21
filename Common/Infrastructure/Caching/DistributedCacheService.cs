using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common.Infrastructure.Caching;

public class DistributedCacheService : IDistributedCacheService
{
    private readonly IDistributedCache _cache;
    private readonly JsonSerializerOptions _jsonOptions;

    public DistributedCacheService(IDistributedCache cache)
    {
        _cache = cache;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var bytes = await _cache.GetAsync(key, cancellationToken);
        if (bytes == null || bytes.Length == 0)
            return default;

        var json = Encoding.UTF8.GetString(bytes);
        return JsonSerializer.Deserialize<T>(json, _jsonOptions);
    }

    public async Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _cache.GetStringAsync(key, cancellationToken);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(value, _jsonOptions);
        var bytes = Encoding.UTF8.GetBytes(json);

        var options = new DistributedCacheEntryOptions();
        if (expiration.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = expiration.Value;
        }

        await _cache.SetAsync(key, bytes, options, cancellationToken);
    }

    public async Task SetStringAsync(string key, string value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions();
        if (expiration.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = expiration.Value;
        }

        await _cache.SetStringAsync(key, value, options, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(key, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var value = await _cache.GetAsync(key, cancellationToken);
        return value != null;
    }

    public async Task RefreshAsync(string key, CancellationToken cancellationToken = default)
    {
        await _cache.RefreshAsync(key, cancellationToken);
    }
}
