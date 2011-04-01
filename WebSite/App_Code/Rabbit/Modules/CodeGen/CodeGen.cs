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
        ((IList<string>)data).Add("Create Module/Controller/Test|~/Rabbit-Admin/CodeGen");
        return data;
    }

    #region templates

    private const string ModuleTemplate =
@"using System;
using System.Collections.Generic;

public static partial class {0}
{
    //[Hook]
    //public static object get_module_admin_menu(object data)
    //{
    //    ((IList<string>)data).Add(""{0} Admin|~/{0}/Admin"");
    //    return data;
    //}
}";

    private const string ControllerTemplate =
@"using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;

//hooks to the module class
public static partial class {0}
{
    [Hook]
    public static dynamic get_{1}_controller(object data)
    {
        return new {1}Controller();
    }

    [Hook(""get_module_admin_menu"")]
    public static object get_{1}_admin_menu(object data)
    {
        ((IList<string>)data).Add(""{1} Admin|~/{1}/Admin"");
        return data;
    }
}

//controller
public class {1}Controller : Controller
{
    public {1}Controller()
    {
        this.Name = ""{1}"";
        this.ModuleName = ""{0}"";
        this.ContentType = ""{1}"";
        this.Model = new {1}Model();
    }

    [Get(""/"")]
    public virtual object Default()
    {
        return List();
    }

    [Post(""/"")]
    public virtual object Default(NameValueCollection form)
    {
        return Redirect("""");
    }

    [Get(""List"")]
    public virtual object List()
    {
        dynamic data = new ExpandoObject();
        return RenderView((ExpandoObject) data);
    }

    [Get(""Edit"")]
    public virtual object Edit()
    {
        dynamic data = new ExpandoObject();
        return RenderView((ExpandoObject) data);
    }

    [Post(""Edit"")]
    public virtual object Edit(NameValueCollection form)
    {
        return Redirect("""");
    }

    [Get(""Create"")]
    public virtual object Create()
    {
        dynamic data = new ExpandoObject();
        return RenderView((ExpandoObject) data);
    }

    [Post(""Create"")]
    public virtual object Create(NameValueCollection form)
    {
        return Redirect("""");
    }

    [Post(""Delete"")]
    public virtual object Delete(NameValueCollection form)
    {
        return Redirect("""");
    }

    [Get(""Admin"")]
    public virtual object Admin()
    {
        dynamic data = new ExpandoObject();
        return RenderView((ExpandoObject) data);
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

    private const string ControllerTestTemplate =
@"using System;
using System.Collections.Specialized;

[TestClass]
public class {0}ControllerTest
{
    [TestMethod]
    public void Default_Get()
    {
        dynamic page = new MockGet(new string[] { ""{0}"", ""/"" });
        Mvc.Run(page, new {0}Controller ());
        Assert.AreEqual(""{0}Controller"", page.Page.Model.Id);
    }

    [TestMethod]
    public void Default_Post()
    {
        dynamic page = new MockGet(new string[] { ""{0}"", ""/"" });
        Mvc.Run(page, new {0}Controller());
        Assert.AreEqual(""Default"", page.Page.Model.Id);
    }
    
    [TestMethod]
    public void Edit_Get()
    {
        dynamic page = new MockGet(new string[] { ""{0}"", ""Edit"", ""id"" });
        Mvc.Run(page, new {0}Controller());
        Assert.AreEqual(""id"", page.Page.Model.Id);
    }

    [TestMethod]
    public void Edit_Post()
    {
        var form = new NameValueCollection();
        dynamic page = new MockPost(new string[] { ""{0}"", ""Edit"" }, form);
        Mvc.Run(page, new {0}Controller());
        Assert.AreEqual(""~/{0}/"", page.Page.Redirect);
    }

    [TestMethod]
    public void Create_Get()
    {
        dynamic page = new MockGet(new string[] { ""{0}"", ""Create"", ""id"" });
        Mvc.Run(page, new {0}Controller());
        Assert.IsTrue(page.Page.Model.Id == null);
    }

    [TestMethod]
    public void Create_Post()
    {
        var form = new NameValueCollection();
        dynamic page = new MockPost(new string[] { ""{0}"", ""Create"" }, form);
        Mvc.Run(page, new {0}Controller());
        Assert.AreEqual(""~/{0}/"", page.Page.Redirect);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))] // Get is not allowed
    public void Delete_Get()
    {
        dynamic page = new MockGet(new string[] { ""{0}"", ""Delete"", ""id"" });
        Mvc.Run(page, new {0}Controller());
    }

    [TestMethod]
    public void Delete_Post()
    {
        //dynamic model = new Mock();
        //model.Setup(""Delete"", new object[] { It.Is<dynamic>(item => item.Id == ""id"") }, null);
        dynamic page = new MockPost(new string[] { ""{0}"", ""Delete"" }, null);
        var controller = new {0}Controller();
        //controller.Model = model;
        Mvc.Run(page, controller);
        //model.Verify();
        Assert.AreEqual(""~/{0}/"", page.Page.Redirect);
    }
}";

    #endregion

    public static string ModuleDirectory;
    public static string TestDirectory;
    public static string ViewDirectory;

    #region code generating

    private static void CreateDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    private static void CreateFile(string file, string content)
    {
        if (!File.Exists(file))
        {
            File.WriteAllText(file, content);
        }
    }

    private static void CreateModuleFile(string module, string file, string content)
    {
        var path = Path.Combine(ModuleDirectory, module);
        CreateDirectory(path);
        file = Path.Combine(path, file);
        CreateFile(file, content);
    }

    public static void CreateModule(string module, string contentType)
    {
        if (string.IsNullOrWhiteSpace(module)) return;

         CreateModuleFile(module, module + ".cs",
            ModuleTemplate.Replace("{0}", module).Replace("{1}", contentType));

        if (string.IsNullOrWhiteSpace(contentType)) return;

        CreateModuleFile(module, contentType + "Controller.cs",
            ControllerTemplate.Replace("{0}", module).Replace("{1}", contentType));

        CreateModuleFile(module, contentType + "Model.cs",
            ModelTemplate.Replace("{0}", module).Replace("{1}", contentType));

        CreateViews(module, contentType);

        CreateDirectory(TestDirectory);
        var file = Path.Combine(TestDirectory, contentType + "ControllerTest.cs");
        CreateFile(file, ControllerTestTemplate.Replace("{0}", contentType));

    } 

    public static void CreateTest(string testClass)
    {
        CreateDirectory(TestDirectory);
        var file = Path.Combine(TestDirectory, testClass + ".cs");
        CreateFile(file, TestTemplate.Replace("{0}", testClass));
    }

    public static void CreateViews(string module, string contentType)
    {
        var path = Path.Combine(ViewDirectory, module);
        CreateDirectory(path);
        CreateView(path, "List", contentType);
        CreateView(path, "Detail", contentType);
        CreateView(path, "Edit", contentType);
        CreateView(path, "Create", contentType);
        CreateView(path, "Admin", contentType);
    }

    private static void CreateView(string path, string action, string contentType)
    {
        var file = Path.Combine(path, string.Format("_{1}_{0}.cshtml", action, contentType));
        CreateFile(file, string.Format("<h2>{1} {0}</h2>", action, contentType));
    }

    #endregion
}