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
            //TODO: read from file/db
            //return "~/Templates/Default/_SiteLayout.cshtml";

            return "~/Templates/White/_SiteLayout.cshtml";
        });
	}
}