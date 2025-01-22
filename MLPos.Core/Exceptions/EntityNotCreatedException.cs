using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Core.Exceptions
{
    public class EntityNotCreatedException : Exception
    {
        public EntityNotCreatedException() { }
        public EntityNotCreatedException(Type type) : base($"Could not create {type}") { }

        public EntityNotCreatedException(string message) : base(message) { }

        public EntityNotCreatedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
