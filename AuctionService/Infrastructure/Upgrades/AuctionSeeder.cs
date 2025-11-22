using AuctionService.Domain.Entities;
using AuctionService.Infrastructure.Data;
using Common.Core.Constants;
using System.Text.Json;

namespace AuctionService.Infrastructure.Upgrades;

public static class AuctionSeeder
{
    public static async Task SeedAuctionsAsync(AuctionDbContext context)
    {
        
        if (context.Auctions.Any())
        {
            return;
        }

        var seedDataPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "../../../Infrastructure/SeedData/auctions.json"
        );

        if (!File.Exists(seedDataPath))
        {
            throw new FileNotFoundException($"Seed data file not found: {seedDataPath}");
        }

        var json = await File.ReadAllTextAsync(seedDataPath);
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var seedData = JsonSerializer.Deserialize<List<AuctionSeedDto>>(json, options)
            ?? throw new InvalidOperationException("Failed to deserialize seed data");

        var auctions = seedData.Select(dto => new Auction
        {
            Id = Guid.NewGuid(),
            ReversePrice = dto.ReversePrice,
            Seller = dto.Seller,
            Winner = null,
            SoldAmount = null,
            CurrentHighBid = null,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = SystemGuids.System,
            AuctionEnd = DateTimeOffset.UtcNow.AddDays(dto.AuctionEndDays),
            Status = Status.Live,
            IsDeleted = false,
            Item = new Item
            {
                Id = Guid.NewGuid(),
                Make = dto.Item.Make,
                Model = dto.Item.Model,
                Year = dto.Item.Year,
                Color = dto.Item.Color,
                Mileage = dto.Item.Mileage,
                ImageUrl = dto.Item.ImageUrl,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = SystemGuids.System,
                IsDeleted = false
            }
        }).ToList();

        await context.Auctions.AddRangeAsync(auctions);
        await context.SaveChangesAsync();
    }

    private class AuctionSeedDto
    {
        public int ReversePrice { get; set; }
        public string Seller { get; set; }
        public int AuctionEndDays { get; set; }
        public ItemSeedDto Item { get; set; }
    }

    private class ItemSeedDto
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public int Mileage { get; set; }
        public string ImageUrl { get; set; }
    }
}
