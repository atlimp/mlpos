using Npgsql;

namespace MLPos.Data.Postgres.Helpers;

public class SqlHelper
{
    public static async Task<IEnumerable<T>> ExecuteQuery<T>(string connectionString, string query,
        Func<NpgsqlDataReader, T> mapper, Dictionary<string, object>? parameters = null)
    {
        List<T> result = new List<T>();
        
        using (var con = new NpgsqlConnection(connectionString))
        {
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = query;
                
                if (parameters != null)
                {
                    foreach (string key in parameters.Keys)
                    {
                        cmd.Parameters.AddWithValue(key, parameters[key]);
                    }
                }
                
                con.Open();

                var reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    result.Add(mapper(reader));
                }
                
            }
        }

        return result;
    }
    
    public static async Task ExecuteNonQuery(string connectionString, string query, Dictionary<string, object>? parameters = null)
    {
        using (var con = new NpgsqlConnection(connectionString))
        {
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = query;
                
                if (parameters != null)
                {
                    foreach (string key in parameters.Keys)
                    {
                        cmd.Parameters.AddWithValue(key, parameters[key]);
                    }
                }
                
                con.Open();

                var reader = await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}