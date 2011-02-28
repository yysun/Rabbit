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

    private static List<KeyValuePair<string, Func<dynamic, dynamic>>> hooks;
    private static List<string> modules;

    static SiteEngine()
    {
        InitModules();
    }

    public static void AddHook(string name, Func<dynamic, dynamic> action)
    {
        Log.Write("SiteEngine: \tAddHook {0}", name);

        hooks.Add(new KeyValuePair<string, Func<dynamic, dynamic>>(name, action));
    }

    public static object RunHook(string name, dynamic data = null)
    {        
        var foundhooks = hooks.Where(a => string.Compare(a.Key, name, true) == 0)
               .Select(a => a.Value)
               .ToList();

        Log.Write("SiteEngine: RunHook {0} -> {1} Hook", name, foundhooks.Count());

        foundhooks.ForEach(f => data = f(data));
        return data;
    }

    public static void ClearHook(string name)
    {
        var foundhooks = hooks.Where(a => string.Compare(a.Key, name, true) == 0)
                .ToList();

        Log.Write("SiteEngine: ClearHook {0} -> {1} Hook", name, foundhooks.Count());

        foundhooks.ForEach(h=>hooks.Remove(h));
    }

    private static string configFileName
    {
        get
        {
            return HttpContext.Current.Server.MapPath("~/App_Data/Rabbit/Modules");
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
                Log.Write("SiteEngine: Start Loading Module {0}", m);

                Type module = Type.GetType(m);
                module.InvokeMember("Init", BindingFlags.InvokeMethod, null, module, new object[0]);

                Log.Write("SiteEngine: Done Loading Module {0}\r\n", m);
            }
            catch (Exception ex)
            {
                Log.Write("SiteEngine: Failed Loading Module: {0}: Error: {1}\r\n", m, ex.Message);
            }
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

