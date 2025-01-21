using MLPos.Core.Interfaces.Repositories;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Data.Postgres
{
    public class DBAccessor : IDBAccessor
    {
        private readonly string _connectionString;
        public DBAccessor(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CommitDbTransaction(DbTransaction transaction)
        {
            transaction.Commit();
            transaction.Connection?.Close();
            transaction.Dispose();
        }

        public DbTransaction CreateDbTransaction()
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            return connection.BeginTransaction();
        }

        public void RollbackDbTransaction(DbTransaction transaction)
        {
            transaction.Rollback();
            transaction.Connection?.Close();
            transaction.Dispose();
        }
    }
}
