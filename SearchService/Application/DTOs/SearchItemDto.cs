namespace SearchService.Application.DTOs
{
    public class SearchItemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Tags { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
        public string Source { get; set; }
        public Guid SourceId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public int ViewCount { get; set; }
        public decimal Relevance { get; set; }
        public DateTimeOffset LastIndexed { get; set; }
    }
}