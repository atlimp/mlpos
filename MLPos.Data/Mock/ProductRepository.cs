using MLPos.Core.Enums;
using MLPos.Core.Interfaces.Repositories;
using MLPos.Core.Model;

namespace MLPos.Data.Mock;

public class ProductRepository : IProductRepository
{
    private List<Product> _products;
    public ProductRepository()
    {
        _products = new List<Product>()
        {
            new Product
            {
                Id = 1,
                Description = "Bjór",
                Type = ProductType.Item,
                Image = "https://dutyfree.b-cdn.net/118_2481_daaa8b323b.jpg",
                Price = 500
            },
            new Product
            {
                Id = 2,
                Description = "Gos",
                Image = "https://drdrinksusa.com/cdn/shop/products/Coke_grande.jpg?v=1546134439",
                Type = ProductType.Item,
                Price = 250
            },
            new Product
            {
                Id = 3,
                Description = "Súkkulaði",
                Image = "https://dutyfree.b-cdn.net/324_4506_1_ead80ea982.jpg",
                Type = ProductType.Item,
                Price = 100
            },
            new Product
            {
                Id = 4,
                Description = "Klukkutími í hermi",
                Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/6e/Golfer_swing.jpg/1200px-Golfer_swing.jpg",
                Type = ProductType.Service,
                Price = 3000
            },
        };
    }
    public async Task<Product> GetProductAsync(long id)
    {
        return _products.FirstOrDefault(x => x.Id == id);
    }

    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        return _products;
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        _products.Add(product);
        return product;
    }

    public async Task<Product> UpdateProductAsync(Product product)
    {
        int index = _products.FindIndex(x => x.Id == product.Id);
        
        if (index != -1)
        {
            _products[index] = product;
        }

        return product;
    }

    public async Task DeleteProductAsync(long id)
    {
        int index = _products.FindIndex(x => x.Id == id);
        if (index != -1)
        {
            _products.RemoveAt(index);
        }
    }

    public async Task<bool> ProductExistsAsync(long id)
    {
        return _products.FindIndex(x => x.Id == id) != -1;
    }
}