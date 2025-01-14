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

        [HttpPost("{posClientId}")]
        public async Task<IActionResult> CreateTransaction(long posClientId, CreateTransactionRequest request)
        {
            Customer customer = await _customerService.GetCustomerAsync(request.CustomerId);

            return Ok(await _transactionService.CreateTransactionAsync(posClientId, customer));
        }

        [HttpGet("{posClientId}/active")]
        public async Task<IActionResult> GetActiveTransactions(long posClientId)
        {
            return Ok(await _transactionService.GetActiveTransactionsAsync(posClientId));
        }

        [HttpGet("{posClientId}/{transactionId}")]
        public async Task<IActionResult> GetTransaction(long posClientId, long transactionId)
        {
            TransactionHeader transactionHeader = await _transactionService.GetTransactionHeaderAsync(transactionId, posClientId);

            if (transactionHeader == null)
            {
                return NotFound();
            }

            return Ok(transactionHeader);
        }

        [HttpGet("{posClientId}/active/summary")]
        public async Task<IActionResult> GetActiveTransactionsSummary(long posClientId)
        {
            return Ok(await _transactionService.GetAllTransactionSummaryAsync(posClientId));
        }

        [HttpGet("{posClientId}/{transactionId}/summary")]
        public async Task<IActionResult> GetTransactionSummary(long posClientId, long transactionId)
        {
            TransactionSummary summary = await _transactionService.GetTransactionSummaryAsync(transactionId, posClientId);

            if (summary == null)
            {
                return NotFound();
            }

            return Ok(summary);
        }

        [HttpDelete("{posClientId}/{transactionId}")]
        public async Task<IActionResult> DeleteTransaction(long posClientId, long transactionId)
        {
            await _transactionService.DeleteTransactionAsync(transactionId, posClientId);
            return NoContent();
        }
        
        [HttpPost("{posClientId}/{transactionId}/Lines")]
        public async Task<IActionResult> AddItem(long posClientId, long transactionId, AddItemRequest request)
        {
            TransactionHeader transactionHeader = await _transactionService.GetTransactionHeaderAsync(transactionId, posClientId);

            Product product = await _productService.GetProductAsync(request.ProductId);

            if (request.Quantity <= 0)
            {
            }

            return Ok(await _transactionService.AddItemAsync(transactionHeader, product, request.Quantity));
        }

        [HttpDelete("{posClientId}/{transactionId}/Lines/{lineId}")]
        public async Task<IActionResult> DeleteLine(long posClientId, long transactionId, long lineId)
        {
            return Ok(await _transactionService.RemoveItemAsync(transactionId, posClientId, lineId));
        }
    }
}
