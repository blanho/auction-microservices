using System.ComponentModel.DataAnnotations;

namespace SearchService.Application.DTOs
{
    public class CreateSearchItemDto
    {
        [Required]
        public required string Title { get; set; }

        [Required]
        public required string Description { get; set; }

        [Required]
        public required string Category { get; set; }

        public string Tags { get; set; }

        public string ImageUrl { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public required string Status { get; set; }

        [Required]
        public required string Source { get; set; }

        [Required]
        public Guid SourceId { get; set; }
    }
}