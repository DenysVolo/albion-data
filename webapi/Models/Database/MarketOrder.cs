public class MarketOrder
{
    public long Id { get; set; }
    public long AlbionId { get; set; }
    public required string ItemId { get; set; }
    public int QualityLevel { get; set; }
    public int EnchantmentLevel { get; set; }
    public long Price { get; set; }
    public int InitialAmount { get; set; }
    public int Amount { get; set; }
    public required string AuctionType { get; set; }
    public DateTime Expires { get; set; }
    public int Location { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
