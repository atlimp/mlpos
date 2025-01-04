using MLPos.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Core.Interfaces.Repositories
{
    public interface ITransactionHeaderRepository
    {
        public Task<TransactionHeader?> GetTransactionHeaderAsync(long id);
        public Task<IEnumerable<TransactionHeader>> GetAllTransactionHeaderAsync();
        public Task<TransactionHeader?> CreateTransactionHeaderAsync(TransactionHeader transactionHeader);
        public Task DeleteTransactionHeaderAsync(long id);

    }
}
