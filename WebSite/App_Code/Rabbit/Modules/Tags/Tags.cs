using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

/// <summary>
/// Summary description for Tags
/// </summary>
public static class Tags
{
    [Hook]
    public static dynamic get_tags_controller(dynamic data)
    {
        return new TagController();
    }

    [Hook]
    public static dynamic get_pages_page_detail_view(dynamic data)
    {
        return "~/Views/Tags/_Page_Detail.cshtml";
    }

    [Hook]
    public static dynamic get_pages_page_edit_view(dynamic data)
    {
        return "~/Views/Tags/_Page_Edit.cshtml";
    }

    [Hook]
    public static dynamic update_pages_page(dynamic data)
    {
        data.Tags = (HttpContext.Current.Request.Form["Tags"] ?? "").Split(',')
                    .Where(s=>!string.IsNullOrWhiteSpace(s))
                    .Select(s => s.Trim()).ToArray();
        return data;
    }

    [Hook]
    public static dynamic pages_page_detail_aside(dynamic data)
    {
        data.Add("~/Views/Tags/_Page_List_Related.cshtml");
        data.Add("~/Views/Tags/_Tag_Cloud.cshtml");

        return data;
    }

    [Hook]
    public static dynamic pages_page_detail_home_aside(dynamic data)
    {
        data.Add("~/Views/Tags/_Page_List_Home_Aside.cshtml");
        return data;
    }

    [Hook]
    public static dynamic get_pages_by_tag(dynamic data)
    {
        var tag = data as string;
        dynamic list = new List<dynamic>();

        if (tag != null)
        {
            var model = new PageModel();
            list = model.List(1, 20, (p) =>
            {
                if (!p.HasProperty("Tags")) return false;
                object[] tags = ((dynamic)p).Tags;
                return tags == null ? false : tags.Any(t => t.Equals(tag));
            }).Value.List;
        }

        return list;
    }
}