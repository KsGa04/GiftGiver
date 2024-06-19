using GiftGiver.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

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

                Пользователь пользователь = db.Пользовательs.Where(c => (c.Email == loginOrEmail || c.Логин == loginOrEmail) && c.Пароль == password).FirstOrDefault();
                пользователь.КоличествоПосещений += 1;
                пользователь.ДатаПосещения = DateTime.Now;
                db.SaveChanges();
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
                //return Redirect("/swagger/index.html");
                return RedirectToAction("AdminPanel", "Home");
            }
            else
            {
                var result = new SuccessResponse
                {
                    Success = false,
                    Message = "Данные неверные",
                };
                ViewBag.Enter = "Данные неверные";
                return View();
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
            if (CurrentUser.CurrentClientId != 0)
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
            else
            {
                return RedirectToAction("Authorization", "Home");
            }
            
        }
        [Authorize]
        [HttpPost]
        public IActionResult PrivateAcc(string mail, string pass, string log, DateTime year)
        {
            if (string.IsNullOrEmpty(mail) || string.IsNullOrEmpty(pass) || string.IsNullOrEmpty(log))
            {
                ViewBag.Enter = "Не все данные заполнены";
                Пользователь пользователь = db.Пользовательs.Where(x => x.ПользовательId == CurrentUser.CurrentClientId).FirstOrDefault();
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
            else
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
           
        }
        [Authorize]
        public IActionResult AdminPanel()
        {
            var result = _allProductsApi.GetAll();
            var viewModel = new PrivateAccViewModel
            {
                Products = result.Value
            };
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult AdminPanel(string link)
        {
            var productList = new List<Подарки>();
            var result = _allProductsApi.GetAll();
            if (link != null)
            {
                var addproduct = _addProductApi.AddWBProduct(link);
                Task.Delay(2000);
                result = _allProductsApi.GetAll();
                var viewModel = new PrivateAccViewModel
                {
                    Products = result.Value
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

        [Authorize]
        public IActionResult ChatBot()
        {
            if (CurrentUser.CurrentClientId != 0)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Authorization", "Home");
            }
        }

        public class Gift
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Link { get; set; }
            public byte[] Image { get; set; }
            public decimal Count { get; set; }
        }

        public class GiftResponse
        {
            public int TotalCount { get; set; }
            public List<Gift> Gifts { get; set; }
        }
        private static string CapitalizeFirstLetter(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            return char.ToUpper(str[0]) + str.Substring(1);
        }


        private static List<int> displayedGiftIds = new List<int>();

        [HttpPost("GetGifts")]
        public ActionResult GetGifts([FromBody] Dictionary<string, string> userAnswers)
        {
            var recipient = CapitalizeFirstLetter(userAnswers["Кому хотите подарить подарок?"]);
            var holiday = CapitalizeFirstLetter(userAnswers["Какой категории вам необходим подарок?"]);
            var yearStr = userAnswers["Какого возраста получатель?"];
            var year = Convert.ToInt32(yearStr);
            var ages = db.Подаркиs.Select(x => x.МинВозраст).Distinct().OrderByDescending(x => x).ToList();

            // Находим ближайший минимальный возраст
            var nearestAge = ages.FirstOrDefault(a => a <= year);
            if (nearestAge == 0)
            {
                nearestAge = ages.LastOrDefault();
            }

            var gifts = db.Подаркиs.Where(x => x.Жанр.Contains(holiday) && x.Получатель.Contains(recipient) && x.МинВозраст == nearestAge)
                                   .Select(x => new Gift { Name = x.Наименование, Link = x.Ссылка, Image = x.Изображение, Id = x.ПодаркиId, Count = x.Цена })
                                   .ToList();
            while (gifts.Count == 0)
            {
                nearestAge = ages.FirstOrDefault(a => a <= nearestAge - 1);
                if (nearestAge == 0)
                {
                    nearestAge = ages.LastOrDefault();
                }
                gifts = db.Подаркиs.Where(x => x.Жанр.Contains(holiday) && x.Получатель.Contains(recipient) && x.МинВозраст == nearestAge)
                                   .Select(x => new Gift { Name = x.Наименование, Link = x.Ссылка, Image = x.Изображение, Id = x.ПодаркиId, Count = x.Цена })
                                   .ToList();
            }

            var uniqueGifts = gifts.Where(g => !displayedGiftIds.Contains(g.Id)).ToList();


            if (uniqueGifts.Count > 3)
            {
                var selectedGifts = uniqueGifts.Take(3).ToList();
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
                displayedGiftIds.AddRange(uniqueGifts.Select(g => g.Id));
                return Ok(response);
            }
        }
        /// <summary>
        /// -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        /// </summary>
        public class Similar
        {
            public string Наименование { get; set; }
            public byte[] Изображение { get; set; }
            public string Ссылка { get; set; }
            public int ПодаркиId { get; set; }
            public string Логин { get; set; }
            public int Цена { get; set; }
        }
        public class ListSimilar
        {
            public IEnumerable<Similar> Products { get; set; }
        }
        [Authorize]
        public IActionResult SimilarGifts()
        {
            if (CurrentUser.CurrentClientId != 0)
            {
                // Получаем ID текущего пользователя
                var currentUserId = CurrentUser.CurrentClientId;

                // Получаем список похожих подарков для текущего пользователя
                var products = _allProductsApi.GetUsersProductById(currentUserId).Value;

                // Создаем модель
                var model = new ListSimilar
                {
                    Products = products
                };

                // Возвращаем представление с моделью
                return View(model);
            }
            else
            {
                return RedirectToAction("Authorization", "Home");
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