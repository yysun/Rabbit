using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Dynamic;

/// <summary>
/// Summary description for ContentStore
/// </summary>
public static class ContentStore
{
    private static Dictionary<string, object> cache = new Dictionary<string, object>();
    private static object locker = new object();

    private static string BaseFolder
    {
        get { return HttpContext.Current.Server.MapPath("~/App_Data/Rabbit/"); }
    }

    public static void SaveContent(string type, string id, dynamic data)
    {
        var cachekey = type + "." + id;
        var fileName = Path.Combine(BaseFolder + type, id);
        var text = ((ExpandoObject)data).ToJson();

        lock (locker)
        {
            cache[cachekey] = data;
            File.WriteAllText(fileName, text);
        }
    }

    public static dynamic LoadContent(string type, string id)
    {
        var cachekey = type + "." + id;
        
        if (cache.ContainsKey(cachekey))
        {
            return cache[cachekey] as dynamic;
        }

        lock (locker)
        {
            var fileName = Path.Combine(BaseFolder + type, id);
            dynamic data = new ExpandoObject();

            if (!File.Exists(fileName))
            {
                data.Id = id;    
            }
            else
            {
                var text = File.ReadAllText(fileName);
                data = text.ToDynamic();
            }
            cache[cachekey] = data;
            return data;
        }
    }
    
    public static void DeleteContent(string type, string id)
    {
        var fileName = Path.Combine(BaseFolder + type, id);
        if (File.Exists(fileName)) File.Delete(fileName);
    }

    public static IEnumerable<dynamic> LoadContent(string type)
    {
        var folder = Path.Combine(BaseFolder, type);
        if (!Directory.Exists(folder)) return null;
        return Directory.EnumerateFiles(folder)
                        .Select(f => LoadContent(type, Path.GetFileName(f)));
    }
}

