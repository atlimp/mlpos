using Microsoft.AspNetCore.Mvc;
using MLPos.Core.Exceptions;
using MLPos.Core.Interfaces.Services;
using MLPos.Core.Model;
using MLPos.Services;
using MLPos.Web.Models;

namespace MLPos.Web.Controllers;

[Area("Admin")]
public class CustomerController : AdminControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly IImageService _imageService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILogger<CustomerController> _logger;

    public CustomerController(
        ICustomerService customerService,
        IImageService imageService,
        IWebHostEnvironment webHostEnvironment,
        ILogger<CustomerController> logger)
    {
        _customerService = customerService;
        _imageService = imageService;
        _webHostEnvironment = webHostEnvironment;
        _logger = logger;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ModelState.Clear();
        CustomerListViewModel model = new CustomerListViewModel();
        model.Customers = await _customerService.GetCustomersAsync();
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        CustomerDetailsViewModel model = new CustomerDetailsViewModel();
        Customer customer = await _customerService.GetCustomerAsync(id);
    
        if (customer == null)
        {
            return RedirectToAction("Index");
        }
        
        model.Customer = customer;
        
        return View(model);
    }
    
    [HttpGet]
    public async Task<IActionResult> Edit(int id = -1)
    { 
        CustomerDetailsViewModel model = new CustomerDetailsViewModel() { Editing = true, NewCustomer = true };
        Customer customer = null;

        try
        {
            _logger.LogInformation("Fetching customer with id {id}", id);
            customer = await _customerService.GetCustomerAsync(id);
        }
        catch (EntityNotFoundException e)
        {
            _logger.LogInformation("Customer with id {id} was not found.  Creating new customer", id);
        }
        
        if (customer != null)
        {
            model.NewCustomer = false;
            model.Customer = customer;
            model.Editing = model.Editing && !model.Customer.ReadOnly;
        }
            
        
        return View("Details", model);
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    { 
        await _customerService.DeleteCustomerAsync(id);
        
        return RedirectToAction("Index");
    }
    
    
    [HttpPost]
    public async Task<IActionResult> Edit(CustomerDetailsViewModel model, IFormFile? image)
    {
        Tuple<bool, IEnumerable<ValidationError>> validationResults;
        if (model.NewCustomer)
        {
            validationResults = await _customerService.ValidateInsert(model.Customer);
        }
        else
        {
            validationResults = await _customerService.ValidateUpdate(model.Customer);
        }
        
        if (!validationResults.Item1)
        {
            model.Editing = true;
            model.ValidationErrors = validationResults.Item2;
            return View("Details", model);
        }
        
        Customer customer = model.Customer;
        string fileName;
        if (image != null)
        {
            string savePath = Path.Combine(_webHostEnvironment.WebRootPath, "img/customers");

            fileName = _imageService.SaveImage(new Image
            {
                FileName = image.FileName,
                Path = savePath,
                ImageStream = image.OpenReadStream()
            });

            model.Customer.Image = @Url.Content($"~/img/customers/{fileName}");
        }
        
        if (model.NewCustomer)
        {
            await _customerService.CreateCustomerAsync(customer);
        }
        else
        {
            await _customerService.UpdateCustomerAsync(customer);
        }
        
        return RedirectToAction("Index");
    }
}