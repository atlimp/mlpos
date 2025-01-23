using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MLPos.Core.Exceptions;
using MLPos.Core.Interfaces.Services;
using MLPos.Core.Model;
using MLPos.Services;
using MLPos.Web.Controllers;
using MLPos.Web.Models;

namespace MLPos.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PosClientController : Controller
    {
        private readonly IPosClientService _posClientService;
        private readonly ILogger<PosClientController> _logger;

        public PosClientController(
            IPosClientService posClientService,
            ILogger<PosClientController> logger)
        {
            _posClientService = posClientService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ModelState.Clear();
            PosClientListViewModel model = new PosClientListViewModel();
            model.PosClients = await _posClientService.GetPosClientsAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            PosClientDetailsViewModel model = new PosClientDetailsViewModel();
            PosClient posClient = await _posClientService.GetPosClientAsync(id);

            if (posClient == null)
            {
                return RedirectToAction("Index");
            }

            model.PosClient = posClient;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id = -1)
        {
            PosClientDetailsViewModel model = new PosClientDetailsViewModel() { Editing = true, NewPosClient = true };
            PosClient posClient = null;

            try
            {
                _logger.LogInformation("Fetching pos client with id {id}", id);
                posClient = await _posClientService.GetPosClientAsync(id);
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogInformation("Pos client with id {id} was not found.  Creating new pos client", id);
            }

            if (posClient != null)
            {
                model.NewPosClient = false;
                model.PosClient = posClient;
                model.Editing = model.Editing && !model.PosClient.ReadOnly;
            }


            return View("Details", model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _posClientService.DeletePosClientAsync(id);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PosClientDetailsViewModel model)
        {
            Tuple<bool, IEnumerable<ValidationError>> validationResults;
            if (model.NewPosClient)
            {
                validationResults = await _posClientService.ValidateInsert(model.PosClient);
            }
            else
            {
                validationResults = await _posClientService.ValidateUpdate(model.PosClient);
            }

            if (!validationResults.Item1)
            {
                model.Editing = true;
                model.ValidationErrors = validationResults.Item2;
                return View("Details", model);
            }

            PosClient posClient = model.PosClient;

            if (model.NewPosClient)
            {
                await _posClientService.CreatePosClientAsync(posClient);
            }
            else
            {
                await _posClientService.UpdatePosClientAsync(posClient);
            }

            return RedirectToAction("Index");
        }
    }
}
