using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

/// <summary>
/// Summary description for Test
/// </summary>
public static class PagesExt
{
    [Hook]
    public static dynamic get_module_admin_menu(dynamic data)
    {
        var menus = (IList<string>)data;
        menus.Add("Edit Home Page Layout|~/Pages/EditHomePageLayout");
        menus.Add("Edit Section Page Layout|~/Pages/EditSectionPageLayout");
        menus.Add("Edit Article Page Layout|~/Pages/EditArticlePageLayout");
        menus.Add("Edit Categories|~/Pages/EditCategories");
        menus.Add("Edit MenuTree|~/Pages/EditMenuTree");
        return data;
    }

    //support multiple level dropdown menu
    [Hook]
    public static dynamic get_menu(dynamic data)
    {
        var filename = HttpContext.Current.Server.MapPath("~/App_Data/Rabbit/Menu.txt");
        return File.Exists(filename) ? File.ReadAllText(filename) : data; //TODO: cache it
    }

    [Hook]
    public static dynamic get_pages_page(dynamic data)
    {
        return PageExtModel.Load(data).Value;
    }

    [Hook]
    public static dynamic get_pages_controller(dynamic data)
    {
        var page = data.WebPage;
        return new PagesExtController(page);
    }

    [Hook]
    public static dynamic get_pages_page_itemview(dynamic data)
    {
        return "~/PagesExt/_Page_View.cshtml";
    }
}