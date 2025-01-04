using MLPos.Core.Interfaces.Repositories;
using MLPos.Core.Interfaces.Services;
using MLPos.Core.Model;

namespace MLPos.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    
    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;    
    }
    
    public async Task<Customer> GetCustomerAsync(long id)
    {
        return await _customerRepository.GetCustomerAsync(id);
    }

    public async Task<IEnumerable<Customer>> GetCustomersAsync()
    {
        return await _customerRepository.GetCustomersAsync();
    }

    public async Task<Customer> CreateCustomerAsync(Customer customer)
    {
        return await _customerRepository.CreateCustomerAsync(customer);
    }

    public async Task<Customer> UpdateCustomerAsync(Customer customer)
    {
        return await _customerRepository.UpdateCustomerAsync(customer);
    }

    public async Task DeleteCustomerAsync(long id)
    {
        await _customerRepository.DeleteCustomerAsync(id);
    }

    public async Task<Tuple<bool, IEnumerable<ValidationError>>> ValidateUpdate(Customer customer)
    {
        bool ret = true;
        List<ValidationError> validationErrors = new List<ValidationError>();

        bool exists = await _customerRepository.CustomerExistsAsync(customer.Id);
        
        if (!exists)
        {
            validationErrors.Add(new ValidationError{ Error = $"Customer with Id {customer.Id} was not found" });
            ret = false;
        }

        if (string.IsNullOrEmpty(customer.Email))
        {
            validationErrors.Add(new ValidationError{ Error = "Customer email is required!" });
            ret = false;
        }

        return new Tuple<bool, IEnumerable<ValidationError>>(ret, validationErrors);
    }

    public async Task<Tuple<bool, IEnumerable<ValidationError>>> ValidateInsert(Customer customer)
    {
        bool ret = true;
        List<ValidationError> validationErrors = new List<ValidationError>();
        
        bool exists = await _customerRepository.CustomerExistsAsync(customer.Id);
        
        if (exists)
        {
            validationErrors.Add(new ValidationError{ Error = $"Customer with Id {customer.Id} already exists!" });
            ret = false;
        }

        if (string.IsNullOrEmpty(customer.Email))
        {
            validationErrors.Add(new ValidationError{ Error = "Customer email is required!" });
            ret = false;
        }
        
        return new Tuple<bool, IEnumerable<ValidationError>>(ret, validationErrors);
    }
}