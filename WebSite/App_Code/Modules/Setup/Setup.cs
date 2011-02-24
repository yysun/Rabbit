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
        SiteEngine.AddHook("get_homepage", (data) => 
        {
            return SiteSettings.Load()[0] == null ? "/Setup" : data;
        });
	}
}