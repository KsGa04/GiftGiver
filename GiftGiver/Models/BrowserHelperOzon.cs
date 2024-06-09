using CefSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Browser
{
    public static class ExtensionsOzon
    {
        /// <summary>
        /// Ждет пока условие не станет истинным
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="func"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        public static async Task WaitUntillOzon(this IChromiumWebBrowserBase browser, Func<IChromiumWebBrowserBase, Task<bool>> func, int delay = 100)
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

        public static JSElementOzon GetElementByClassNameOzon(this IChromiumWebBrowserBase browser, string className, int index = 0)
        {
            return new JSElementOzon(browser, () => $"(function () {{ return document.getElementsByClassName('{className}')[{index}]; }})()");
        }

        public static JSElementOzon GetElementByTagOzon(this IChromiumWebBrowserBase browser, string tag, int index = 0)
        {
            return new JSElementOzon(browser, () => $"(function () {{ return document.getElementsByTagName('{tag}')[{index}]; }})()");
        }


    }

    public class JSElementOzon
    {
        public Func<string> Get { get; set; }
        public IChromiumWebBrowserBase ExecutingBrowser { get; set; }

        public JSElementOzon(IChromiumWebBrowserBase executingBrowser, Func<string> getFunc)
        {
            Get = getFunc;
            ExecutingBrowser = executingBrowser;
        }

        public Func<string> Click
        {
            get
            {
                return () => $"(function () {{ {Get.Invoke()}.click(); }})()";
            }
        }

        //<span class="visually-hidden">Хэштег</span>
        public string GetInnerTextOzon()
        {
            return $@"
                    (function ()
                    {{
                        var elem = {this.Get()};
                        return elem.innerText;
                    }})()";
        }

        public string GetAttributeOzon(string attributeName)
        {
            return $@"
                    (function ()
                    {{
                        var elem = {this.Get()};
                        return elem.getAttribute('{attributeName}');
                    }})()";
        }

        public JSElementOzon GetChildByAttributeOzon(string attributeName, string attributeValue, int index = 0)
        {
            //elementForSearchingIn.nodeType === Node.ELEMENT_NODE
            return new JSElementOzon(ExecutingBrowser, () => $@"
                (function ()
                {{
                    var index = 0;
                    function getChildrenRecursive(tmpElement)
                    {{
                        for (var i = 0, n = tmpElement.childNodes.length; i < n; i++)
                        {{
                            if (tmpElement.childNodes[i].nodeType === Node.ELEMENT_NODE && tmpElement.childNodes[i].getAttribute('{attributeName}') === '{attributeValue}')
                            {{
                                if(index === {index})
                                {{
                                    return tmpElement.childNodes[i];
                                }}
                                else
                                {{
                                    index++;
                                }}
                            }}
                            else
                            {{
                                var tmpResult = getChildrenRecursive(tmpElement.childNodes[i]);
                                if (tmpResult !== null)
                                {{
                                    return tmpResult;
                                }}
                            }}
                        }}
                        return null;
                    }}

                    return getChildrenRecursive({Get()});
                }})()");
        }

        public IEnumerable<JSElementOzon> GetChildrenByTagOzon(string tag)
        {
            for (int i = 0; ; i++)
            {
                var tmpI = i;
                var js = GetChildByTagOzon(tag, i);
                var task = ExecutingBrowser.EvaluateScriptAsync(GetChildByTagOzon(tag, tmpI).GetInnerTextOzon());
                task.Wait();

                if (task.Result.Success)
                {
                    yield return GetChildByTagOzon(tag, tmpI);
                }
                else
                {
                    yield break;
                }
            }
        }

        public JSElementOzon GetChildByTagOzon(string tag, int index = 0)
        {
            //elementForSearchingIn.nodeType === Node.ELEMENT_NODE
            return new JSElementOzon(ExecutingBrowser, () => $@"
                (function ()
                {{
                    var index = 0;
                    function getChildrenRecursive(tmpElement)
                    {{
                        for (var i = 0, n = tmpElement.childNodes.length; i < n; i++)
                        {{
                            if (tmpElement.childNodes[i].nodeType === Node.ELEMENT_NODE && tmpElement.childNodes[i].tagName === '{tag.ToUpper()}')
                            {{
                                if(index === {index})
                                {{
                                    return tmpElement.childNodes[i];
                                }}
                                else
                                {{
                                    index++;
                                }}
                            }}
                            else
                            {{
                                var tmpResult = getChildrenRecursive(tmpElement.childNodes[i]);
                                if (tmpResult !== null)
                                {{
                                    return tmpResult;
                                }}
                            }}
                        }}
                        return null;
                    }}

                    return getChildrenRecursive({Get()});
                }})()");
        }
    }
}
