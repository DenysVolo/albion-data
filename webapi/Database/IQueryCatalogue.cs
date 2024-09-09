public interface IQueryCatalogue
{
    Task<MarketOrderResponse> GetMarketOrdersAsync(
        MarketOrderRequest request,
        int limit,
        string? sessionId = null);

    Task<IEnumerable<MarketHistory>> GetMarketHistoryAsync();
}
