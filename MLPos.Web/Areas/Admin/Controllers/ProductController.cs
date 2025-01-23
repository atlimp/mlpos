using Microsoft.AspNetCore.Mvc;
using MLPos.Core.Exceptions;
using MLPos.Core.Interfaces.Services;
using MLPos.Core.Model;
using MLPos.Web.Models;

namespace MLPos.Web.Controllers;

[Area("Admin")]
public class ProductController : AdminControllerBase
{
    private readonly IProductService _productService;
    private readonly IImageService _imageService;
    private readonly ILogger<ProductController> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductController(
        ILogger<ProductController> logger,
        IProductService productService,
        IImageService imageService,
        IWebHostEnvironment webHostEnvironment)
    {
        _logger = logger;
        _productService = productService;
        _imageService = imageService;
        _webHostEnvironment = webHostEnvironment;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ModelState.Clear();
        ProductListViewModel model = new ProductListViewModel();
        model.Products = await _productService.GetProductsAsync();
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        ProductDetailsViewModel model = new ProductDetailsViewModel();
        Product product = await _productService.GetProductAsync(id);

        if (product == null)
        {
            return RedirectToAction("Index");
        }
        
        model.Product = product;
        
        return View(model);
    }
    
    [HttpGet]
    public async Task<IActionResult> Edit(int id = -1)
    { 
        ProductDetailsViewModel model = new ProductDetailsViewModel() { Editing = true, NewProduct = true };
        Product product = null;
        
        try
        {
            _logger.LogInformation("Fetching product with id {id}", id);
            product = await _productService.GetProductAsync(id);
        }
        catch(EntityNotFoundException e)
        {
            _logger.LogInformation("Product with id {id} was not found.  Creating new product", id);
        }

        if (product != null)
        {
            model.NewProduct = false;
            model.Product = product;
            model.Editing = model.Editing && !model.Product.ReadOnly;
        }


        return View("Details", model);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    { 
        await _productService.DeleteProductAsync(id);
        
        return RedirectToAction("Index");
    }


    [HttpPost]
    public async Task<IActionResult> Edit(ProductDetailsViewModel model, IFormFile? image)
    {
        Tuple<bool, IEnumerable<ValidationError>> validationResults;
        if (model.NewProduct)
        {
            validationResults = await _productService.ValidateInsert(model.Product);
        }
        else
        {
            validationResults = await _productService.ValidateUpdate(model.Product);
        }
        
        if (!validationResults.Item1)
        {
            model.Editing = true;
            model.ValidationErrors = validationResults.Item2;
            return View("Details", model);
        }
        
        Product product = model.Product;
        string fileName;
        if (image != null)
        {
            string savePath = Path.Combine(_webHostEnvironment.WebRootPath, "img/products");

            fileName = _imageService.SaveImage(new Image
            {
                FileName = image.FileName,
                Path = savePath,
                ImageStream = image.OpenReadStream()
            });

            model.Product.Image = @Url.Content($"~/img/products/{fileName}");
        }
        
        if (model.NewProduct)
        {
            await _productService.CreateProductAsync(product);
        }
        else
        {
            await _productService.UpdateProductAsync(product);
        }
        
        return RedirectToAction("Index");
    }
}