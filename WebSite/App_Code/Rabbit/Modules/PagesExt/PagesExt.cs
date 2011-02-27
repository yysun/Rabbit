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
    public static void Init()
    {
        //SiteEngine.AddHook("get_rabbit_setup_view", (data) =>
        //{
        //    return "~/Sample/_get_site_settings.cshtml";
        //});

        SiteEngine.AddHook("get_module_admin_menu", (data) =>
        {
            ((IList<string>)data).Add("Edit Home Page Layout|~/PagesExt/HomePageLayout");
            ((IList<string>)data).Add("Edit Section Page Layout|~/PagesExt/EditSectionPageLayout");
            ((IList<string>)data).Add("Edit Article Page Layout|~/PagesExt/EditArticlePageLayout");
            ((IList<string>)data).Add("Edit Categories|~/PagesExt/EditCategories");
            ((IList<string>)data).Add("Edit MenuTree|~/PagesExt/EditMenuTree");
            return data;
        });

        SiteEngine.AddHook("get_menu", (data) =>
        {
            var filename = HttpContext.Current.Server.MapPath("~/App_Data/Rabbit/Menu.txt");
            return File.Exists(filename) ? File.ReadAllText(filename) : data; //TODO: cache it
        });

        SiteEngine.AddHook("get_pages_page_itemview", (data) =>
        {
            return "~/PagesExt/_HomePage_Layout.cshtml";
        });

        SiteEngine.AddHook("get_pages_page_editview", (data) =>
        {
            return "~/PagesExt/_Page_Edit.cshtml";
        });

        SiteEngine.AddHook("get_pages_page_createview", (data) =>
        {
            return "~/PagesExt/_Page_Create.cshtml";
        });
       
        SiteEngine.AddHook("get_homepage_zones", (data) =>
        {
            dynamic homePage = HomePageModel.Load().Value;

            data = "Zone1, Zone2, Zone3";
            return data;
        });

        SiteEngine.AddHook("get_pagepart", (data) =>
        {
            var zone = data.ZoneName as string;
            data.Parts.Add(new KeyValuePair<string, dynamic>("~/PagesExt/_Page_Edit.cshtml", data));
            return data;
        });
    }
}