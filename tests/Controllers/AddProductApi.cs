using GiftGiver.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml;
using System;
using HtmlAgilityPack;
using tests.Models;
using CefSharp;
using CefSharp.OffScreen;
using Common.Browser;

namespace tests.Controllers
{
    [Route("api/addProduct")]
    [ApiController]
    public class AddProductApi : Controller
    {
        // GET: HomeController
        private giftgiverContext? db;
        public ProductResponce gift;
        public AddProductApi(giftgiverContext cookingBook)
        {
            db = cookingBook;
        }

        [HttpPost]
        [Route("addWBproduct")]
        public async Task<ProductResponce> AddProduct(string link)
        {
            var result = await LoadProduct(link);
            gift = new ProductResponce
            {
                Name = result.Item1,
                Cost = result.Item2,
                Link = link
            };
            return gift;

        }
        protected async Task<JavascriptResponse> ExecuteJavaScript(ChromiumWebBrowser wb1, string s)
        {
            try
            {
                return await wb1.EvaluateScriptAsync(s);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private async Task<(bool Success, string Result)> TryGetStringResult(ChromiumWebBrowser wb1, string script)
        {
            var shopIdresponse = await ExecuteJavaScript(wb1, script);
            if (shopIdresponse.Success && shopIdresponse.Result is string resultString && !string.IsNullOrEmpty(resultString))
            {
                return (true, resultString);
            }
            else
            {
                return (true, null);
            }
        }

        private async Task<(string, string)> LoadProduct(string url)
        {
            using (var chromium = new ChromiumWebBrowser(string.Empty))
            {
                await Task.Delay(2000);
                await chromium.LoadUrlAsync(url);

                string productName = string.Empty;
                string price = string.Empty;

                await chromium.WaitUntill(async browser =>
                {
                    var res = await TryGetStringResult(chromium, browser.GetElementByClassName("product-page__title", 0).GetInnerText());
                    var res2 = await TryGetStringResult(chromium, browser.GetElementByClassName("price-block__final-price", 0).GetInnerText());

                    productName = res.Result?.Trim();
                    price = res2.Result?.Trim();
                    return res.Success && res2.Success && res.Result != null && res2.Result != null;
                });

                return (productName, price);
            }
        }
    }
}
