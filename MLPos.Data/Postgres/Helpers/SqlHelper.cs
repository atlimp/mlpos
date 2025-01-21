using Npgsql;
using System.Data.Common;

namespace MLPos.Data.Postgres.Helpers;

public class SqlHelper
{
    public static async Task<IEnumerable<T>> ExecuteQuery<T>(NpgsqlConnection connection, NpgsqlTransaction? transaction, string query,
        Func<NpgsqlDataReader, T> mapper, Dictionary<string, object>? parameters = null)
    {
        List<T> result = new List<T>();

        using (var cmd = new NpgsqlCommand(query, connection, transaction))
        {
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

            await reader.CloseAsync();
        }

        return result;
    }
    
    public static async Task ExecuteNonQuery(NpgsqlConnection connection, NpgsqlTransaction? transaction, string query, Dictionary<string, object>? parameters = null)
    {
        using (var cmd = new NpgsqlCommand(query, connection, transaction))
        {
            if (parameters != null)
            {
                foreach (string key in parameters.Keys)
                {
                    cmd.Parameters.AddWithValue(key, parameters[key]);
                }
            }

            await cmd.ExecuteNonQueryAsync();
        }
    }
}