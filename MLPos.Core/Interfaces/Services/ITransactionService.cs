using MLPos.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Core.Interfaces.Services
{
    public interface ITransactionService
    {
        public Task<TransactionHeader> GetTransactionHeaderAsync(long id, long posClientId);
        public Task<TransactionHeader> CreateTransactionAsync(long posClientId, Customer customer);
        public Task<IEnumerable<TransactionHeader>> GetActiveTransactionsAsync(long posClientId);
        public Task<TransactionHeader> AddItemAsync(TransactionHeader transactionHeader, Product product, decimal qty);
        public Task<TransactionHeader> RemoveItemAsync(long transactionId, long posClientId, long lineId);
        public Task DeleteTransactionAsync(long id, long posClientId);
        public Task PostTransactionAsync(TransactionHeader transactionHeader, PaymentMethod paymentMethod);
    }
}
