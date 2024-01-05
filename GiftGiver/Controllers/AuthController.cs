using GiftGiver.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using tests.Controllers;

namespace GiftGiver.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AuthApi _apiController;

        public HomeController(ILogger<HomeController> logger, AuthApi apiController)
        {
            _logger = logger;
            _apiController = apiController;
        }
        [AllowAnonymous]
        public IActionResult Authorization()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Authorization(string loginOrEmail, string password)
        {
            var result = _apiController.GetAuth(loginOrEmail, password);
            if (result.Success == true)
            {
                return RedirectToAction("Privacy", "Home");
            }
            else
            {
                ViewBag.Enter = result.Message;
                return View();
            }
        }

            [AllowAnonymous]
            public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}