using MLPos.Core.Interfaces.Repositories;
using MLPos.Core.Model;
using MLPos.Core.Enums;
using MLPos.Core.Utilities;
using MLPos.Data.Postgres.Helpers;
using Npgsql;
using MLPos.Core.Exceptions;

namespace MLPos.Data.Postgres
{
    public class ProductRepository : RepositoryBase, IProductRepository
    {
        public ProductRepository(string connectionString) : base(connectionString) { }

        public async Task<Product> GetProductAsync(long id)
        {
            IEnumerable<Product> products = await this.ExecuteQuery(
                "SELECT id, name, description, type, image, price, date_inserted, date_updated, date_deleted, visible_on_pos FROM PRODUCT WHERE id = @id",
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
            return await this.ExecuteQuery(
                "SELECT id, name, description, type, image, price, date_inserted, date_updated, date_deleted, visible_on_pos FROM PRODUCT WHERE date_deleted IS NULL ORDER BY NAME",
                MapToProduct);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            IEnumerable<Product> products = await this.ExecuteQuery(
                @"INSERT INTO PRODUCT(name, description, type, image, price, visible_on_pos)
                        VALUES(@name, @description, @type, @image, @price, @visible_on_pos) RETURNING id, name, description, type, image, price, date_inserted, date_updated, date_deleted, visible_on_pos",
                MapToProduct,
                new Dictionary<string, object>(){ ["@name"] = product.Name, ["@description"] = product.Description, ["@type"] = (int)product.Type, ["@image"] = product.Image, ["@price"] = product.Price, ["@visible_on_pos"] = product.VisibleOnPos }
            );
        
            if (products.Any())
            {
                return products.First();
            }

            throw new EntityNotFoundException(typeof(Product), product.Id);
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            IEnumerable<Product> products = await this.ExecuteQuery(
                @"UPDATE PRODUCT set name=@name, description = @description, type = @type, image = @image, price = @price, visible_on_pos = @visible_on_pos WHERE id = @id AND date_deleted IS NULL RETURNING id, name, description, type, image, price, date_inserted, date_updated, date_deleted, visible_on_pos",
                MapToProduct,
                new Dictionary<string, object>(){ ["@id"] = product.Id, ["@name"] = product.Name, ["@description"] = product.Description, ["@type"] = (int)product.Type, ["@image"] = product.Image, ["@price"] = product.Price, ["@visible_on_pos"] = product.VisibleOnPos }
            );
        
            if (products.Any())
            {
                return products.First();
            }

            throw new EntityNotFoundException(typeof(Product), product.Id);
        }

        public async Task DeleteProductAsync(long id)
        {
            await this.ExecuteNonQuery("UPDATE PRODUCT SET date_deleted = CURRENT_TIMESTAMP WHERE id = @id", new Dictionary<string, object>(){ ["@id"] = id });
        }

        public async Task<bool> ProductExistsAsync(long id)
        {
            IEnumerable<Product> products = await this.ExecuteQuery(
                "SELECT id FROM PRODUCT WHERE id = @id AND date_deleted IS NULL",
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
                Name = reader.GetSafeString(1),
                Description = reader.GetSafeString(2),
                Type = (ProductType)reader.GetInt32(3),
                Image = reader.GetSafeString(4),
                Price = (decimal)reader.GetDecimal(5),
                DateInserted = reader.GetDateTime(6),
                DateUpdated = reader.GetDateTime(7),
                ReadOnly = !reader.SafeIsDBNull(8),
                VisibleOnPos = reader.GetSafeBoolean(9),
            };
        }
    }
}
