using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Dynamic;

/// <summary>
/// Summary description for ContentStore
/// </summary>
public class Repository
{
    private static Dictionary<string, object> cache = new Dictionary<string, object>();
    private static object locker = new object();

    public string type { get; private set; }

    private static string BaseFolder
    {
        get { return HttpContext.Current.Server.MapPath("~/App_Data/Rabbit/"); }
    }

    public Repository(string type)
    {
        this.type = type;
    }
    
    public void Save(string id, dynamic data)
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

    public ExpandoObject Load(string id)
    {
        var cachekey = type + "." + id;
        
        if (cache.ContainsKey(cachekey))
        {
            return cache[cachekey] as dynamic;
        }

        lock (locker)
        {
            var fileName = Path.Combine(BaseFolder + type, id);
            if (!File.Exists(fileName)) return null;

            dynamic data = new ExpandoObject();
            var text = File.ReadAllText(fileName);
            data = text.ToDynamic();

            cache[cachekey] = data;
            return data;
        }
    }

    public bool Exists(string id)
    {
        return Load(id) != null;
    }

    public void Delete(string id)
    {
        var cachekey = type + "." + id;

        var fileName = Path.Combine(BaseFolder + type, id);
        if (File.Exists(fileName)) File.Delete(fileName);

        if (cache.ContainsKey(cachekey))
        {
            cache.Remove(cachekey);
        }
    }

    public IEnumerable<ExpandoObject> List()
    {
        var folder = Path.Combine(BaseFolder, type);
        if (!Directory.Exists(folder)) return null;
        return Directory.EnumerateFiles(folder)
                        .Select(f => Load(Path.GetFileName(f)));
    }
}

