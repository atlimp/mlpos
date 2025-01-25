using Microsoft.AspNetCore.Mvc;
using MLPos.Core.Interfaces.Services;
using MLPos.Web.Controllers;
using MLPos.Web.Models;
using MLPos.Web.Utils;

namespace MLPos.Web.Controllers
{
    [Area("Admin")]
    public class InvoiceController : AdminControllerBase
    {
        private readonly IInvoicingService _invoicingService;
        public InvoiceController(IInvoicingService invoicingService)
        {
            _invoicingService = invoicingService;
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
    }
}
