using Microsoft.AspNetCore.Mvc;
using MLPos.Core.Interfaces.Services;
using MLPos.Web.Models;

namespace MLPos.Web.Controllers
{
    [Area("Admin")]
    public class InventoryController : Controller
    {
        private readonly IProductService _productService;

        public InventoryController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            InventoryListViewModel model = new InventoryListViewModel();
            model.Inventory = await _productService.GetProductInventoryAsync();
            return View(model);
        }
    }
}
