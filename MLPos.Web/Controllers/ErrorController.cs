using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace MLPos.Web.Controllers
{
    [Route("/Error")]
    public class ErrorController : Controller
    {
        private readonly IStringLocalizer<SharedResources> _localizer;

        public ErrorController(IStringLocalizer<SharedResources> localizer)
        {
            _localizer = localizer;
        }

        [Route("500")]
        public IActionResult Index()
        {
            ViewData["ErrorMessage"] = _localizer["Error500"];
            return View();
        }

        [Route("404")]
        public IActionResult PageNotFound()
        {
            string originalPath = "unknown";
            if (HttpContext.Items.ContainsKey("originalPath"))
            {
                originalPath = HttpContext.Items["originalPath"] as string;
            }

            ViewData["ErrorMessage"] = _localizer["Error404", originalPath];
            return View("Index");
        }
    }
}
