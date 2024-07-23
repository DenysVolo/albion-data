using Npgsql;
using System.Data;

public class DatabaseHandler : IDatabaseHandler
{
    private readonly string _connectionString;
    private NpgsqlConnection _connection;

    public DatabaseHandler(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("PostgresDb") ?? throw new MissingFieldException("Missing the Postgres connection string");
        _connection = new NpgsqlConnection(_connectionString);
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

    public async Task ExecuteQueryAsync(string query, Action<IDataReader> handleData)
    {
        await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(query, _connection);
        await using var reader = await cmd.ExecuteReaderAsync();
        handleData(reader);
    }

    public async Task ExecuteNonQueryAsync(string query, NpgsqlParameter[] parameters)
    {
        await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(query, _connection);
        cmd.Parameters.AddRange(parameters);
        await cmd.ExecuteNonQueryAsync();
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}

