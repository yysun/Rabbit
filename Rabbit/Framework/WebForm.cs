using System;
using System.Linq;
using System.Reflection;
using System.Web.WebPages;

namespace Rabbit
{
    public static class WebForm
    {
        public static void Run(this WebPage page)
        {
            try
            {
                page.InvokeMethod("Page_Load");

                var eventSource = page.Request["__event_source"];
                if (!string.IsNullOrWhiteSpace(eventSource))
                {
                    var eventTarget = page.Request["__event_target"];
                    if (string.IsNullOrWhiteSpace(eventTarget))
                    {
                        eventTarget = eventSource + "_click";
                    }

                    page.InvokeMethod(eventTarget);
                }

                page.InvokeMethod("Page_Unload");

                //TODO: Emit JavaScript with scroll position?
            }
            catch (Exception ex)
            {
                //TODO: yellow page of death
                throw;
            }
        }

        public static void Run_WebForm(this WebPage webpage)
        {
            Run(webpage);
        }
    }
}