using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MLPos.Core.Interfaces.Services;
using MLPos.Core.Model;
using MLPos.Services;

namespace MLPos.Web.Controllers
{
    [Route("api/Product")]
    [ApiController]
    public class ProductApiController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductApiController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllProducts()
        {
            IEnumerable<Product> products = await _productService.GetProductsAsync();

            return Ok(products.Where(x => x.VisibleOnPos));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(long id)
        {
            Product product = await _productService.GetProductAsync(id);

            if (product == null || !product.VisibleOnPos)
            {
                return NotFound();
            }

            return Ok(product);
        }
    }
}
