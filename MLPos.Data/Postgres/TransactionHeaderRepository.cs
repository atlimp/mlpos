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
                "INSERT INTO TRANSACTIONHEADER(customer_id, posclient_id) VALUES (@customer_id, @posclient_id) RETURNING id, posclient_id, customer_id, date_inserted, date_updated",
                MapToTransactionHeader,
                new Dictionary<string, object>() { ["@posclient_id"] = transactionHeader.PosClientId, ["@customer_id"] = transactionHeader.Customer.Id }
            );

            if (transactionHeaders.Any())
            {
                return transactionHeaders.First();
            }

            return null;
        }

        public async Task DeleteTransactionHeaderAsync(long id, long posClientId)
        {
            await SqlHelper.ExecuteNonQuery(_connectionString, "DELETE FROM TRANSACTIONHEADER WHERE id=@id AND posclient_id = @posclient_id", new Dictionary<string, object>() { ["@id"] = id, ["@posclient_id"] = posClientId });
        }

        public async Task<TransactionHeader?> GetTransactionHeaderAsync(long id, long posClientId)
        {
            IEnumerable<TransactionHeader> transactionHeaders = await SqlHelper.ExecuteQuery(_connectionString,
                "SELECT id, posclient_id, customer_id, date_inserted, date_updated FROM TRANSACTIONHEADER WHERE id = @id AND posclient_id = @posclient_id",
                MapToTransactionHeader,
                new Dictionary<string, object>() { ["@id"] = id, ["@posclient_id"] = posClientId }
            );

            if (transactionHeaders.Any())
            {
                return transactionHeaders.First();
            }

            return null;
        }

        public async Task<IEnumerable<TransactionHeader>> GetAllTransactionHeaderAsync(long posClientId)
        {
            return await SqlHelper.ExecuteQuery(_connectionString,
                "SELECT id, posclient_id, customer_id, date_inserted, date_updated FROM TRANSACTIONHEADER WHERE posclient_id = @posclient_id",
                MapToTransactionHeader,
                new Dictionary<string, object> { ["@posclient_id"] = posClientId }
            );
        }

        private TransactionHeader MapToTransactionHeader(NpgsqlDataReader reader)
        {
            return new TransactionHeader()
            {
                Id = reader.GetInt64(0),
                PosClientId = reader.GetInt64(1),
                Customer = new Customer
                {
                    Id = reader.GetInt64(2)
                },
                DateInserted = reader.GetDateTime(3),
                DateUpdated = reader.GetDateTime(4),
            };
        }
    }
}
