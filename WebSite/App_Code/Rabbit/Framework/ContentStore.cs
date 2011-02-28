using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Script.Serialization;
using System.Dynamic;

/// <summary>
/// Summary description for ContentStore
/// </summary>
public static class ContentStore
{
    private static string BaseFolder
    {
        get { return HttpContext.Current.Server.MapPath("~/App_Data/Rabbit/"); }
    }

    public static void SaveContent(string type, string id, dynamic data)
    {
        var fileName = Path.Combine(BaseFolder + type, id);
        var json = new JavaScriptSerializer();
        File.WriteAllText(fileName, json.Serialize(data));
    }

    private static dynamic CreateDynamic(object input, dynamic parent=null)
    {
        if (input is IDictionary<string, object>[])
        {
            dynamic result = new ExpandoObject();
            foreach (var item in (IDictionary<string, object>[])input)
            {
                ((IDictionary<string, object>)result)[item["Key"].ToString()] = item["Value"];             
            }
            return result;
        }
        return input;
    }

    public static dynamic LoadContent(string type, string id)
    {
        var fileName = Path.Combine(BaseFolder + type, id);
        if (!File.Exists(fileName)) return null;

        var json = new JavaScriptSerializer();
        var text = File.ReadAllText(fileName);
        var content = json.Deserialize<Dictionary<string, object>[]>(text);
        return CreateDynamic(content);
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

