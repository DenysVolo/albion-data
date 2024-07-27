public interface INatsStatsTracker {
    public int RollingAveragePeriods { get; }
    public void AddMessageCount (string topicName, int count);
    public void AddDbUpsertCount(int count);
    public (Dictionary<string, Tuple<int, double>>, Tuple<int, double>) GetStats();
}