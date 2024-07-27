using System.Data;
using Npgsql;

public class QueryCatalogue : IQueryCatalogue
{
    private readonly IDatabaseHandler _databaseHandler;
    private readonly IItemCatalogue _itemCatalogue;
    private readonly ILocationCatalogue _locationCatalogue;

    private const string MarketOrdersTable = "albiondb.market_orders";

    private const string LocationFormat = "0000.##";

    public QueryCatalogue(IDatabaseHandler databaseHandler, IItemCatalogue itemCatalogue, ILocationCatalogue locationCatalogue) 
    {
        _databaseHandler = databaseHandler;
        _itemCatalogue = itemCatalogue;
        _locationCatalogue = locationCatalogue;
    }

    public async Task<IEnumerable<MarketOrderResponse>> GetMarketOrdersAsync(
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
        int? limit = null)
    {
        var queryBuilder = new QueryBuilder();
        queryBuilder.FromTables.Add(MarketOrdersTable);

        var parameters = new List<NpgsqlParameter>();

        if (albionId != null) {
            queryBuilder.WhereAttribute.Add("albion_id=@p1");
            parameters.Add(new NpgsqlParameter("p1", albionId));
        }
        if (itemNumId != null) {
            queryBuilder.WhereAttribute.Add("item_id=@p2");
            parameters.Add(new NpgsqlParameter("p2", _itemCatalogue.GetItemByNumId(itemNumId.ToString()!).Item2));
        }
        if (itemTextId != null) {
            queryBuilder.WhereAttribute.Add("item_id=@p3");
            parameters.Add(new NpgsqlParameter("p3", itemTextId));
        }
        if (qualityLevel != null) {
            queryBuilder.WhereAttribute.Add("quality_level=@p4");
            parameters.Add(new NpgsqlParameter("p4", qualityLevel));
        }
        if (enchantmentLevel != null) {
            queryBuilder.WhereAttribute.Add("enchantment_level=@p5");
            parameters.Add(new NpgsqlParameter("p5", enchantmentLevel));
        }
        if (minPrice != null) {
            queryBuilder.WhereAttribute.Add("price>=@p6");
            parameters.Add(new NpgsqlParameter("p6", minPrice));
        }
        if (maxPrice != null) {
            queryBuilder.WhereAttribute.Add("price<=@p7");
            parameters.Add(new NpgsqlParameter("p7", maxPrice));
        }
        if (minInitialAmount != null) {
            queryBuilder.WhereAttribute.Add("initial_amount>=@p8");
            parameters.Add(new NpgsqlParameter("p8", minInitialAmount));
        }
        if (maxInitialAmount != null) {
            queryBuilder.WhereAttribute.Add("initial_amount<=@p9");
            parameters.Add(new NpgsqlParameter("p9", maxInitialAmount));
        }
        if (minAmount != null) {
            queryBuilder.WhereAttribute.Add("amount>=@p10");
            parameters.Add(new NpgsqlParameter("p10", minAmount));
        }
        if (maxAmount != null) {
            queryBuilder.WhereAttribute.Add("amount<=@p11");
            parameters.Add(new NpgsqlParameter("p11", maxAmount));
        }
        if (auctionType != null) {
            queryBuilder.WhereAttribute.Add("auction_type=@p12");
            parameters.Add(new NpgsqlParameter("p12", auctionType));
        }
        if (minExpiryDate != null || maxExpiryDate != null) {
            queryBuilder.WhereAttribute.Add("expires BETWEEN @p13 AND @p14");
            parameters.Add(new NpgsqlParameter("p13", minExpiryDate ?? DateTime.MinValue));
            parameters.Add(new NpgsqlParameter("p14", maxExpiryDate ?? DateTime.MaxValue));
        }
        if (locationId != null) {
            queryBuilder.WhereAttribute.Add("location=@p15");
            parameters.Add(new NpgsqlParameter("p15", locationId));
        }
        if (minCreationDate != null || maxCreationDate != null) {
            queryBuilder.WhereAttribute.Add("created_at BETWEEN @p16 AND @p17");
            parameters.Add(new NpgsqlParameter("p16", minCreationDate ?? DateTime.MinValue));
            parameters.Add(new NpgsqlParameter("p17", maxCreationDate ?? DateTime.MaxValue));
        }
        if (minUpdateDate != null || maxUpdateDate != null) {
            queryBuilder.WhereAttribute.Add("updated_at BETWEEN @p18 AND @p19");
            parameters.Add(new NpgsqlParameter("p18", minUpdateDate ?? DateTime.MinValue));
            parameters.Add(new NpgsqlParameter("p19", maxUpdateDate ?? DateTime.MaxValue));
        }
        queryBuilder.Limit = limit;

        var marketOrders = new List<MarketOrder>();

        await _databaseHandler.ExecuteQueryAsync(queryBuilder.BuildQuery(), parameters.ToArray(), reader =>
        {
            while (reader.Read())
            {
                marketOrders.Add(ReadMarketOrder(reader));
            }
        });

        var marketOrderResponse = marketOrders.Select(x => new MarketOrderResponse {
            AlbionId = x.AlbionId,
            ItemNumId = _itemCatalogue.GetItemByTextId(x.ItemId).Item1,
            ItemTextId = x.ItemId,
            ItemDisplayName = _itemCatalogue.GetItemByTextId(x.ItemId).Item3,
            QualityLevel = x.QualityLevel,
            EnchantmentLevel = x.EnchantmentLevel,
            Price = x.Price,
            InitialAmount = x.InitialAmount,
            Amount = x.Amount,
            AuctionType = x.AuctionType,
            Expires = x.Expires,
            LocationId = x.Location,
            LocationDisplayName = _locationCatalogue.GetLocationById(x.Location.ToString(LocationFormat)).Item2,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        });

        return marketOrderResponse;
    }

    public async Task<IEnumerable<MarketHistory>> GetMarketHistoryAsync()
    {
        var query = "SELECT * FROM albiondb.market_history limit 100";
        var marketHistory = new List<MarketHistory>();

        await _databaseHandler.ExecuteQueryAsync(query, [],  reader =>
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
