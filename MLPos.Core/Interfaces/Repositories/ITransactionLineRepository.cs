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
        public Task<TransactionLine?> GetTransactionLineAsync(long transactionId, long lineId);
        public Task<TransactionLine?> CreateTransactionLineAsync(long transactionId, TransactionLine line);
        public Task<IEnumerable<TransactionLine>> GetAllTransactionLinesAsync(long transactionId);
        public Task DeleteTransactionLineAsync(long transactionId, long lineId);
    }
}
