using MLPos.Core.Interfaces.Services;
using MLPos.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Services
{
    public class InvoicingService : IInvoicingService
    {
        public Task GenerateInvoice(PostedTransactionHeader transactionHeader)
        {
            throw new NotImplementedException();
        }

        public Task GenerateInvoice(Customer customer, PaymentMethod paymentMethod)
        {
            throw new NotImplementedException();
        }
    }
}
