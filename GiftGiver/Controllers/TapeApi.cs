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
    }
}