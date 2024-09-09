public class MarketOrderRequest
{
    [ColumnName("albion_id", "=")]
    public List<long>? AlbionId { get; set; }

    [ColumnName("item_id", "=")]
    public List<string>? ItemTextId { get; set; }

    [ColumnName("quality_level", "=")]
    public List<int>? QualityLevel { get; set; }

    [ColumnName("enchantment_level", "=")]
    public List<int>? EnchantmentLevel { get; set; }

    [ColumnName("price", ">=")]
    public long? MinPrice { get; set; }

    [ColumnName("price", "<=")]
    public long? MaxPrice { get; set; }

    [ColumnName("initial_amount", ">=")]
    public int? MinInitialAmount { get; set; }

    [ColumnName("initial_amount", "<=")]
    public int? MaxInitialAmount { get; set; }

    [ColumnName("amount", ">=")]
    public int? MinAmount { get; set; }

    [ColumnName("amount", "<=")]
    public int? MaxAmount { get; set; }

    [ColumnName("auction_type", "=")]
    public List<string>? AuctionType { get; set; }

    [ColumnName("expires", ">=")]
    public DateTime? MinExpiryDate { get; set; }

    [ColumnName("expires", "<=")]
    public DateTime? MaxExpiryDate { get; set; }

    [ColumnName("location", "=")]
    public List<int>? LocationId { get; set; }

    [ColumnName("created_at", ">=")]
    public DateTime? MinCreationDate { get; set; }

    [ColumnName("created_at", "<=")]
    public DateTime? MaxCreationDate { get; set; }

    [ColumnName("updated_at", ">=")]
    public DateTime? MinUpdateDate { get; set; }

    [ColumnName("updated_at", "<=")]
    public DateTime? MaxUpdateDate { get; set; }
}
