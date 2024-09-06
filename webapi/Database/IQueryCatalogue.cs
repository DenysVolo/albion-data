public interface IQueryCatalogue
{
    Task<MarketOrderResponse> GetMarketOrdersAsync(
        MarketOrderRequest request,
        int? limit = null,
        string? sessionId = null);

    Task<IEnumerable<MarketHistory>> GetMarketHistoryAsync();
}
