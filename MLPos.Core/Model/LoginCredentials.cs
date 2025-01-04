using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Core.Model
{
    public record LoginCredentials
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
