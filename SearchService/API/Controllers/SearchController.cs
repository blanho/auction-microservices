using Asp.Versioning;
using SearchService.Application.DTOs;
using SearchService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SearchService.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/search")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet]
        public async Task<ActionResult<SearchResultDto>> Search([FromQuery] SearchRequestDto request, CancellationToken cancellationToken)
        {
            var result = await _searchService.SearchAsync(request, cancellationToken);
            return Ok(result);
        }

        [HttpGet("items")]
        public async Task<ActionResult<List<SearchItemDto>>> GetAllItems(CancellationToken cancellationToken)
        {
            var items = await _searchService.GetAllItemsAsync(cancellationToken);
            return Ok(items);
        }

        [HttpGet("items/{id:guid}")]
        public async Task<ActionResult<SearchItemDto>> GetItemById(Guid id, CancellationToken cancellationToken)
        {
            
            var item = await _searchService.GetItemByIdAsync(id, cancellationToken);
            return Ok(item);
        }

        [HttpPost("items")]
        public async Task<ActionResult<SearchItemDto>> CreateItem(
            CreateSearchItemDto createSearchItemDto,
            CancellationToken cancellationToken)
        {
            var item = await _searchService.CreateItemAsync(createSearchItemDto, cancellationToken);

            return CreatedAtAction(
                nameof(GetItemById),
                new { id = item.Id },
                item);
        }

        [HttpPut("items/{id:guid}")]
        public async Task<ActionResult> UpdateItem(
            Guid id,
            UpdateSearchItemDto updateSearchItemDto,
            CancellationToken cancellationToken)
        {
            
            await _searchService.UpdateItemAsync(id, updateSearchItemDto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("items/{id:guid}")]
        public async Task<ActionResult> DeleteItem(Guid id, CancellationToken cancellationToken)
        {
            
            await _searchService.DeleteItemAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpPost("items/{id:guid}/index")]
        public async Task<ActionResult> IndexItem(Guid id, CancellationToken cancellationToken)
        {
            await _searchService.IndexItemAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpPost("reindex")]
        public async Task<ActionResult> ReindexAll(CancellationToken cancellationToken)
        {
            await _searchService.ReindexAllAsync(cancellationToken);
            return Ok(new { message = "Reindexing started successfully" });
        }
    }
}