using MLPos.Core.Exceptions;
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
    public class InventoryRepository : RepositoryBase, IInventoryRepository
    {
        public InventoryRepository(string connectionString) : base(connectionString) { }

        public async Task CreateInventoryTransactionAsync(InventoryTransaction transaction)
        {
            await this.ExecuteNonQuery(@"INSERT INTO INVENTORYTRANSACTION(type, transaction_id, posclient_id, line_id, product_id, quantity)
                                            VALUES(@type, @transaction_id, @posclient_id, @line_id, @product_id, @quantity)",
                                            new Dictionary<string, object>()
                                            {
                                                ["@type"] = (int)transaction.Type,
                                                ["@transaction_id"] = transaction.TransactionId,
                                                ["@posclient_id"] = transaction.PosClientId,
                                                ["@line_id"] = transaction.TransactionLineId,
                                                ["@product_id"] = transaction.ProductId,
                                                ["@quantity"] = transaction.Quantity
                                            });
        }

        public async Task<int> GetProductInventoryStatusAsync(long productId)
        {
            IEnumerable<int> status = await this.ExecuteQuery(
                "SELECT current_balance FROM INVENTORYBALANCES WHERE product_id = @product_id",
                (NpgsqlDataReader r) => { return r.GetInt32(0); },
                new Dictionary<string, object>() { ["@product_id"] = productId }
            );

            if (status.Any())
            {
                return status.First();
            }

            return 0;
        }

        public async Task<IEnumerable<Tuple<long, int>>> GetProductInventoryStatusAsync()
        {
            return await this.ExecuteQuery(
                "SELECT product_id, current_balance FROM INVENTORYBALANCES",
                (NpgsqlDataReader r) => { return new Tuple<long, int>(r.GetSafeInt64(0), r.GetInt32(1)); }
            );
        }
    }
}
