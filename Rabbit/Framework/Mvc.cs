using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.WebPages;

namespace Rabbit
{
    public static class Mvc
    {
        static Dictionary<string, IList<RouteAttribute>> cache = new Dictionary<string, IList<RouteAttribute>>();
        
        //[System.Diagnostics.DebuggerStepThrough]
        public static void Run(this WebPage page)
        {
            try
            {
                Assert.IsTrue(page != null);
                
                page.ParseForm();

                IList<string> urlData = page.UrlData;
                urlData = urlData.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

                var action = urlData.Count() > 0 ? urlData[0].ToLower() : "";

                var cacheKey = page.ToString();
                IList<RouteAttribute> routes = null;

                if (cache.ContainsKey(cacheKey))
                {
                    routes = cache[cacheKey] as IList<RouteAttribute>;
                }
                else
                {
                    routes = new List<RouteAttribute>();
                    var methods = page.GetType().GetMethods(
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (var method in methods)
                    {
                        foreach (var attr in method.GetCustomAttributes(typeof(RouteAttribute), true))
                        {
                            ((RouteAttribute)attr).Method = method;
                            routes.Add((RouteAttribute)attr);
                        }
                    }

                    //not to cache, if web.config says debug=true
                    //if (!HttpContext.Current.IsDebuggingEnabled) cache[cacheKey] = routes;
                    if (!page.Context.IsDebuggingEnabled) cache[cacheKey] = routes;
                }

                var rs = routes.Where(r => string.Compare(r.Action, action, true) == 0);

                if (rs.Count() == 0)
                {
                    rs = urlData.Count == 0 ?
                        routes.Where(r => r.Action == "/") :
                        routes.Where(r => r.Action == "/*");
                }

                var route = rs.Where(r => r.IsPost == page.IsPost).FirstOrDefault();

                if (route != null && route.Method != null)
                {
                    Trace.WriteLine(string.Format("Controller: Run Action {0} -> {1}", action, route.Action));

                    page.InvokeMethod(route.Method.Name);

                    dynamic actionResult = page.Page;
                    if (page.Request.AcceptTypes.Contains("application/json"))
                    {
                        var json = actionResult.Model == null ? "{\"d\":[]}" :
                            "{\"d\":[" + ((ExpandoObject)actionResult.Model).ToJson() + "]}";
                        page.Response.Write(json);
                    }
                    else if (actionResult.Redirect != null)
                    {
                        page.Response.Redirect(actionResult.Redirect, false);
                    }
                    else
                    {
                        var controller = Path.GetFileNameWithoutExtension(page.Request.Path.Split('/')[1]);
                        var view = actionResult.View ?? route.Method.Name;                          
                        view = string.Format("~/Views/{0}/{1}.cshtml", controller, view);
                        page.Write(page.RenderPage(view, actionResult.Model));
                    }
                }
                else
                {
                    throw new Exception(string.Format("Controller: Cannot Find Action {0}", action));
                }
            }
            catch (Exception ex)
            {
                //TODO: yellow page of death
                throw;
            }
        }

        public static void Run_Mvc(this WebPage webpage)
        {
            Run(webpage);
        }
    }

    #region Attributes

    [AttributeUsage(AttributeTargets.Method)]
    public class RouteAttribute : Attribute
    {
        internal bool IsPost { get; set; }
        internal string Action { get; set; }
        internal MethodInfo Method { get; set; }

        public RouteAttribute(bool isPost, string action)
        {
            //remove leading /
            if (action != "/" && action != "/*" && action[0] == '/')
                action = action.Substring(1, action.Length - 1);

            this.IsPost = isPost;
            this.Action = action;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class GetAttribute : RouteAttribute
    {
        public GetAttribute(string action)
            : base(false, action)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class PostAttribute : RouteAttribute
    {
        public PostAttribute(string action)
            : base(true, action)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HandleError : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class Authroize : Attribute
    {
    }

    #endregion
}