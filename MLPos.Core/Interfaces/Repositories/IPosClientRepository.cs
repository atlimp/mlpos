using MLPos.Core.Model;

namespace MLPos.Core.Interfaces.Repositories;

public interface IPosClientRepository : IBaseRepository
{
    public Task<PosClient> GetPosClientAsync(long id);
    public Task<IEnumerable<PosClient>> GetPosClientsAsync();
    public Task<PosClient> CreatePosClientAsync(PosClient posClient);
    public Task<PosClient> UpdatePosClientAsync(PosClient posClient);
    public Task DeletePosClientAsync(long id);
    public Task<bool> PosClientExistsAsync(long id);
    public Task<PosClient> GetPosClientByLoginCodeAsync(string loginCode);
}