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
        Site.AddHook("get_homepage", (sender, data) => 
        {            
            return "~/Pages/Default";  
        });

        Site.AddHook("get_menu", (sender, data) => 
        {
            return new string[] { "Home|~/Pages/Default", "About|~/Pages/About" };
        });

	}
}