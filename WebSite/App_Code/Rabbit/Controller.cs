using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using System.Reflection;
using System.Dynamic;

public abstract class Controller : WebPage
{
    static Dictionary<string, IList<RouteAttribute>> cache = new Dictionary<string, IList<RouteAttribute>>();

    public void Run()
    {
        var cacheKey = this.ToString();
        IList<RouteAttribute> routes = null;

        if (cache.Keys.Contains(cacheKey))
        {
            routes = cache[cacheKey] as IList<RouteAttribute>;
        }
        else
        {
            routes = new List<RouteAttribute>();
            var methods = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var method in methods)
            {
                foreach (var attr in method.GetCustomAttributes(typeof(RouteAttribute), true))
                {
                    ((RouteAttribute)attr).Method = method;
                    routes.Add((RouteAttribute)attr);
                }
            }
#if (!DEBUG)
            //cache[cacheKey] = routes;
#endif
        }

        Run(routes);
    }

    private void Run(IList<RouteAttribute> routes)
    {
        var action = UrlData[0].ToLower();
        var rs = routes.Where(r => string.Compare(r.Action, action, true) == 0);

        if (rs.Count() == 0)
        {
            rs = UrlData.Count == 0 ?
                routes.Where(r => r.Action == "/") :
                routes.Where(r => r.Action == "/*");
        }
        
        var route = rs.Where(r => r.IsPost == this.IsPost).FirstOrDefault();

        //if (route == null) route = routes.Where(r => r.Action == "*").FirstOrDefault();

        if (route != null && route.Method != null)
        {
            route.Method.Invoke(this, null);
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
