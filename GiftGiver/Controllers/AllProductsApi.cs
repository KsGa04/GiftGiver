using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GiftGiver.Controllers
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
        public ActionResult<IEnumerable<Подарки>> GetAll()
        {
            var result = db.Подаркиs.ToList();
            return result;
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