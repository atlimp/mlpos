using MLPos.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Core.Model
{
    public class PostedTransactionQueryFilter : QueryFilter
    {
        public Period? Period { get; set; }
        public TransactionStatus? Status { get; set; }
        public long? CustomerId { get; set; }
        public long? PaymentMethodId { get; set; }

    }
}
