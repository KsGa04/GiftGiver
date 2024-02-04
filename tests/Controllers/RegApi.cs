using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GiftGiver.Models;
using tests.Models;

namespace tests.Controllers
{
    [ApiController]
    [Route("api/reg")]
    public class RegApi : Controller
    {
        // GET: HomeController
        private giftgiverContext? db;
        public SuccessResponse result;
        public RegApi(giftgiverContext cookingBook)
        {
            db = cookingBook;
        }

        [HttpPost]
        [Route("postreg")]
        public SuccessResponse Registration(string login, string email, string password)
        {
            using (giftgiverContext db = new giftgiverContext())
            {
                int countClients = (from c in db.Пользовательs where c.Email == email || c.Логин == login select c).Count();
                if (countClients > 0)
                {
                    result = new SuccessResponse
                    {
                        Success = false,
                        Message = "Пользователь с данным логином или email уже зарегистрирован",
                    };
                    return result;
                }
                else if (email == null || password == null || login == null)
                {
                    result = new SuccessResponse
                    {
                        Success = false,
                        Message = "Некоторые поля пустые",
                    };
                    return result;
                }
                else
                {
                    db.Add(new Пользователь { Логин = login, Email = email, Пароль = password, РолиId = 1 });
                    db.SaveChanges();
                    result = new SuccessResponse
                    {
                        Success = true,
                        Message = "Регистрация успешна",
                    };
                    return result;
                }
            }
        }
    }
}
