using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Core.Model
{
    public class PostedTransactionLine : Entity
    {
        public Product Product { get; set; }
        public decimal Amount { get; set; }
        public decimal Quantity { get; set; }
    }
}
