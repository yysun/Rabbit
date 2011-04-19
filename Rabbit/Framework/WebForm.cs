using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using System.Reflection;

public static class WebForm
{
    //cache
   
    public static void Run(this WebPage page)
    {
        try
        {
            // run Page_Load
            // run events
            // run Page_Unload

            RunMethod(page, "Page_Load");

            var eventSource = page.Request["__event_source"];

            if (!string.IsNullOrWhiteSpace(eventSource))
            {
                var eventTarget = page.Request["__event_target"];

                if (string.IsNullOrWhiteSpace(eventTarget))
                {
                    eventTarget = eventSource + "_click";
                }

                RunMethod(page, eventTarget);
            }

            RunMethod(page, "Page_Unload");
        }
        catch (Exception ex)
        {
            // yellow page of death
            throw;
        }
    }

    private static void RunMethod(object page, string name)
    {
        //cache
        var methods = page.GetType().GetMethods(
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Instance);

        var method = methods.Where(m => string.Compare(m.Name, name, true) == 0).FirstOrDefault();
        if (method != null)
        {
            var parameters = method.GetParameters();
            if (parameters == null || parameters.Length == 0)
            {
                method.Invoke(page, null);
            }
            else
            {
                var objects = new object[parameters.Length];
                //method.Invoke(page, objects);
            }
        }

    }


	public static void Run_WebForm(this WebPage webpage)
	{
        Run(webpage);
	}
}