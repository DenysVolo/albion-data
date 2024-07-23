public class MarketOrder
{
    public long Id { get; set; }
    public long AlbionId { get; set; }
    public string ItemId { get; set; }
    public int QualityLevel { get; set; }
    public int EnchantmentLevel { get; set; }
    public long Price { get; set; }
    public int InitialAmount { get; set; }
    public int Amount { get; set; }
    public string AuctionType { get; set; }
    public DateTimeOffset Expires { get; set; }
    public int Location { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
