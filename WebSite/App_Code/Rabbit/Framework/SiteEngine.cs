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

    static SiteEngine()
    {
        InitModules();
    }

    public static void AddHook(string name, Func<object, object> action)
    {
        Log.Write("SiteEngine: \tAddHook {0}", name);

        hooks.Add(new KeyValuePair<string, Func<object, object>>(name, action));
    }

    [System.Diagnostics.DebuggerStepThrough]
    public static dynamic RunHook(string name, object data = null)
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
        var modules = GetActivatedModules().ToList();
        modules.ForEach(module =>
        {
            try
            {
                Log.Write("SiteEngine: Start Loading Module {0}", module);

                Type moduleType = Type.GetType(module);

                //Init function is obsolete
                var initMmethod = moduleType.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                                 .Where(m => m.Name == "Init").FirstOrDefault();
                if (initMmethod != null)
                {
                    moduleType.InvokeMember("Init", BindingFlags.InvokeMethod, null, moduleType, new object[0]);
                }

                //Use Hook attribute
                var methods = moduleType.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                              .Where(m => m.GetCustomAttributes(typeof(HookAttribute), true).Count() > 0);

                foreach (var method in methods)
                {
                    foreach (var attr in method.GetCustomAttributes(typeof(HookAttribute), true))
                    {
                        var func = Delegate.CreateDelegate(typeof(Func<object, object>), method) as Func<object, object>;
                        SiteEngine.AddHook(((HookAttribute)attr).Name ?? method.Name, func);
                    }
                }

                Log.Write("SiteEngine: Done Loading Module {0}\r\n", module);
            }
            catch (Exception ex)
            {
                Log.Write("SiteEngine: Failed Loading Module: {0}: Error: {1}\r\n", module, ex.Message);
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

[AttributeUsage(AttributeTargets.Method)]
public class HookAttribute : Attribute
{
    public string Name { get; private set; }
    
    public HookAttribute()
        : this(null)
    {

    }
    
    public HookAttribute(string name)
        : base()
    {
        this.Name = name;
    }
}

