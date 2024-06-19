using CefSharp.OffScreen;
using CefSharp;
using GiftGiver.Models;
using ImageMagick;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Common.Browser;

namespace GiftGiver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddAliExpressApi : ControllerBase
    {
        private giftgiverContext db = new giftgiverContext();
        public ProductResponce gift;
        public AddAliExpressApi(giftgiverContext giftgiver)
        {
            db = giftgiver;
        }
        [HttpPost]
        [Route("addAliproduct")]
        public async Task<ProductResponce> AddAliProduct(string link)
        {
            var result = await LoadProduct(link);
            //int itemId = ExtractProductId(link);
            decimal cost;
            NumberStyles style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;
            CultureInfo culture = new CultureInfo("ru-RU");

            if (decimal.TryParse(result.Item2, style, culture, out cost))
            {
                Подарки подарки = new Подарки();
                подарки.ПодаркиId = GenerateUniqueId();
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
        public static int GenerateUniqueId()
        {
            int uniqueId;
            bool isIdUnique;

            do
            {
                // Генерируем уникальный идентификатор
                uniqueId = GenerateRandomId();

                // Проверяем, есть ли такой идентификатор в базе данных
                isIdUnique = CheckIfIdIsUnique(uniqueId);
            } while (!isIdUnique);

            return uniqueId;
        }

        public static int GenerateRandomId()
        {
            // Генерация случайного идентификатора
            Random random = new Random();
            return random.Next(1, int.MaxValue);
        }

        public static bool CheckIfIdIsUnique(int id)
        {
            using (giftgiverContext db = new giftgiverContext())
            {
                bool isUnique = !db.Подаркиs.Any(p => p.ПодаркиId == id);
                return isUnique;
            }
            // Проверка наличия идентификатора в базе данных
            
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
                return (true, Convert.ToString(shopIdresponse.Result));
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

                await chromium.WaitUntillAli(async browser =>
                {
                    (bool Success, string Result) res1 = default;

                    foreach (var img in browser.GetElementByTagAli("body", 0).GetChildByAttributeAli("exp_type", "product_image", 0).GetChildrenByTagAli("img"))
                    {
                        res1 = await TryGetStringResult(chromium, img.GetAttributeAli("src"));
                        if (res1.Success && res1.Result != null)
                        {
                            break;
                        }
                    }

                    (bool Success, string Result) res2 = await TryGetStringResult(chromium, browser.GetElementByTagAli("body", 0).GetChildByHasAttributeAli("data-unformatted-price").GetAttributeAli("data-unformatted-price"));


                    (bool Success, string Result) res3 = await TryGetStringResult(chromium, browser.GetElementByTagAli("body", 0).GetChildByAttributeAli("data-product-description", "true", 0).GetChildByTagAli("div", 0).GetInnerTextAli());


                    productName = res3.Result?.Trim();
                    price = res2.Result?.Trim() + '₽';
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
