using MLPos.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Core.Interfaces.Services
{
    public interface ITransactionPostingService
    {
        public Task<PostedTransactionHeader> PostTransactionAsync(TransactionHeader transactionHeader, PaymentMethod paymentMethod);
    }
}
