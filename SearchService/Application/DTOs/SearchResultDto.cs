namespace SearchService.Application.DTOs
{
    public class SearchResultDto
    {
        public List<SearchItemDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public string Query { get; set; } = string.Empty;
        public TimeSpan SearchTime { get; set; }
    }
}