using MLPos.Core.Interfaces.Repositories;
using MLPos.Core.Model;
using MLPos.Data.Postgres.Helpers;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Data.Postgres
{
    public class TransactionHeaderRepository : ITransactionHeaderRepository
    {
        private readonly string _connectionString;
        public TransactionHeaderRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<TransactionHeader?> CreateTransactionHeaderAsync(TransactionHeader transactionHeader)
        {
            IEnumerable<TransactionHeader> transactionHeaders = await SqlHelper.ExecuteQuery(_connectionString,
            "INSERT INTO TRANSACTIONHEADER(customer_id) VALUES (@customer_id) RETURNING id, customer_id, date_inserted, date_updated",
            MapToTransactionHeader,
                new Dictionary<string, object>() { ["@customer_id"] = transactionHeader.Customer.Id }
            );

            if (transactionHeaders.Any())
            {
                return transactionHeaders.First();
            }

            return null;
        }

        public async Task DeleteTransactionHeaderAsync(long id)
        {
            await SqlHelper.ExecuteNonQuery(_connectionString, "DELETE FROM TRANSACTIONHEADER WHERE id=@id", new Dictionary<string, object>() { ["@id"] = id });
        }

        public async Task<TransactionHeader?> GetTransactionHeaderAsync(long id)
        {
            IEnumerable<TransactionHeader> transactionHeaders = await SqlHelper.ExecuteQuery(_connectionString,
            "SELECT id, customer_id, date_inserted, date_updated FROM TRANSACTIONHEADER WHERE id = @id",
            MapToTransactionHeader,
                new Dictionary<string, object>() { ["@id"] = id }
            );

            if (transactionHeaders.Any())
            {
                return transactionHeaders.First();
            }

            return null;
        }

        public async Task<IEnumerable<TransactionHeader>> GetAllTransactionHeaderAsync()
        {
            return await SqlHelper.ExecuteQuery(_connectionString,
                "SELECT id, customer_id, date_inserted, date_updated FROM TRANSACTIONHEADER",
                MapToTransactionHeader
            );
        }

        private TransactionHeader MapToTransactionHeader(NpgsqlDataReader reader)
        {
            return new TransactionHeader()
            {
                Id = reader.GetInt64(0),
                Customer = new Customer
                {
                    Id = reader.GetInt64(1)
                },
                DateInserted = reader.GetDateTime(2),
                DateUpdated = reader.GetDateTime(3),
            };
        }
    }
}
