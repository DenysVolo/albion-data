using System.Data;
using System.Reflection;
using System.Text.Json;
using Npgsql;

public class QueryCatalogue : IQueryCatalogue
{
    private readonly IDatabaseHandler _databaseHandler;
    private readonly IItemCatalogue _itemCatalogue;
    private readonly ILocationCatalogue _locationCatalogue;
    private readonly ISessionDatabase _inMemorySessionDatabase;

    private const string MarketOrdersTable = "albiondb.market_orders";

    private const string LocationFormat = "0000.##";

    public QueryCatalogue(IDatabaseHandler databaseHandler, IItemCatalogue itemCatalogue, ILocationCatalogue locationCatalogue, ISessionDatabase sessionDatabase) 
    {
        _databaseHandler = databaseHandler;
        _itemCatalogue = itemCatalogue;
        _locationCatalogue = locationCatalogue;
        _inMemorySessionDatabase = sessionDatabase;
    }

  

    public async Task<MarketOrderResponse> GetMarketOrdersAsync(
        MarketOrderRequest request,
        int? limit = null,
        string? sessionId = null)
    {
        if (sessionId == null || !_inMemorySessionDatabase.IsSessionIdActive(sessionId)) 
        {
            sessionId = _inMemorySessionDatabase.CreateSession(
                nameof(MarketOrderRequest), 
                JsonSerializer.Serialize(request), 
                await GetRequestDataAsync(request, MarketOrdersTable, limit));
        }
        
        else if(!Equals(typeof(MarketOrderRequest).ToString(), _inMemorySessionDatabase.GetRequestType(sessionId)) || !new DeepComparer<MarketOrderRequest>().Equals(request, JsonSerializer.Deserialize<MarketOrderRequest>(_inMemorySessionDatabase.GetRequestDetails(sessionId)))) 
        {
            _inMemorySessionDatabase.UpsertSession(
                sessionId,
                nameof(MarketOrderRequest), 
                JsonSerializer.Serialize(request), 
                await GetRequestDataAsync(request, MarketOrdersTable, limit));
        }

        var marketOrders = new List<MarketOrder>();
        var requestDataReader = _inMemorySessionDatabase.GetRequestData(sessionId).CreateDataReader();
        while (requestDataReader.Read())
        {
            marketOrders.Add(ReadMarketOrder(requestDataReader));
        }
    
        var marketOrderResponseRows = marketOrders.Select(x => new MarketOrderResponseRow {
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
        var marketOrderResponse = new MarketOrderResponse {
            Orders = marketOrderResponseRows,
            SessionId = sessionId
        };
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

    private MarketOrder ReadMarketOrder(DataTableReader reader) {
        return 
            new MarketOrder{
                Id = Convert.ToInt64(reader.GetValue(0)),
                AlbionId = Convert.ToInt64(reader.GetValue(1)),
                ItemId = reader.GetString(2),
                QualityLevel = Convert.ToInt32(reader.GetValue(3)),
                EnchantmentLevel = Convert.ToInt32(reader.GetValue(4)),
                Price = Convert.ToInt64(reader.GetValue(5)),
                InitialAmount = Convert.ToInt32(reader.GetValue(6)),
                Amount = Convert.ToInt32(reader.GetValue(7)),
                AuctionType = reader.GetString(8),
                Expires = reader.GetDateTime(9),
                Location = Convert.ToInt32(reader.GetValue(10)),
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

    private async Task<DataTable> GetRequestDataAsync<TRequest>(TRequest request, string table, int? limit = null)
    {
        var queryBuilder = new QueryBuilder();
        queryBuilder.FromTables.Add(table);

        var parameters = new List<NpgsqlParameter>();
        var paramCounter = 1;

        // Iterate through all properties in the request object
        var properties = typeof(TRequest).GetProperties();
        foreach (var property in properties)
        {
            var columnAttr = property.GetCustomAttribute<ColumnNameAttribute>();
            if (columnAttr != null)
            {
                var value = property.GetValue(request);
                AddQueryParameter(queryBuilder, parameters, columnAttr.Name, value, ref paramCounter);
            }
        }
        queryBuilder.Limit = limit;

        // Execute the query here (e.g., call a database with the query and parameters)
        var requestData = new DataTable("requestData");
        Console.WriteLine(queryBuilder.BuildQuery());
        await _databaseHandler.ExecuteQueryAsync(queryBuilder.BuildQuery(), parameters.ToArray(), requestData.Load);

        return requestData;
    }

   private void AddQueryParameter<T>(
        QueryBuilder queryBuilder, 
        List<NpgsqlParameter> parameters, 
        string columnName, 
        T? value, 
        ref int paramCounter)
    {
        if (value != null)
        {
            string paramName = $"p{paramCounter++}";
            if (value is IEnumerable<int> intList)
            {
                ProcessList(intList, paramName);
            }
            else if (value is IEnumerable<long> longList)
            {
                ProcessList(longList, paramName);
            }
            else if (value is IEnumerable<string> stringList)
            {
                ProcessList(stringList, paramName);
            }
            else
            {
                queryBuilder.WhereAttribute.Add($"{columnName}=@{paramName}");
                parameters.Add(new NpgsqlParameter(paramName, value));
            }
        }

        void ProcessList<U>(IEnumerable<U> list, string paramName)
        {
            queryBuilder.WhereAttribute.Add($"{columnName} IN ({string.Join(", ", list.Select(_ => "@" + paramName))})");
            foreach (var item in list)
            {
                parameters.Add(new NpgsqlParameter(paramName, item));
            }
        }
    }
}
