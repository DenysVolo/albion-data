public class MarketHistoryDTO
{
    public int AlbionId { get; set; }
    public int LocationId { get; set; }
    public byte QualityLevel { get; set; }
    public Timescale Timescale { get; set; }
    public required List<MarketHistory> MarketHistories { get; set; }
}

public class MarketHistory
{
    public long ItemAmount { get; set; }
    public long SilverAmount { get; set; }
    public long Timestamp { get; set; }
}

public enum Timescale
{
    Unknown = 0,
    Hourly = 1,
    Daily = 2,
    Weekly = 3,
    Monthly = 4
}
