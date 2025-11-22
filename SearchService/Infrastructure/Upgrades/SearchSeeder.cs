using SearchService.Domain.Entities;
using SearchService.Infrastructure.Data;
using Common.Core.Constants;
using MongoDB.Driver;
using System.Text.Json;

namespace SearchService.Infrastructure.Upgrades;

public static class SearchSeeder
{
    public static async Task SeedSearchItemsAsync(MongoSearchDbContext context)
    {
        
        var existing = await context.SearchItems.CountDocumentsAsync(_ => true);
        if (existing > 0)
        {
            return;
        }

        var seedDataPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "../../../Infrastructure/SeedData/search-items.json"
        );

        if (!File.Exists(seedDataPath))
        {
            throw new FileNotFoundException($"Seed data file not found: {seedDataPath}");
        }

        var json = await File.ReadAllTextAsync(seedDataPath);
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var seedData = JsonSerializer.Deserialize<List<SearchItemSeedDto>>(json, options)
            ?? throw new InvalidOperationException("Failed to deserialize seed data");

        var searchItems = seedData.Select(dto => new SearchItem
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            Category = dto.Category,
            Tags = dto.Tags,
            ImageUrl = dto.ImageUrl,
            Price = dto.Price,
            Status = "Active",
            Source = "AuctionService",
            SourceId = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = SystemGuids.System,
            IsDeleted = false,
            Metadata = new SearchMetadata
            {
                Id = Guid.NewGuid(),
                SearchVector = $"{dto.Title} {dto.Description} {dto.Tags}".ToLower(),
                ViewCount = dto.ViewCount,
                Relevance = dto.Relevance,
                LastIndexed = DateTimeOffset.UtcNow,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = SystemGuids.System,
                IsDeleted = false
            }
        }).ToList();

        await context.SearchItems.InsertManyAsync(searchItems);
    }

    private class SearchItemSeedDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Tags { get; set; }
        public string? ImageUrl { get; set; }
        public int Price { get; set; }
        public int ViewCount { get; set; }
        public decimal Relevance { get; set; }
    }
}