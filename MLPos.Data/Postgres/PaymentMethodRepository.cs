using MLPos.Core.Interfaces.Repositories;
using MLPos.Core.Model;
using MLPos.Core.Enums;
using MLPos.Core.Utilities;
using MLPos.Data.Postgres.Helpers;
using Npgsql;
using MLPos.Core.Exceptions;

namespace MLPos.Data.Postgres;

public class PaymentMethodRepository : IPaymentMethodRepository
{
    private readonly string _connectionString;
    public PaymentMethodRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<PaymentMethod> GetPaymentMethodAsync(long id)
    {
        IEnumerable<PaymentMethod> paymentMethods = await SqlHelper.ExecuteQuery<PaymentMethod>(_connectionString,
            "SELECT id, name, description, image, date_inserted, date_updated FROM PAYMENTMETHOD WHERE id = @id",
            MapToPaymentMethod,
            new Dictionary<string, object>(){ ["@id"] = id }
        );

        if (paymentMethods.Any())
        {
            return paymentMethods.First();
        }

        throw new EntityNotFoundException(typeof(PaymentMethod), id);
    }

    public async Task<IEnumerable<PaymentMethod>> GetPaymentMethodsAsync()
    {
        return await SqlHelper.ExecuteQuery(_connectionString,
            "SELECT id, name, description, image, date_inserted, date_updated FROM PAYMENTMETHOD",
            MapToPaymentMethod);
    }

    public async Task<PaymentMethod> CreatePaymentMethodAsync(PaymentMethod paymentMethod)
    {
        IEnumerable<PaymentMethod> paymentMethods = await SqlHelper.ExecuteQuery<PaymentMethod>(_connectionString,
            @"INSERT INTO PAYMENTMETHOD(name, description, image)
                    VALUES(@name, @description, @image) RETURNING id, name, description, image, date_inserted, date_updated",
            MapToPaymentMethod,
            new Dictionary<string, object>(){ ["@name"] = paymentMethod.Name, ["@description"] = paymentMethod.Description, ["@image"] = paymentMethod.Image }
        );
        
        if (paymentMethods.Any())
        {
            return paymentMethods.First();
        }

        throw new EntityNotFoundException(typeof(PaymentMethod), paymentMethod.Id);
    }

    public async Task<PaymentMethod> UpdatePaymentMethodAsync(PaymentMethod paymentMethod)
    {
        IEnumerable<PaymentMethod> paymentMethods = await SqlHelper.ExecuteQuery<PaymentMethod>(_connectionString,
            @"UPDATE PAYMENTMETHOD SET name = @name, description = @description, image = @image WHERE id = @id RETURNING id, name, description, image, date_inserted, date_updated",
            MapToPaymentMethod,
            new Dictionary<string, object>(){ ["@id"] = paymentMethod.Id, ["@name"] = paymentMethod.Name, ["@description"] = paymentMethod.Description, ["@image"] = paymentMethod.Image }
        );
        
        if (paymentMethods.Any())
        {
            return paymentMethods.First();
        }

        throw new EntityNotFoundException(typeof(PaymentMethod), paymentMethod.Id);
    }

    public async Task DeletePaymentMethodAsync(long id)
    {
        await SqlHelper.ExecuteNonQuery(_connectionString, "DELETE FROM PAYMENTMETHOD WHERE id=@id", new Dictionary<string, object>(){ ["@id"] = id });
    }

    public async Task<bool> PaymentMethodExistsAsync(long id)
    {
        IEnumerable<PaymentMethod> paymentMethods = await SqlHelper.ExecuteQuery<PaymentMethod>(_connectionString,
            "select id from PAYMENTMETHOD where id = @id",
            (reader =>
                new PaymentMethod()
                {
                    Id = reader.GetInt32(0),
                }),
            new Dictionary<string, object>(){ ["@id"] = id }
        );

        return paymentMethods.Any();    }
    
    private PaymentMethod MapToPaymentMethod(NpgsqlDataReader reader)
    {
        return new PaymentMethod()
        {
            Id = reader.GetInt32(0),
            Name = reader.GetSafeString((1)),
            Description = reader.GetSafeString(2),
            Image = reader.GetSafeString(3),
            DateInserted = reader.GetDateTime(4),
            DateUpdated = reader.GetDateTime(5),
        };
    }
}