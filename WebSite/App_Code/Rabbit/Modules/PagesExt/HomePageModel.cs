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

    private static string FileName = "HomePage.Layout";

    public static HomePageModel Load()
    {
        var settings = new HomePageModel();
        settings.Value = ContentStore.LoadContent("Pages", FileName);
        return settings;
    }
    
    public HomePageModel Save()
    {
        if (Value != null)
        {
            ContentStore.SaveContent("Pages", FileName, Value);
        }
        return this;
    }
     
}    
      
      