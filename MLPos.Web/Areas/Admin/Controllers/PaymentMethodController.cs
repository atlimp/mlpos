using Microsoft.AspNetCore.Mvc;
using MLPos.Core.Exceptions;
using MLPos.Core.Interfaces.Services;
using MLPos.Core.Model;
using MLPos.Web.Models;
using System.Transactions;

namespace MLPos.Web.Controllers;

[Area("Admin")]
public class PaymentMethodController : AdminControllerBase
{
    private readonly IPaymentMethodService _paymentMethodService;
    private readonly IImageService _imageService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILogger<PaymentMethodController> _logger; 

    public PaymentMethodController(
        IPaymentMethodService paymentMethodService,
        IImageService imageService,
        IWebHostEnvironment webHostEnvironment,
        ILogger<PaymentMethodController> logger)
    {
        _paymentMethodService = paymentMethodService;
        _imageService = imageService;
        _webHostEnvironment = webHostEnvironment;
        _logger = logger;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ModelState.Clear();
        PaymentMethodListViewModel model = new PaymentMethodListViewModel();
        model.PaymentMethods = await _paymentMethodService.GetPaymentMethodsAsync();
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        PaymentMethodDetailsViewModel model = new PaymentMethodDetailsViewModel();
        PaymentMethod paymentMethod = await _paymentMethodService.GetPaymentMethodAsync(id);
    
        if (paymentMethod == null)
        {
            return RedirectToAction("Index");
        }
        
        model.PaymentMethod = paymentMethod;
        
        return View(model);
    }
    
    [HttpGet]
    public async Task<IActionResult> Edit(int id = -1)
    { 
        PaymentMethodDetailsViewModel model = new PaymentMethodDetailsViewModel() { Editing = true, NewPaymentMethod = true };
        PaymentMethod paymentMethod = null;

        try
        {
            _logger.LogInformation("Fetching payment method with id {id}", id);
            paymentMethod = await _paymentMethodService.GetPaymentMethodAsync(id);
        }
        catch (EntityNotFoundException e)
        {
            _logger.LogInformation("Payment method with id {id} was not found.  Creating new payment method", id);
        }
        
        if (paymentMethod != null)
        {
            model.NewPaymentMethod = false;
            model.PaymentMethod = paymentMethod;
        }
            
        
        return View("Details", model);
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    { 
        await _paymentMethodService.DeletePaymentMethodAsync(id);
        
        return RedirectToAction("Index");
    }
    
    
    [HttpPost]
    public async Task<IActionResult> Edit(PaymentMethodDetailsViewModel model, IFormFile? image)
    {
        Tuple<bool, IEnumerable<ValidationError>> validationResults;
        if (model.NewPaymentMethod)
        {
            validationResults = await _paymentMethodService.ValidateInsert(model.PaymentMethod);
        }
        else
        {
            validationResults = await _paymentMethodService.ValidateUpdate(model.PaymentMethod);
        }
        
        if (!validationResults.Item1)
        {
            model.Editing = true;
            model.ValidationErrors = validationResults.Item2;
            return View("Details", model);
        }
        
        PaymentMethod paymentMethod = model.PaymentMethod;
        string fileName;
        if (image != null)
        {
            string savePath = Path.Combine(_webHostEnvironment.WebRootPath, "img/paymentMethods");

            fileName = _imageService.SaveImage(new Image
            {
                FileName = image.FileName,
                Path = savePath,
                ImageStream = image.OpenReadStream()
            });
            
            model.PaymentMethod.Image = @Url.Content($"~/img/paymentMethods/{fileName}");
        }
        
        if (model.NewPaymentMethod)
        {
            await _paymentMethodService.CreatePaymentMethodAsync(paymentMethod);
        }
        else
        {
            await _paymentMethodService.UpdatePaymentMethodAsync(paymentMethod);
        }
        
        return RedirectToAction("Index");
    }
}