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
        Site.AddHook("get_site_settings", (sender, data) =>
        {
            ((IDictionary<string, object>) data).EnsureProperty("Template", "Default");
            return data;
        });

        Site.AddHook("get_layout", (sender, data) =>
        {
            //TODO: check files exists
            return string.Format("~/Templates/{0}/_SiteLayout.cshtml", ((dynamic)data).Template ?? "Default");
        });

    }
}