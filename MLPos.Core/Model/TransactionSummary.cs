using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Core.Model
{
    public class TransactionSummary
    {
        public long Id { get; set; }
        public long PosClientId { get; set; }
        public decimal TotalAmount { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerImage { get; set; } = string.Empty;
    }
}
