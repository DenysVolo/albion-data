public interface IQueryCatalogue
{
    Task<IEnumerable<MarketOrder>> GetMarketOrdersAsync();
    Task<IEnumerable<MarketHistory>> GetMarketHistoryAsync();
}
