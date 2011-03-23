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

    [Hook]
    public static dynamic get_pages_page_detail_view(dynamic data)
    {
        return "~/PagesExt/_Page_Detail.cshtml";
    }

    [Hook]
    public static dynamic get_pages_page_edit_view(dynamic data)
    {
        return "~/PagesExt/_Page_Edit.cshtml";
    }
}