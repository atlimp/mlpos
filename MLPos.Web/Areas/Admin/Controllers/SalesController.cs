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
    public class SalesController : AdminControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ICustomerService _customerService;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;
        public SalesController(ITransactionService transactionService, ICustomerService customerService, IPaymentMethodService paymentMethodService, IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _transactionService = transactionService;
            _customerService = customerService;
            _paymentMethodService = paymentMethodService;
            _sharedLocalizer = sharedLocalizer;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, [FromQuery] PostedTransactionQueryFilter? queryFilter = null, [FromQuery] string? dateFrom = null, [FromQuery] string? dateTo = null)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (queryFilter == null)
            {
                queryFilter = new PostedTransactionQueryFilter();
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
            SalesTransactionListViewModel viewModel = new SalesTransactionListViewModel();
            viewModel.PageNum = page;
            viewModel.Transactions = await _transactionService.GetPostedTransactionHeadersAsync(queryFilter, limit, offset);
            viewModel.HasMorePages = viewModel.Transactions.Count() >= Constants.LIST_PAGE_SIZE;

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
            foreach (int val in Enum.GetValues(typeof(TransactionStatus)))
            {
                viewModel.FilterOptions.StatusValues[val] = _sharedLocalizer[((TransactionStatus)val).ToString()];
            }
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(long posClientId, long transactionId)
        {
            SalesTransactionViewModel viewModel = new SalesTransactionViewModel();
            viewModel.Transaction = await _transactionService.GetPostedTransactionHeaderAsync(transactionId, posClientId);
            return View(viewModel);
        }
    }
}
