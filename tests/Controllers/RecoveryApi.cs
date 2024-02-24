using GiftGiver.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;


namespace tests.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class RecoveryApi : Controller
    {
        // GET: HomeController
        public SuccessResponse result;

        [HttpPost]
        [Route("sendcode")]
        public SuccessResponse SendCode([FromBody] EmailParams emailParams, string login)
        {
            try
            {
                var fromAddress = new MailAddress("kseniagaranceva@gmail.com", "Admin");
                var toAddress = new MailAddress(emailParams.ToEmail, "Recipient Name");
                const string fromPassword = "Arthur3007!";
                const string subject = "Verification Code";
                string body = "Your verification code is: " + emailParams.VerificationCode;

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com", // Используйте нужный SMTP-сервер
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.SendMailAsync(message);
                }

                result = new SuccessResponse
                {
                    Success = true,
                    Message = "Код был отправлен",
                };
                return result;
            }
            catch (Exception ex)
            {
                result = new SuccessResponse
                {
                    Success = true,
                    Message = ex.Message,
                };
                return result;
            }
        }
        public class EmailParams
        {
            public string ToEmail { get; set; }
            public string VerificationCode { get; set; }
        }
    }
}
