using CefSharp;

namespace Common.Browser
{
    public static class Extensions
    {
        public static async Task WaitUntillAli(this IChromiumWebBrowserBase browser, Func<IChromiumWebBrowserBase, Task<bool>> func, int delay = 100)
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

        public static JSElementAli GetElementByClassNameAli(this IChromiumWebBrowserBase browser, string className, int index = 0)
        {
            return new JSElementAli(browser, () => $"(function () {{ return document.getElementsByClassName('{className}')[{index}]; }})()");
        }

        public static JSElementAli GetElementByTagAli(this IChromiumWebBrowserBase browser, string tag, int index = 0)
        {
            return new JSElementAli(browser, () => $"(function () {{ return document.getElementsByTagName('{tag}')[{index}]; }})()");
        }


    }

    public class JSElementAli
    {
        public Func<string> Get { get; set; }
        public IChromiumWebBrowserBase ExecutingBrowser { get; set; }

        public JSElementAli(IChromiumWebBrowserBase executingBrowser, Func<string> getFunc)
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
        public string GetInnerTextAli()
        {
            return $@"
                    (function ()
                    {{
                        var elem = {this.Get()};
                        return elem.innerText;
                    }})()";
        }

        public string GetAttributeAli(string attributeName)
        {
            return $@"
                    (function ()
                    {{
                        var elem = {this.Get()};
                        return elem.getAttribute('{attributeName}');
                    }})()";
        }

        public string HasAttributeAli(string attributeName)
        {
            return $@"
                    (function ()
                    {{
                        var elem = {this.Get()};
                        return elem.hasAttribute('{attributeName}');
                    }})()";
        }

        public JSElementAli GetChildByAttributeAli(string attributeName, string attributeValue, int index = 0)
        {
            //elementForSearchingIn.nodeType === Node.ELEMENT_NODE
            return new JSElementAli(ExecutingBrowser, () => $@"
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

        public IEnumerable<JSElementAli> GetChildrenByTagAli(string tag)
        {
            for (int i = 0; ; i++)
            {
                var tmpI = i;
                var js = GetChildByTagAli(tag, i);
                var task = ExecutingBrowser.EvaluateScriptAsync(GetChildByTagAli(tag, tmpI).GetInnerTextAli());
                task.Wait();

                if (task.Result.Success)
                {
                    yield return GetChildByTagAli(tag, tmpI);
                }
                else
                {
                    yield break;
                }
            }
        }

        public JSElementAli GetChildByTagAli(string tag, int index = 0)
        {
            //elementForSearchingIn.nodeType === Node.ELEMENT_NODE
            return new JSElementAli(ExecutingBrowser, () => $@"
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


        public JSElementAli GetChildByHasAttributeAli(string attributeName, int index = 0)
        {
            //elementForSearchingIn.nodeType === Node.ELEMENT_NODE
            return new JSElementAli(ExecutingBrowser, () => $@"
                (function ()
                {{
                    var index = 0;
                    function getChildrenRecursive(tmpElement)
                    {{
                        for (var i = 0, n = tmpElement.childNodes.length; i < n; i++)
                        {{
                            if (tmpElement.childNodes[i].nodeType === Node.ELEMENT_NODE && tmpElement.childNodes[i].hasAttribute('{attributeName}'))
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
