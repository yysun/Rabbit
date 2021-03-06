﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using System.Reflection;
using System.Dynamic;
using System.Collections.Specialized;
using System.IO;
using System.Web.Routing;

public static class Mvc
{
    public static void Start()
    {
        RouteTable.Routes.Add(new Route("{*pathInfo}", new MvcRouteHandler()));
    }

    static Dictionary<string, IList<RouteAttribute>> cache = new Dictionary<string, IList<RouteAttribute>>();

    [System.Diagnostics.DebuggerStepThrough]
    public static void Run(dynamic page, object controller)
    {
        Assert.IsTrue(page != null);

        IList<string> urlData = page.UrlData;
        urlData = urlData.Where(s=>!string.IsNullOrWhiteSpace(s)).Skip(1).ToList();

        var action = urlData.Count() > 0 ? urlData[0].ToLower() : "";
        
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
                    HandleResult(page, route.Method.Invoke(controller, null));
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

                    HandleResult(page, route.Method.Invoke(controller, objects));
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

    private static void HandleResult(dynamic page, dynamic val)
    {
        //Assert.IsTrue(val != null);
        if (val == null) return;

        if (val is string)
        {
            page.Page.Text = val;
        }
        else if (val is ExpandoObject)
        {
            if (((ExpandoObject)val).HasProperty("Redirect"))
            {
                page.Page.Redirect = val.Redirect;
            }
            else
            {
                page.Page.Model = val.Model;
                page.Page.Hook = val.PageHook;
                page.Page.View = SiteEngine.RunHook(val.ViewHook, val.DefaultView) as string;
                if (val.Model is ExpandoObject && ((ExpandoObject)val.Model).HasProperty("Title"))
                {
                    page.Page.Title = val.Model.Title;
                }
            }
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

// rabbit routing 
public class MvcRouteHandler : IRouteHandler
{
    public IHttpHandler GetHttpHandler(RequestContext requestContext)
    {
        return new MvcHandler();
    }
}

public class MvcHandler : IHttpHandler
{
    public bool IsReusable
    {
        get { return false; }
    }

    public void ProcessRequest(HttpContext context)
    {
        context.Server.TransferRequest("~/Default.cshtml" + context.Request.RawUrl, true);
    }
}