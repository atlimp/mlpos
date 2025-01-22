using MLPos.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Core.Interfaces.Repositories
{
    public interface IUserRepository : IBaseRepository
    {
        public Task<User> CreateUserAsync(User user);
        public Task<User> UpdateUserAsync(User user);
        public Task DeleteUserAsync(long id);
        public Task<User> GetUserByIdAsync(long id);
        public Task<User> GetUserByUsernameAsync(string username);
    }
}
