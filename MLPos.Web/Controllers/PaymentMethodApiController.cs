using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MLPos.Core.Interfaces.Services;
using MLPos.Core.Model;

namespace MLPos.Web.Controllers
{
    [Route("api/PaymentMethod")]
    [ApiController]
    public class PaymentMethodApiController : ControllerBase
    {
        private readonly IPaymentMethodService _paymentMethodService;

        public PaymentMethodApiController(IPaymentMethodService paymentMethodService)
        {
            _paymentMethodService = paymentMethodService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllPaymentMethods()
        {
            return Ok(await _paymentMethodService.GetPaymentMethodsAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentMethod(long id)
        {
            PaymentMethod paymentMethod = await _paymentMethodService.GetPaymentMethodAsync(id);

            if (paymentMethod == null)
            {
                return NotFound();
            }

            return Ok(paymentMethod);
        }
    }
}
