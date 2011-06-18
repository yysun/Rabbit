using System;
using System.Linq;
using System.Reflection;
using System.Web.WebPages;
using System.Text;
using System.Web.WebPages.Html;
using System.Web;

namespace Rabbit
{
    public static class WebForm
    {
        public static IHtmlString Run(WebPage page)
        {
            try
            {
                page.ParseForm();

                page.InvokeMethod("Page_Load");

                var eventSource = page.Request["__event_source"];
                var eventTarget = page.Request["__event_target"];

                if (string.IsNullOrWhiteSpace(eventTarget) && !string.IsNullOrWhiteSpace(eventSource))
                {
                    eventTarget = eventSource + "_click";
                }
   
                page.InvokeMethod(eventTarget);
                
                page.InvokeMethod("Page_Unload");

                return GenerateScript(page);

            }
            catch (Exception ex)
            {
                //TODO: yellow page of death
                throw;
            }
        }

        internal static IHtmlString GenerateScript(WebPage page)
        {
            var sb = new StringBuilder();
            var js = page.Href("~/WebForms.js");
            sb.AppendFormat("<script src=\"{0}\" type=\"text/javascript\"></script>\r\n", js);

            sb.Append("<script type=\"text/javascript\">\r\n $(function () {");
            sb.Append("webForm.init();");

            foreach (var key in page.Request.Form.AllKeys)
            {
                if (key.StartsWith("@"))
                {
                    sb.AppendFormat("webForm['{0}']={1};", key.Substring(1), page.Request.Form[key]);
                }
            }
            var x = page.Request["__scroll_x"];
            var y = page.Request["__scroll_y"];
            if (!string.IsNullOrWhiteSpace(x) && !string.IsNullOrWhiteSpace(y))
            {
                sb.AppendFormat("window.scrollTo({0},{1});", x, y);
            }
            sb.Append("}); \r\n</script>");

            return page.Html.Raw(sb.ToString());
        }
    }
}