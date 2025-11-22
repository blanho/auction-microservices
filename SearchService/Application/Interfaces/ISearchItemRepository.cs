using SearchService.Domain.Entities;
using Common.Repository.Interfaces;

namespace SearchService.Application.Interfaces;

public interface ISearchItemRepository : IRepository<SearchItem>
{
    Task<IEnumerable<SearchItem>> AddRangeAsync(IEnumerable<SearchItem> items, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(IEnumerable<SearchItem> items, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<List<SearchItem>> SearchAsync(string query, string category, decimal? minPrice, decimal? maxPrice, string status, string source, int skip, int take, CancellationToken cancellationToken = default);
    Task<int> GetSearchCountAsync(string query, string category, decimal? minPrice, decimal? maxPrice, string status, string source, CancellationToken cancellationToken = default);
    Task<SearchItem?> GetBySourceIdAsync(string source, Guid sourceId, CancellationToken cancellationToken = default);
}