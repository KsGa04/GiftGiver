using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace tests.Controllers
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
            var WishList = db.Желаемоеs.Where(r => r.ПользовательId == id).FirstOrDefault();
            if (WishList == null)
            {
                return NotFound();
            }
            else
            {

                return Ok(WishList);
            }
        }
    }
}