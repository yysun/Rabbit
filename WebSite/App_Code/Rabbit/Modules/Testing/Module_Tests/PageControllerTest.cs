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
        Assert.AreEqual("~/Pages/_Page_View.cshtml", page.Page.View);
        Assert.AreEqual("Default", page.Page.Model.Id);
        page.Verify();
    }

    [TestMethod]
    public void PageController_Default_Get()
    {
        dynamic page = new MockGet(new string[] { "/" });
        Mvc.Run(new PageController(page));
        Assert_ShowDefaultPage(page);
    }

    [TestMethod]
    public void PageController_Default_Post()
    {
        dynamic page = new MockGet(new string[] { "/" });
        Mvc.Run(new PageController(page));
        Assert_ShowDefaultPage(page);
    }

    [TestMethod]
    public void PageController_Edit_No_Id_Shoud_Goto_Default()
    {
        dynamic page = new MockGet(new string[] { "Edit" });
        Mvc.Run(new PageController(page));
        Assert_ShowDefaultPage(page);
    }

    [TestMethod]
    public void PageController_Edit_Wrong_Id_Shoud_Goto_Default()
    {
        dynamic page = new MockGet(new string[] { "Edit", "id" });
        Mvc.Run(new PageController(page));
        Assert_ShowDefaultPage(page);
    }

    [TestMethod]
    public void PageController_List_Should_Use_Default_PageNo_PageSize()
    {
        dynamic model = new Mock();
        model.Setup("List", new object[] { 1, 20 }, model);
        model.SetupGet("Value", new ExpandoObject());
        
        dynamic page = new MockGet(new string[] { "List" });
        var controller = new PageController(page);
        controller.Model = model;

        Mvc.Run(controller);
        Assert.AreEqual("~/Pages/_Page_List.cshtml", page.Page.View);
        model.Verify();
    }

    [TestMethod]
    public void PageController_List_Should_Use_PageNo()
    {
        dynamic model = new Mock();
        model.Setup("List", new object[] { 2, 20 }, model);
        model.SetupGet("Value", new ExpandoObject());

        dynamic page = new MockGet(new string[] { "List", "2" });
        var controller = new PageController(page);
        controller.Model = model;

        Mvc.Run(controller);
        Assert.AreEqual("~/Pages/_Page_List.cshtml", page.Page.View);
        model.Verify();
    }

    [TestMethod]
    public void PageController_List_Should_Use_PageSize()
    {
        dynamic model = new Mock();
        model.Setup("List", new object[] { 5, 30 }, model);
        model.SetupGet("Value", new ExpandoObject());

        dynamic page = new MockGet(new string[] { "List", "5", "30" });
        var controller = new PageController(page);
        controller.Model = model;
        Mvc.Run(controller);
        Assert.AreEqual("~/Pages/_Page_List.cshtml", page.Page.View);
        model.Verify();
    }

    [TestMethod]
    public void PageController_Edit_Get()
    {
        dynamic data = new ExpandoObject();
        data.Id = "id";
        dynamic model = new Mock();
        model.Setup("Load", new object[] { "id" }, model);
        model.SetupGet("Value", data);

        dynamic page = new MockGet(new string[] { "Edit", "id" });
        var controller = new PageController(page);
        controller.Model = model;

        Mvc.Run(controller);
        Assert.AreEqual("~/Pages/_Page_Edit.cshtml", page.Page.View);
        Assert.AreEqual("id", page.Page.Model.Id);
        page.Verify();
    }

    [TestMethod]
    public void PageController_Edit_Post()
    {
        dynamic data = new ExpandoObject();
        data.Id = "id";

        dynamic model = new Mock();
        model.Setup("Update", new object[] { It.Is<dynamic>(item=>item.Id=="id") }, model);
        model.SetupGet("Value", data);
        model.SetupGet("HasError", false);
        model.Setup("Load", new object[] { It.Is<dynamic>(item => item.Id == "id") }, model);
        model.SetupGet("Value", data);

        dynamic page = new MockPost(new string[] { "Edit", "id" }, new NameValueCollection());
        var controller = new PageController(page);
        controller.Model = model;

        Mvc.Run(controller);

        Assert.AreEqual("~/Pages/_Page_View.cshtml", page.Page.View);
        page.Verify();
    }

    [TestMethod]
    public void PageController_Edit_Post_Error()
    {
        dynamic data = new ExpandoObject();
        data.Id = "id";

        dynamic model = new Mock();
        model.Setup("Update", new object[] { It.Is<dynamic>(item => item.Id == "id") }, model);
        model.SetupGet("Value", data);
        model.SetupGet("HasError", true);
        model.SetupGet("Errors", "x");

        dynamic page = new MockPost(new string[] { "Edit", "id" }, new NameValueCollection());
        var controller = new PageController(page);
        controller.Model = model;

        Mvc.Run(controller);

        Assert.AreEqual("~/Pages/_Page_Edit.cshtml", page.Page.View);
        Assert.AreEqual("x", page.Page.Model.Errors);
        page.Verify();
    }


    [TestMethod]
    public void PageController_Create_Get()
    {
        dynamic page = new MockGet(new string[] { "Create", "id" });
        Mvc.Run(new PageController(page));

        Assert.AreEqual("~/Pages/_Page_Create.cshtml", page.Page.View);
        Assert.IsTrue(page.Page.Model.Id == null);
        Assert.AreEqual("[New Page]", page.Page.Model.Title);

        page.Verify();
    }

    [TestMethod]
    public void PageController_Create_Post()
    {
        dynamic data = new ExpandoObject();
        data.Id = "id";

        dynamic model = new Mock();
        model.Setup("Create", new object[] { It.Is<dynamic>(item => item.Id == "id") }, model);
        model.SetupGet("Value", data);
        model.SetupGet("HasError", false);
        model.Setup("Load", new object[] { It.Is<dynamic>(item => item.Id == "id") }, model);
        model.SetupGet("Value", data);

        dynamic page = new MockPost(new string[] { "Create" }, new NameValueCollection());
        var controller = new PageController(page);
        controller.Model = model;

        Mvc.Run(controller);

        Assert.AreEqual("~/Pages/_Page_View.cshtml", page.Page.View);
        page.Verify();
    }

    [TestMethod]
    public void PageController_Create_Post_Error()
    {
        dynamic data = new ExpandoObject();
        data.Id = "id";

        dynamic model = new Mock();
        model.Setup("Create", new object[] { It.Is<dynamic>(item => item.Id == "id") }, model);
        model.SetupGet("Value", data);
        model.SetupGet("HasError", true);
        model.SetupGet("Errors", "x");

        dynamic page = new MockPost(new string[] { "Create" }, new NameValueCollection());
        var controller = new PageController(page);
        controller.Model = model;

        Mvc.Run(controller);

        Assert.AreEqual("~/Pages/_Page_Create.cshtml", page.Page.View);
        Assert.AreEqual("x", page.Page.Model.Errors);
        page.Verify();
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void PageController_Delete_Get()
    {
        dynamic page = new MockGet(new string[] { "Delete", "id" });
        Mvc.Run(new PageController(page));
    }

    [TestMethod]
    public void PageController_Delete_Post()
    {
        dynamic page = new MockPost(new string[] { "Delete" }, new NameValueCollection());
        Mvc.Run(new PageController(page));
        Assert_ShowDefaultPage(page);
    }

}

