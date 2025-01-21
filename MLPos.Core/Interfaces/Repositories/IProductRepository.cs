using MLPos.Core.Model;

namespace MLPos.Core.Interfaces.Repositories;

public interface IProductRepository : IBaseRepository
{
    public Task<Product> GetProductAsync(long id);
    public Task<IEnumerable<Product>> GetProductsAsync();
    public Task<Product> CreateProductAsync(Product product);
    public Task<Product> UpdateProductAsync(Product product);
    public Task DeleteProductAsync(long id);
    public Task<bool> ProductExistsAsync(long id);
}