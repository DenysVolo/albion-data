public class MarketHistory
{
    public long Id { get; set; }
    public long ItemAmount { get; set; }
    public long SilverAmount { get; set; }
    public string ItemId { get; set; }
    public int Location { get; set; }
    public int QualityLevel { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public int Aggregation { get; set; }
}
