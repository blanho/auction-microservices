using SearchService.Application.Interfaces;
using SearchService.Domain.Entities;
using SearchService.Infrastructure.Data;
using AutoMapper;
using Common.Caching.Abstractions;
using Common.Caching.Keys;
using Common.Repository.Interfaces;
using Microsoft.Extensions.Logging;

namespace SearchService.Infrastructure.Repositories;

public class CachedSearchItemRepository : ISearchItemRepository
{
    private readonly ISearchItemRepository _inner;
    private readonly ICacheService _cache;
    private readonly IMapper _mapper;
    private readonly IAppLogger<CachedSearchItemRepository> _logger;
    private static readonly TimeSpan SingleItemTtl = TimeSpan.FromMinutes(10);
    private static readonly TimeSpan SearchResultTtl = TimeSpan.FromMinutes(2);

    public CachedSearchItemRepository(ISearchItemRepository inner, ICacheService cache, IMapper mapper, IAppLogger<CachedSearchItemRepository> logger)
    {
        _inner = inner;
        _cache = cache;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<SearchItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var key = "search:items:all";
        var cachedDtos = await _cache.GetAsync<List<Application.DTOs.SearchItemDto>>(key, cancellationToken);
        if (cachedDtos != null)
        {
            _logger.LogInformation("Cache HIT for all search items");
            return _mapper.Map<List<SearchItem>>(cachedDtos);
        }

        _logger.LogInformation("Cache MISS for all search items - fetching from database");
        var items = await _inner.GetAllAsync(cancellationToken);
        var dtos = _mapper.Map<List<Application.DTOs.SearchItemDto>>(items);
        await _cache.SetAsync(key, dtos, SingleItemTtl, cancellationToken);
        return items;
    }

    public async Task<SearchItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var key = $"search:item:{id}";
        var cachedDto = await _cache.GetAsync<Application.DTOs.SearchItemDto>(key, cancellationToken);
        if (cachedDto != null)
        {
            _logger.LogInformation("Cache HIT for search item {ItemId}", id);
            return _mapper.Map<SearchItem>(cachedDto);
        }

        _logger.LogInformation("Cache MISS for search item {ItemId} - fetching from database", id);
        var item = await _inner.GetByIdAsync(id, cancellationToken);
        if (item != null)
        {
            var dto = _mapper.Map<Application.DTOs.SearchItemDto>(item);
            await _cache.SetAsync(key, dto, SingleItemTtl, cancellationToken);
            _logger.LogInformation("Cache SET for search item {ItemId}", id);
        }
        return item;
    }

    public async Task<SearchItem?> GetBySourceIdAsync(string source, Guid sourceId, CancellationToken cancellationToken = default)
    {
        
        return await _inner.GetBySourceIdAsync(source, sourceId, cancellationToken);
    }

    public async Task<List<SearchItem>> SearchAsync(string query, string category, decimal? minPrice, decimal? maxPrice, string status, string source, int skip, int take, CancellationToken cancellationToken = default)
    {
        
        var keyParts = new[]
        {
            $"q:{query ?? ""}",
            $"cat:{category ?? ""}",
            $"minp:{minPrice?.ToString() ?? ""}",
            $"maxp:{maxPrice?.ToString() ?? ""}",
            $"st:{status ?? ""}",
            $"src:{source ?? ""}",
            $"skip:{skip}",
            $"take:{take}"
        };
        var key = $"search:results:{string.Join(":", keyParts).Replace(" ", "_")}";
        
        var cachedDtos = await _cache.GetAsync<List<Application.DTOs.SearchItemDto>>(key, cancellationToken);
        if (cachedDtos != null)
        {
            _logger.LogInformation("Cache HIT for search query: {Query}", query);
            return _mapper.Map<List<SearchItem>>(cachedDtos);
        }

        _logger.LogInformation("Cache MISS for search query: {Query} - fetching from database", query);
        var items = await _inner.SearchAsync(query, category, minPrice, maxPrice, status, source, skip, take, cancellationToken);
        var dtos = _mapper.Map<List<Application.DTOs.SearchItemDto>>(items);
        await _cache.SetAsync(key, dtos, SearchResultTtl, cancellationToken);
        return items;
    }

    public async Task<int> GetSearchCountAsync(string query, string category, decimal? minPrice, decimal? maxPrice, string status, string source, CancellationToken cancellationToken = default)
    {
        
        return await _inner.GetSearchCountAsync(query, category, minPrice, maxPrice, status, source, cancellationToken);
    }

    public async Task<SearchItem> CreateAsync(SearchItem item, CancellationToken cancellationToken = default)
    {
        var result = await _inner.CreateAsync(item, cancellationToken);
        await InvalidateAfterWrite(result.Id, cancellationToken);
        return result;
    }

    public async Task<IEnumerable<SearchItem>> AddRangeAsync(IEnumerable<SearchItem> items, CancellationToken cancellationToken = default)
    {
        var result = await _inner.AddRangeAsync(items, cancellationToken);
        foreach (var item in result)
        {
            await InvalidateAfterWrite(item.Id, cancellationToken);
        }
        return result;
    }

    public async Task UpdateAsync(SearchItem item, CancellationToken cancellationToken = default)
    {
        await _inner.UpdateAsync(item, cancellationToken);
        await InvalidateAfterWrite(item.Id, cancellationToken);
    }

    public async Task UpdateRangeAsync(IEnumerable<SearchItem> items, CancellationToken cancellationToken = default)
    {
        await _inner.UpdateRangeAsync(items, cancellationToken);
        foreach (var item in items)
        {
            await InvalidateAfterWrite(item.Id, cancellationToken);
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _inner.DeleteAsync(id, cancellationToken);
        await InvalidateAfterWrite(id, cancellationToken);
    }

    public async Task DeleteRangeAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        await _inner.DeleteRangeAsync(ids, cancellationToken);
        foreach (var id in ids)
        {
            await InvalidateAfterWrite(id, cancellationToken);
        }
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => _inner.ExistsAsync(id, cancellationToken);

    private async Task InvalidateAfterWrite(Guid id, CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync($"search:item:{id}", cancellationToken);
        await _cache.RemoveAsync("search:items:all", cancellationToken);
        
        
        _logger.LogInformation("Invalidated cache for search item {ItemId}", id);
    }
}