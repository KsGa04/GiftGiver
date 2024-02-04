using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace tests.Controllers
{
    [ApiController]
    [Route("api/wishlist")]
    public class WishListApi : Controller
    {
        private giftgiverContext? _db;
        public WishListApi(giftgiverContext giftgiver)
        {
            _db = giftgiver;
        }
        [HttpGet]
        [Route("/UserWish")]
        public async Task<ActionResult<Желаемое>> Get(int id)
        {
            // Ваш код для получения ресурса по указанному id
            bool isIdExists = _db.Желаемоеs.Any(r => r.ПользовательId == id);
            if (isIdExists == false)
            {
                return NotFound();
            }
            else
            {

                return Ok(_db.Желаемоеs.Where(r => r.ПользовательId == id).FirstOrDefault());
            }
        }
    }
}