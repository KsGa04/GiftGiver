using GiftGiver.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

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
        [Authorize]
        public IActionResult AllGift()
        {
            var result = _allProductsApi.GetAll();
                result = _allProductsApi.GetAll();
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
            var result = _allProductsApi.GetAll();
            if (find != null)
            {
                productList = db.Подаркиs.Where(x => x.Наименование.Contains(find)).ToList();
                var viewModel = new PrivateAccViewModel
                {
                    Products = productList
                };
                return View(viewModel);
            }
            else
            {
                    result = _allProductsApi.GetAll();
                var viewModel = new PrivateAccViewModel
                {
                    Products = result.Value
                };
                return View(viewModel);
            }
            
            
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
        

        public IActionResult ChatBot()
        {
            return View();
        }

        public class Gift
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Link { get; set; }
            public byte[] Image { get; set; }
        }

        public class GiftResponse
        {
            public int TotalCount { get; set; }
            public List<Gift> Gifts { get; set; }
        }

        private List<int> displayedGiftIds = new List<int>();

        [HttpPost("GetGifts")]
        public ActionResult GetGifts([FromBody] Dictionary<string, string> userAnswers)
        {
            var recipient = userAnswers["Кому хотите подарить подарок?"];
            var holiday = userAnswers["Какой категории вам необходим подарок?"];

            var gifts = db.Подаркиs.Where(x => x.Жанр.Contains(holiday) || x.Получатель.Contains(recipient))
                                   .Select(x => new Gift { Name = x.Наименование, Link = x.Ссылка, Image = x.Изображение, Id = x.ПодаркиId })
                                   .ToList();

            var uniqueGifts = gifts.Where(g => !displayedGiftIds.Contains(g.Id)).ToList();

            if (uniqueGifts.Count > 3)
            {
                var random = new Random();
                var selectedGifts = uniqueGifts.OrderBy(x => random.Next()).Take(3).ToList();
                var response = new GiftResponse
                {
                    TotalCount = uniqueGifts.Count,
                    Gifts = selectedGifts
                };

                displayedGiftIds.AddRange(selectedGifts.Select(g => g.Id));

                return Ok(response);
            }
            else
            {
                var response = new GiftResponse
                {
                    TotalCount = uniqueGifts.Count,
                    Gifts = uniqueGifts
                };
                return Ok(response);
            }
        }
        /// <summary>
        /// -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        /// </summary>
        [Authorize]
        public IActionResult SimilarGifts()
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