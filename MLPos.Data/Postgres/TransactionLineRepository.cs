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
    public class TransactionLineRepository : ITransactionLineRepository
    {
        private readonly string _connectionString;

        public TransactionLineRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<TransactionLine?> CreateTransactionLineAsync(long transactionId, TransactionLine line)
        {
            IEnumerable<TransactionLine> transactionLines = await SqlHelper.ExecuteQuery(_connectionString,
            "INSERT INTO TRANSACTIONLINE(transaction_id, product_id, amount, quantity) VALUES (@transaction_id, @product_id, @amount, @quantity) RETURNING id, product_id, amount, quantity, date_inserted, date_updated",
            MapToTransactionLine,
                new Dictionary<string, object>() { ["@transaction_id"] = transactionId, ["@product_id"] = line.Product.Id, ["@amount"] = line.Amount, ["@quantity"] = line.Quantity }
            );

            if (transactionLines.Any())
            {
                return transactionLines.First();
            }

            return null;
        }

        public async Task DeleteTransactionLineAsync(long transactionId, long lineId)
        {
            await SqlHelper.ExecuteNonQuery(_connectionString, "DELETE FROM TRANSACTIONLINE WHERE id=@id AND transaction_id = @transaction_id", new Dictionary<string, object>() { ["@id"] = lineId, ["@transaction_id"] = transactionId });
        }

        public async Task<IEnumerable<TransactionLine>> GetAllTransactionLinesAsync(long transactionId)
        {
            return await SqlHelper.ExecuteQuery(_connectionString,
                "SELECT id, product_id, amount, quantity, date_inserted, date_updated FROM TRANSACTIONLINE WHERE transaction_id = @transaction_id",
                MapToTransactionLine,
                new Dictionary<string, object>() { ["@transaction_id"] = transactionId }
            );
        }

        public async Task<TransactionLine?> GetTransactionLineAsync(long transactionId, long lineId)
        {
            IEnumerable<TransactionLine> transactionLines = await SqlHelper.ExecuteQuery(_connectionString,
                "SELECT id, product_id, amount, quantity, date_inserted, date_updated FROM TRANSACTIONLINE WHERE transaction_id = @transaction_id AND id = @id",
                MapToTransactionLine,
                new Dictionary<string, object>() { ["@transaction_id"] = transactionId, ["@id"] = lineId }
            );

            if (transactionLines.Any())
            {
                return transactionLines.First();
            }

            return null;
        }

        private TransactionLine MapToTransactionLine(NpgsqlDataReader reader)
        {
            return new TransactionLine()
            {
                Id = reader.GetInt64(0),
                Product = new Product
                {
                    Id = reader.GetInt64(1)
                },
                Amount = reader.GetDecimal(2),
                Quantity = reader.GetDecimal(3),
                DateInserted = reader.GetDateTime(4),
                DateUpdated = reader.GetDateTime(5),
            };
        }
    }
}
