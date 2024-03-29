﻿using GiftGiver.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml;
using System;
using HtmlAgilityPack;
using CefSharp;
using CefSharp.OffScreen;
using Common.Browser;
using ImageMagick;
using CefSharp.DevTools.CSS;
using System.Globalization;

namespace GiftGiver.Controllers
{
    [Route("api/addProduct")]
    [ApiController]
    public class AddProductApi : Controller
    {
        // GET: HomeController
        private giftgiverContext db = new giftgiverContext();
        public ProductResponce gift;
        public AddProductApi(giftgiverContext giftgiver)
        {
            db = giftgiver;
        }

        [HttpPost]
        [Route("addWBproduct")]
        public async Task<ProductResponce> AddWBProduct(string link)
        {
            var result = await LoadProduct(link);
            string[] segments = link.Split('/');
            string itemId = segments[4];
            decimal cost;
            NumberStyles style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;
            CultureInfo culture = new CultureInfo("ru-RU");

            if (decimal.TryParse(result.Item2, style, culture, out cost))
            {
                Подарки подарки = new Подарки();
                подарки.ПодаркиId = Convert.ToInt32(itemId);
                подарки.Наименование = result.Item1;
                подарки.Цена = cost;
                подарки.Ссылка = link;
                подарки.Изображение = result.Item3;
                db.Подаркиs.Add(подарки);
                db.SaveChanges();
                gift = new ProductResponce
                {
                    Name = result.Item1,
                    Cost = result.Item2,
                    Image = result.Item3.ToString(),
                    Link = link
                };
            }
            else
            {
                Console.WriteLine("Невозможно преобразовать строку в формат decimal");
            }
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

        private async Task<(string, string, byte[])> LoadProduct(string url)
        {
            using (var chromium = new ChromiumWebBrowser(string.Empty))
            {
                await Task.Delay(2000);
                await chromium.LoadUrlAsync(url);

                string productName = string.Empty;
                string price = string.Empty;
                string photoUrl = string.Empty;
                byte[] photoData = null;

                await chromium.WaitUntill(async browser =>
                {
                    var res = await TryGetStringResult(chromium, browser.GetElementByClassName("product-page__title", 0).GetInnerText());
                    var res2 = await TryGetStringResult(chromium, browser.GetElementByClassName("price-block__final-price", 0).GetInnerText());
                    var res3 = await TryGetStringResult(chromium, browser.GetElementByClassName("photo-zoom__preview j-zoom-image", 0).GetAttribute("src"));
                    
                    productName = res.Result?.Trim();
                    price = res2.Result?.Trim();
                    photoUrl = res3.Result?.Trim();

                    return res.Success && res2.Success && res3.Success
                    && res.Result != null && res2.Result != null && res3.Result != null;
                });


                using (HttpClient client = new HttpClient())
                {
                    photoData = ConvertToJpg(await client.GetByteArrayAsync(photoUrl));
                }

                return (productName, price, photoData);
            }
        }
        public static byte[] ConvertToJpg(byte[] input)
        {
            using var image = new MagickImage(input);
            image.Format = MagickFormat.Jpeg;

            // Create byte array that contains a jpeg file
            var result = image.ToByteArray();
            return result;
        }
    }
}
