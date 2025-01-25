using MLPos.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Core.Interfaces.Repositories
{
    public interface IInvoiceHeaderRepository : IBaseRepository
    {
        public Task<InvoiceHeader> CreateInvoiceHeaderAsync(InvoiceHeader invoiceHeader);
    }
}
