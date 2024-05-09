using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GiftGiver.Controllers
{
    [ApiController]
    [Route("api/wishlist")]
    public class WishListApi : Controller
    {
        public giftgiverContext db = new giftgiverContext();
        public WishListApi(giftgiverContext giftgiver)
        {
            db = giftgiver;
        }
        [HttpGet]
        [Route("/UserWish")]
        public async Task<ActionResult<Желаемое>> Get(int id)
        {
            // Ваш код для получения ресурса по указанному id
            var WishList = db.Желаемоеs.Where(r => r.ПользовательId == id).ToList();
            if (WishList == null)
            {
                return NotFound();
            }
            else
            {

                return Ok(WishList);
            }
        }
        [HttpPost]
        [Route("/AddWish")]
        public async Task<ActionResult<Желаемое>> AddWish(int idПользователь, int idПодарка)
        {
            Желаемое желаемое = new Желаемое()
            {
                ПользовательId = idПользователь,
                ПодаркиId = idПодарка,
            };
            db.Желаемоеs.Add(желаемое);
            db.SaveChanges();
            return Ok(желаемое);

        }

        [HttpGet("get-similar-gifts")]
        public async Task<ActionResult<IEnumerable<SimilarGiftResponse>>> GetSimilarGifts(int id_user)
        {
            var similarGifts = await db.Желаемоеs
                .Where(w => w.Пользователь.ПользовательId != id_user)
                .Where(w => w.Подарки.Наименование.Contains(db.Желаемоеs.Where(x => x.ПользовательId == id_user).Select(x => x.Подарки.Наименование).FirstOrDefault() ?? ""))
                .Select(s => new SimilarGiftResponse
                {
                    ПользовательId = s.ПользовательId,
                    ПодаркиId = s.ПодаркиId,
                    Название = s.Подарки.Наименование
                })
                .ToListAsync();

            return Ok(similarGifts);
        }

        public class SimilarGiftResponse
        {
            public int ПользовательId { get; set; }
            public int ПодаркиId { get; set; }
            public string Название { get; set; }
        }
    }
}