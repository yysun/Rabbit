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

    public static void Run(object controller)
    {
        var cacheKey = controller.ToString();
        IList<RouteAttribute> routes = null;

        if (cache.Keys.Contains(cacheKey))
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
            if (!HttpContext.Current.IsDebuggingEnabled) cache[cacheKey] = routes;
        }

        dynamic page = ((dynamic)controller).WebPage;
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
                route.Method.Invoke(controller, null);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }
        else
        {
            Log.Write("Controller: Cannot Run Action {0}", action);
        }
    }
}

[AttributeUsage(AttributeTargets.Method)]
public class RouteAttribute : Attribute
{
    internal bool IsPost { get; set; }
    internal string Action { get; set; }
    internal MethodInfo Method { get; set; }

    public RouteAttribute(bool isPost, string action)
    {
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
