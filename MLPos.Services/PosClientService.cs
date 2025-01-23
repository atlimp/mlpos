using MLPos.Core.Interfaces.Repositories;
using MLPos.Core.Interfaces.Services;
using MLPos.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Services
{
    public class PosClientService : IPosClientService
    {
        private readonly IPosClientRepository _posClientRepository;

        public PosClientService(IPosClientRepository posClientRepository)
        {
            _posClientRepository = posClientRepository;
        }

        public async Task<PosClient> CreatePosClientAsync(PosClient posClient)
        {
            return await _posClientRepository.CreatePosClientAsync(posClient);
        }

        public async Task DeletePosClientAsync(long id)
        {
            await _posClientRepository.DeletePosClientAsync(id);
        }

        public async Task<PosClient> GetPosClientAsync(long id)
        {
            return await _posClientRepository.GetPosClientAsync(id);
        }

        public async Task<IEnumerable<PosClient>> GetPosClientsAsync()
        {
            return await _posClientRepository.GetPosClientsAsync();
        }

        public async Task<PosClient> UpdatePosClientAsync(PosClient posClient)
        {
            return await _posClientRepository.UpdatePosClientAsync(posClient);
        }

        public async Task<Tuple<bool, IEnumerable<ValidationError>>> ValidateUpdate(PosClient posClient)
        {
            bool ret = true;
            List<ValidationError> validationErrors = new List<ValidationError>();

            bool exists = await _posClientRepository.PosClientExistsAsync(posClient.Id);

            if (!exists)
            {
                validationErrors.Add(new ValidationError { Error = $"Pos client with Id {posClient.Id} does not exist!" });
                ret = false;
            }

            var fromDB = await _posClientRepository.GetPosClientAsync(posClient.Id);
            if (fromDB.ReadOnly)
            {
                validationErrors.Add(new ValidationError { Error = "Pos client is read only!" });
                ret = false;
            }

            fromDB = await _posClientRepository.GetPosClientByLoginCodeAsync(posClient.LoginCode);
            if (fromDB != null)
            {
                validationErrors.Add(new ValidationError { Error = $"Pos client {fromDB.Id} already has login code {fromDB.LoginCode}!" });
                ret = false;
            }

            return new Tuple<bool, IEnumerable<ValidationError>>(ret, validationErrors);
        }

        public async Task<Tuple<bool, IEnumerable<ValidationError>>> ValidateInsert(PosClient posClient)
        {
            bool ret = true;
            List<ValidationError> validationErrors = new List<ValidationError>();

            bool exists = await _posClientRepository.PosClientExistsAsync(posClient.Id);

            if (exists)
            {
                validationErrors.Add(new ValidationError { Error = $"Pos client with Id {posClient.Id} already exists!" });
                ret = false;
            }

            try
            {
                var fromDB = await _posClientRepository.GetPosClientByLoginCodeAsync(posClient.LoginCode);
                if (fromDB != null)
                {
                    validationErrors.Add(new ValidationError { Error = $"Pos client {fromDB.Id} already has login code {fromDB.LoginCode}!" });
                    ret = false;
                }
            }
            catch
            {

            }

            return new Tuple<bool, IEnumerable<ValidationError>>(ret, validationErrors);
        }

        public Task<PosClient> GetPosClientByLoginCodeAsync(string loginCode)
        {
            return _posClientRepository.GetPosClientByLoginCodeAsync(loginCode);
        }
    }
}
