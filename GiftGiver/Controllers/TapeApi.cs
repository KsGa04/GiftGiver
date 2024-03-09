using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GiftGiver.Controllers
{
    [ApiController]
    [Route("api/tape")]
    public class TapeApi : Controller
    {
        public giftgiverContext db = new giftgiverContext();
        public TapeApi(giftgiverContext giftgiver)
        {
            db = giftgiver;
        }
        [HttpGet]
        [Route("/UserTape")]
        public async Task<ActionResult<Лента>> Get(int id)
        {
            var ListTape = db.Лентаs.Where(r => r.ПользовательId == id).FirstOrDefault();
            if (ListTape == null)
            {
                return NotFound();
            }
            else
            {

                return Ok(ListTape);
            }
        }
        [HttpPost]
        [Route("/FormattingAdd")]
        public async Task<ActionResult<Лента>> FormattingAdd(int idПользователь, int idПодарка)
        {
            var ListTape = db.Лентаs.Where(r => r.ПользовательId == idПользователь).ToList();
            if (ListTape == null)
            {
                return NotFound();
            }
            else
            {
                if (ListTape.Count < 5)
                {
                    Лента лента = new Лента()
                    {
                        ПользовательId = idПользователь,
                        ПодаркиId = idПодарка,
                        ВремяЗапроса = DateTime.Now
                    };
                    db.Лентаs.Add(лента);
                    db.SaveChanges();
                }
                else
                {
                    Лента самаяСтараяЛента = ListTape.OrderBy(l => l.ВремяЗапроса).First();
                    db.Лентаs.Remove(самаяСтараяЛента);
                    Лента новаяЛента = new Лента()
                    {
                        ПользовательId = idПользователь,
                        ПодаркиId = idПодарка,
                        ВремяЗапроса = DateTime.Now
                    };
                    db.Лентаs.Add(новаяЛента);
                    db.SaveChanges();
                }
                return Ok(ListTape);
            }
        }
    }
}