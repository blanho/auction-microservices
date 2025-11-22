using System.ComponentModel.DataAnnotations;

namespace SearchService.Application.DTOs
{
    public class UpdateSearchItemDto
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public string Tags { get; set; }

        public string ImageUrl { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Price { get; set; }

        public string Status { get; set; }
    }
}