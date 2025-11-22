namespace SearchService.Application.DTOs
{
    public class SearchRequestDto
    {
        public string Query { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "Relevance";
        public string SortOrder { get; set; } = "desc";
    }
}