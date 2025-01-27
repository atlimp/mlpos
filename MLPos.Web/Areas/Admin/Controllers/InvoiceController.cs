using Microsoft.AspNetCore.Mvc;
using MLPos.Core.Interfaces.Services;
using MLPos.Core.Model;
using MLPos.Web.Controllers;
using MLPos.Web.Models;
using MLPos.Web.Utils;

namespace MLPos.Web.Controllers
{
    [Area("Admin")]
    public class InvoiceController : AdminControllerBase
    {
        private readonly IInvoicingService _invoicingService;
        private readonly ICustomerService _customerService;
        private readonly IPaymentMethodService _paymentMethodService;
        public InvoiceController(IInvoicingService invoicingService, ICustomerService customerService, IPaymentMethodService paymentMethodService)
        {
            _invoicingService = invoicingService;
            _customerService = customerService;
            _paymentMethodService = paymentMethodService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            if (page < 1)
            {
                page = 1;
            }

            int limit = Constants.LIST_PAGE_SIZE;
            int offset = (page - 1) * limit;
            InvoiceListViewModel viewModel = new InvoiceListViewModel();
            viewModel.PageNum = page;
            viewModel.Invoices = await _invoicingService.GetInvoicesAsync(limit, offset);
            viewModel.HasMorePages = viewModel.Invoices.Count() >= Constants.LIST_PAGE_SIZE;
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(long id)
        {
            InvoiceViewModel viewModel = new InvoiceViewModel();
            viewModel.Invoice = await _invoicingService.GetInvoiceAsync(id);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Pay(long id)
        {
            await _invoicingService.MarkAsPaid(id);
            return RedirectToAction("Details", new { id = id });
        }

        [HttpGet]
        public async Task<IActionResult> GenerateInvoice([FromQuery] long? customerId = null, [FromQuery] long? paymentMethodId = null)
        {
            GenerateInvoiceViewModel viewModel = new GenerateInvoiceViewModel();

            viewModel.SelectedCustomerId = customerId;
            viewModel.SelectedPaymentMethodId = paymentMethodId;

            viewModel.Customers = await _customerService.GetCustomersAsync();
            viewModel.PaymentMethods = await _paymentMethodService.GetPaymentMethodsAsync();

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> GenerateInvoice(GenerateInvoiceViewModel model)
        {
            Customer customer = new Customer
            {
                Id = (long)model.SelectedCustomerId
            };

            PaymentMethod paymentMethod = new PaymentMethod
            {
                Id = (long)model.SelectedPaymentMethodId,
            };

            var validateResult = await _invoicingService.ValidateInvoiceGeneration(customer, paymentMethod, model.Period);
            List<ValidationError> validationErrors = validateResult.Item2.ToList();

            if (validateResult.Item1)
            {
                InvoiceHeader invoice = await _invoicingService.GenerateInvoice(customer, paymentMethod, model.Period);

                if (invoice != null)
                {
                    return RedirectToAction("Details", new { id = invoice.Id });
                }

                validationErrors.Add(new ValidationError{
                    Error = "No lines were found for given parameters"
                });            }

            model.Customers = await _customerService.GetCustomersAsync();
            model.PaymentMethods = await _paymentMethodService.GetPaymentMethodsAsync();
            model.ValidationErrors = validationErrors;
            return View(model);
        }
    }
}
