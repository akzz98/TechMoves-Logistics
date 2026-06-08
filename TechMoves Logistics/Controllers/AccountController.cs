using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechMoves_Logistics.Models;
using TechMoves_Logistics.Services;

namespace TechMoves_Logistics.Controllers
{
    public class AccountController : Controller
    {
        private readonly IApiAuthService _apiAuthService;

        public AccountController(IApiAuthService apiAuthService)
        {
            _apiAuthService = apiAuthService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var loginResult = await _apiAuthService.LoginAsync(model.Username, model.Password);
            if (loginResult == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }

            HttpContext.Session.SetString(AuthSessionKeys.JwtToken, loginResult.Token);
            HttpContext.Session.SetString(AuthSessionKeys.Username, loginResult.Username);
            HttpContext.Session.SetString(AuthSessionKeys.ExpiresAt, loginResult.ExpiresAt.ToString("O"));

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, loginResult.Username)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
    }
}
