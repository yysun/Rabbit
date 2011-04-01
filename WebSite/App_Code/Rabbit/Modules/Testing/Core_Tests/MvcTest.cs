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
        dynamic webPage = new MockGet(new string[] { "Mock", "/" });
        dynamic controller = new MockController();
        Mvc.Run(webPage, controller);
        Assert.IsTrue(controller.Default_Called);

        controller.Verify();
    }

    [TestMethod]
    public void Routing_Default_Post()
    {
        dynamic webPage = new MockPost(new string[] { "Mock", "/" }, null);
        dynamic controller = new MockController();
        Mvc.Run(webPage, controller);
        Assert.IsTrue(controller.Default_Post_Called);

        controller.Verify();
    }

    [TestMethod]
    public void Routing_Get()
    {
        dynamic webPage = new MockGet(new string[] { "Mock", "a", "1", "2" });
        dynamic controller = new MockController();
        Mvc.Run(webPage, controller);
        Assert.IsTrue(controller.Get_Called);
        controller.Verify();
    }

    [TestMethod]
    public void Routing_Post()
    {
        dynamic webPage = new MockPost(new string[] { "Mock", "a", "b", "c" }, null);
        dynamic controller = new MockController();
        Mvc.Run(webPage, controller);
        Assert.IsTrue(controller.Post_Called);
        controller.Verify();
    }

    [TestMethod]
    public void Routing_All()
    {
        dynamic webPage = new MockGet(new string[] { "Mock", "x", "y", "z" });
        dynamic controller = new MockController();
        Mvc.Run(webPage, controller);
        Assert.IsTrue(controller.All_Called);
        Assert.AreEqual("y", controller.UrlData[1]);
        Assert.AreEqual("z", controller.UrlData[2]);
        controller.Verify();
    }

    [TestMethod]
    public void Get_Edit_Pass_String()
    {
        dynamic webPage = new MockGet(new string[] { "Mock", "Edit", "y", "z" });
        dynamic controller = new MockController();
        Mvc.Run(webPage, controller);
        Assert.AreEqual("y/z", controller.Id);
        controller.Verify();
    }

    [TestMethod]
    public void Post_Edit_Pass_Form()
    {
        dynamic webPage = new MockPost(new string[] { "Mock", "Edit", "y", "z" }, null);
        webPage.Request.TestValue = -20.3m;

        dynamic controller = new MockController();
        Mvc.Run(webPage, controller);

        Assert.AreEqual("y/z", controller.Id);
        Assert.AreEqual(-20.3m, controller.TestValue);

        controller.Verify();
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))] //delete through GET should not be allowed
    public void Get_Delete()
    {
        dynamic webPage = new MockGet(new string[] { "Mock", "Delete", "y", "z" });
        dynamic controller = new MockController();
        Mvc.Run(webPage, controller);
        controller.Verify();
    }

    [TestMethod]
    public void Post_Delete()
    {
        dynamic webPage = new MockPost(new string[] { "Mock", "Delete", "y", "z" }, null);
        dynamic controller = new MockController();
        Mvc.Run(webPage, controller);
        Assert.AreEqual("y/z", controller.Id);
        controller.Verify();
    }

}

public class MockController : Mock
{
    [Get("/")]
    public object Default()
    {
        SetupGet("Default_Called", true);
        return null;
    }

    [Post("/")]
    public object Default_Post()
    {
        SetupGet("Default_Post_Called", true);
        return null;
    }

    [Get("/a")]
    public object Get_Test()
    {
        SetupGet("Get_Called", true);
        return null;
    }

    [Post("a")]
    public object Post_Test()
    {
        SetupGet("Post_Called", true);
        return null;
    }

    [Get("/*")]
    public object All(string[] urlData)
    {
        SetupGet("All_Called", true);
        SetupGet("UrlData", urlData);
        SetupGet("UrlData", urlData);
        return null;
    }

    [Get("Edit")] //same as [Get("/Edit")]
    public object Edit(string id)
    {
        SetupGet("Id", id);
        return null;
    }

    [Post("/Edit")]
    public object Post(string id, dynamic request)
    {
        SetupGet("Id", id);
        SetupGet("TestValue", request.TestValue);
        return null;
    }

    [Post("Delete")]
    public object Post(string id)
    {
        SetupGet("Id", id);
        return null;
    }
}