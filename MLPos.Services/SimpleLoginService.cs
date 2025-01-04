using MLPos.Core.Interfaces.Services;
using MLPos.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Services
{
    public class SimpleLoginService : ILoginService
    {
        public async Task<User> ValidateUser(LoginCredentials credentials)
        {
            if (credentials.Username == "admin" && credentials.Password == "admin")
            {
                return new User
                {
                    Name = "admin",
                    Username = "admin",
                    Email = "admin@example.com",
                };
            }

            throw new InvalidCredentialException();
        }
    }
}
