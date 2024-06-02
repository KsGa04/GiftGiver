using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GiftGiver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticApi : ControllerBase
    {
        public giftgiverContext db = new giftgiverContext();


        [HttpPost]
        [Route("UpdateStatistics")]
        public IActionResult UpdateStatistics()
        {
            // Получаем сегодняшнюю дату
            DateTime today = DateTime.Today;

            // Проверяем, является ли сегодня первым днем месяца
            if (today.Day == 1)
            {
                // Получаем общее количество посещений за месяц
                int totalVisits = db.Пользовательs.Sum(u => u.КоличествоПосещений ?? 0);

                // Создаем запись в таблице Статистика
                Статистика statistics = new Статистика
                {
                    Месяц = today.Date,
                    КолПосещений = totalVisits
                };

                db.Статистикаs.Add(statistics);
                db.SaveChanges();

                var users = db.Пользовательs.ToList();
                foreach (var user in users)
                {
                    user.КоличествоПосещений = 0;
                }
                db.SaveChanges();

                return Ok(new { message = "Статистика обновлена успешно." });
            }
            else
            {
                return Ok(new { message = "Сегодня не первый день месяца." });
            }
        }
        [HttpGet]
        [Route("GetUserVisitsThisMonth")]
        public IActionResult GetUserVisitsThisMonth()
        {
            // Получаем сегодняшнюю дату
            DateTime today = DateTime.Now;

            // Получаем первый день текущего месяца
            DateTime startOfMonth = new DateTime(today.Year, today.Month, 1);

            // Получаем количество посещений пользователей с начала месяца до сегодняшнего дня
            var userVisits = db.Пользовательs
                .Where(u => u.ДатаПосещения >= startOfMonth && u.ДатаПосещения <= today)
                .Sum(u => u.КоличествоПосещений ?? 0);

            return Ok(new { TotalVisits = userVisits });
        }
    }
}
