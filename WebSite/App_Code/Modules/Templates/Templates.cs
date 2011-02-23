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
        Site.AddHook("get_layout", (sender, data) =>
        {
            return string.Format("~/Templates/{0}/_SiteLayout.cshtml", ((dynamic)data).Template ?? "Default");
        });
    }
}