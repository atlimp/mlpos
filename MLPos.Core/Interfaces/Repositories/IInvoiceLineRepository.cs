using MLPos.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Core.Interfaces.Repositories
{
    public interface IInvoiceLineRepository : IBaseRepository
    {
        public Task<InvoiceLine> CreateInvoiceLineAsync(long invoiceId, InvoiceLine line);
    }
}
