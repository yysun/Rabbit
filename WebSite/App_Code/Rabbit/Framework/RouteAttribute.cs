using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using System.Reflection;
using System.Dynamic;
using System.Collections.Specialized;

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
