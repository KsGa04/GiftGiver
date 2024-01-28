using GiftGiver.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml;
using System;
using HtmlAgilityPack;
using tests.Models;

namespace tests.Controllers
{
    [Route("api/addProduct")]
    [ApiController]
    public class AddProductApi : Controller
    {
        // GET: HomeController
        private giftgiverContext? db;
        public ProductResponce result;
        public AddProductApi(giftgiverContext cookingBook)
        {
            db = cookingBook;
        }

        [HttpPost]
        [Route("postreg")]
        public ProductResponce AddProduct(string link)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(link);

            // Получение названия товара
            HtmlNode titleNode = doc.DocumentNode.SelectSingleNode("//h1[@class='brand-and-name j-product-title']");
            string title = titleNode.InnerText.Trim();

            // Получение цены товара
            HtmlNode priceNode = doc.DocumentNode.SelectSingleNode("//span[@class='final-cost']");
            string price = priceNode.InnerText.Trim();

            result = new ProductResponce {
                Name = title,
                Cost = price
            };
            return result;
        }
    }
}
