using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

/// <summary>
/// Summary description for Core
/// </summary>
public static class Core
{

    //public static void Init()
    //{
    //    SiteEngine.AddHook("get_site_settings", (data) =>
    //    {
    //        var settings = (IDictionary<string, object>)data;
    //        settings.EnsureProperty("Name", "[Your Site Name]");
    //        settings.EnsureProperty("Author", "");
    //        settings.EnsureProperty("Template", "Default");
    //        return data;
    //    });

    //    SiteEngine.AddHook("get_layout", (data) =>
    //    {
    //        dynamic settings = SiteSettings.Load().Value;   //TODO: cache it
    //        return settings == null ? "" :
    //            string.Format("~/Templates/{0}/_SiteLayout.cshtml", settings.Template ?? "Default");
    //    });

    //    SiteEngine.AddHook("get_templates", (data) =>
    //    {
    //        var path = HttpContext.Current.Server.MapPath("~/Templates");
            
    //        return Directory.Exists(path) ?
    //               from d in Directory.EnumerateDirectories(path)
    //               select Path.GetFileName(d)
    //               : data;
            
    //    });
    //}


    [Hook]
    public static dynamic Get_Site_Settings(dynamic data)
    {
        var settings = (IDictionary<string, object>)data;
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