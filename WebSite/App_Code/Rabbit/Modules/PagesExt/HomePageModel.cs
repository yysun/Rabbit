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

    static HomePageModel()
    {
        Store = new ContentStore();
    }

    public static dynamic Store { get; set; }

    private static string FileName = "HomePage.Layout";

    public static HomePageModel Load()
    {
        var settings = new HomePageModel();
        settings.Value = Store.LoadContent("Pages", FileName);
        return settings;
    }
    
    public HomePageModel Save()
    {
        if (Value != null)
        {
            Store.SaveContent("Pages", FileName, Value);
        }
        return this;
    }
     
}    
      
      