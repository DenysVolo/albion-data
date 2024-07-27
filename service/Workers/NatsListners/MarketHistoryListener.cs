using Npgsql;

namespace service;

public class MarketHistoryListener(ILogger<MarketHistoryListener> logger, IDatabaseHandler databaseHandler, INatsStatsTracker statsTracker) : NatsListenerBase<MarketHistoryListener>(logger, databaseHandler, statsTracker)
{
    private const string CommandText = @"
        INSERT INTO albiondb.market_history (
            item_amount, 
            silver_amount, 
            timestamp, 
            item_id, 
            location, 
            quality, 
            aggregation
        ) VALUES (
            @item_amount, 
            @silver_amount, 
            @timestamp, 
            @item_id, 
            @location, 
            @quality, 
            @aggregation
        )
    ";

    private const string NATS_URL = "nats://public:thenewalbiondata@nats.albion-online-data.com:34222";
    private const string NATS_TOPIC= "markethistories.deduped";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        ProccessItem<MarketHistoryDTO> itemHandler = ProcessItem;
        while (!stoppingToken.IsCancellationRequested)
        {
           await SubscribeToTopic(NATS_URL, NATS_TOPIC, itemHandler, stoppingToken);
        }
    }

    protected void ProcessItem(MarketHistoryDTO marketHistoryItem) {
        foreach (var item in marketHistoryItem.MarketHistories) {
            var dbParams = new NpgsqlParameter[]
                {
                    new ("item_amount", NpgsqlTypes.NpgsqlDbType.Bigint) { Value = item.ItemAmount },
                    new ("silver_amount", NpgsqlTypes.NpgsqlDbType.Bigint) { Value = item.SilverAmount },
                    new ("timestamp", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = DateTimeOffset.FromUnixTimeMilliseconds((long)((item.Timestamp / 10000) - 62136892800000)).DateTime },
                    new ("item_id", NpgsqlTypes.NpgsqlDbType.Varchar, 128) { Value = marketHistoryItem.AlbionId.ToString() },
                    new ("location", NpgsqlTypes.NpgsqlDbType.Smallint) { Value = marketHistoryItem.LocationId },
                    new ("quality", NpgsqlTypes.NpgsqlDbType.Smallint) { Value = marketHistoryItem.QualityLevel },
                    new ("aggregation", NpgsqlTypes.NpgsqlDbType.Smallint) { Value = 0 }
                };
            AddDataToDB(CommandText, dbParams);
        }
    }
}
