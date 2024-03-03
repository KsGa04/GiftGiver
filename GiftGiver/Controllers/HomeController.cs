using GiftGiver.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using GiftGiver;
using GiftGiver.Controllers;
using Microsoft.VisualBasic;

namespace GiftGiver.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RegApi _regApi;
        private readonly AuthApi _authApi;
        private readonly AllProductsApi _allProductsApi;
        private giftgiverContext db = new giftgiverContext();


        public HomeController(ILogger<HomeController> logger, AuthApi apiController, giftgiverContext giftgiver, AllProductsApi allProductsApi)
        {
            _logger = logger;
            db = giftgiver;
            _authApi = apiController;
            _allProductsApi = allProductsApi;
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
            CurrentUser.CurrentClientId = (from c in db.Пользовательs where (c.Email == loginOrEmail || c.Логин == loginOrEmail) && c.Пароль == password select c.ПользовательId).FirstOrDefault();
            Пользователь client = (from c in db.Пользовательs where (c.Email == loginOrEmail || c.Логин == loginOrEmail) && c.Пароль == password select c).FirstOrDefault();
            if (CurrentUser.CurrentClientId > 0)
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, "test"),
                new Claim(ClaimTypes.Email, "testc@mail.ru")};
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");

                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                var result = new SuccessResponse
                {
                    Success = true,
                    Message = "Авторизация успешна",
                };
                return RedirectToAction("PrivateAcc", "Home");
            }
            else
            {
                var result = new SuccessResponse
                {
                    Success = false,
                    Message = "Данные неверные",
                };
                return ViewBag.Enter = result.Message;
            }
        }
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registration(string login, string email, string password)
        {
            var result = _regApi.Registration(login, email, password);
            if (result.Success == true)
            {
                return RedirectToAction("Authorization", "Home");
            }
            else
            {
                ViewBag.Enter = result.Message;
                return View();
            }
        }

        public IActionResult Recovery1()
        {
            return View();
        }

        public IActionResult Recovery2()
        {
            return View();
        }
        public class PrivateAccViewModel
        {
            public IEnumerable<Подарки> Products { get; set; } // замените Product на ваш реальный тип данных
            public Пользователь User { get; set; } // замените User на ваш реальный тип данных
        }
        public IActionResult PrivateAcc()
        {
            var result = _allProductsApi.GetAll();
            Пользователь пользователь = db.Пользовательs.Where(x => x.ПользовательId == CurrentUser.CurrentClientId).FirstOrDefault();
            var viewModel = new PrivateAccViewModel
            {
                Products = result.Value,
                User = пользователь
            };
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult PrivateAcc(string email, string pass, string log, DateTime year)
        {
            var result = _allProductsApi.GetAll();
            Пользователь пользователь = db.Пользовательs.Where(x => x.ПользовательId == CurrentUser.CurrentClientId).FirstOrDefault();
            пользователь.Email = email;
            пользователь.Пароль = pass;
            пользователь.Логин = log;
            пользователь.Возраст = year;
            db.SaveChanges();
            var viewModel = new PrivateAccViewModel
            {
                Products = result.Value,
                User = пользователь
            };
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не выбран");
            }

            var allowedExtensions = new[] { ".png", ".jpeg", ".jpg" };
            var ext = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(ext))
            {
                return BadRequest("Неверный формат файла. Выберите изображение в формате .png, .jpeg или .jpg");
            }

            var filePath = Path.Combine(
                Directory.GetCurrentDirectory(), "wwwroot", "images", "userImage",
                "newImage" + ext);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Теперь сохраните путь к загруженному изображению в базе данных или отправьте его на фронт-энд для отображения

            return Ok("Изображение успешно загружено");
        }

        public IActionResult AllGift()
        {
            var result = _allProductsApi.GetAll();
            return View(result.Value);
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