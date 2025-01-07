using MLPos.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Core.Interfaces.Services
{
    public interface IPosClientService
    {
        public Task<PosClient> GetPosClientAsync(long id);
        public Task<IEnumerable<PosClient>> GetPosClientsAsync();
        public Task<PosClient> CreatePosClientAsync(PosClient posClient);
        public Task<PosClient> UpdatePosClientAsync(PosClient posClient);
        public Task DeletePosClientAsync(long id);
        public Task<Tuple<bool, IEnumerable<ValidationError>>> ValidateUpdate(PosClient posClient);
        public Task<Tuple<bool, IEnumerable<ValidationError>>> ValidateInsert(PosClient posClient);
    }
}
