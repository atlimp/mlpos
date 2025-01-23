using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Data.Postgres.Helpers
{
    public static class NpgsqlDataReaderExtensions
    {
        public static long GetSafeInt64(this NpgsqlDataReader reader, int columnIndex)
        {
            if (reader.IsDBNull(columnIndex))
            {
                return -1;
            }

            return reader.GetInt64(columnIndex);
        }

        public static string GetSafeString(this NpgsqlDataReader reader, int columnIndex)
        {
            if (reader.IsDBNull(columnIndex))
            {
                return string.Empty;
            }

            return reader.GetString(columnIndex);
        }
        public static bool SafeIsDBNull(this NpgsqlDataReader reader, int columnIndex)
        {
            try
            {
                return reader.IsDBNull(columnIndex);
            }
            catch (Exception e)
            {
                return true;
            }
        }
    }
}
