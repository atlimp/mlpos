using MLPos.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Core.Interfaces.Repositories
{
    public interface ITransactionLineRepository
    {
        public Task<TransactionLine?> GetTransactionLineAsync(long transactionId, long posClientId, long lineId);
        public Task<TransactionLine?> CreateTransactionLineAsync(long transactionId, long posClientId, TransactionLine line);
        public Task<IEnumerable<TransactionLine>> GetAllTransactionLinesAsync(long transactionId, long posClientId);
        public Task DeleteTransactionLineAsync(long transactionId, long posClientId, long lineId);
    }
}
