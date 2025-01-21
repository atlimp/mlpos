using MLPos.Core.Interfaces.Repositories;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Data.Postgres
{
    public class DbContext : IDbContext
    {
        private readonly string _connectionString;
        private NpgsqlConnection _connection;
        private NpgsqlTransaction? _transaction;
        public DbContext(string connectionString)
        {
            _connectionString = connectionString;
            _connection = new NpgsqlConnection(connectionString);
            _connection.Open();
            _transaction = null;
        }

        public DbConnection Connection => _connection;

        public DbTransaction Transaction => _transaction;

        public DbTransaction BeginDbTransaction()
        {
            _transaction = _connection.BeginTransaction();
            return _transaction;
        }

        public void CommitDbTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Commit();
                _transaction.Dispose();
                _transaction = null;
            }
        }
        public void RollbackDbTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction.Dispose();
                _transaction = null;
            }
        }
        public void Dispose()
        {
            if (_transaction != null)
            {
                Transaction.Dispose();
            }

            if (_connection != null)
            {
                _connection.Dispose();
            }
        }
    }
}
