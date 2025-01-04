using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MLPos.Core.Interfaces.Services;
using MLPos.Core.Model;
using MLPos.Web.Models;

namespace MLPos.Web.Controllers
{
    [Route("api/Transaction")]
    [ApiController]
    public class TransactionApiController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;

        public TransactionApiController(ITransactionService transactionService, ICustomerService customerService, IProductService productService)
        {
            _transactionService = transactionService;
            _customerService = customerService;
            _productService = productService;
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateTransaction(CreateTransactionRequest request)
        {
            Customer customer = await _customerService.GetCustomerAsync(request.CustomerId);


            return Ok(await _transactionService.CreateTransactionAsync(customer));
        }

        [HttpGet("{transactionId}")]
        public async Task<IActionResult> GetTransaction(long transactionId)
        {
            TransactionHeader transactionHeader = await _transactionService.GetTransactionHeaderAsync(transactionId);

            if (transactionHeader == null)
            {
                return NotFound();
            }

            return Ok(transactionHeader);
        }

        [HttpDelete("{transactionId}")]
        public async Task<IActionResult> DeleteTransaction(long transactionId)
        {
            await _transactionService.DeleteTransactionAsync(transactionId);
            return NoContent();
        }
        
        [HttpPost("{transactionId}/Lines")]
        public async Task<IActionResult> AddItem(long transactionId, AddItemRequest request)
        {
            TransactionHeader transactionHeader = await _transactionService.GetTransactionHeaderAsync(transactionId);

            Product product = await _productService.GetProductAsync(request.ProductId);

            if (request.Quantity <= 0)
            {
            }

            return Ok(await _transactionService.AddItemAsync(transactionHeader, product, request.Quantity));
        }

        [HttpDelete("{transactionId}/Lines/{lineId}")]
        public async Task<IActionResult> DeleteLine(long transactionId, long lineId)
        {
            await _transactionService.RemoveItemAsync(transactionId, lineId);
            return NoContent();
        }
    }
}
