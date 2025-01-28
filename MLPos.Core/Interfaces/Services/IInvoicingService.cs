using MLPos.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Core.Interfaces.Services
{
    public interface IInvoicingService
    {
        public Task<InvoiceHeader> GenerateInvoice(PostedTransactionHeader transactionHeader);
        public Task<InvoiceHeader> GenerateInvoice(Customer customer, PaymentMethod paymentMethod, Period period);
        public Task<InvoiceHeader> MarkAsPaid(long invoiceId);
        public Task<IEnumerable<InvoiceHeader>> GetInvoicesAsync(int limit, int offset);
        public Task<IEnumerable<InvoiceHeader>> GetInvoicesAsync(InvoiceQueryFilter queryFilter, int limit, int offset);
        public Task<InvoiceHeader> GetInvoiceAsync(long invoiceId);
        public Task<Tuple<bool, IEnumerable<ValidationError>>> ValidateInvoiceGeneration(Customer customer, PaymentMethod paymentMethod, Period period);

    }
}
