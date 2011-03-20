using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;
using System.Web.Script.Serialization;
using System.Collections;
using System.Text;

//[TestClass]
public class MvcTest
{
    [TestMethod]
    public void Routing_Default_Get()
    {
        dynamic webPage = new MockGet(new string[] { "/" });
        dynamic controller = new MockController(webPage);
        Mvc.Run(controller);
        Assert.IsTrue(controller.Default_Called);

        controller.Verify();
    }

    [TestMethod]
    public void Routing_Default_Post()
    {
        dynamic webPage = new MockPost(new string[] { "/" }, null);
        dynamic controller = new MockController(webPage);
        Mvc.Run(controller);
        Assert.IsTrue(controller.Default_Post_Called);

        controller.Verify();
    }

    [TestMethod]
    public void Routing_Get()
    {
        dynamic webPage = new MockGet(new string[] { "a", "1", "2" });
        dynamic controller = new MockController(webPage);
        Mvc.Run(controller);
        Assert.IsTrue(controller.Get_Called);
        controller.Verify();
    }

    [TestMethod]
    public void Routing_Post()
    {
        dynamic webPage = new MockPost(new string[] { "a", "b", "c" }, null);
        dynamic controller = new MockController(webPage);
        Mvc.Run(controller);
        Assert.IsTrue(controller.Post_Called);
        controller.Verify();
    }

    [TestMethod]
    public void Routing_All()
    {
        dynamic webPage = new MockGet(new string[] { "x", "y", "z" });
        dynamic controller = new MockController(webPage);
        Mvc.Run(controller);
        Assert.IsTrue(controller.All_Called);
        Assert.AreEqual("y", controller.UrlData[1]);
        Assert.AreEqual("z", controller.UrlData[2]);
        controller.Verify();
    }

    [TestMethod]
    public void Get_Edit_Pass_String()
    {
        dynamic webPage = new MockGet(new string[] { "Edit", "y", "z" });
        dynamic controller = new MockController(webPage);
        Mvc.Run(controller);
        Assert.AreEqual("y/z", controller.Id);
        controller.Verify();
    }

    [TestMethod]
    public void Post_Edit_Pass_Form()
    {
        dynamic webPage = new MockPost(new string[] { "Edit", "y", "z" }, null);
        webPage.Request.TestValue = -20.3m;

        dynamic controller = new MockController(webPage);
        Mvc.Run(controller);

        Assert.AreEqual("y/z", controller.Id);
        Assert.AreEqual(-20.3m, controller.TestValue);

        controller.Verify();
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))] //delete through GET should not be allowed
    public void Get_Delete()
    {
        dynamic webPage = new MockGet(new string[] { "Delete", "y", "z" });
        dynamic controller = new MockController(webPage);
        Mvc.Run(controller);
        controller.Verify();
    }

    [TestMethod]
    public void Post_Delete()
    {
        dynamic webPage = new MockPost(new string[] { "Delete", "y", "z" }, null);
        dynamic controller = new MockController(webPage);
        Mvc.Run(controller);
        Assert.AreEqual("y/z", controller.Id);
        controller.Verify();
    }

}

public class MockController : Mock
{
    public dynamic WebPage { get; protected set; }
    public MockController(dynamic webPage)
    {
        WebPage = webPage;
    }
    
    [Get("/")]
    public void Default()
    {
        SetupGet("Default_Called", true);
    }

    [Post("/")]
    public void Default_Post()
    {
        SetupGet("Default_Post_Called", true);
    }

    [Get("/a")]
    public void Get_Test()
    {
        SetupGet("Get_Called", true);
    }

    [Post("a")]
    public void Post_Test()
    {
        SetupGet("Post_Called", true);
    }

    [Get("/*")]
    public void All(string[] urlData)
    {
        SetupGet("All_Called", true);
        SetupGet("UrlData", urlData);
        SetupGet("UrlData", urlData);
    }

    [Get("Edit")] //same as [Get("/Edit")]
    public void Edit(string id)
    {
        SetupGet("Id", id);
    }

    [Post("/Edit")]
    public void Post(string id, dynamic request)
    {
        SetupGet("Id", id);
        SetupGet("TestValue", request.TestValue);
    }

    [Post("Delete")]
    public void Post(string id)
    {
        SetupGet("Id", id);
    }
}