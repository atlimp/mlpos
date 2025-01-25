using MLPos.Core.Interfaces.Repositories;
using MLPos.Core.Model;
using MLPos.Core.Enums;
using MLPos.Core.Utilities;
using MLPos.Data.Postgres.Helpers;
using Npgsql;
using MLPos.Core.Exceptions;

namespace MLPos.Data.Postgres;

public class PaymentMethodRepository : RepositoryBase, IPaymentMethodRepository
{
    public PaymentMethodRepository(string connectionString): base(connectionString) { }

    public async Task<PaymentMethod> GetPaymentMethodAsync(long id)
    {
        IEnumerable<PaymentMethod> paymentMethods = await this.ExecuteQuery(
            "SELECT id, name, description, image, date_inserted, date_updated, date_deleted, visible_on_pos, invoice_on_post FROM PAYMENTMETHOD WHERE id = @id",
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
        return await this.ExecuteQuery(
            "SELECT id, name, description, image, date_inserted, date_updated, date_deleted, visible_on_pos, invoice_on_post FROM PAYMENTMETHOD WHERE date_deleted IS NULL ORDER BY name",
            MapToPaymentMethod);
    }

    public async Task<PaymentMethod> CreatePaymentMethodAsync(PaymentMethod paymentMethod)
    {
        IEnumerable<PaymentMethod> paymentMethods = await this.ExecuteQuery(
            @"INSERT INTO PAYMENTMETHOD(name, description, image, visible_on_pos, invoice_on_post)
                    VALUES(@name, @description, @image, @visible_on_pos, @invoice_on_post) RETURNING id, name, description, image, date_inserted, date_updated, date_deleted, visible_on_pos, invoice_on_post",
            MapToPaymentMethod,
            new Dictionary<string, object>(){
                ["@name"] = paymentMethod.Name,
                ["@description"] = paymentMethod.Description,
                ["@image"] = paymentMethod.Image,
                ["@visible_on_pos"] = paymentMethod.VisibleOnPos,
                ["@invoice_on_post"] = paymentMethod.InvoiceOnPost
            }
        );
        
        if (paymentMethods.Any())
        {
            return paymentMethods.First();
        }

        throw new EntityNotFoundException(typeof(PaymentMethod), paymentMethod.Id);
    }

    public async Task<PaymentMethod> UpdatePaymentMethodAsync(PaymentMethod paymentMethod)
    {
        IEnumerable<PaymentMethod> paymentMethods = await this.ExecuteQuery(
            @"UPDATE PAYMENTMETHOD SET name = @name, description = @description, image = @image, visible_on_pos = @visible_on_pos, invoice_on_post = @invoice_on_post
                WHERE id = @id AND date_deleted IS NULL
                RETURNING id, name, description, image, date_inserted, date_updated, date_deleted, visible_on_pos, invoice_on_post = @invoice_on_post",
            MapToPaymentMethod,
            new Dictionary<string, object>(){
                ["@id"] = paymentMethod.Id,
                ["@name"] = paymentMethod.Name,
                ["@description"] = paymentMethod.Description,
                ["@image"] = paymentMethod.Image,
                ["@visible_on_pos"] = paymentMethod.VisibleOnPos,
                ["@invoice_on_post"] = paymentMethod.InvoiceOnPost
            }
        );
        
        if (paymentMethods.Any())
        {
            return paymentMethods.First();
        }

        throw new EntityNotFoundException(typeof(PaymentMethod), paymentMethod.Id);
    }

    public async Task DeletePaymentMethodAsync(long id)
    {
        await this.ExecuteNonQuery("UPDATE PAYMENTMETHOD SET date_deleted=CURRENT_TIMESTAMP WHERE id=@id", new Dictionary<string, object>(){ ["@id"] = id });
    }

    public async Task<bool> PaymentMethodExistsAsync(long id)
    {
        IEnumerable<PaymentMethod> paymentMethods = await this.ExecuteQuery(
            "SELECT id FROM PAYMENTMETHOD WHERE id = @id AND date_deleted IS NULL",
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
            ReadOnly = !reader.SafeIsDBNull(6),
            VisibleOnPos = reader.GetSafeBoolean(7),
            InvoiceOnPost = reader.GetSafeBoolean(8),
        };
    }
}