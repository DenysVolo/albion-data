public class StatsWorker(ILogger<StatsWorker> logger, INatsStatsTracker statsTracker) : BackgroundService {

    private readonly ILogger<StatsWorker> _logger = logger;
    private readonly INatsStatsTracker _statsTracker = statsTracker;
    private const int PeriodMinutes = 5;

    private const int TopicNameWitdth = 25;
    private const int LastPeriodWidth = 20;
    private const int AverageWidth = 20;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        _logger.LogInformation("Service stat collection starting");
        while (!stoppingToken.IsCancellationRequested)
        {
            var (messageStats, dbStats) = _statsTracker.GetStats();

            LogBuffer();
            _logger.LogInformation($"{"Name",-TopicNameWitdth} | {"Last " + PeriodMinutes + " Mins Events",-LastPeriodWidth} | {"Last " + (PeriodMinutes * _statsTracker.RollingAveragePeriods) + " Mins Average",-AverageWidth}");
            LogBuffer();
            foreach (var messageStat in messageStats) {
                _logger.LogInformation($"{messageStat.Key,-TopicNameWitdth} | {messageStat.Value.Item1,-LastPeriodWidth} | {messageStat.Value.Item2,-AverageWidth:F2}");
            }
            _logger.LogInformation($"{"Database Upserts",-TopicNameWitdth} | {dbStats.Item1,-LastPeriodWidth} | {dbStats.Item2,-AverageWidth:F2}");
            LogBuffer();
            await Task.Delay(TimeSpan.FromMinutes(PeriodMinutes), stoppingToken);
        }
    }

    private void LogBuffer() {
        _logger.LogInformation( new string('-', TopicNameWitdth + LastPeriodWidth + AverageWidth + 6));
    }
}