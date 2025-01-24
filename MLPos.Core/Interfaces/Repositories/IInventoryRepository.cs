using MLPos.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Core.Interfaces.Repositories
{
    public interface IInventoryRepository : IBaseRepository
    {
        public Task CreateInventoryTransactionAsync(InventoryTransaction transaction);
        public Task<int> GetProductInventoryStatusAsync(long productId);
        public Task<IEnumerable<Tuple<long, int>>> GetProductInventoryStatusAsync();
    }
}
