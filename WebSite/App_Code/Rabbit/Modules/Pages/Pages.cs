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
    [Hook]
    public static dynamic get_homepage(dynamic data)
    {
        return "~/Pages";
    }

    [Hook]
    public static dynamic get_menu(dynamic data)
    {
        var filename = HttpContext.Current.Server.MapPath("~/App_Data/Rabbit/Menu");
        return File.Exists(filename) ? File.ReadAllText(filename) : data;
    }

    [Hook]
    public static dynamic save_menu(dynamic data)
    {
        var filename = HttpContext.Current.Server.MapPath("~/App_Data/Rabbit/Menu");
        File.WriteAllText(filename, data.Menu);
        return data;
    }

    [Hook]
    public static dynamic get_module_admin_menu(dynamic data)
    {
        ((IList<string>)data).Add("Manage Pages|~/Pages/List");
        ((IList<string>)data).Add("Manage Menus|~/Pages/EditMenu");
        return data;
    }
}