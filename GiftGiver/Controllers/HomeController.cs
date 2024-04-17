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
using System;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace GiftGiver.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RegApi _regApi;
        private readonly AuthApi _authApi;
        private readonly AllProductsApi _allProductsApi;
        private readonly AddProductApi _addProductApi;
        private readonly WishListApi _wishListApi;
        private readonly TapeApi _apeApi;
        private giftgiverContext db = new giftgiverContext();

        public int messageCount = 0;


        public HomeController(ILogger<HomeController> logger, AuthApi apiController, giftgiverContext giftgiver, AllProductsApi allProductsApi, WishListApi wishListApi, TapeApi apeApi, AddProductApi addProductApi, RegApi regApi)
        {
            _logger = logger;
            db = giftgiver;
            _authApi = apiController;
            _allProductsApi = allProductsApi;
            _wishListApi = wishListApi;
            _apeApi = apeApi;
            _addProductApi = addProductApi;
            _regApi = regApi;
        }
        /// <summary>
        /// -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        /// </summary>
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
        public IActionResult Registration(string login, string email, string pass, string FIO)
        {
            var result = _regApi.Registration(login, email, pass, FIO);
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
        /// <summary>
        /// -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        /// </summary>
        public class PrivateAccViewModel
        {
            public IEnumerable<Подарки> Products { get; set; }
            public Пользователь User { get; set; }
        }
        [Authorize]
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
        [Authorize]
        [HttpPost]
        public IActionResult PrivateAcc(string mail, string pass, string log, DateTime year)
        {
            Пользователь пользователь = db.Пользовательs.Where(x => x.ПользовательId == CurrentUser.CurrentClientId).FirstOrDefault();
            пользователь.Email = mail;
            пользователь.Пароль = pass;
            пользователь.Логин = log;
            пользователь.Возраст = year;
            db.SaveChanges();
            пользователь = db.Пользовательs.Where(x => x.ПользовательId == CurrentUser.CurrentClientId).FirstOrDefault();
            var wishlist = db.Желаемоеs
        .Where(w => w.ПользовательId == CurrentUser.CurrentClientId)
        .Include(w => w.Подарки)
        .Select(w => w.Подарки)
        .ToList();
            var viewModel = new PrivateAccViewModel
            {
                Products = wishlist,
                User = пользователь
            };
            return View(viewModel);
        }
        //[HttpPost]
        //public async Task<IActionResult> ChangeImage(IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //    {
        //        return BadRequest("Файл не выбран");
        //    }

        //    var allowedExtensions = new[] { ".png", ".jpeg", ".jpg" };
        //    var ext = Path.GetExtension(file.FileName).ToLower();
        //    if (!allowedExtensions.Contains(ext))
        //    {
        //        return BadRequest("Неверный формат файла. Выберите изображение в формате .png, .jpeg или .jpg");
        //    }

        //    var filePath = Path.Combine(
        //        Directory.GetCurrentDirectory(), "wwwroot", "images", "userImage",
        //        "newImage" + ext);

        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await file.CopyToAsync(stream);
        //    }

        //    // Теперь сохраните путь к загруженному изображению в базе данных или отправьте его на фронт-энд для отображения

        //    return Ok("Изображение успешно загружено");
        //}
        //[Authorize]
        public IActionResult AllGift()
        {
            var result = _allProductsApi.GetAll();
            if (CurrentUser.CurrentClientId != 0)
            {
                result = _allProductsApi.GetAllById(CurrentUser.CurrentClientId);
            }
            else
            {
                result = _allProductsApi.GetAll();
            }
            var viewModel = new PrivateAccViewModel
            {
                Products = result.Value
            };
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult AllGift(string find)
        {
            var productList = new List<Подарки>();
            if (find != null)
            {
                productList = db.Подаркиs.Where(x => x.Наименование.Contains(find)).ToList();
            }
            else
            {
                productList = db.Подаркиs.ToList();
            }
            var viewModel = new PrivateAccViewModel
            {
                Products = productList
            };
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult ДобавитьЖелаемое(int подарокId)
        {
            if (CurrentUser.CurrentClientId != 0)
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
            else
            {
                return RedirectToAction("Authorization", "Home");
            }
        }
        [HttpDelete]
        public ActionResult УдалитьЖелаемое(int подарокId)
        {
            var желаемое = db.Желаемоеs.Where(x => x.ПользовательId == CurrentUser.CurrentClientId && x.ПодаркиId == подарокId).FirstOrDefault();
            db.Желаемоеs.Remove(желаемое);
            db.SaveChanges();
            var url = Url.Action("PrivateAcc", "Home");
            return Json(new { success = true, redirectUrl = url });
        }

        public IActionResult Exit()
        {
            CurrentUser.CurrentAdminId = 0;
            CurrentUser.CurrentClientId = 0;
            return RedirectToAction("Authorization", "Home");
        }
        /// <summary>
        /// -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        /// </summary>
        private static readonly ReadOnlyDictionary<string, string> botDictionary = new ReadOnlyDictionary<string, string>(
        new Dictionary<string, string>
        {
            { "На какой праздник необходим подарок?", "" },
            { "Кому хотите подарит подарок?", "" },
            { "Какого возраста получатель?", "" }
        }
    );
        public IActionResult ChatBot()
        {
            return View();
        }

        //[HttpPost("GetMessages")]
        //public IActionResult GetMessages()
        //{
        //    string[] messages = new string[] { "Привет!", "Привет, как дела?" };
        //    return Ok(messages);
        //}

        //[HttpPost("MessageCount")]
        //public IActionResult MessageCount([FromBody] JsonElement data)
        //{
        //    messageCount = data.GetProperty("messageCount").GetInt32();
        //    var messagesArrayBot = data.GetProperty("messages").EnumerateArray().Select(x => x.GetString()).ToList();
        //    var messagesArrayUser = data.GetProperty("messagesUser").EnumerateArray().Select(x => x.GetString()).ToList();
        //    // Обработка полученного количества сообщений
        //    // ...

        //    return Ok(); // Можно вернуть что-то для клиента, если необходимо
        //}
        [HttpPost("GetGifts")]
        public ActionResult GetGifts([FromBody] Dictionary<string, string> userAnswers)
        {
            // Обработка ответов пользователя и получение подарков
            var gifts = db.Подаркиs.Where(x => x.Жанр == userAnswers["На какой праздник необходим подарок?"]).ToList();
            return Json(new { success = true, gifts = gifts }); ;
        }

        private List<Подарки> GetGiftsBasedOnAnswers(Dictionary<string, string> userAnswers)
        {
            var gifts = db.Подаркиs.Where(x => x.Жанр == userAnswers["На какой праздник необходим подарок?"]).ToList();
            return gifts;
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