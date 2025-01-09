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
        public Task<TransactionHeader?> GetTransactionHeaderAsync(long id, long posClientId);
        public Task<IEnumerable<TransactionHeader>> GetAllTransactionHeaderAsync(long posClientId);
        public Task<TransactionHeader?> CreateTransactionHeaderAsync(TransactionHeader transactionHeader);
        public Task DeleteTransactionHeaderAsync(long id, long posClientId);
        public Task<TransactionSummary> GetTransactionSummaryAsync(long id, long posClientId);
        public Task<IEnumerable<TransactionSummary>> GetAllTransactionSummaryAsync(long posClientId);

    }
}
