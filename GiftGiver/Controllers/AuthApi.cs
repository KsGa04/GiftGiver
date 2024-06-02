using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GiftGiver.Models;
using Swashbuckle.Swagger;
using Swashbuckle.AspNetCore.Annotations;

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
        [SwaggerOperation(
    Summary = "Аутентификация пользователя",
    Description = "Данный метод позволяет аутентифицировать пользователя по его логину/email и паролю. " +
                  "Если данные верны, метод возвращает объект SuccessResponse с успешным результатом и выполняет вход пользователя. " +
                  "Если данные неверны, метод возвращает объект SuccessResponse с неуспешным результатом.",
    Tags = new[] { "Аутентификация" }
)]
        public async Task<ActionResult<SuccessResponse>> GetAuth(string loginOrEmail, string password)
        {
            CurrentUser.CurrentClientId = (from c in db.Пользовательs where (c.Email == loginOrEmail || c.Логин == loginOrEmail) && c.Пароль == password select c.ПользовательId).FirstOrDefault();
            Пользователь client = (from c in db.Пользовательs where (c.Email == loginOrEmail || c.Логин == loginOrEmail) && c.Пароль == password select c).FirstOrDefault();
            if (CurrentUser.CurrentClientId > 0)
            {

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
