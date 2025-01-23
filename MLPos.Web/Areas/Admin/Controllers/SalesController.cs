using Microsoft.AspNetCore.Mvc;
using MLPos.Core.Interfaces.Services;
using MLPos.Web.Controllers;
using MLPos.Web.Models;
using MLPos.Web.Utils;

namespace MLPos.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SalesController : AdminControllerBase
    {
        private readonly ITransactionService _transactionService;
        public SalesController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
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
            SalesTransactionListViewModel viewModel = new SalesTransactionListViewModel();
            viewModel.PageNum = page;
            viewModel.Transactions = await _transactionService.GetPostedTransactionHeadersAsync(limit, offset);
            viewModel.HasMorePages = viewModel.Transactions.Count() >= Constants.LIST_PAGE_SIZE;
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
