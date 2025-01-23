using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MLPos.Core.Interfaces.Services;
using MLPos.Core.Model;
using MLPos.Services;

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
            IEnumerable<PaymentMethod> customers = await _paymentMethodService.GetPaymentMethodsAsync();

            return Ok(customers.Where(x => x.VisibleOnPos));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentMethod(long id)
        {
            PaymentMethod paymentMethod = await _paymentMethodService.GetPaymentMethodAsync(id);

            if (paymentMethod == null || !paymentMethod.VisibleOnPos)
            {
                return NotFound();
            }

            return Ok(paymentMethod);
        }
    }
}
