using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MLPos.Core.Interfaces.Services;
using MLPos.Core.Model;

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
            return Ok(await _productService.GetProductsAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(long id)
        {
            Product product = await _productService.GetProductAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }
    }
}
