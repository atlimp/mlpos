using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MLPos.Core.Enums;
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
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public InvoiceController(IInvoicingService invoicingService, ICustomerService customerService, IPaymentMethodService paymentMethodService, IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _invoicingService = invoicingService;
            _customerService = customerService;
            _paymentMethodService = paymentMethodService;
            _sharedLocalizer = sharedLocalizer;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, [FromQuery] InvoiceQueryFilter? queryFilter = null, [FromQuery] string? dateFrom = null, [FromQuery] string? dateTo = null)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (queryFilter == null)
            {
                queryFilter = new InvoiceQueryFilter();
            }

            if (!(dateFrom == null && dateTo == null))
            {
                Period? period = null;
                if (DateTime.TryParse(dateFrom, out DateTime fromDate))
                {
                    if (period == null)
                    {
                        period = new Period();
                    }

                    period.DateFrom = fromDate;
                }
                if (DateTime.TryParse(dateTo, out DateTime toDate))
                {
                    if (period == null)
                    {
                        period = new Period();
                    }

                    period.DateTo = toDate;
                }

                queryFilter.Period = period;
            }

            int limit = Constants.LIST_PAGE_SIZE;
            int offset = (page - 1) * limit;
            InvoiceListViewModel viewModel = new InvoiceListViewModel();
            viewModel.PageNum = page;
            viewModel.Invoices = await _invoicingService.GetInvoicesAsync(queryFilter, limit, offset);
            viewModel.HasMorePages = viewModel.Invoices.Count() >= Constants.LIST_PAGE_SIZE;

            viewModel.FilterOptions = new FilterOptions();

            viewModel.FilterOptions.Customers = await _customerService.GetCustomersAsync();
            viewModel.FilterOptions.SelectedCustomerId = queryFilter.CustomerId;

            viewModel.FilterOptions.PaymentMethods = await _paymentMethodService.GetPaymentMethodsAsync();
            viewModel.FilterOptions.SelectedPaymentMethodId = queryFilter.PaymentMethodId;

            viewModel.FilterOptions.DateFrom = queryFilter.Period?.DateFrom.ToString("yyyy-MM-dd");
            viewModel.FilterOptions.DateTo = queryFilter.Period?.DateTo.ToString("yyyy-MM-dd");

            viewModel.FilterOptions.Status = -1;
            if (queryFilter.Status != null)
            {
                viewModel.FilterOptions.Status = (int)queryFilter.Status;
            }


            viewModel.FilterOptions.StatusValues = new Dictionary<int, string>();
            foreach (int val in Enum.GetValues(typeof(InvoiceStatus)))
            {
                viewModel.FilterOptions.StatusValues[val] = _sharedLocalizer[((InvoiceStatus)val).ToString()];
            }

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
