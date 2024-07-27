using System.Text;
using System.Text.Json;
using NATS.Client;
using Npgsql;

namespace service
{
    public class NatsListenerBase<L>(ILogger<L> logger, IDatabaseHandler databaseHandler, INatsStatsTracker statsTracker) : BackgroundService
    {
        protected readonly ILogger<L> _logger = logger;
        private readonly IDatabaseHandler _dbHandler = databaseHandler;

        private readonly INatsStatsTracker _statsTracker = statsTracker;

        protected async Task SubscribeToTopic<T>(string url, string topic, ProccessItem<T> itemHandler, CancellationToken stoppingToken) {
            await _dbHandler.OpenConnectionAsync();

            ConnectionFactory cf = new();
            Options opts = ConnectionFactory.GetDefaultOptions();
            opts.Url = url;
            using IConnection c = cf.CreateConnection(opts);

            using IAsyncSubscription s = c.SubscribeAsync(topic, (sender, msgArgs) =>
            {
                _statsTracker.AddMessageCount(topic, 1);

                var item = JsonSerializer.Deserialize<T>(msgArgs.Message.Data)!;
                itemHandler(item);
            });

            while (!stoppingToken.IsCancellationRequested) {
                await Task.Delay(5000, stoppingToken);
            }
        }

        protected delegate void ProccessItem<T>(T item);

        protected void AddDataToDB(string commandText, NpgsqlParameter[] dbParams) {
            try {
                _dbHandler.AddToBatch(commandText,dbParams);
                var rowsAdded = _dbHandler.TryFlushBatch();
            }
            catch (Exception ex) {
                _logger.LogError("Exception when executing query to db. Query: {Query} \nParams: {Params}  \nException: {Message} : {} \nStack trace: {Trace}", commandText, ToReadableString(dbParams), ex.Message, ex.InnerException, ex.StackTrace);
            }
        }

        private static string ToReadableString(NpgsqlParameter[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
                return "No parameters";

            var stringBuilder = new StringBuilder();
            foreach (var param in parameters)
            {
                stringBuilder.AppendLine($"{param.ParameterName}: {param.Value} ({param.NpgsqlDbType})");
            }
            return stringBuilder.ToString();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken) {
            throw new NotImplementedException("Not implemented in base class");
        }
    }
}
