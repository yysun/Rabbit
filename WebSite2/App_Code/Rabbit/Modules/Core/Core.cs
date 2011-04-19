using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Dynamic;
using Rabbit;

/// <summary>
/// Summary description for Core
/// </summary>
public static class Core
{
    [Hook]
    public static object Get_Site_Settings(object data)
    {
        var settings = (ExpandoObject)data ?? new ExpandoObject();
        settings.EnsureProperty("Name", "[Your Site Name]");
        settings.EnsureProperty("Author", "");
        settings.EnsureProperty("Template", "Default");
        return settings;
    }

    [Hook]
    public static object Get_Layout(object data)
    {
        dynamic settings = SiteSettings.Load().Value; 
        return settings == null ? "" :
            string.Format("~/Templates/{0}/_SiteLayout.cshtml", settings.Template ?? "Default");
    }

    [Hook]
    public static object get_templates(dynamic data)
    {
        var path = HttpContext.Current.Server.MapPath("~/Templates");

        return Directory.Exists(path) ?
               from d in Directory.EnumerateDirectories(path)
               select Path.GetFileName(d)
               : data;
    }
}