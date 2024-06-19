using CefSharp;

namespace Common.Browser
{
    public static class ExtensionsWB
    {
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
