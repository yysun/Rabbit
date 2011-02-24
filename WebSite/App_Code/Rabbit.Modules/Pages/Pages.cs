using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Test
/// </summary>
public static class Pages
{
	public static void Init()
	{
        SiteEngine.AddHook("get_homepage", (data) => 
        {            
            return "~/Pages/Default";  
        });

        SiteEngine.AddHook("get_menu", (data) => 
        {
            return new string[] { "Home|~/Pages/Default", "About|~/Pages/About" };
        });

	}
}