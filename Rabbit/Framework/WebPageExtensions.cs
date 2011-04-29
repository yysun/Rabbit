using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Specialized;
using System.Web.WebPages;
using System.Web;
using System.Dynamic;

namespace Rabbit
{
    public static class WebPageExtensions
    {
        static bool enableCache = false;
        static Dictionary<string, MethodInfo[]> cache = new Dictionary<string, MethodInfo[]>();

        public static object InvokeMethod(this WebPage page, string name)
        {
            MethodInfo[] methods = GetMethods(page);

            var method = methods.Where(m => string.Compare(m.Name, name, true) == 0).FirstOrDefault();
            if (method != null)
            {
                var parameters = method.GetParameters();
                if (parameters == null || parameters.Length == 0)
                {
                    return method.Invoke(page, null);
                }
                else
                {
                    var objects = CreateMethodParameters(page, parameters);
                    return method.Invoke(page, objects);
                }
            }

            return null;
        }

        private static MethodInfo[] GetMethods(object page)
        {
            MethodInfo[] methods;

            var cacheKey = page.GetType().FullName;
            if (enableCache && cache.ContainsKey(cacheKey))
            {
                methods = cache[cacheKey] as MethodInfo[];
            }
            else
            {
                methods = page.GetType().GetMethods(
                    BindingFlags.Public | BindingFlags.NonPublic |
                    BindingFlags.Instance);

                cache[cacheKey] = methods;
            }

            return methods;
        }

        private static object[] CreateMethodParameters(WebPage page, ParameterInfo[] parameters)
        {
            var eventArgument = page.Request["__event_argument"];
            var objects = new object[parameters.Length];

            if (!string.IsNullOrWhiteSpace(eventArgument))
            {
                var argumentData = eventArgument.Split(',');

                for (int i = 0; i < parameters.Length && i < argumentData.Length; i++)
                {
                    objects[i] = Convert.ChangeType(argumentData[i], parameters[i].ParameterType);
                }
            }
            else
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    var name = parameters[i].Name;
                    if (page.Request[name] != null)
                    {
                        objects[i] = Convert.ChangeType(page.Request[name], parameters[i].ParameterType);
                    }
                    else if (name == "urlData" || parameters[i].ParameterType == typeof(string[]))
                    {
                        objects[i] = page.UrlData.ToArray();
                    }
                    else if (name == "request" || parameters[i].ParameterType == typeof(HttpRequest))
                    {
                        objects[i] = page.Request;
                    }
                    else if (parameters[i].ParameterType == typeof(ExpandoObject))
                    {
                        objects[i] = page.Request.Form.ToDynamic();
                    }
                }
            }
            return objects;
        }
    }
}
