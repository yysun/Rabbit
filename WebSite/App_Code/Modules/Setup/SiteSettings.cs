using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;
using System.IO;
using System.Collections.Specialized;

/// <summary>
/// Summary description for SiteSettings
/// </summary>
public class SiteSettings : Model
{
	private SiteSettings()
	{
	}

    private static string FileName
    {
        get { return HttpContext.Current.Server.MapPath("~/App_Data/Site.txt"); }
    }

    public static SiteSettings Load()
    {
        var settings = new SiteSettings();
        if (File.Exists(FileName))
        {
            var lines = File.ReadAllLines(FileName).Where(s => !s.StartsWith("#"));
            dynamic value = lines.ToDynamic();
            value = Site.RunHook("get_site_settings", null, value);
            settings.Value = value;
        }
        return settings;
    }

    public static SiteSettings New()
    {
        dynamic value = new ExpandoObject();
        value.Name = "[New Site]";
        value.Author = "";
        value = Site.RunHook("get_site_settings", null, value);
        return new SiteSettings { Value = value };
    }

    public static SiteSettings New(NameValueCollection form)
    {
        return new SiteSettings { Value = form.ToDynamic() };
    }

    public SiteSettings Save()
    {
        if (Value != null)
        {
            ((IDictionary<string, object>)Value).SaveToFile(FileName);
        }
        return this;
    }


}