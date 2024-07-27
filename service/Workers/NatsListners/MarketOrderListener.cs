using Npgsql;

namespace service;

public class MarketOrderListener(ILogger<MarketOrderListener> logger, IDatabaseHandler databaseHandler,  INatsStatsTracker statsTracker) : NatsListenerBase<MarketOrderListener>(logger, databaseHandler, statsTracker)
{
    private const string CommandText =  @"
        INSERT INTO albiondb.market_orders (
            albion_id, 
            item_id, 
            quality_level, 
            enchantment_level, 
            price, 
            initial_amount, 
            amount, 
            auction_type, 
            expires, 
            location,
            created_at,
            updated_at
        ) VALUES (
            @albion_id, 
            @item_id, 
            @quality_level, 
            @enchantment_level, 
            @price, 
            @initial_amount, 
            @amount, 
            @auction_type, 
            @expires, 
            @location,
            current_timestamp, 
            current_timestamp
        )
        ON CONFLICT (albion_id) 
        DO UPDATE SET
            item_id = EXCLUDED.item_id,
            quality_level = EXCLUDED.quality_level,
            enchantment_level = EXCLUDED.enchantment_level,
            price = EXCLUDED.price,
            initial_amount = EXCLUDED.initial_amount,
            amount = EXCLUDED.amount,
            auction_type = EXCLUDED.auction_type,
            expires = EXCLUDED.expires,
            location = EXCLUDED.location,
            updated_at = current_timestamp;
    ";

    private const string NATS_URL = "nats://public:thenewalbiondata@nats.albion-online-data.com:34222";
    private const string NATS_TOPIC= "marketorders.deduped";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        ProccessItem<MarketOrderDTO> itemHandler = ProcessItem;
        while (!stoppingToken.IsCancellationRequested)
        {
           await SubscribeToTopic(NATS_URL, NATS_TOPIC, itemHandler, stoppingToken);
        }
    }

    protected void ProcessItem(MarketOrderDTO marketOrderItem)
    {

        var dbParams = new NpgsqlParameter[] {
            new ("albion_id", NpgsqlTypes.NpgsqlDbType.Bigint) { Value = marketOrderItem.Id },
            new ("item_id", NpgsqlTypes.NpgsqlDbType.Varchar, 255) { Value = marketOrderItem.ItemTypeId },
            new ("quality_level", NpgsqlTypes.NpgsqlDbType.Smallint) { Value = marketOrderItem.QualityLevel },
            new ("enchantment_level", NpgsqlTypes.NpgsqlDbType.Smallint) { Value = marketOrderItem.EnchantmentLevel },
            new ("price", NpgsqlTypes.NpgsqlDbType.Bigint) { Value = marketOrderItem.UnitPriceSilver },
            new ("initial_amount", NpgsqlTypes.NpgsqlDbType.Integer) { Value = marketOrderItem.Amount },
            new ("amount", NpgsqlTypes.NpgsqlDbType.Integer) { Value = marketOrderItem.Amount },
            new ("auction_type", NpgsqlTypes.NpgsqlDbType.Varchar, 255) { Value = marketOrderItem.AuctionType },
            new ("expires", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = DateTime.Parse(marketOrderItem.Expires) },
            new ("location", NpgsqlTypes.NpgsqlDbType.Smallint) { Value = marketOrderItem.LocationId }
        };

        AddDataToDB(CommandText, dbParams);
    }
}
