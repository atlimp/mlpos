using MLPos.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Core.Interfaces.Services
{
    public interface ILoginService
    {
        public Task<User> ValidateUser(LoginCredentials credentials);
    }
}
