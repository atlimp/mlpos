using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Core.Model
{
    public record Image
    {
        public required Stream ImageStream { get; set; }
        public required string FileName { get; set; }
        public required string Path { get; set; }
    }
}
