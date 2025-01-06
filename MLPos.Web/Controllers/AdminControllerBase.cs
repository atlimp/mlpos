using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MLPos.Web.Controllers
{
    [Authorize]
    public class AdminControllerBase : Controller
    {
    }
}
