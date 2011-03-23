using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Dynamic;
using System.Collections.Specialized;

/// <summary>
/// Summary description for Test
/// </summary>
public static class Pages
{
    private static string[] HomePages = new string[]{"~/pages", "~/pages/", "~/pages/default", "~/pages/edit/default"};
    
    [Hook]
    public static dynamic get_homepage(object data)
    {
        return "~/Pages";
    }
    
    [Hook]
    public static object get_menu(object data)
    {
        var filename = HttpContext.Current.Server.MapPath("~/App_Data/Rabbit/Menu");
        return File.Exists(filename) ? File.ReadAllText(filename) : data;
    }

    [Hook]
    public static object save_menu(dynamic data)
    {
        var filename = HttpContext.Current.Server.MapPath("~/App_Data/Rabbit/Menu");
        File.WriteAllText(filename, data);
        return data;
    }

    [Hook]
    public static object get_module_admin_menu(object data)
    {
        ((IList<string>)data).Add("Manage Pages|~/Pages/List");
        ((IList<string>)data).Add("Manage Menus|~/Pages/EditMenu");
        return data;
    }

    [Hook]
    public static object IsHomePage(object data)
    {
        string path = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.ToLower();
        return HomePages.Any(s => s == path);
    }

}