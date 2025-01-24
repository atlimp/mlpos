using MLPos.Core.Model;

namespace MLPos.Core.Interfaces.Services;

public interface IProductService
{
    public Task<Product> GetProductAsync(long id);
    public Task<IEnumerable<Product>> GetProductsAsync();
    public Task<Product> CreateProductAsync(Product product);
    public Task<Product> UpdateProductAsync(Product product);
    public Task DeleteProductAsync(long id);
    public Task<int> GetProductInventoryAsync(long id);
    public Task<IEnumerable<Tuple<long, int>>> GetProductInventoryAsync();
    public Task<Tuple<bool, IEnumerable<ValidationError>>> ValidateUpdate(Product product);
    public Task<Tuple<bool, IEnumerable<ValidationError>>> ValidateInsert(Product product);
}