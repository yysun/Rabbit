using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;
using System.IO;
using System.Collections.Specialized;


/// <summary>
/// Summary description for PageExt
/// </summary>
public class HomePageModel : Model
{
    private HomePageModel()
    {
    }

    private static string FileName
    {
        get { return HttpContext.Current.Server.MapPath("~/App_Data/Rabbit/PagesExt/HomePage.txt"); }
    }

    public static HomePageModel Load()
    {
        var settings = new HomePageModel();
        if (File.Exists(FileName))
        {
            var lines = File.ReadAllLines(FileName).Where(s => !s.StartsWith("#"));
            settings.Value = lines.ToDynamic();
        }
        else
        {
            settings.Value = new ExpandoObject();
        }
        
        return settings;
    }
    
    public HomePageModel Save()
    {
        if (Value != null)
        {
            ((IDictionary<string, object>)Value).SaveToFile(FileName);
        }
        return this;
    }
     
}    
      
      