using MLPos.Core.Exceptions;
using MLPos.Core.Interfaces.Repositories;
using MLPos.Core.Model;
using MLPos.Data.Postgres.Helpers;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Data.Postgres
{
    public class PosClientRepository : IPosClientRepository
    {
        private readonly string _connectionString;
        public PosClientRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<PosClient> GetPosClientAsync(long id)
        {
            IEnumerable<PosClient> posClients = await SqlHelper.ExecuteQuery(_connectionString,
                "SELECT id, name, description, date_inserted, date_updated FROM POSCLIENT WHERE id = @id",
                MapToPosClient,
                new Dictionary<string, object>() { ["@id"] = id }
            );

            if (posClients.Any())
            {
                return posClients.First();
            }

            throw new EntityNotFoundException(typeof(PosClient), id);
        }

        public async Task<IEnumerable<PosClient>> GetPosClientsAsync()
        {
            return await SqlHelper.ExecuteQuery(_connectionString,
                "SELECT id, name, description, date_inserted, date_updated FROM POSCLIENT",
                MapToPosClient);
        }

        public async Task<PosClient> CreatePosClientAsync(PosClient posClient)
        {
            IEnumerable<PosClient> posClients = await SqlHelper.ExecuteQuery(_connectionString,
                @"INSERT INTO POSCLIENT(name, description)
                    VALUES(@name, @description) RETURNING id, name, description, date_inserted, date_updated",
                MapToPosClient,
                new Dictionary<string, object>() { ["@name"] = posClient.Name, ["@description"] = posClient.Description }
            );

            if (posClients.Any())
            {
                return posClients.First();
            }

            throw new EntityNotFoundException(typeof(PosClient), posClient.Id);
        }

        public async Task<PosClient> UpdatePosClientAsync(PosClient posClient)
        {
            IEnumerable<PosClient> posClients = await SqlHelper.ExecuteQuery(_connectionString,
                @"UPDATE POSCLIENT SET name = @name, description = @description, WHERE id = @id RETURNING id, name, description, date_inserted, date_updated",
                MapToPosClient,
                new Dictionary<string, object>() { ["@name"] = posClient.Name, ["@email"] = posClient.Description }
            );

            if (posClients.Any())
            {
                return posClients.First();
            }

            throw new EntityNotFoundException(typeof(PosClient), posClient.Id);
        }

        public async Task DeletePosClientAsync(long id)
        {
            await SqlHelper.ExecuteNonQuery(_connectionString, "DELETE FROM POSCLIENT WHERE id=@id", new Dictionary<string, object>() { ["@id"] = id });
        }

        public async Task<bool> PosClientExistsAsync(long id)
        {
            IEnumerable<PosClient> posClients = await SqlHelper.ExecuteQuery(_connectionString,
                "select id from POSCLIENT where id = @id",
                (reader =>
                    new PosClient()
                    {
                        Id = reader.GetInt32(0),
                    }),
                new Dictionary<string, object>() { ["@id"] = id }
            );

            return posClients.Any();
        }

        private PosClient MapToPosClient(NpgsqlDataReader reader)
        {
            return new PosClient()
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString((1)),
                Description = reader.GetString(2),
                DateInserted = reader.GetDateTime(3),
                DateUpdated = reader.GetDateTime(4),
            };
        }
    }
}
