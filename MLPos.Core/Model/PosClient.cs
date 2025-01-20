using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Core.Model
{
    public class PosClient : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string LoginCode { get; set; }
    }
}
