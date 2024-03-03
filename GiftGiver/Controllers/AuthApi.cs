using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GiftGiver.Models;
using GiftGiver.Models;

namespace GiftGiver.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthApi : Controller
    {
        // GET: HomeController
        private giftgiverContext db = new giftgiverContext();
        public SuccessResponse result;
        public AuthApi(giftgiverContext giftgiver)
        {
            db = giftgiver;
        }

        [HttpPost]
        [Route("postauth")]
        public async Task<ActionResult<SuccessResponse>> GetAuth(string loginOrEmail, string password)
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
                return Ok(result); // Возвращаем 200 OK и объект SuccessResponse
            }
            else
            {
                var result = new SuccessResponse
                {
                    Success = false,
                    Message = "Данные неверные",
                };
                return BadRequest(result); // Возвращаем 400 Bad Request и объект SuccessResponse
            }
        }
    }
}
