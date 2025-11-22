using SearchService.Application.DTOs;
using SearchService.Application.Interfaces;
using AutoMapper;
using Common.Core.Exceptions;
using Common.Core.Interfaces;
using Common.Repository.Interfaces;
using System.Diagnostics;

namespace SearchService.Application.Services;

public class SearchServiceImpl : ISearchService
{
    private readonly ISearchItemRepository _repository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<SearchServiceImpl> _logger;
    private readonly IDateTimeProvider _dateTime;

    public SearchServiceImpl(
        ISearchItemRepository repository, 
        IMapper mapper,
        IAppLogger<SearchServiceImpl> logger,
        IDateTimeProvider dateTime)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
        _dateTime = dateTime;
    }

    public async Task<SearchResultDto> SearchAsync(SearchRequestDto request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        
        _logger.LogInformation("Performing search with query: {Query}, Category: {Category}, Page: {Page}", 
            request.Query, request.Category, request.Page);

        var skip = (request.Page - 1) * request.PageSize;
        
        var items = await _repository.SearchAsync(
            request.Query, 
            request.Category, 
            request.MinPrice, 
            request.MaxPrice, 
            request.Status, 
            request.Source, 
            skip, 
            request.PageSize, 
            cancellationToken);

        var totalCount = await _repository.GetSearchCountAsync(
            request.Query, 
            request.Category, 
            request.MinPrice, 
            request.MaxPrice, 
            request.Status, 
            request.Source, 
            cancellationToken);

        stopwatch.Stop();

        var result = new SearchResultDto
        {
            Items = items.Select(i => _mapper.Map<SearchItemDto>(i)).ToList(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
            HasNextPage = request.Page * request.PageSize < totalCount,
            HasPreviousPage = request.Page > 1,
            Query = request.Query,
            SearchTime = stopwatch.Elapsed
        };

        _logger.LogInformation("Search completed in {ElapsedMs}ms, found {TotalCount} items", 
            stopwatch.ElapsedMilliseconds, totalCount);

        return result;
    }

    public async Task<List<SearchItemDto>> GetAllItemsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching all search items at {Timestamp}", _dateTime.UtcNow);

        var items = await _repository.GetAllAsync(cancellationToken);
        var result = items.Select(i => _mapper.Map<SearchItemDto>(i)).ToList();
        
        _logger.LogInformation("Retrieved {Count} search items", result.Count);
        return result;
    }

    public async Task<SearchItemDto> GetItemByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching search item {ItemId}", id);

        var item = await _repository.GetByIdAsync(id, cancellationToken);
        
        if (item == null)
        {
            _logger.LogWarning("Search item {ItemId} not found", id);
            throw new NotFoundException($"Search item with ID {id} was not found");
        }
        
        return _mapper.Map<SearchItemDto>(item);
    }

    public async Task<SearchItemDto> CreateItemAsync(CreateSearchItemDto dto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating search item for source {Source}:{SourceId} at {Timestamp}", 
            dto.Source, dto.SourceId, _dateTime.UtcNow);

        var item = _mapper.Map<Domain.Entities.SearchItem>(dto);
        
        var createdItem = await _repository.CreateAsync(item, cancellationToken);
        
        _logger.LogInformation("Created search item {ItemId} for source {Source}:{SourceId}", 
            createdItem.Id, dto.Source, dto.SourceId);
        return _mapper.Map<SearchItemDto>(createdItem);
    }

    public async Task<bool> UpdateItemAsync(Guid id, UpdateSearchItemDto dto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating search item {ItemId} at {Timestamp}", id, _dateTime.UtcNow);

        var item = await _repository.GetByIdAsync(id, cancellationToken);
        
        if (item == null)
        {
            _logger.LogWarning("Search item {ItemId} not found for update", id);
            throw new NotFoundException($"Search item with ID {id} was not found");
        }

        item.Title = dto.Title ?? item.Title;
        item.Description = dto.Description ?? item.Description;
        item.Category = dto.Category ?? item.Category;
        item.Tags = dto.Tags ?? item.Tags;
        item.ImageUrl = dto.ImageUrl ?? item.ImageUrl;
        item.Price = dto.Price ?? item.Price;
        item.Status = dto.Status ?? item.Status;
        
        await _repository.UpdateAsync(item, cancellationToken);
        
        _logger.LogInformation("Updated search item {ItemId}", id);
        return true;
    }

    public async Task<bool> DeleteItemAsync(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting search item {ItemId} at {Timestamp}", id, _dateTime.UtcNow);

        var exists = await _repository.ExistsAsync(id, cancellationToken);
        
        if (!exists)
        {
            _logger.LogWarning("Search item {ItemId} not found for deletion", id);
            throw new NotFoundException($"Search item with ID {id} was not found");
        }

        await _repository.DeleteAsync(id, cancellationToken);
        
        _logger.LogInformation("Deleted search item {ItemId}", id);
        return true;
    }

    public async Task<bool> IndexItemAsync(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Indexing search item {ItemId}", id);

        var item = await _repository.GetByIdAsync(id, cancellationToken);
        
        if (item == null)
        {
            _logger.LogWarning("Search item {ItemId} not found for indexing", id);
            throw new NotFoundException($"Search item with ID {id} was not found");
        }

        
        if (item.Metadata == null)
        {
            item.Metadata = new Domain.Entities.SearchMetadata
            {
                SearchItemId = item.Id
            };
        }

        item.Metadata.LastIndexed = _dateTime.UtcNow;
        item.Metadata.SearchVector = $"{item.Title} {item.Description} {item.Category} {item.Tags}".ToLowerInvariant();
        
        await _repository.UpdateAsync(item, cancellationToken);
        
        _logger.LogInformation("Indexed search item {ItemId}", id);
        return true;
    }

    public async Task ReindexAllAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting reindex of all search items at {Timestamp}", _dateTime.UtcNow);

        var items = await _repository.GetAllAsync(cancellationToken);
        
        foreach (var item in items)
        {
            await IndexItemAsync(item.Id, cancellationToken);
        }
        
        _logger.LogInformation("Completed reindex of {Count} search items", items.Count);
    }
}