using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.Swagger.Annotations;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
using SwaggerResponseAttribute = Swashbuckle.AspNetCore.Annotations.SwaggerResponseAttribute;
using static GiftGiver.Controllers.HomeController;

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
        [SwaggerResponse(StatusCodes.Status200OK, "Успешное выполнение", typeof(Подарки))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Элемент не найден")]
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
                ListProduct.Жанр = genre.ToString();
                db.SaveChanges();
                ListProduct = db.Подаркиs.Where(r => r.ПодаркиId == id).FirstOrDefault();
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
        [Route("/allproducts/{id}")]
        public ActionResult<IEnumerable<Подарки>> GetAllById(int id)
        {
            var giftsFromFeed = db.Лентаs.Where(лента => лента.ПользовательId == id)
                                         .Select(лента => лента.ПодаркиId)
                                         .ToList();
            if (giftsFromFeed.Count != 0)
            {
                var similarGifts = new List<Подарки>();

                foreach (var giftId in giftsFromFeed)
                {
                    var gift = db.Подаркиs.FirstOrDefault(подарок => подарок.ПодаркиId == giftId);
                    if (gift != null)
                    {
                        var words = gift.Наименование.Split(' '); // разбиваем название подарка на слова
                        foreach (var word in words)
                        {
                            var similarGiftsForWord = db.Подаркиs.Where(подарок => подарок.Наименование.Contains(word) && подарок.ПодаркиId != giftId)
                                                                 .ToList();
                            similarGifts.AddRange(similarGiftsForWord);
                        }
                    }
                }

                return similarGifts.Distinct().ToList(); // возвращаем уникальные подарки
            }
            else
            {
                var result = db.Подаркиs.ToList();
                return result;
            }
            
        }

        [HttpGet]
        [Route("/usersproducts/{id}")]
        public ActionResult<IEnumerable<Similar>> GetUsersProductById(int id)
        {
            var userWishlist = db.Желаемоеs.Where(w => w.Пользователь.ПользовательId == id)
                                   .Select(w => new
                                   {
                                       Наименование = w.Подарки.Наименование,
                                       Жанр = w.Подарки.Жанр,
                                       Получатель = w.Подарки.Получатель,
                                       ПодаркиId = w.Подарки.ПодаркиId
                                   })
                                   .ToList();
         

            var similarProducts = new Dictionary<int, Similar>();

            foreach (var product in userWishlist)
            {
                foreach (var genre in product.Жанр.Split(";"))
                {
                    foreach (var recipient in product.Получатель.Split(";"))
                    {
                        var similarProductsForProduct = db.Желаемоеs.Where(w => w.Пользователь.ПользовательId != id &&
                                                         (w.Подарки.Наименование.Contains(product.Наименование) ||
                                                          w.Подарки.Жанр.Contains(genre.Trim()) ||
                                                          w.Подарки.Получатель.Contains(recipient.Trim())) &&
                                                         w.Подарки.ПодаркиId != product.ПодаркиId)
                                                     .Select(w => new Similar
                                                     {
                                                         Наименование = w.Подарки.Наименование,
                                                         Изображение = w.Подарки.Изображение,
                                                         Ссылка = w.Подарки.Ссылка,
                                                         ПодаркиId = w.Подарки.ПодаркиId,
                                                         Логин = w.Пользователь.Логин,
                                                         Цена = (int)w.Подарки.Цена
                                                     })
                                                     .ToList();

                        foreach (var similar in similarProductsForProduct)
                        {
                            if (!similarProducts.ContainsKey(similar.ПодаркиId))
                            {
                                similarProducts.Add(similar.ПодаркиId, similar);
                            }
                        }
                    }
                }
            }

            return new List<Similar>(similarProducts.Values);
        }
    }
}