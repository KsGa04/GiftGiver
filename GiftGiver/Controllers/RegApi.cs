using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GiftGiver.Models;
using GiftGiver.Models;

namespace GiftGiver.Controllers
{
    [ApiController]
    [Route("api/reg")]
    public class RegApi : Controller
    {
        // GET: HomeController
        public giftgiverContext db = new giftgiverContext();
        public SuccessResponse result;
        public RegApi(giftgiverContext giftgiver)
        {
            db = giftgiver;
        }

        [HttpPost]
        [Route("postreg")]
        public SuccessResponse Registration(string login, string email, string password, string fio)
        {
            Пользователь Clients = (from c in db.Пользовательs where c.Email == email || c.Логин == login select c).FirstOrDefault();
            if (Clients != null)
            {
                result = new SuccessResponse
                {
                    Success = false,
                    Message = "Пользователь с данным логином или email уже зарегистрирован",
                };
                return result;
            }
            else
            if (email == null || password == null || login == null)
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
                Пользователь пользователь = new Пользователь()
                {
                    Логин = login,
                    Email = email,
                    Пароль = password,
                    РолиId = 1,
                    Фио = fio,
                    КоличествоПосещений = 0
                };
                db.Пользовательs.Add(пользователь);
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
