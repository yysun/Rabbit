using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Reflection;


//Don't know why WebActivator dose not work
//[assembly: WebActivator.PreApplicationStartMethod(typeof(SiteEngine), "Start")]

public static class SiteEngine
{

    public static void Start()
    {
        SiteEngine.RunHook("start");
    }

    private static List<KeyValuePair<string, Func<object, object>>> hooks;
    private static List<string> modules;

    static SiteEngine()
    {
        InitModules();
    }

    public static void AddHook(string name, Func<object, object> action)
    {
        hooks.Add(new KeyValuePair<string, Func<object, object>>(name, action));
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

        if (File.Exists(filename))
        {
            modules = File.ReadAllLines(filename).Where(s => !string.IsNullOrWhiteSpace(s) && !s.StartsWith("#")).ToList();

            modules.ForEach(m =>
            {
                Type module = Type.GetType(m);
                module.InvokeMember("Init", BindingFlags.InvokeMethod, null, module, new object[0]);
            });
        }
    }
    
    public static string GetModules()
    {
        return "";
    }

    public static string SetModules(string list)
    {
        //var filename = HttpContext.Current.Server.MapPath("~/App_Data/Modules.txt");
        //if (File.Exists(filename))
        //{
        //    txt = File.ReadAllText(filename);
        //}
        return "";
    }
}

