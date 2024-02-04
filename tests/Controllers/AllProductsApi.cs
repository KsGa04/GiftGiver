using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace tests.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class AllProductsApi : Controller
    {
        private giftgiverContext? _db;
        public AllProductsApi(giftgiverContext giftgiver)
        {
            _db = giftgiver;
        }
        [HttpGet]
        [Route("/allproducts")]
        public async Task<ActionResult<IEnumerable<Подарки>>> Get()
        {
            return await _db.Подаркиs.ToListAsync();
        }
        [HttpGet]
        [Route("/IdProduct")]
        public async Task<ActionResult<Подарки>> Get(int id)
        {
            // Ваш код для получения ресурса по указанному id
            bool isIdExists = _db.Подаркиs.Any(r => r.ПодаркиId == id);
            if (isIdExists == false)
            {
                return NotFound();
            }
            else
            {

                return Ok(_db.Подаркиs.Where(r => r.ПодаркиId == id).FirstOrDefault());
            }
        }
    }
}