using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Reflection;

//WebActivator dose not support web sites?
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

    private static string configFileName
    {
        get
        {
            return HttpContext.Current.Server.MapPath("~/App_Data/Rabbit.Modules.txt");
        }
    }

    private static IEnumerable<string> GetActivatedModules()
    {
        return File.Exists(configFileName) ? 
               File.ReadAllLines(configFileName)
                   .Where(s => !string.IsNullOrWhiteSpace(s) && !s.StartsWith("#"))
               : new string[]{};
    }

    private static IEnumerable<string> GetGetDeployedModules()
    {
        var deployPath = HttpContext.Current.Server.MapPath("~/App_Code/Rabbit/Modules");
        return Directory.Exists(deployPath) ?
               from d in Directory.EnumerateDirectories(deployPath)
               select Path.GetFileName(d)
               : new string[] { };
    }

    private static void InitModules()
    {
        hooks = new List<KeyValuePair<string, Func<object, object>>>();
        modules = GetActivatedModules().ToList();
        modules.ForEach(m =>
        {
            try
            {
                Type module = Type.GetType(m);
                module.InvokeMember("Init", BindingFlags.InvokeMethod, null, module, new object[0]);
            }
            catch { }
        });
    }
    
    public static string GetModules()
    {
        return MergeModules(GetActivatedModules());    
    }

    public static string SetModules(string list)
    {
        var modules = list.Replace("\r", "").Split('\n');
        File.WriteAllText(configFileName, MergeModules(modules));
        InitModules();
        return GetModules();
    }

    private static string MergeModules(IEnumerable<string> modules)
    {
        var list1 = from a in modules
                    join b in GetGetDeployedModules() on a equals b
                    select a;

        var list2 = from a in GetGetDeployedModules()
                    join b in modules on a equals b into cc
                    from c in cc.DefaultIfEmpty()
                    where c == null
                    select "# " + a;

        return string.Join("\r\n", list1.Union(list2));

    }
}

