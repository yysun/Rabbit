using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;
using System.Web.Script.Serialization;
using System.Collections;
using System.Text;
using System.Collections.Specialized;

[TestClass]
public class PageControllerTest
{
    private void Assert_ShowDefaultPage(MockWebPage page)
    {
        var view = SiteEngine.RunHook("GET_Pages_Page_Detail_View", page.Page.View) as string;
        Assert.AreEqual(view, page.Page.View);
        Assert.AreEqual("Default", page.Page.Model.Id);
        page.Verify();
    }

    [TestMethod]
    public void Default_Get()
    {
        dynamic page = new MockGet(new string[] { "/" });
        Mvc.Run(page, new PageController());
        Assert_ShowDefaultPage(page);
    }

    [TestMethod]
    public void Default_Post()
    {
        dynamic page = new MockGet(new string[] { "/" });
        Mvc.Run(page, new PageController());
        Assert_ShowDefaultPage(page);
    }

    [TestMethod]
    public void Edit_No_Id_Shoud_Goto_Default()
    {
        dynamic page = new MockGet(new string[] { "Edit" });
        Mvc.Run(page, new PageController());
        Assert_ShowDefaultPage(page);
    }

    [TestMethod]
    public void Edit_Wrong_Id_Shoud_Goto_Default()
    {
        dynamic page = new MockGet(new string[] { "Edit", "id" });
        Mvc.Run(page, new PageController());
        Assert_ShowDefaultPage(page);
    }

    [TestMethod]
    public void List_Should_Use_Default_PageNo_PageSize()
    {
        dynamic model = new Mock();
        model.Setup("List", new object[] { 1, 20 }, model);
        model.SetupGet("Value", new ExpandoObject());
        
        dynamic page = new MockGet(new string[] { "List" });
        var controller = new PageController();
        controller.Model = model;

        Mvc.Run(page, controller);
        Assert.AreEqual("~/Pages/_Page_List.cshtml", page.Page.View);
        model.Verify();
    }

    [TestMethod]
    public void List_Should_Use_PageNo()
    {
        dynamic model = new Mock();
        model.Setup("List", new object[] { 2, 20 }, model);
        model.SetupGet("Value", new ExpandoObject());

        dynamic page = new MockGet(new string[] { "List", "2" });
        var controller = new PageController();
        controller.Model = model;

        Mvc.Run(page, controller);
        Assert.AreEqual("~/Pages/_Page_List.cshtml", page.Page.View);
        model.Verify();
    }

    [TestMethod]
    public void List_Should_Use_PageSize()
    {
        dynamic model = new Mock();
        model.Setup("List", new object[] { 5, 30 }, model);
        model.SetupGet("Value", new ExpandoObject());

        dynamic page = new MockGet(new string[] { "List", "5", "30" });
        var controller = new PageController();
        controller.Model = model;
        Mvc.Run(page, controller);
        Assert.AreEqual("~/Pages/_Page_List.cshtml", page.Page.View);
        model.Verify();
    }

    [TestMethod]
    public void Edit_Get()
    {
        dynamic data = new ExpandoObject();
        data.Id = "id";
        dynamic model = new Mock();
        model.Setup("Load", new object[] { It.Is<dynamic>(item => item.Id == "id") }, model);
        model.SetupGet("Value", data);

        dynamic page = new MockGet(new string[] { "Edit", "id" });
        var controller = new PageController();
        controller.Model = model;

        Mvc.Run(page, controller);
        var view = SiteEngine.RunHook("GET_Pages_Page_Edit_View", page.Page.View) as string;
        Assert.AreEqual(view, page.Page.View);
        Assert.AreEqual("id", page.Page.Model.Id);
        model.Verify();
    }

    [TestMethod]
    public void Edit_Post_Update()
    {
        dynamic data = new ExpandoObject();
        data.Id = "id";

        dynamic model = new Mock();
        model.Setup("Update", new object[] { It.Is<dynamic>(item=>item.Id=="id") }, model);
        model.SetupGet("Value", data);
        model.SetupGet("HasError", false); //this bypasses validation

        var form = new NameValueCollection();
        form["OldId"] = "id";

        dynamic page = new MockPost(new string[] { "Edit" }, form);
        var controller = new PageController();
        controller.Model = model;

        Mvc.Run(page, controller);

        Assert.AreEqual("~/Pages/id", page.Page.Redirect); // redirect 
        model.Verify();
    }

    [TestMethod]
    public void Edit_Post_SaveAs()
    {
        dynamic data = new ExpandoObject();
        data.Id = "new-id";

        dynamic model = new Mock();
        model.Setup("SaveAs", new object[] { It.Is<dynamic>(item => item.Id == "old-id"), "new-id" }, model);
        model.SetupGet("Value", data);
        model.SetupGet("HasError", false); //this bypasses validation
        model.Setup("Load", new object[] { It.Is<dynamic>(item => item.Id == "new-id") }, model);
        model.SetupGet("Value", data);

        var form = new NameValueCollection();
        form["OldId"] = "old-id";
        form["Id"] = "new-id";

        dynamic page = new MockPost(new string[] { "Edit", "id" }, form);
        var controller = new PageController();
        controller.Model = model;

        Mvc.Run(page, controller);

        Assert.AreEqual("~/Pages/new-id", page.Page.Redirect); // redirect 
        page.Verify();
    }

    [TestMethod]
    public void Edit_Post_Should_Handle_Error()
    {
        dynamic data = new ExpandoObject();
        data.Id = "id";

        dynamic model = new Mock();
        model.Setup("Update", new object[] { It.Is<dynamic>(item => item.Id == "id") }, model);
        model.SetupGet("Value", data);
        model.SetupGet("HasError", true); // this raises validation error
        model.SetupGet("Errors", "x");

        var form = new NameValueCollection();
        form["OldId"] = "id";

        dynamic page = new MockPost(new string[] { "Edit", "id" }, form);
        var controller = new PageController();
        controller.Model = model;

        Mvc.Run(page, controller);
        
        var view = SiteEngine.RunHook("GET_Pages_Page_Edit_View", page.Page.View) as string;
        Assert.AreEqual(view, page.Page.View); //stayed on edit page
        Assert.AreEqual("x", page.Page.Model.Errors); // pushed error message
        model.Verify();
    }


    [TestMethod]
    public void Create_Get()
    {
        dynamic page = new MockGet(new string[] { "Create", "id" });
        Mvc.Run(page, new PageController());

        Assert.AreEqual("~/Pages/_Page_Create.cshtml", page.Page.View);
        Assert.IsTrue(page.Page.Model.Id == null);
        Assert.AreEqual("[New Page]", page.Page.Model.Title);

        page.Verify();
    }

    [TestMethod]
    public void Create_Post()
    {
        dynamic data = new ExpandoObject();
        data.Id = "new-page";

        dynamic model = new Mock();
        model.Setup("Create", new object[] { It.Is<dynamic>(item => item.Title == "new page") }, model);
        model.SetupGet("Value", data);
        model.SetupGet("HasError", false);

        var form = new NameValueCollection();
        form["title"] = "new page";

        dynamic page = new MockPost(new string[] { "Create" }, form);

        var controller = new PageController();
        controller.Model = model;

        Mvc.Run(page, controller);

        Assert.AreEqual("~/Pages/new-page", page.Page.Redirect); // redirect 
        model.Verify();
    }

    [TestMethod]
    public void Create_Post_Should_Handle_Error()
    {
        dynamic data = new ExpandoObject();
        data.Id = "id";

        dynamic model = new Mock();
        model.Setup("Create", new object[] { It.Is<dynamic>(item => item.Title == "new page") }, model);
        model.SetupGet("Value", data);
        model.SetupGet("HasError", true); // this raises validation error
        model.SetupGet("Errors", "x");

        var form = new NameValueCollection();
        form["title"] = "new page";

        dynamic page = new MockPost(new string[] { "Create" }, form);
        var controller = new PageController();
        controller.Model = model;

        Mvc.Run(page, controller);

        Assert.AreEqual("~/Pages/_Page_Create.cshtml", page.Page.View); // stayed on create page
        Assert.AreEqual("x", page.Page.Model.Errors); // pushed error messages
        model.Verify();
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))] // Get is not allowed
    public void Delete_Get()
    {
        dynamic page = new MockGet(new string[] { "Delete", "id" });
        Mvc.Run(page, new PageController());
    }

    [TestMethod]
    public void Delete_Post()
    {
        dynamic model = new Mock();
        model.Setup("Delete", new object[] { It.Is<dynamic>(item => item.Id == "id") }, null);
        dynamic page = new MockPost(new string[] { "Delete", "id" }, null); //get id from url
        var controller = new PageController();
        controller.Model = model;
        Mvc.Run(page, controller);
        model.Verify();
        Assert.AreEqual("~/Pages/List", page.Page.Redirect); // redirect to List
    }
}

