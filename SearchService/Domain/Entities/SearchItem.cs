using Common.Domain.Entities;

namespace SearchService.Domain.Entities
{
    public class SearchItem : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Tags { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
        public string Source { get; set; } 
        public Guid SourceId { get; set; } 
        public SearchMetadata Metadata { get; set; }
    }
}