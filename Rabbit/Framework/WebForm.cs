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
    public class WebForm : WebPage
    {
        public override void ExecutePageHierarchy()
        {
            Run();
            base.ExecutePageHierarchy();
            GenerateScript();
        }

        public override void Execute()
        {
        }

        protected virtual void Run()
        {
            try
            {
                this.ParseForm();

                this.InvokeMethod("Page_Load");

                var eventSource = Request["__event_source"];
                var eventTarget = Request["__event_target"];

                if (string.IsNullOrWhiteSpace(eventTarget) && !string.IsNullOrWhiteSpace(eventSource))
                {
                    eventTarget = eventSource + "_click";
                }
   
                this.InvokeMethod(eventTarget);

                this.InvokeMethod("Page_Unload");
            }
            catch (Exception ex)
            {
                //TODO: yellow page of death
                throw;
            }
        }

        protected virtual void GenerateScript()
        {
            var sb = new StringBuilder();
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Rabbit.WebForms.js"))
            {
                var webformjs = new StreamReader(stream).ReadToEnd();
                webformjs = webformjs.Replace("\r", "").Replace("\n", "");
                sb.Append("<script type=\"text/javascript\">\r\n" + webformjs);
            }
            sb.Append("\r\n$(function () { webForm.init();");

            var fields = this.GetType().GetFields(BindingFlags.Public| BindingFlags.Instance);

            foreach (var field in fields)
            {
                if(field.IsPublic)
                    sb.AppendFormat("webForm['{0}']={1};", field.Name, JsonConvert.SerializeObject(field.GetValue(this)));
            }

            var x = this.Request["__scroll_x"];
            var y = this.Request["__scroll_y"];
            if (!string.IsNullOrWhiteSpace(x) && !string.IsNullOrWhiteSpace(y))
            {
                sb.AppendFormat("window.scrollTo({0},{1});", x, y);
            }
            sb.Append("}); \r\n</script>");

            var script =  sb.ToString();
            this.Write(this.Html.Raw(script));
        }
    }
}