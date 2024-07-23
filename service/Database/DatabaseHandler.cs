using System.Data;
using System.Runtime.CompilerServices;
using Npgsql;

namespace service;

public class DatabaseHandler : IDatabaseHandler, IDisposable
{
    private readonly string _connectionString;
    private NpgsqlConnection _connection;
    protected readonly ILogger<DatabaseHandler> _logger;

    private List<(string, NpgsqlParameter[])> _batchQueue;
    private const int MaxBatchSize = 100;
    private readonly object _batchQueueLock = new();

    private int _dbWriteCount = 0;

    public DatabaseHandler(ILogger<DatabaseHandler> logger, IConfiguration configuration)
    {
        _logger = logger;
        _connectionString = configuration.GetConnectionString("PostgresDb") ?? throw new MissingFieldException("Missing the Postgres connection string");;
        _connection = new NpgsqlConnection(_connectionString);
        _batchQueue = [];
    }

    public async Task OpenConnectionAsync()
    {
        if (_connection.State == ConnectionState.Closed)
        {
            await _connection.OpenAsync();
        }
    }

    public async Task CloseConnectionAsync()
    {
        if (_connection.State == ConnectionState.Open)
        {
            await _connection.CloseAsync();
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void AddToBatch(string query, params NpgsqlParameter[] parameters) {
        lock (_batchQueueLock) {
            _batchQueue.Add((query, parameters));
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public int TryFlushBatch() {
        if (MaxBatchSize > _batchQueue.Count) {
            return 0;
        }

        lock (_batchQueueLock) {
            var batchCommands = new List<NpgsqlBatchCommand>();

            foreach (var query in _batchQueue) {
                var cmd = new NpgsqlBatchCommand(query.Item1);
                cmd.Parameters.AddRange(query.Item2);
                batchCommands.Add(cmd);
            }

            using var batch = new NpgsqlBatch(_connection);
            foreach (var cmd in batchCommands) {
                batch.BatchCommands.Add(cmd);
            }

            var rowsReturned = batch.ExecuteNonQuery();
            _dbWriteCount += _batchQueue.Count;
            _logger.LogInformation("Items written to db: {Count} ", _dbWriteCount);
            _batchQueue = [];
            
            return rowsReturned;
        }
    }

    public async Task<int> ExecuteNonQueryAsync(string query, params NpgsqlParameter[] parameters)
    {
        await OpenConnectionAsync();

        using (var cmd = new NpgsqlCommand(query, _connection))
        {
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }

            var res = await cmd.ExecuteNonQueryAsync();
            await CloseConnectionAsync();
            return res;
        }
    }

    public async Task<object?> ExecuteScalarAsync(string query, params NpgsqlParameter[] parameters)
    {
        await OpenConnectionAsync();

        using (var cmd = new NpgsqlCommand(query, _connection))
        {
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }
            return await cmd.ExecuteScalarAsync();
        }
    }

    public async Task<DataTable> ExecuteQueryAsync(string query, params NpgsqlParameter[] parameters)
    {
        await OpenConnectionAsync();

        using (var cmd = new NpgsqlCommand(query, _connection))
        {
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }

            using (var adapter = new NpgsqlDataAdapter(cmd))
            {
                var dataTable = new DataTable();
                await Task.Run(() => adapter.Fill(dataTable));
                await CloseConnectionAsync();
                return dataTable;
            }
        }

    }

    public void Dispose()
    {
        if (_connection != null)
        {
            _connection.Dispose();
            _connection = null;
        }
    }
}
