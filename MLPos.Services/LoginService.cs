using Microsoft.AspNetCore.Identity;
using MLPos.Core.Exceptions;
using MLPos.Core.Interfaces.Repositories;
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
    public class LoginService : ILoginService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        public LoginService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }
        public async Task<User> ValidateUser(LoginCredentials credentials)
        {
            User user = null;
            try
            {
                user = await _userRepository.GetUserByUsernameAsync(credentials.Username);
            }
            catch (EntityNotFoundException e)
            {
                throw new InvalidCredentialException();
            }

            if (user == null)
            {
                throw new InvalidCredentialException();
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.HashedPassword, credentials.Password);

            if (result == PasswordVerificationResult.Success)
            {
                return user;
            }

            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                var rehashed = _passwordHasher.HashPassword(user, credentials.Password);
                user.HashedPassword = rehashed;
                user = await _userRepository.UpdateUserAsync(user);
                return user;
            }

            throw new InvalidCredentialException();
        }
    }
}
