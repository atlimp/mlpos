using MLPos.Core.Model;

namespace MLPos.Core.Interfaces.Services;

public interface ICustomerService
{
    public Task<Customer> GetCustomerAsync(long id);
    public Task<IEnumerable<Customer>> GetCustomersAsync();
    public Task<Customer> CreateCustomerAsync(Customer customer);
    public Task<Customer> UpdateCustomerAsync(Customer customer);
    public Task DeleteCustomerAsync(long id);
    public Task<Tuple<bool, IEnumerable<ValidationError>>> ValidateUpdate(Customer customer);
    public Task<Tuple<bool, IEnumerable<ValidationError>>> ValidateInsert(Customer customer);
}