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
using Microsoft.EntityFrameworkCore;

namespace GiftGiver.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RegApi _regApi;
        private readonly AuthApi _authApi;
        private readonly AllProductsApi _allProductsApi;
        private readonly WishListApi _wishListApi;
        private readonly TapeApi _apeApi;
        private giftgiverContext db = new giftgiverContext();


        public HomeController(ILogger<HomeController> logger, AuthApi apiController, giftgiverContext giftgiver, AllProductsApi allProductsApi, WishListApi wishListApi, TapeApi apeApi)
        {
            _logger = logger;
            db = giftgiver;
            _authApi = apiController;
            _allProductsApi = allProductsApi;
            _wishListApi = wishListApi;
            _apeApi = apeApi;
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
            CurrentUser.CurrentAdminId = (from c in db.Администраторs where (c.Логин == loginOrEmail && c.Пароль == password) select c.АдминистраторId).FirstOrDefault();
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
            else if (CurrentUser.CurrentAdminId > 0)
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
                return Redirect("/swagger/index.html");
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
            public IEnumerable<Подарки> Products { get; set; }
            public Пользователь User { get; set; }
        }
        public IActionResult PrivateAcc()
        {
            Пользователь пользователь = db.Пользовательs.Where(x => x.ПользовательId == CurrentUser.CurrentClientId).FirstOrDefault();
            var wishlist = db.Желаемоеs
        .Where(w => w.ПользовательId == CurrentUser.CurrentClientId)
        .Include(w => w.Подарки)
        .Select(w => w.Подарки)
        .ToList();

            // Создаем модель представления и передаем список желаемых подарков
            var viewModel = new PrivateAccViewModel
            {
                Products = wishlist,
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
        [Authorize]
        public IActionResult AllGift()
        {
            var result = _allProductsApi.GetAll();
            var viewModel = new PrivateAccViewModel
            {
                Products = result.Value
            };
            return View(viewModel);
        }
        [HttpGet]
        public ActionResult Find(string text)
        {
            var productList = db.Подаркиs.Where(x => x.Наименование.Contains(text)).ToList();
            var viewModel = new PrivateAccViewModel
            {
                Products = productList
            };
            return Json(viewModel);
        }
        [HttpPost]
        public ActionResult ДобавитьЖелаемое(int подарокId)
        {
            var желаемое = db.Желаемоеs.Where(x => x.ПользовательId == CurrentUser.CurrentClientId).ToList();
            var ужеЖелаемый = желаемое.Any(ж => ж.ПодаркиId == подарокId);

            if (ужеЖелаемый)
            {
                return Json(new { success = false });
            }
            else
            {
                var wish = _wishListApi.AddWish(CurrentUser.CurrentClientId, подарокId);
                var tape = _apeApi.FormattingAdd(CurrentUser.CurrentClientId, подарокId);
                return Json(new { success = true });
            }
        }
        [HttpDelete]
        public ActionResult УдалитьЖелаемое(int подарокId)
        {
            var желаемое = db.Желаемоеs.Where(x => x.ПользовательId == CurrentUser.CurrentClientId && x.ПодаркиId == подарокId).FirstOrDefault();
            db.Желаемоеs.Remove(желаемое);
            db.SaveChanges();
                return Json(new { success = true });
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