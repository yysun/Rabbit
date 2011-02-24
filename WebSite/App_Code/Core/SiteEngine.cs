using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Reflection;

public static class SiteEngine
{
    private static List<KeyValuePair<string, Func<object, object>>> hooks;
    private static List<string> modules;

    static SiteEngine()
    {
        InitModules();
    }

    public static void AddHook(string name, Func<object, object> action)
    {
        hooks.Add(new KeyValuePair<string, Func<object, object>> (name, action));
    }

    public static object RunHook(string name, object data = null)
    {
        hooks.Where(a => a.Key == name)
               .Select(a => a.Value)
               .ToList()
               .ForEach(f => data = f(data));
        return data;
    }

    private static void InitModules()
    {
        hooks = new List<KeyValuePair<string, Func<object, object>>>();

        var filename = HttpContext.Current.Server.MapPath("~/App_Data/Modules.txt");
        modules = File.ReadAllLines(filename).Where(s=>!string.IsNullOrWhiteSpace(s) && !s.StartsWith("#")).ToList();

        modules.ForEach(m=>{
            Type module = Type.GetType(m);
            module.InvokeMember("Init", BindingFlags.InvokeMethod, null, module, new object[0]);
        });
    }
}

