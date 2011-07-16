using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.WebPages;
using Newtonsoft.Json;

namespace Rabbit
{
    public static class WebForm
    {

        public static IHtmlString Run(WebPage page)
        {
            try
            {
                var fields = ParseForm(page);

                page.InvokeMethod("Page_Load");

                var eventSource = page.Request["__event_source"];
                var eventTarget = page.Request["__event_target"];

                if (string.IsNullOrWhiteSpace(eventTarget) && !string.IsNullOrWhiteSpace(eventSource))
                {
                    eventTarget = eventSource + "_click";
                }
   
                page.InvokeMethod(eventTarget);

                page.InvokeMethod("Page_Unload");

                var script = GenerateScript(page, fields);
                return page.Html.Raw(script);

            }
            catch (Exception ex)
            {
                //TODO: yellow page of death
                throw;
            }
        }

        internal static IEnumerable<FieldInfo> ParseForm(WebPage page)
        {
            var fields = page.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                var value = page.Request.Form[field.Name];
                if (value != null)
                {
                    field.SetValue(page, Convert.ChangeType(value, field.FieldType));
                }
                else
                {
                    value = page.Request.Form["@" + field.Name];
                    if (value != null) field.SetValue(page, JsonConvert.DeserializeObject(value, field.FieldType));
                }
            }

            return fields;
        }

        internal static string GenerateScript(WebPage page, IEnumerable<FieldInfo> fields)
        {
            var sb = new StringBuilder();
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Rabbit.WebForms.js"))
            {
                var webformjs = new StreamReader(stream).ReadToEnd();
                sb.Append("<script type=\"text/javascript\">\r\n" + webformjs);
            }
            sb.Append("\r\n$(function () { webForm.init();");           

            foreach (var field in fields)
            {
                if(field.IsPublic)
                    sb.AppendFormat("webForm['{0}']={1};", field.Name, JsonConvert.SerializeObject(field.GetValue(page)));
            }

            var x = page.Request["__scroll_x"];
            var y = page.Request["__scroll_y"];
            if (!string.IsNullOrWhiteSpace(x) && !string.IsNullOrWhiteSpace(y))
            {
                sb.AppendFormat("window.scrollTo({0},{1});", x, y);
            }
            sb.Append("}); \r\n</script>");

            return sb.ToString();
        }
    }
}