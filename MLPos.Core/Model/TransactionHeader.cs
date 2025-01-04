using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Core.Model
{
    public class TransactionHeader : Entity
    {
        public Customer? Customer { get; set; }
        public IEnumerable<TransactionLine> Lines { get; set; } = Enumerable.Empty<TransactionLine>();
    }
}
