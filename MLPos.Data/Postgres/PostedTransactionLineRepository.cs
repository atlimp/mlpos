using MLPos.Core.Interfaces.Repositories;
using MLPos.Core.Model;
using MLPos.Data.Postgres.Helpers;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Data.Postgres
{
    public class PostedTransactionLineRepository : RepositoryBase, IPostedTransactionLineRepository
    {
        public PostedTransactionLineRepository(string connectionString) : base(connectionString) { }

        public async Task<PostedTransactionLine> CreatePostedTransactionLineAsync(long transactionId, long posClientId, PostedTransactionLine line)
        {
            IEnumerable<PostedTransactionLine> transactionLines = await this.ExecuteQuery(
                @"INSERT INTO POSTEDTRANSACTIONLINE(transaction_id, posclient_id, product_id, amount, quantity)
                    VALUES (@transaction_id, @posclient_id, @product_id, @amount, @quantity)
                    RETURNING id, product_id, amount, quantity, date_inserted, date_updated",
                MapToPostedTransactionLine,
                new Dictionary<string, object>()
                {
                    ["@transaction_id"] = transactionId,
                    ["@posclient_id"] = posClientId,
                    ["@product_id"] = line.Product.Id,
                    ["@amount"] = line.Amount,
                    ["@quantity"] = line.Quantity
                }
            );

            if (transactionLines.Any())
            {
                return transactionLines.First();
            }

            return null;
        }

        private PostedTransactionLine MapToPostedTransactionLine(NpgsqlDataReader reader)
        {
            return new PostedTransactionLine()
            {
                Id = reader.GetSafeInt64(0),
                Product = new Product
                {
                    Id = reader.GetSafeInt64(1)
                },
                Amount = reader.GetDecimal(2),
                Quantity = reader.GetDecimal(3),
                DateInserted = reader.GetDateTime(4),
                DateUpdated = reader.GetDateTime(5),
            };
        }
    }
}
