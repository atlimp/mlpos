using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MLPos.Core.Interfaces.Services;
using MLPos.Core.Model;

namespace MLPos.Web.Controllers
{
    [Route("api/Customer")]
    [ApiController]
    public class CustomerApiController : ControllerBase
    {
        private readonly ILogger<CustomerApiController> _logger;
        private readonly ICustomerService _customerService;

        public CustomerApiController(ICustomerService customerService, ILogger<CustomerApiController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllCustomers()
        {
            return Ok(await _customerService.GetCustomersAsync());

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(long id)
        {
            Customer customer = await _customerService.GetCustomerAsync(id);
            
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }
    }
}
