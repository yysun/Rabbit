using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Test
/// </summary>
public static class Templates
{
    public static void Init()
    {
        SiteEngine.AddHook("get_homepage", (data) =>
        {
            return SiteSettings.Load()[0] == null ? "/Setup" : data;
        });

        SiteEngine.AddHook("get_site_settings", (data) =>
        {
            ((IDictionary<string, object>) data).EnsureProperty("Template", "Default");
            return data;
        });

        SiteEngine.AddHook("get_layout", (data) =>
        {
            //TODO: check files exists
            return string.Format("~/Templates/{0}/_SiteLayout.cshtml", ((dynamic)data).Template ?? "Default");
        });

    }
}