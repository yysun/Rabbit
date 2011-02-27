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
        get { return HttpContext.Current.Server.MapPath("~/App_Data/Rabbit/Site.txt"); }
    }

    public static SiteSettings Load()
    {
        var settings = new SiteSettings();
        if (File.Exists(FileName))
        {
            var lines = File.ReadAllLines(FileName).Where(s => !s.StartsWith("#"));
            settings.Value = lines.ToDynamic();
        }
        else
        {
            settings.Value = new ExpandoObject();
        }

        settings.Value = SiteEngine.RunHook("get_site_settings", settings.Value);
        return settings;
    }

    public static SiteSettings New()
    {
        dynamic value = new ExpandoObject();
        value = SiteEngine.RunHook("get_site_settings", value);
        return new SiteSettings { Value = value };
    }

    public static SiteSettings New(NameValueCollection form)
    {
        var value = form.ToDynamic();
        ((IDictionary<string, object>)value).Remove("modules"); //maybe remove keys start with lowercase letters

        Log.Enabled = form.AllKeys.Contains("enableLog");
        return new SiteSettings { Value = value };
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