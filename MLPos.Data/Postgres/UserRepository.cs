using MLPos.Core.Exceptions;
using MLPos.Core.Interfaces.Repositories;
using MLPos.Core.Model;
using MLPos.Data.Postgres.Helpers;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Data.Postgres
{
    public class UserRepository : RepositoryBase, IUserRepository
    {
        public UserRepository(string connectionString) : base(connectionString) { }

        public async Task<User> CreateUserAsync(User user)
        {
            IEnumerable<User> users = await this.ExecuteQuery(
                @"INSERT INTO MLUSER(username, hashed_password, email, image)
                        VALUES(@username, @hashed_password, @email, @image) RETURNING id, username, hashed_password, email, image, date_inserted, date_updated",
                MapToUser,
                new Dictionary<string, object>() { ["@username"] = user.Username, ["@hashed_password"] = user.HashedPassword, ["@email"] = user.Email, ["@image"] = user.Image }
            );

            if (users.Any())
            {
                return users.First();
            }

            throw new EntityNotCreatedException(typeof(User));
        }

        public async Task DeleteUserAsync(long id)
        {
            await this.ExecuteNonQuery("UPDATE MLUSER SET hashed_password='', date_deleted=CURRENT_TIMESTAMP WHERE id=@id", new Dictionary<string, object>() { ["@id"] = id });
        }

        public async Task<User> GetUserByIdAsync(long id)
        {
            IEnumerable<User> users = await this.ExecuteQuery(
                            "SELECT id, username, hashed_password, email, image, date_inserted, date_updated FROM MLUSER WHERE id = @id AND date_deleted IS NULL",
                            MapToUser,
                                new Dictionary<string, object>() { ["@id"] = id }
                            );

            if (users.Any())
            {
                return users.First();
            }

            throw new EntityNotFoundException(typeof(User), id);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            IEnumerable<User> users = await this.ExecuteQuery(
                            "SELECT id, username, hashed_password, email, image, date_inserted, date_updated FROM MLUSER WHERE username = @username AND date_deleted IS NULL",
                            MapToUser,
                                new Dictionary<string, object>() { ["@username"] = username }
                            );

            if (users.Any())
            {
                return users.First();
            }

            throw new EntityNotFoundException(typeof(User), username);
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            IEnumerable<User> users = await this.ExecuteQuery(
                            @"UPDATE MLUSER set username = @username, hashed_password = @hashed_password, email = @email, image = @image
                                WHERE id = @id AND date_deleted IS NULL RETURNING id, username, hashed_password, email, image, date_inserted, date_updated",
                            MapToUser,
                            new Dictionary<string, object>() { ["@id"] = user.Id, ["@username"] = user.Username, ["@hashed_password"] = user.HashedPassword, ["@email"] = user.Email, ["@image"] = user.Image }
                        );

            if (users.Any())
            {
                return users.First();
            }

            throw new EntityNotFoundException(typeof(User), user.Id);
        }

        private User MapToUser(NpgsqlDataReader reader)
        {
            return new User
            {
                Id = reader.GetSafeInt64(0),
                Username = reader.GetSafeString(1),
                HashedPassword = reader.GetSafeString(2),
                Email = reader.GetSafeString(3),
                Image = reader.GetSafeString(4),
                DateInserted = reader.GetDateTime(5),
                DateUpdated = reader.GetDateTime(6),
            };
        }
    }
}
