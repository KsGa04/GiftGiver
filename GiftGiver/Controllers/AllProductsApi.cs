using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.Swagger.Annotations;
using Swashbuckle.AspNetCore.Swagger;

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
        [HttpPut]
        [Route("/IdProduct/update")]
        public async Task<ActionResult<Подарки>> Update(int id, int minAge,  string giver, string genre)
        {
            var ListProduct = db.Подаркиs.Where(r => r.ПодаркиId == id).FirstOrDefault();
            if (ListProduct == null)
            {
                return NotFound();
            }
            else
            {
                ListProduct.МинВозраст = minAge;
                ListProduct.Получатель = giver;
                ListProduct.Жанр = genre;
                db.SaveChanges();
                return Ok(ListProduct);
            }
        }
        [HttpDelete]
        [Route("/IdProduct/delete")]
        public async Task<ActionResult<Подарки>> Delete(int id)
        {
            var ListProduct = db.Подаркиs.Where(r => r.ПодаркиId == id).FirstOrDefault();
            if (ListProduct == null)
            {
                return NotFound();
            }
            else
            {
                db.Подаркиs.Remove(ListProduct);
                db.SaveChanges();
                return Ok(ListProduct);
            }
        }
        [HttpGet]
        [Route("/productsForPerson")]
        public ActionResult<IEnumerable<Подарки>> GetForPerson(int id)
        {
            var lenta = db.Лентаs.Where(x => x.ПользовательId ==id).ToList();
            if (lenta == null)
            {
                var result = db.Подаркиs.ToList();
                return result;
            }
            else
            {
                List<Подарки> похожиеПодарки = new List<Подарки>();
                foreach (var запись in lenta)
                {
                    // Получаем данные о подарке по ПодаркиId из таблицы Лента
                    var подарок = db.Подаркиs.Where(p => p.ПодаркиId == запись.ПодаркиId).FirstOrDefault();
                    // Получаем список похожих товаров из таблицы Подарки
                    var похожиеТовары = db.Подаркиs.Where(p => p.Наименование.Contains(подарок.Наименование) && p.Жанр == подарок.Жанр).ToList();
                    похожиеПодарки.AddRange(похожиеТовары);
                }
                return похожиеПодарки;
            }
        }
    }
}