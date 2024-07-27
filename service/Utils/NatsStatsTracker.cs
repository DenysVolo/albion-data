using System.Runtime.CompilerServices;

public class NatsStatsTracker : INatsStatsTracker
{
    private Dictionary<string,int[]> MessagesRecieved;
    private int[] DbRowsUpserted;
    public int RollingAveragePeriods { get; }
    private int PeriodPointer = 0;

    private readonly object _progressPeriodLock = new();

    public NatsStatsTracker(int rollingAveragePeriods = 12) {
        RollingAveragePeriods = rollingAveragePeriods;
        MessagesRecieved = [];
        DbRowsUpserted = new int[RollingAveragePeriods];
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void AddMessageCount (string topicName, int count) {
        lock (_progressPeriodLock) {
            if (MessagesRecieved.TryGetValue(topicName, out int[]? value)) {
                value[PeriodPointer] += count;
            }
            else {
                RegisterTopic(topicName);
                MessagesRecieved[topicName][PeriodPointer] += count;
            }
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void AddDbUpsertCount(int count) {
        lock (_progressPeriodLock) {
            DbRowsUpserted[PeriodPointer] += count;
        }
    }

    public (Dictionary<string, Tuple<int, double>>, Tuple<int, double>) GetStats() {
         lock (_progressPeriodLock) {
            var stats = (GetMessagesRecievedInPeriodWithAverage(), GetDbRowsUpsertedInPeriodWithAverage());
            ProgressPeriod();
            return stats;
         }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private void RegisterTopic (string topicName) {
        MessagesRecieved.Add(topicName, new int[RollingAveragePeriods]);
    }

    private Dictionary<string, Tuple<int, double>> GetMessagesRecievedInPeriodWithAverage() {
        return MessagesRecieved.ToDictionary(
            x => x.Key, 
            x => new Tuple<int,double>(x.Value[PeriodPointer], x.Value.Average()));
    }

    private Tuple<int,double> GetDbRowsUpsertedInPeriodWithAverage() {
        return new Tuple<int,double>(DbRowsUpserted[PeriodPointer], DbRowsUpserted.Average());
    }

    private void ProgressPeriod() {
        PeriodPointer = (PeriodPointer + 1) % RollingAveragePeriods;
        foreach (var topic in MessagesRecieved.Keys) {
            MessagesRecieved[topic][PeriodPointer] = 0;
        }
        DbRowsUpserted[PeriodPointer] = 0;
    }
}