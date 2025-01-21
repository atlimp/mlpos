using MLPos.Core.Model;

namespace MLPos.Core.Interfaces.Repositories;

public interface ICustomerRepository : IBaseRepository
{
    public Task<Customer> GetCustomerAsync(long id);
    public Task<IEnumerable<Customer>> GetCustomersAsync();
    public Task<Customer> CreateCustomerAsync(Customer customer);
    public Task<Customer> UpdateCustomerAsync(Customer customer);
    public Task DeleteCustomerAsync(long id);
    public Task<bool> CustomerExistsAsync(long id);
}