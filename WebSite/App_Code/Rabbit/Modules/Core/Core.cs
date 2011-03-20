using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Dynamic;

/// <summary>
/// Summary description for Core
/// </summary>
public static class Core
{
    [Hook]
    public static dynamic Get_Site_Settings(dynamic data)
    {
        var settings = (ExpandoObject)data;
        settings.EnsureProperty("Name", "[Your Site Name]");
        settings.EnsureProperty("Author", "");
        settings.EnsureProperty("Template", "Default");
        return data;
    }

    [Hook]
    public static dynamic Get_Layout(dynamic data)
    {
        dynamic settings = SiteSettings.Load().Value; 
        return settings == null ? "" :
            string.Format("~/Templates/{0}/_SiteLayout.cshtml", settings.Template ?? "Default");
    }

    [Hook]
    public static dynamic get_templates(dynamic data)
    {
        var path = HttpContext.Current.Server.MapPath("~/Templates");

        return Directory.Exists(path) ?
               from d in Directory.EnumerateDirectories(path)
               select Path.GetFileName(d)
               : data;
    }
 
}