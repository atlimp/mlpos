using Microsoft.AspNetCore.Mvc;

namespace MLPos.Web.Controllers
{
    [Route("/")]
    public class PosController : Controller
    {
        public IActionResult Index()
        {
            return RedirectPermanent("/posclient");
        }
    }
}
