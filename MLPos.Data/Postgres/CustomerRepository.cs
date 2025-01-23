using System.Data;
using MLPos.Core.Exceptions;
using MLPos.Core.Interfaces.Repositories;
using MLPos.Core.Model;
using MLPos.Core.Utilities;
using MLPos.Data.Postgres.Helpers;
using Npgsql;

namespace MLPos.Data.Postgres;

public class CustomerRepository : RepositoryBase, ICustomerRepository
{
    public CustomerRepository(string connectionString): base(connectionString) { }

    public async Task<Customer> GetCustomerAsync(long id)
    {
        IEnumerable<Customer> customers = await this.ExecuteQuery(
            "SELECT id, name, email, image, date_inserted, date_updated, date_deleted FROM CUSTOMER WHERE id = @id",
            MapToCustomer,
                new Dictionary<string, object>(){ ["@id"] = id }
            );

        if (customers.Any())
        {
            return customers.First();
        }

        throw new EntityNotFoundException(typeof(Customer), id);
    }

    public async Task<IEnumerable<Customer>> GetCustomersAsync()
    {
        return await this.ExecuteQuery(
            "SELECT id, name, email, image, date_inserted, date_updated FROM CUSTOMER WHERE date_deleted IS NULL ORDER BY name",
            MapToCustomer);
    }

    public async Task<Customer> CreateCustomerAsync(Customer customer)
    {
        IEnumerable<Customer> customers = await this.ExecuteQuery(
            @"INSERT INTO CUSTOMER(name, email, image)
                    VALUES(@name, @email, @image) RETURNING id, name, email, image, date_inserted, date_updated",
            MapToCustomer,
            new Dictionary<string, object>(){ ["@name"] = customer.Name, ["@email"] = customer.Email, ["@image"] = customer.Image }
        );
        
        if (customers.Any())
        {
            return customers.First();
        }

        throw new EntityNotFoundException(typeof(Customer), customer.Id);
    }

    public async Task<Customer> UpdateCustomerAsync(Customer customer)
    {
        IEnumerable<Customer> customers = await this.ExecuteQuery(
            @"UPDATE CUSTOMER SET name = @name, email = @email, image = @image WHERE id = @id AND date_deleted IS NULL RETURNING id, name, email, image, date_inserted, date_updated",
            MapToCustomer,
            new Dictionary<string, object>(){ ["@id"] = customer.Id, ["@name"] = customer.Name, ["@email"] = customer.Email, ["@image"] = customer.Image }
        );
        
        if (customers.Any())
        {
            return customers.First();
        }

        throw new EntityNotFoundException(typeof(Customer), customer.Id);
    }

    public async Task DeleteCustomerAsync(long id)
    {
        await this.ExecuteNonQuery("UPDATE CUSTOMER SET date_deleted=CURRENT_TIMESTAMP WHERE id=@id", new Dictionary<string, object>(){ ["@id"] = id });
    }

    public async Task<bool> CustomerExistsAsync(long id)
    {
        IEnumerable<Customer> customers = await this.ExecuteQuery(
            "SELECT id FROM CUSTOMER WHERE id = @id AND date_deleted IS NULL",
            (reader =>
                new Customer()
                {
                    Id = reader.GetInt32(0),
                }),
            new Dictionary<string, object>(){ ["@id"] = id }
        );

        return customers.Any();
    }

    private Customer MapToCustomer(NpgsqlDataReader reader)
    {
        return new Customer()
        {
            Id = reader.GetInt32(0),
            Name = reader.GetSafeString((1)),
            Email = reader.GetSafeString(2),
            Image = reader.GetSafeString(3),
            DateInserted = reader.GetDateTime(4),
            DateUpdated = reader.GetDateTime(5),
            ReadOnly = !reader.SafeIsDBNull(6)
        };
    }
}