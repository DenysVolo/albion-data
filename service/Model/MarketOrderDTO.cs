public class MarketOrderDTO
{
    public int Id { get; set; }
    public required string ItemTypeId { get; set; }
    public required string ItemGroupTypeId { get; set; }
    public int LocationId { get; set; }
    public int QualityLevel { get; set; }
    public int EnchantmentLevel { get; set; }
    public int UnitPriceSilver { get; set; }
    public int Amount { get; set; }
    public required string AuctionType { get; set; }
    public required string Expires { get; set; }
}
