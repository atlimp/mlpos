using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MLPos.Core.Interfaces.Services;
using MLPos.Core.Model;
using MLPos.Web.Controllers;
using MLPos.Web.Models;
using System.Security.Authentication;
using System.Security.Claims;

namespace MLPos.Web.Controllers
{
    [Route("/Admin")]
    public class LoginController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoginService _loginService;
        public LoginController(IHttpContextAccessor httpContextAccessor, ILoginService loginService)
        {
            _httpContextAccessor = httpContextAccessor;
            _loginService = loginService;
        }

        [HttpGet("Login")]
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("~/Admin");
            }

            if (TempData["ErrorMessage"] != null)
            {
                return View(new LoginViewModel
                {
                    Error = (string)TempData["ErrorMessage"]
                });
            }

            return View();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Index(LoginViewModel model, string ReturnUrl = "/Admin")
        {
            User? user = null;
            try
            {
                user = await _loginService.ValidateUser(new LoginCredentials()
                {
                    Username = model.Username,
                    Password = model.Password,
                });
            }
            catch (InvalidCredentialException e)
            {
                TempData["ErrorMessage"] = "Invalid username or password";

                return RedirectToAction("Index", "Login");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties());

            return Redirect(ReturnUrl);
        }

        [HttpGet("Logout")]
        public IActionResult Logout(LoginViewModel model)
        {
            _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect("/Admin/Login");
        }
    }
}
