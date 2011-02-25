using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

/// <summary>
/// Summary description for Test
/// </summary>
public static class Pages
{
	public static void Init()
	{
        SiteEngine.AddHook("get_homepage", (data) => 
        {            
            return "~/Pages";  
        });

        SiteEngine.AddHook("get_menu", (data) => 
        {
            var filename = HttpContext.Current.Server.MapPath("~/Pages/Menu.txt");
            return File.Exists(filename) ? File.ReadAllLines(filename) : data;
        });

	}
}