using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

/// <summary>
/// Summary description for CodeGen
/// </summary>
public static class CodeGen
{
    [Hook]
    public static object get_module_admin_menu(object data)
    {
        ((IList<string>)data).Add("Create Code|~/Rabbit-Admin/CodeGen");
        return data;
    }

    #region templates

    private const string ModuleTemplate =
@"using System;
using System.Collections.Generic;
public static class {0}
{
    //[Hook]
    //public static dynamic get_{0}_controller(object data)
    //{
        //return new {0}Controller();
    //}

    [Hook]
    public static object get_module_admin_menu(object data)
    {
        ((IList<string>)data).Add(""{0} Admin|~/{0}/Admin"");
        return data;
    }
}";

    private const string ControllerTemplate =
@"using System;
public class {0}Controller : Controller
{
    //move this hook function to module class
    //[Hook]
    //public static dynamic get_{1}_controller(object data)
    //{
        //return new {1}Controller();
    //}

    public {0}Controller()
    {
        this.ModuleName = ""{0}"";
        this.ContentTypeName = ""{1}"";
        this.Model = new {1}Model();
    }

    [Get(""/"")]
    public virtual object Default()
    {
        return ""{0}"";
    }

    [Post(""/"")]
    public virtual object Default(object form)
    {
        return Redirect(""~/{0}"");
    }
}";

    private const string ModelTemplate =
@"using System;
using System.Dynamic;
public class {1}Model : Model
{
    public {1}Model()
    {
        Value = new ExpandoObject();
        Repository = new Repository(""{1}"");
    }
}";

    private const string TestTemplate =
@"[TestClass]
public class {0}
{
    [TestMethod]
    public void Assert_Should_True()
    {
        Assert.IsTrue(false);
    }
}";
    #endregion

    public static string ModuleDirectory;
    public static string TestDirectory;

    #region code generating

    private static void CreateModuleFile(string module, string file, string content)
    {
        var path = Path.Combine(ModuleDirectory, module);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        file = Path.Combine(path, file);
        if (!File.Exists(file))
        {

            File.WriteAllText(file, content);
        }
    }

    public static void CreateModule(string module, string contentType)
    {
        if (string.IsNullOrWhiteSpace(module)) return;

        CreateModuleFile(module, module + ".cs", ModuleTemplate.Replace("{0}", module));

        if (string.IsNullOrWhiteSpace(contentType)) return;

        CreateModuleFile(module, contentType + "Controller.cs",
            ControllerTemplate.Replace("{0}", module).Replace("{1}", contentType));

        CreateModuleFile(module, contentType + "Model.cs",
            ModelTemplate.Replace("{0}", module).Replace("{1}", contentType));
    } 
    #endregion


    public static void CreateTest(string testClass)
    {
        if (!Directory.Exists(TestDirectory))
        {
            Directory.CreateDirectory(TestDirectory);
        }
        var file = Path.Combine(TestDirectory, testClass + ".cs");
        if (!File.Exists(file))
        {
            var content = TestTemplate.Replace("{0}", testClass);
            File.WriteAllText(file, content);
        }
    }
}