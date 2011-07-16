using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Diagnostics;

namespace Rabbit
{
    public static class SiteEngine
    {
        private static List<KeyValuePair<string, Func<object, object>>> hooks = 
            new List<KeyValuePair<string, Func<object, object>>>();

        public static void Start()
        {
            Trace.WriteLine("Start SiteEngine ...");
            InitModules();
        }

        public static void AddHook(string name, Func<object, object> action)
        {
            if (string.IsNullOrWhiteSpace(name)) return;

            Trace.WriteLine(string.Format("SiteEngine: \tAddHook {0}", name));

            hooks.Add(new KeyValuePair<string, Func<object, object>>(name, action));
        }

        //[System.Diagnostics.DebuggerStepThrough]
        public static dynamic RunHook(string name, object data = null)
        {
            if (string.IsNullOrWhiteSpace(name)) return data;

            var foundhooks = hooks.Where(a => string.Compare(a.Key, name, true) == 0)
                   .Select(a => a.Value)
                   .ToList();

            Trace.WriteLine(string.Format("SiteEngine: RunHook {0} -> {1} Hook", name, foundhooks.Count()));

            foundhooks.ForEach(f => data = f(data));
            return data;
        }

        public static void ClearHook(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return;

            var foundhooks = hooks.Where(a => string.Compare(a.Key, name, true) == 0)
                    .ToList();

            Trace.WriteLine(string.Format("SiteEngine: ClearHook {0} -> {1} Hook", name, foundhooks.Count()));

            foundhooks.ForEach(h => hooks.Remove(h));
        }

        private static void InitModules()
        {
            hooks.Clear();

            var assemblies = Directory.EnumerateFiles(AppDomain.CurrentDomain.DynamicDirectory, "*.dll");

            foreach (var file in assemblies)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(file);
                    Trace.WriteLine(string.Format("SiteEngine: Start Loading Assembly {0}", file));

                    var types = assembly.GetTypes();

                    foreach(Type moduleType in types)
                    {
                        //Use Hook attribute
                        var methods = moduleType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                                      .Where(m => m.GetCustomAttributes(typeof(HookAttribute), true).Count() > 0).ToList();

                        foreach (var method in methods)
                        {
                            foreach (var attr in method.GetCustomAttributes(typeof(HookAttribute), true))
                            {
                                var hookName = ((HookAttribute)attr).Name ?? method.Name;
                                try
                                {
                                    var func = Delegate.CreateDelegate(typeof(Func<object, object>), method) as Func<object, object>;
                                    SiteEngine.AddHook(hookName, func);
                                }
                                catch (Exception ex)
                                {
                                    Trace.WriteLine(string.Format("SiteEngine: Failed Binding Hook: {0}: Error: {1}\r\n", hookName, ex.Message));
                                }
                            }
                        }
                        Trace.WriteLine(string.Format("SiteEngine: Done Loading Assembly {0}\r\n", file));
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(string.Format("SiteEngine: Failed Loading Assembly: {0}: Error: {1}\r\n", file, ex.Message));
                }
            }
        }

        private static Type GetType(IEnumerable<string> files, string module)
        {
            foreach (var file in files)
            {
                var assembly = Assembly.LoadFrom(file);
                var type = assembly.GetType(module, false);
                if (type != null) return type;
            }
            return null;
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
}
