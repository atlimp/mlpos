using MLPos.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Core.Model
{
    public class PostedTransactionHeader : Entity
    {
        public long PosClientId { get; set; }
        public long? InvoiceId { get; set; }
        public TransactionStatus Status { get; set; }
        public Customer Customer { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public IEnumerable<PostedTransactionLine> Lines { get; set; } = Enumerable.Empty<PostedTransactionLine>();
        public decimal TotalAmount => Lines.Sum(x => x.Amount);
    }
}
