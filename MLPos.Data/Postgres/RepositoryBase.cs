using MLPos.Core.Interfaces.Repositories;
using MLPos.Data.Postgres.Helpers;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Data.Postgres
{
    public abstract class RepositoryBase : IBaseRepository
    {
        private DbContext _dbContext = null;
        private readonly string _connectionString;
        public RepositoryBase(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void SetDBContext(IDbContext dbContext)
        {
            _dbContext = dbContext as DbContext;
        }

        protected async Task<IEnumerable<T>> ExecuteQuery<T>(string query, Func<NpgsqlDataReader, T> mapper, Dictionary<string, object>? parameters = null)
        {
            if (_dbContext == null)
            {
                _dbContext = new DbContext(_connectionString);
            }

            return await SqlHelper.ExecuteQuery(_dbContext.Connection as NpgsqlConnection, _dbContext.Transaction as NpgsqlTransaction, query, mapper, parameters);
        }
        protected async Task ExecuteNonQuery(string query, Dictionary<string, object>? parameters = null)
        {
            if (_dbContext == null)
            {
                _dbContext = new DbContext(_connectionString);
            }

            await SqlHelper.ExecuteNonQuery(_dbContext.Connection as NpgsqlConnection, _dbContext.Transaction as NpgsqlTransaction, query, parameters);
        }
    }
}
