using MLPos.Core.Interfaces.Repositories;
using MLPos.Core.Model;
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

        public Task<int> GetProductInventoryStatus(long productId)
        {
            throw new NotImplementedException();
        }
    }
}
