using MLPos.Core.Interfaces.Repositories;
using MLPos.Core.Model;
using MLPos.Core.Enums;
using MLPos.Core.Utilities;
using MLPos.Data.Postgres.Helpers;
using Npgsql;
using MLPos.Core.Exceptions;

namespace MLPos.Data.Postgres;

public class ProductRepository : IProductRepository
{
    private readonly string _connectionString;
    public ProductRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    public async Task<Product> GetProductAsync(long id)
    {
        IEnumerable<Product> products = await SqlHelper.ExecuteQuery<Product>(_connectionString,
            "select id, description, type, image, price, date_inserted, date_updated from product where id = @id",
            MapToProduct,
                new Dictionary<string, object>(){ ["@id"] = id }
            );

        if (products.Any())
        {
            return products.First();
        }

        throw new EntityNotFoundException(typeof(Product), id);
    }

    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        return await SqlHelper.ExecuteQuery<Product>(_connectionString,
            "SELECT id, description, type, image, price, date_inserted, date_updated FROM PRODUCT",
            MapToProduct);
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        IEnumerable<Product> products = await SqlHelper.ExecuteQuery<Product>(_connectionString,
            @"INSERT INTO PRODUCT(description, type, image, price)
                    VALUES(@description, @type, @image, @price) RETURNING id, description, type, image, price, date_inserted, date_updated",
            MapToProduct,
            new Dictionary<string, object>(){ ["@description"] = product.Description, ["@type"] = (int)product.Type, ["@image"] = product.Image, ["@price"] = product.Price }
        );
        
        if (products.Any())
        {
            return products.First();
        }

        throw new EntityNotFoundException(typeof(Product), product.Id);
    }

    public async Task<Product> UpdateProductAsync(Product product)
    {
        IEnumerable<Product> products = await SqlHelper.ExecuteQuery<Product>(_connectionString,
            @"UPDATE PRODUCT set description = @description, type = @type, image = @image, price = @price WHERE id = @id RETURNING id, description, type, image, price, date_inserted, date_updated",
            MapToProduct,
            new Dictionary<string, object>(){ ["@id"] = product.Id, ["@description"] = product.Description, ["@type"] = (int)product.Type, ["@image"] = product.Image, ["@price"] = product.Price }
        );
        
        if (products.Any())
        {
            return products.First();
        }

        throw new EntityNotFoundException(typeof(Product), product.Id);
    }

    public async Task DeleteProductAsync(long id)
    {
        await SqlHelper.ExecuteNonQuery(_connectionString, "delete from product where id=@id", new Dictionary<string, object>(){ ["@id"] = id });
    }

    public async Task<bool> ProductExistsAsync(long id)
    {
        IEnumerable<Product> products = await SqlHelper.ExecuteQuery<Product>(_connectionString,
            "select id from product where id = @id",
            (reader =>
                new Product()
                {
                    Id = reader.GetInt32(0),
                }),
            new Dictionary<string, object>(){ ["@id"] = id }
        );

        return products.Any();
    }

    private Product MapToProduct(NpgsqlDataReader reader)
    {
        return new Product()
        {
            Id = reader.GetInt32(0),
            Description = reader.GetString((1)),
            Type = (ProductType)reader.GetInt32(2),
            Image = reader.GetString(3),
            Price = (decimal)reader.GetDecimal(4),
            DateInserted = reader.GetDateTime(5),
            DateUpdated = reader.GetDateTime(6),
        };
    }
}