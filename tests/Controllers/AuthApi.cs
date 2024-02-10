using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GiftGiver.Models;
using tests.Models;

namespace tests.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthApi : Controller
    {
        // GET: HomeController
        private giftgiverContext? db;
        public SuccessResponse result;
        public AuthApi(giftgiverContext cookingBook)
        {
            db = cookingBook;
        }

        [HttpPost]
        [Route("postauth")]
        public SuccessResponse GetAuth(string loginOrEmail, string password)
        {
            CurrentUser.CurrentClientId = (from c in db.Пользовательs where (c.Email == loginOrEmail || c.Логин == loginOrEmail) && c.Пароль == password select c.ПользовательId).FirstOrDefault();
            Пользователь client = (from c in db.Пользовательs where (c.Email == loginOrEmail || c.Логин == loginOrEmail) && c.Пароль == password select c).FirstOrDefault();
            if (CurrentUser.CurrentClientId > 0)
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, "test"),
                        new Claim(ClaimTypes.Email, "testc@mail.ru")};
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");

                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                result = new SuccessResponse
                {
                    Success = true,
                    Message = "Авторизация успешна",
                };
                return result;
            }
            else
            {
                result = new SuccessResponse
                {
                    Success = false,
                    Message = "Данные неверные",
                };
                return result;
            }
        }
    }
}
