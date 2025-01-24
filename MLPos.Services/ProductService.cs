using MLPos.Core.Interfaces.Repositories;
using MLPos.Core.Interfaces.Services;
using MLPos.Core.Model;

namespace MLPos.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IInventoryRepository _inventoryRepository;
    
    public ProductService(IProductRepository productRepository, IInventoryRepository inventoryRepository)
    {
        _productRepository = productRepository;
        _inventoryRepository = inventoryRepository;
    }
    
    public async Task<Product> GetProductAsync(long id)
    {
        return await _productRepository.GetProductAsync(id);
    }

    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        return await _productRepository.GetProductsAsync();
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        return await _productRepository.CreateProductAsync(product);
    }

    public async Task<Product> UpdateProductAsync(Product product)
    {
        return await _productRepository.UpdateProductAsync(product);
    }

    public async Task DeleteProductAsync(long id)
    {
        await _productRepository.DeleteProductAsync(id);
    }

    public async Task<Tuple<bool, IEnumerable<ValidationError>>> ValidateUpdate(Product product)
    {
        bool ret = true;
        List<ValidationError> validationErrors = new List<ValidationError>();

        bool exists = await _productRepository.ProductExistsAsync(product.Id);
        
        if (!exists)
        {
            validationErrors.Add(new ValidationError{ Error = $"Product with Id {product.Id} was not found" });
            ret = false;
        }

        if (product.Price < 0)
        {
            validationErrors.Add(new ValidationError{ Error = "Product price cannot be less than 0!" });
            ret = false;
        }

        var fromDB = await _productRepository.GetProductAsync(product.Id);
        if (fromDB.ReadOnly)
        {
            validationErrors.Add(new ValidationError { Error = "Product is read only!" });
            ret = false;
        }

        return new Tuple<bool, IEnumerable<ValidationError>>(ret, validationErrors);
    }

    public async Task<Tuple<bool, IEnumerable<ValidationError>>> ValidateInsert(Product product)
    {
        bool ret = true;
        List<ValidationError> validationErrors = new List<ValidationError>();
        
        bool exists = await _productRepository.ProductExistsAsync(product.Id);
        
        if (exists)
        {
            validationErrors.Add(new ValidationError{ Error = $"Product with Id {product.Id} already exists!" });
            ret = false;
        }

        if (product.Price < 0)
        {
            validationErrors.Add(new ValidationError{ Error = "Product price cannot be less than 0!" });
            ret = false;
        }
        
        return new Tuple<bool, IEnumerable<ValidationError>>(ret, validationErrors);
    }

    public async Task<ProductInventory> GetProductInventoryAsync(long id)
    {
        Product product = await _productRepository.GetProductAsync(id);
        int inventoryStatus = await _inventoryRepository.GetProductInventoryStatusAsync(id);
        return new ProductInventory
        {
            Product = product,
            Quantity = inventoryStatus
        };
    }

    public async Task<IEnumerable<ProductInventory>> GetProductInventoryAsync()
    {
        var inventory = await _inventoryRepository.GetProductInventoryStatusAsync();

        foreach (var inventoryItem in inventory)
        {
            inventoryItem.Product = await _productRepository.GetProductAsync(inventoryItem.Product.Id);
        }

        return inventory;
    }

    public async Task CreateInventoryTransactionAsync(InventoryTransaction transaction)
    {
        await _inventoryRepository.CreateInventoryTransactionAsync(transaction);
    }
}