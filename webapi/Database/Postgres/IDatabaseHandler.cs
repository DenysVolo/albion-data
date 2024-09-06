using Npgsql;
using System.Data;
public interface IDatabaseHandler : IDisposable
{
    Task OpenConnectionAsync();
    Task CloseConnectionAsync();
    Task ExecuteQueryAsync(string query, NpgsqlParameter[] parameters, Action<IDataReader> handleData);
    Task ExecuteNonQueryAsync(string query, NpgsqlParameter[] parameters);
}
