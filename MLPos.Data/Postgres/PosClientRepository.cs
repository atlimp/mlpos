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
    public class PosClientRepository : RepositoryBase, IPosClientRepository
    {
        public PosClientRepository(string connectionString) : base(connectionString) { }

        public async Task<PosClient> GetPosClientAsync(long id)
        {
            IEnumerable<PosClient> posClients = await this.ExecuteQuery(
                "SELECT id, name, description, logincode, date_inserted, date_updated, date_deleted, visible_on_pos FROM POSCLIENT WHERE id = @id",
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
            return await this.ExecuteQuery(
                "SELECT id, name, description, logincode, date_inserted, date_updated, date_deleted, visible_on_pos FROM POSCLIENT WHERE date_deleted IS NULL ORDER BY NAME",
                MapToPosClient);
        }

        public async Task<PosClient> CreatePosClientAsync(PosClient posClient)
        {
            IEnumerable<PosClient> posClients = await this.ExecuteQuery(
                @"INSERT INTO POSCLIENT(name, description, logincode, visible_on_pos)
                    VALUES(@name, @description, @logincode, @visible_on_pos) RETURNING id, name, description, logincode, date_inserted, date_updated, date_deleted, visible_on_pos",
                MapToPosClient,
                new Dictionary<string, object>() { ["@name"] = posClient.Name, ["@description"] = posClient.Description, ["@logincode"] = posClient.LoginCode, ["@visible_on_pos"] = posClient.VisibleOnPos }
            );

            if (posClients.Any())
            {
                return posClients.First();
            }

            throw new EntityNotFoundException(typeof(PosClient), posClient.Id);
        }

        public async Task<PosClient> UpdatePosClientAsync(PosClient posClient)
        {
            IEnumerable<PosClient> posClients = await this.ExecuteQuery(
                @"UPDATE POSCLIENT SET name = @name, description = @description, logincode = @logincode, visible_on_pos = @visible_on_pos WHERE id = @id AND date_deleted IS NULL RETURNING id, name, description, logincode, date_inserted, date_updated, date_deleted, visible_on_pos",
                MapToPosClient,
                new Dictionary<string, object>() { ["@id"] = posClient.Id, ["@name"] = posClient.Name, ["@description"] = posClient.Description, ["@logincode"] = posClient.LoginCode, ["@visible_on_pos"] = posClient.VisibleOnPos }
            );

            if (posClients.Any())
            {
                return posClients.First();
            }

            throw new EntityNotFoundException(typeof(PosClient), posClient.Id);
        }

        public async Task DeletePosClientAsync(long id)
        {
            await this.ExecuteNonQuery("UPDATE POSCLIENT SET date_deleted=CURRENT_TIMESTAMP WHERE id=@id", new Dictionary<string, object>() { ["@id"] = id });
        }

        public async Task<bool> PosClientExistsAsync(long id)
        {
            IEnumerable<PosClient> posClients = await this.ExecuteQuery(
                "SELECT id FROM POSCLIENT WHERE id = @id AND date_deleted IS NULL",
                (reader =>
                    new PosClient()
                    {
                        Id = reader.GetInt32(0),
                    }),
                new Dictionary<string, object>() { ["@id"] = id }
            );

            return posClients.Any();
        }

        public async Task<PosClient> GetPosClientByLoginCodeAsync(string loginCode)
        {
            IEnumerable<PosClient> posClients = await this.ExecuteQuery(
                            "SELECT id, name, description, logincode, date_inserted, date_updated, date_deleted, visible_on_pos FROM POSCLIENT WHERE logincode = @logincode",
                            MapToPosClient,
                            new Dictionary<string, object>() { ["@logincode"] = loginCode }
                        );

            if (posClients.Any())
            {
                return posClients.First();
            }

            throw new EntityNotFoundException(typeof(PosClient), loginCode);
        }

        private PosClient MapToPosClient(NpgsqlDataReader reader)
        {
            return new PosClient()
            {
                Id = reader.GetInt32(0),
                Name = reader.GetSafeString((1)),
                Description = reader.GetSafeString(2),
                LoginCode = reader.GetSafeString(3),
                DateInserted = reader.GetDateTime(4),
                DateUpdated = reader.GetDateTime(5),
                ReadOnly = !reader.SafeIsDBNull(6),
                VisibleOnPos = !reader.SafeIsDBNull(7)
            };
        }
    }
}
