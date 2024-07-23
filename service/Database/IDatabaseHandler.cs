using System.Data;
using Npgsql;

public interface IDatabaseHandler
{
    Task OpenConnectionAsync();
    Task CloseConnectionAsync();
    public void AddToBatch(string query, params NpgsqlParameter[] parameters);
    public int TryFlushBatch();
    Task<int> ExecuteNonQueryAsync(string query, params NpgsqlParameter[] parameters);
    Task<object?> ExecuteScalarAsync(string query, params NpgsqlParameter[] parameters);
    Task<DataTable> ExecuteQueryAsync(string query, params NpgsqlParameter[] parameters);
}
