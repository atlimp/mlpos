using Microsoft.AspNetCore.Mvc;
using MLPos.Core.Interfaces.Services;
using MLPos.Core.Model;
using MLPos.Web.Models;

namespace MLPos.Web.Controllers
{
    [Area("Admin")]
    public class InventoryController : AdminControllerBase
    {
        private readonly IProductService _productService;

        public InventoryController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            InventoryListViewModel model = new InventoryListViewModel();
            model.Inventory = (await _productService.GetProductInventoryAsync()).OrderBy(x => x.Product.Name);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id = -1)
        {
            InventoryViewModel model = new InventoryViewModel();

            model.Inventory = await _productService.GetProductInventoryAsync(id);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(InventoryViewModel viewModel)
        {
            InventoryTransaction inventoryTransaction = new InventoryTransaction();
            inventoryTransaction.Type = Core.Enums.InventoryTransactionType.Counting;
            inventoryTransaction.ProductId = viewModel.Inventory.Product.Id;
            inventoryTransaction.Quantity = viewModel.CountedQuantity;
            await _productService.CreateInventoryTransactionAsync(inventoryTransaction);

            return RedirectToAction("Index");
        }

    }
}
