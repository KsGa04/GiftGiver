using CefSharp.OffScreen;
using CefSharp;
using GiftGiver.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static GiftGiver.Controllers.HomeController;
using System.Globalization;
using Common.Browser;
using ImageMagick;

namespace GiftGiver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddOzonProductApi : ControllerBase
    {
        private giftgiverContext db = new giftgiverContext();
        public ProductResponce gift;
        public AddOzonProductApi(giftgiverContext giftgiver)
        {
            db = giftgiver;
        }
        [HttpPost]
        [Route("addOzonproduct")]
        public async Task<ProductResponce> AddOzonProduct(string link)
        {
            var result = await LoadProduct(link);
            int itemId = ExtractProductId(link);
            decimal cost;
            NumberStyles style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;
            CultureInfo culture = new CultureInfo("ru-RU");

            if (decimal.TryParse(result.Item2, style, culture, out cost))
            {
                //Подарки подарки = new Подарки();
                //подарки.ПодаркиId = itemId;
                //подарки.Наименование = result.Item1;
                //подарки.Цена = cost;
                //подарки.Ссылка = link;
                //подарки.Изображение = result.Item3;
                //db.Подаркиs.Add(подарки);
                //db.SaveChanges();
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
        public static int ExtractProductId(string url)
        {
            string[] parts = url.Split('/');
            foreach (string part in parts)
            {
                if (int.TryParse(part, out int productId))
                    return productId;
            }

            int productIndex = url.IndexOf("product/");
            if (productIndex != -1)
            {
                productIndex += 8;
                string productIdStr = "";
                while (productIndex < url.Length && char.IsDigit(url[productIndex]))
                {
                    productIdStr += url[productIndex];
                    productIndex++;
                }
                if (int.TryParse(productIdStr, out int id))
                    return id;
            }

            return 0;
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

                await chromium.WaitUntillOzon(async browser =>
                {
                    (bool Success, string Result) res1 = default;


                    foreach (var img in browser.GetElementByTagOzon("body", 0).GetChildByAttributeOzon("data-widget", "webGallery", 0).GetChildrenByTagOzon("img"))
                    {
                        var priority = await TryGetStringResult(chromium, img.GetAttributeOzon("fetchpriority"));
                        if (priority.Success && priority.Result == "high")
                        {
                            res1 = await TryGetStringResult(chromium, img.GetAttributeOzon("src"));
                            if (res1.Success && res1.Result != null)
                            {
                                break;
                            }
                        }
                    }

                    (bool Success, string Result) res2 = default;
                    foreach (var span in browser.GetElementByTagOzon("body", 0).GetChildByAttributeOzon("data-widget", "webSale", 0).GetChildrenByTagOzon("span"))
                    {
                        var innerText = await TryGetStringResult(chromium, span.GetInnerTextOzon());
                        if (innerText.Success && innerText.Result.Contains('₽'))
                        {
                            res2 = innerText;
                            break;
                        }
                    }

                    (bool Success, string Result) res3 = await TryGetStringResult(chromium, browser.GetElementByTagOzon("body", 0).GetChildByAttributeOzon("data-widget", "webProductHeading", 0).GetChildByTagOzon("h1", 0).GetInnerTextOzon());



                    productName = res3.Result?.Trim();
                    price = res2.Result?.Trim();
                    photoUrl = res1.Result?.Trim();
                    return res1.Success && res2.Success && res3.Success
                    && res1.Result != null && res2.Result != null && res3.Result != null;
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
