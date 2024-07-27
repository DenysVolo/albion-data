public interface IQueryCatalogue
{
    Task<IEnumerable<MarketOrderResponse>> GetMarketOrdersAsync(
        long? albionId = null,
        int? itemNumId = null,
        string? itemTextId = null,
        int? qualityLevel = null,
        int? enchantmentLevel = null,
        long? minPrice = null,
        long? maxPrice = null,
        int? minInitialAmount = null,
        int? maxInitialAmount = null,
        int? minAmount = null,
        int? maxAmount = null,
        string? auctionType = null,
        DateTime? minExpiryDate = null,
        DateTime? maxExpiryDate = null,
        int? locationId = null,
        DateTime? minCreationDate = null,
        DateTime? maxCreationDate = null,
        DateTime? minUpdateDate = null,
        DateTime? maxUpdateDate = null,
        int? limit = null);

    Task<IEnumerable<MarketHistory>> GetMarketHistoryAsync();
}
