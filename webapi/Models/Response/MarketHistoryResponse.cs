public class MarketHistoryResponse
{
    public long ItemAmount { get; set; }
    public long SilverAmount { get; set; }
    public required string ItemNumId { get; set; }
    public required string ItemTextId { get; set; }
    public string? ItemDisplayName { get; set; }
    public int LocationId { get; set; }
    public string? LocationDisplayName { get; set; }
    public int QualityLevel { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}
