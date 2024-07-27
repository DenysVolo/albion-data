public class MarketOrderResponse
{
    public long AlbionId { get; set; }
    public required string ItemNumId { get; set; }
    public required string ItemTextId { get; set; }
    public string? ItemDisplayName { get; set; }
    public int QualityLevel { get; set; }
    public int EnchantmentLevel { get; set; }
    public long Price { get; set; }
    public int InitialAmount { get; set; }
    public int Amount { get; set; }
    public required string AuctionType { get; set; }
    public DateTime Expires { get; set; }
    public int LocationId { get; set; }
    public string? LocationDisplayName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
