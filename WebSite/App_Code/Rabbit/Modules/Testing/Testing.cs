﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Reflection;

/// <summary>
/// Summary description for Testing
/// </summary>
public static class Testing
{
    public static void Init()
    {
        SiteEngine.AddHook("get_tests", (data) => GetTests());
        SiteEngine.AddHook("run_test", (data) => RunTest(data));
    }

    private static object GetTests()
    {
        var types = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.GetCustomAttributes(typeof(TestClassAttribute), true).Length > 0
                    orderby t.FullName
                    select new KeyValuePair<string, string[]>(t.FullName,
                        (from m in t.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                         where m.GetCustomAttributes(typeof(TestMethodAttribute), true).Length > 0
                         orderby m.Name
                         select m.Name).ToArray()
                    );
        return types.ToArray();
    }

    private static object RunTest(dynamic data)
    {
        try
        {
            Type type = Assembly.GetExecutingAssembly().GetType(data.ClassName);
            var test = Activator.CreateInstance(type);
            type.InvokeMember(data.MethodName, BindingFlags.InvokeMethod, null, test, new object[0]);

            //Ensure no exception expected
            MethodInfo method = type.GetMethod(data.MethodName);
            var attribute = method.GetCustomAttributes(typeof(ExpectedException), true).FirstOrDefault();
            if (attribute != null)
            {
                data.HasPassed = "false";
                data.Message = "Exception "+ ((ExpectedException) attribute).Type.Name + " did not occur as expected";
            }
            else
            {
                data.HasPassed = "true";
            }
        }
        catch (Exception ex)
        {
            data.HasPassed = "false";
            data.Message = ex.InnerException.Message;

            Type type = Assembly.GetExecutingAssembly().GetType(data.ClassName);
            if (type != null)
            {
                //Ensure the exception is expected
                MethodInfo method = type.GetMethod(data.MethodName);
                var attribute = method.GetCustomAttributes(typeof(ExpectedException), true).FirstOrDefault();
                if (attribute != null)
                {
                    if (((ExpectedException) attribute).Type.Equals(ex.InnerException.GetType()))
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