using AdminApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AdminApplication.Controllers
{
    public class AuthController : Controller
    {
        private readonly MainAppSettings _mainAppSettings;
        public AuthController(IOptions<MainAppSettings> mainAppSettings)
        {
            _mainAppSettings = mainAppSettings.Value;
        }

        [HttpGet]
        public IActionResult LogInWithToken(string token)
        {
            if(string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }

            HttpContext.Session.SetString("AdminJwt", token);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminJwt");

            return Redirect(_mainAppSettings.ApiBaseUrl + "ExternalLogout");
        }
    }
}
