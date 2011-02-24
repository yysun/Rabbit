using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Summary description for Test
/// </summary>
public static class Setup
{
	public static void Init()
	{
        Site.AddHook("get_homepage", (sender, data) => 
        {
            return SiteSettings.Load()[0] == null ? "/Setup" : data;
        });
	}
}