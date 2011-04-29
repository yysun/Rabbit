using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Reflection;

namespace Rabbit
{
    public static class Testing
    {
        private static Type GetType(string typeName)
        {
            var assemblies = Directory.EnumerateFiles(AppDomain.CurrentDomain.DynamicDirectory, "*.dll");
            foreach (var file in assemblies)
            {
                if (File.Exists(Path.Combine(AppDomain.CurrentDomain.DynamicDirectory, file + ".delete"))) continue;
                var assembly = Assembly.LoadFrom(file);
                var type = assembly.GetType(typeName, false);
                if (type != null) return type;
            }
            return null;
        }

        //[Hook]
        public static object Get_Tests(dynamic data)
        {
            var tests = new List<KeyValuePair<string, string[]>>();
            var assemblies = Directory.EnumerateFiles(AppDomain.CurrentDomain.DynamicDirectory, "*.dll");
            foreach (var file in assemblies)
            {
                if (File.Exists(Path.Combine(AppDomain.CurrentDomain.DynamicDirectory, file + ".delete"))) continue;
                var assembly = Assembly.LoadFrom(file);
                var types = from t in assembly.GetTypes()
                            where t.GetCustomAttributes(typeof(TestClassAttribute), true).Length > 0
                            select t;

                foreach (var t in types)
                {
                    tests.Add(new KeyValuePair<string, string[]>(
                        t.FullName,
                        (from m in t.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                         where m.GetCustomAttributes(typeof(TestMethodAttribute), true).Length > 0
                         orderby m.Name
                         select m.Name).ToArray()));
                }           
            }
            
            return tests.ToArray();
            
            //var types = from t in Assembly.GetExecutingAssembly().GetTypes()
            //            where t.GetCustomAttributes(typeof(TestClassAttribute), true).Length > 0
            //            orderby t.FullName
            //            select new KeyValuePair<string, string[]>(t.FullName,
            //                (from m in t.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            //                 where m.GetCustomAttributes(typeof(TestMethodAttribute), true).Length > 0
            //                 orderby m.Name
            //                 select m.Name).ToArray()
            //            );
            //return types.ToArray();
        }

        //[Hook]
        public static object Run_Test(dynamic data)
        {
            Type type = GetType(data.ClassName);
            if (type == null)
            {
                throw new ArgumentException("Cannot load test class");
            }
            
            try
            {
                
                var test = Activator.CreateInstance(type);
                type.InvokeMember(data.MethodName, BindingFlags.InvokeMethod, null, test, new object[0]);

                //Ensure no exception expected
                MethodInfo method = type.GetMethod(data.MethodName);
                var attribute = method.GetCustomAttributes(typeof(ExpectedException), true).FirstOrDefault();
                if (attribute != null)
                {
                    data.HasPassed = "false";
                    data.Message = "Exception " + ((ExpectedException)attribute).Type.Name + " did not occur as expected";
                }
                else
                {
                    data.HasPassed = "true";
                }
            }
            catch (Exception ex)
            {
                data.HasPassed = "false";
                data.Message = ex.InnerException;

                if (type != null)
                {
                    //Ensure the exception is expected
                    MethodInfo method = type.GetMethod(data.MethodName);
                    var attribute = method.GetCustomAttributes(typeof(ExpectedException), true).FirstOrDefault();
                    if (attribute != null)
                    {
                        if (((ExpectedException)attribute).Type.Equals(ex.InnerException.GetType()))
                        {
                            data.HasPassed = "true";
                            data.Message = "";
                        }
                    }
                }
            }
            return data;
        }
    }
}