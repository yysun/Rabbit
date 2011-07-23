using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.WebPages;
using Newtonsoft.Json;

namespace Rabbit
{
    public class MvcController : WebPage
    {
        public override void ExecutePageHierarchy()
        {
            Run();
            base.ExecutePageHierarchy();
        }

        public override void Execute()
        {
        }

        static Dictionary<string, IList<RouteAttribute>> cache = new Dictionary<string, IList<RouteAttribute>>();
        
        protected virtual void Run()
        {
            try
            {
                
                this.ParseForm();

                IList<string> urlData = this.UrlData;
                urlData = urlData.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

                var action = urlData.Count() > 0 ? urlData[0].ToLower() : "";

                var cacheKey = this.GetType().FullName;
                IList<RouteAttribute> routes = null;

                if (cache.ContainsKey(cacheKey))
                {
                    routes = cache[cacheKey] as IList<RouteAttribute>;
                }
                else
                {
                    routes = new List<RouteAttribute>();
                    var methods = this.GetType().GetMethods(
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
                    if (!this.Context.IsDebuggingEnabled) cache[cacheKey] = routes;
                }

                var rs = routes.Where(r => string.Compare(r.Action, action, true) == 0);

                if (rs.Count() == 0)
                {
                    rs = urlData.Count == 0 ?
                        routes.Where(r => r.Action == "/") :
                        routes.Where(r => r.Action == "/*");
                }

                var route = rs.Where(r => r.IsPost == this.IsPost).FirstOrDefault();

                if (route != null && route.Method != null)
                {
                    Trace.WriteLine(string.Format("Controller: Run Action {0} -> {1}", action, route.Action));

                    this.InvokeMethod(route.Method.Name);

                    lastAction = route.Method.Name;
                   
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

        protected string lastAction;

        protected virtual void View(object model, string viewName = null)
        {
            if (model == null)
            {
                Response.StatusCode = 404;
                return;
            }

            if (Request.AcceptTypes.Contains("application/json"))
            {
                dynamic json = model is ExpandoObject ? ((ExpandoObject)model).ToJson() :
                        JsonConvert.SerializeObject(model);
                Response.Write(json);
            }

            if (string.IsNullOrWhiteSpace(viewName))
            {
                viewName = lastAction ?? "Index";

                var controller = Path.GetFileNameWithoutExtension(this.Request.Path.Split('/')[1]);
                viewName = string.Format("~/_{0}_{1}.cshtml", controller, viewName);
            }

            Write(RenderPage(viewName, model));
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