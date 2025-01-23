using MLPos.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Core.Model
{
    public class InventoryTransaction
    {
        public InventoryTransactionType Type { get; set; }
        public long TransactionId { get; set; }
        public long PosClientId { get; set; }
        public long TransactionLineId { get; set; }
        public long ProductId { get; set; }
        public decimal Quantity { get; set; }
    }
}
