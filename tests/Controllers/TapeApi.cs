using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace tests.Controllers
{
    [ApiController]
    [Route("api/tape")]
    public class TapeApi : Controller
    {
        private giftgiverContext? _db;
        public TapeApi(giftgiverContext giftgiver)
        {
            _db = giftgiver;
        }
        [HttpGet]
        [Route("/UserTape")]
        public async Task<ActionResult<Лента>> Get(int id)
        {
            // Ваш код для получения ресурса по указанному id
            bool isIdExists = _db.Лентаs.Any(r => r.ПользовательId == id);
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