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

    private static string FileName = "Site";

    public static SiteSettings Load()
    {
        var settings = new SiteSettings();
        settings.Value = new ContentStore().LoadContent("", FileName);
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
            new ContentStore().SaveContent("", FileName, Value);
        }
        return this;
    }


}