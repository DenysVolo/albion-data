using System.Data;

public class QueryCatalogue : IQueryCatalogue
{
    private readonly IDatabaseHandler _databaseHandler;

    public QueryCatalogue(IDatabaseHandler databaseHandler)
    {
        _databaseHandler = databaseHandler;
    }

    public async Task<IEnumerable<MarketOrder>> GetMarketOrdersAsync()
    {
        var query = "SELECT * FROM albiondb.market_orders limit 100";
        var marketOrders = new List<MarketOrder>();

        await _databaseHandler.ExecuteQueryAsync(query, reader =>
        {
            while (reader.Read())
            {
                marketOrders.Add(ReadMarketOrder(reader));
            }
        });

        return marketOrders;
    }

    public async Task<IEnumerable<MarketHistory>> GetMarketHistoryAsync()
    {
        var query = "SELECT * FROM albiondb.market_history limit 100";
        var marketHistory = new List<MarketHistory>();

        await _databaseHandler.ExecuteQueryAsync(query, reader =>
        {
            while (reader.Read())
            {
                marketHistory.Add(ReadMarketHistory(reader));
            }
        });

        return marketHistory;
    }

    private MarketOrder ReadMarketOrder(IDataReader reader) {
        return 
            new MarketOrder{
                Id = reader.GetInt64(0),
                AlbionId = reader.GetInt64(1),
                ItemId = reader.GetString(2),
                QualityLevel = reader.GetInt16(3),
                EnchantmentLevel = reader.GetInt16(4),
                Price = reader.GetInt64(5),
                InitialAmount = reader.GetInt16(6),
                Amount = reader.GetInt16(7),
                AuctionType = reader.GetString(8),
                Expires = reader.GetDateTime(9),
                Location = reader.GetInt16(10),
                CreatedAt = reader.GetDateTime(11),
                UpdatedAt = reader.GetDateTime(12),
                DeletedAt = reader.IsDBNull(13) ? null : reader.GetDateTime(13),
            };
    }

    private MarketHistory ReadMarketHistory(IDataReader reader) {
        return 
            new MarketHistory{
                Id = reader.GetInt64(0),
                ItemAmount = reader.GetInt64(1),
                SilverAmount = reader.GetInt64(2),
                ItemId = reader.GetString(3),
                Location = reader.GetInt16(4),
                QualityLevel = reader.GetInt16(5),
                Timestamp = reader.GetDateTime(6),
                Aggregation = reader.GetInt16(7),
            };
    }
}
