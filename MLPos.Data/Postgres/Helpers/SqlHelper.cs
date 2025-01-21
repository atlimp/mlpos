using Npgsql;
using System.Data.Common;

namespace MLPos.Data.Postgres.Helpers;

public class SqlHelper
{
    public static async Task<IEnumerable<T>> ExecuteQuery<T>(string connectionString, string query,
        Func<NpgsqlDataReader, T> mapper, Dictionary<string, object>? parameters = null)
    {
        using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();
            return await ExecuteQuery(conn, null, query, mapper, parameters);
        }
    }
    public static async Task ExecuteNonQuery(string connectionString, string query,
        Dictionary<string, object>? parameters = null)
    {
        using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();
            await ExecuteNonQuery(conn, null, query, parameters);
        }
    }
    public static async Task<IEnumerable<T>> ExecuteQuery<T>(NpgsqlTransaction transaction, string query,
        Func<NpgsqlDataReader, T> mapper, Dictionary<string, object>? parameters = null)
    {
        return await ExecuteQuery(transaction.Connection, transaction, query, mapper, parameters);
    }
    public static async Task ExecuteNonQuery(NpgsqlTransaction transaction, string query,
        Dictionary<string, object>? parameters = null)
    {
        await ExecuteNonQuery(transaction.Connection, transaction, query, parameters);
    }



    private static async Task<IEnumerable<T>> ExecuteQuery<T>(NpgsqlConnection connection, NpgsqlTransaction? transaction, string query,
        Func<NpgsqlDataReader, T> mapper, Dictionary<string, object>? parameters = null)
    {
        List<T> result = new List<T>();
        
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = query;

            if (transaction != null)
            {
                cmd.Transaction = transaction;
            }
                
            if (parameters != null)
            {
                foreach (string key in parameters.Keys)
                {
                    if (parameters[key] == null)
                    {
                        cmd.Parameters.AddWithValue(key, DBNull.Value);
                        continue;
                    }
                        
                    cmd.Parameters.AddWithValue(key, parameters[key]);
                }
            }

            var reader = await cmd.ExecuteReaderAsync();

            while (reader.Read())
            {
                result.Add(mapper(reader));
            }
                
        }

        return result;
    }
    
    private static async Task ExecuteNonQuery(NpgsqlConnection connection, NpgsqlTransaction? transaction, string query, Dictionary<string, object>? parameters = null)
    {
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = query;

            if (transaction != null)
            {
                cmd.Transaction = transaction;
            }

            if (parameters != null)
            {
                foreach (string key in parameters.Keys)
                {
                    cmd.Parameters.AddWithValue(key, parameters[key]);
                }
            }

            var reader = await cmd.ExecuteNonQueryAsync();
        }
    }
}