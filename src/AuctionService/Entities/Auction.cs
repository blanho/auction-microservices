namespace AuctionService.Entities
{
    public class Auction
    {
        public Guid Id { get; set; }
        public int ReversePrice { get; set; } = 0;
        public string Seller { get; set; }
        public string Winner { get; set; }
        public int? SoldAmount { get; set; }
        public int? CurrentHighBid { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset AuctionEnd { get; set; }
        public Status Status { get; set; }
        public Item Item { get; set; }
    }
}
