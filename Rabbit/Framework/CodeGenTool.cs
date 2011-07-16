using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
namespace Rabbit
{
    public static class CodeGenTool
    {
        #region templates

        public const string WebFormTemplate = @"@using Rabbit
@{
    Layout = SiteEngine.RunHook(""get_layout""); // dynamic layout
}

@WebForm.Run(this)

<h1>Hello Web Form</h1>
<form method=""post"" action="""">

</form>
";

        public const string MvcControllerTemplate =@"@using Rabbit
@using System.Collections.Specialized
@{
    Rabbit.Mvc.Run(this);
}
@functions {
   
    {0}
}
";
        public const string MvcActionTemplate = @"
    [Get(""/{0}"")]
    void {0}(){
    }

    [Post(""/{0}"")]
    void {0}(NameValueCollection form){
    }
";

        public const string MvcViewTemplate = @"@{
    Page.Title = ""{0} {1} View"";
    Layout = ""~/_SiteLayout.cshtml"";
}
<h2>@Page.Model</h2>
";
        public const string TestTemplate = @"using Rabbit;

[TestClass]
public class {0}
{
    [TestMethod]
    public void Assert_Should_True()
    {
        Assert.IsTrue(false);
    }
}";

        #endregion

        public static Dictionary<string, string[]> Templates = new Dictionary<string, string[]>() {
            {"Web Form",  new string[] { WebFormTemplate, "~/WebForm.cshtml"} },
            //{"MVC",       new string[] { MvcControllerTemplate, "~/MvcDemo.cshtml"} },
            {"Unit Test", new string[] { TestTemplate, "~/App_Code/TestClass.cs"} }
        };

        public static string GetDefaultFileName(string code_type)
        {
            return Templates[code_type][1];
        }

        public static string GetTemplate(string code_type, string fileName, string actions)
        {          
            var code = Templates[code_type][0];

            if (code_type == "MVC")
            {
                var fs = "";
                var ss = actions.Split(',');
                foreach (var s in ss)
                {
                    var action = s.Trim();                   
                    var tmp = MvcActionTemplate.Replace("{0}", action);
                    if (action.ToLower() == "default") // default page, could be home, index etc.
                    {
                        tmp = tmp.Replace("/" + action, "/");
                    }
                    fs += tmp;
                }
                code = code.Replace("{0}", fs);
            }
            
            return code;
        }

        public static void SaveCode(string code_type, string folder, string fileName, string ncode, string actions)
        {
            switch (code_type)
            {
                case "Web Form":
                    fileName = Path.GetFileNameWithoutExtension(fileName);
                    fileName = Path.Combine(folder, fileName + ".cshtml");
                    File.WriteAllText(fileName, ncode);
                    break;

                case "MVC":
                    var controller = Path.GetFileNameWithoutExtension(fileName);
                    fileName = Path.Combine(folder, controller + ".cshtml");
                    File.WriteAllText(fileName, ncode);
                    folder = Path.Combine(folder, string.Format("Views/{0}", controller));
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                    foreach (var s in actions.Split(','))
                    {
                        var action = s.Trim();
                        fileName = Path.Combine(folder, action + ".cshtml");
                        File.WriteAllText(fileName, MvcViewTemplate.Replace("{0}", controller).Replace("{1}", action));    
                    }
                    break;

                case "Unit Test":
                    var test = Path.GetFileNameWithoutExtension(fileName);
                    ncode = ncode.Replace("{0}", test);
                    folder = Path.Combine(folder, "App_Code");
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                    fileName = Path.Combine(folder, test + ".cs");
                    File.WriteAllText(fileName, ncode);
                    break;
            }
        }
    }
}