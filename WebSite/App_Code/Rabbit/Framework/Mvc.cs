using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using System.Reflection;
using System.Dynamic;
using System.Collections.Specialized;

public static class Mvc
{
    static Dictionary<string, IList<RouteAttribute>> cache = new Dictionary<string, IList<RouteAttribute>>();

    [System.Diagnostics.DebuggerStepThrough]
    public static void Run(dynamic controller)
    {
        dynamic page = controller.WebPage;
        Assert.IsTrue(page != null);
        
        var cacheKey = controller.ToString();
        IList<RouteAttribute> routes = null;

        if (cache.ContainsKey(cacheKey))
        {
            routes = cache[cacheKey] as IList<RouteAttribute>;
        }
        else
        {
            routes = new List<RouteAttribute>();
            var methods = controller.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
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

        IList<string> urlData = page.UrlData;

        var action = urlData[0].ToLower();
        var rs = routes.Where(r => string.Compare(r.Action, action, true) == 0);

        if (rs.Count() == 0)
        {
            rs = urlData.Count == 0 ?
                routes.Where(r => r.Action == "/") :
                routes.Where(r => r.Action == "/*");
        }

        var route = rs.Where(r => r.IsPost == page.IsPost).FirstOrDefault();

        //if (route == null) route = routes.Where(r => r.Action == "*").FirstOrDefault();

        if (route != null && route.Method != null)
        {
            Log.Write("Controller: Run Action {0} -> {1}", action, route.Action);

            try
            {
                var parameters = route.Method.GetParameters();
                if(parameters==null || parameters.Length ==0)
                {
                    route.Method.Invoke(controller, null);
                }
                else
                {
                    var objects = new object[parameters.Length];
                    for(int i=0; i<parameters.Length; i++)
                    {
                        if(parameters[i].ParameterType == typeof(string[]))
                        {
                            objects[i] = urlData.ToArray();
                        }
                        else if(parameters[i].ParameterType == typeof(string))
                        {
                            //action name removed
                            objects[i] = string.Join("/", urlData.ToArray(), 1, urlData.Count() - 1);
                        }
                        else if (parameters[i].ParameterType == typeof(ExpandoObject))
                        {
                            objects[i] = ((NameValueCollection)page.Request.Form).ToDynamic();
                        }
                        else if (parameters[i].Name == "form" ||
                                 parameters[i].ParameterType == typeof(NameValueCollection))
                        {
                            objects[i] = page.Request.Form;
                        }
                        else if (parameters[i].Name=="request" ||
                                 parameters[i].ParameterType == typeof(HttpRequest))
                        {
                            objects[i] = page.Request;
                        }
                    }

                    //controller._viewName = route.Method.Name;
                    route.Method.Invoke(controller, objects);
                }
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }
        else
        {
            throw new Exception(string.Format("Controller: Cannot Find Action {0}", action));
        }
    }
}
