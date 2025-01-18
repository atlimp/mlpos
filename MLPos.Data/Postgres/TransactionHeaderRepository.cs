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

        private TransactionSummary MapToTransactionSummary(NpgsqlDataReader reader)
        {
            return new TransactionSummary()
            {
                Id = reader.GetInt64(0),
                PosClientId = reader.GetInt64(1),
                CustomerName = reader.GetSafeString(2),
                CustomerImage = reader.GetSafeString(3),
                TotalAmount = reader.GetDecimal(4),
            };
        }

        public async Task<TransactionSummary> GetTransactionSummaryAsync(long id, long posClientId)
        {
            IEnumerable<TransactionSummary> transactionHeaders = await SqlHelper.ExecuteQuery(_connectionString,
                            @"SELECT
	                            t1.id,
	                            t1.posclient_id,
	                            t2.name,
	                            t2.image,
	                            coalesce(sum(t3.amount), 0) total_amount
                            FROM TRANSACTIONHEADER t1
	                            join CUSTOMER t2 on t1.customer_id = t2.id
	                            join TRANSACTIONLINE t3 on t1.id = t3.transaction_id and t1.posclient_id = t3.posclient_id
	                            WHERE 
		                            t1.id = @id
		                            AND t1.posclient_id = @posclient_id
	                            GROUP BY
		                            t1.id, t1.posclient_id, t2.name, t2.image",
                            MapToTransactionSummary,
                            new Dictionary<string, object>() { ["@id"] = id, ["@posclient_id"] = posClientId }
                        );

            if (transactionHeaders.Any())
            {
                return transactionHeaders.First();
            }

            return null;
        }

        public async Task<IEnumerable<TransactionSummary>> GetAllTransactionSummaryAsync(long posClientId)
        {
            return await SqlHelper.ExecuteQuery(_connectionString,
                            @"SELECT
	                            t1.id,
	                            t1.posclient_id,
	                            t2.name,
	                            t2.image,
	                            coalesce(sum(t3.amount), 0) total_amount
                            FROM TRANSACTIONHEADER t1
	                            join CUSTOMER t2 on t1.customer_id = t2.id
	                            left outer join TRANSACTIONLINE t3 on t1.id = t3.transaction_id and t1.posclient_id = t3.posclient_id
	                            WHERE 
		                            t1.posclient_id = @posclient_id
	                            GROUP BY
		                            t1.id, t1.posclient_id, t2.name, t2.image",
                            MapToTransactionSummary,
                            new Dictionary<string, object>() { ["@posclient_id"] = posClientId }
                        );
        }
    }
}
