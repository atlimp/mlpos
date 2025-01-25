using MLPos.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Core.Model
{
    public class InvoiceHeader : Entity
    {
        public InvoiceStatus Status { get; set; }
        public Customer Customer { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public Period Period { get; set; }
        public IEnumerable<InvoiceLine> Lines { get; set; } = Enumerable.Empty<InvoiceLine>();
    }
}
