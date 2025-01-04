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
        public Task<TransactionHeader> GetTransactionHeaderAsync(long id);
        public Task<TransactionHeader> CreateTransactionAsync(Customer customer);
        public Task<TransactionHeader> AddItemAsync(TransactionHeader transactionHeader, Product product, decimal qty);
        public Task<TransactionHeader> RemoveItemAsync(long transactionId, long lineId);
        public Task DeleteTransactionAsync(long id);
        public Task PostTransactionAsync(TransactionHeader transactionHeader, PaymentMethod paymentMethod);
    }
}
