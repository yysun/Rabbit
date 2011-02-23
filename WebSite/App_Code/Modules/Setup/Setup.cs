using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Test
/// </summary>
public static class Setup
{
	public static void Init()
	{
        Site.AddHook("get_homepage", (sender, data) => 
        {
            //var page = ((dynamic)sender);
            //page.Write(page.RenderPage("~/Admin/Default.cshtml"));
            
            //return "~/Admin/Default.cshtml"; 

            return data;

        });
	}
}