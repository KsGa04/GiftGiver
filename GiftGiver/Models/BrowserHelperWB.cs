using CefSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Browser
{
    public static class ExtensionsWB
    {
        /// <summary>
        /// Ждет пока условие не станет истинным
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="func"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        public static async Task WaitUntillWB(this IChromiumWebBrowserBase browser, Func<IChromiumWebBrowserBase, Task<bool>> func, int delay = 100)
        {
            while (true)
            {
                if (await func.Invoke(browser))
                {
                    return;
                }
                else
                {
                    await Task.Delay(delay);
                }
            }
        }

        public static JSElementWB GetElementByClassNameWB(this IChromiumWebBrowserBase browser, string className, int index = 0)
        {
            return new JSElementWB(browser, () => $"(function () {{ return document.getElementsByClassName('{className}')[{index}]; }})()");
        }
    }

    public class JSElementWB
    {
        public Func<string> Get { get; set; }
        public IChromiumWebBrowserBase ExecutingBrowser { get; set; }

        public JSElementWB(IChromiumWebBrowserBase executingBrowser, Func<string> getFunc)
        {
            Get = getFunc;
            ExecutingBrowser = executingBrowser;
        }


        //<span class="visually-hidden">Хэштег</span>
        public string GetInnerTextWB()
        {
            return $@"
                    (function ()
                    {{
                        var elem = {this.Get()};
                        return elem.innerText;
                    }})()";
        }

        public string GetAttributeWB(string attributeName)
        {
            return $@"
                    (function ()
                    {{
                        var elem = {this.Get()};
                        return elem.getAttribute('{attributeName}');
                    }})()";
        }
    }
}
