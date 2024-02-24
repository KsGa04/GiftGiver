using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace tests.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class AllProductsApi : Controller
    {
        public giftgiverContext db = new giftgiverContext();
        public AllProductsApi(giftgiverContext giftgiver)
        {
            db = giftgiver;
        }
        [HttpGet]
        [Route("/allproducts")]
        public async Task<ActionResult<IEnumerable<Подарки>>> Get()
        {
            return await db.Подаркиs.ToListAsync();
        }
        [HttpGet]
        [Route("/IdProduct")]
        public async Task<ActionResult<Подарки>> Get(int id)
        {
            var ListProduct = db.Подаркиs.Where(r => r.ПодаркиId == id).FirstOrDefault();
            if (ListProduct == null)
            {
                return NotFound();
            }
            else
            {

                return Ok(ListProduct);
            }
        }
    }
}