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
        dynamic request = new MockGet(new string[] { "/" });
        dynamic controller = new MockController(request);
        Mvc.Run(controller);
        Assert.IsTrue(controller.Default_Called);

        controller.Verify();
    }

    [TestMethod]
    public void Routing_Default_Post()
    {
        dynamic request = new MockPost(new string[] { "/" }, null);
        dynamic controller = new MockController(request);
        Mvc.Run(controller);
        Assert.IsTrue(controller.Default_Post_Called);

        controller.Verify();
    }

    [TestMethod]
    public void Routing_Get()
    {
        dynamic request = new MockGet(new string[] { "a", "1", "2" });
        dynamic controller = new MockController(request);
        Mvc.Run(controller);
        Assert.IsTrue(controller.Get_Called);
        Assert.AreEqual("1", request.UrlData[1]);
        Assert.AreEqual("2", request.UrlData[2]);
        controller.Verify();
    }

    [TestMethod]
    public void Routing_Post()
    {
        dynamic request = new MockPost(new string[] { "a", "b", "c" }, null);
        dynamic controller = new MockController(request);
        Mvc.Run(controller);
        Assert.IsTrue(controller.Post_Called);
        Assert.AreEqual("b", request.UrlData[1]);
        Assert.AreEqual("c", request.UrlData[2]);
        controller.Verify();
    }

    [TestMethod]
    public void Routing_All()
    {
        dynamic request = new MockGet(new string[] { "x", "y", "z" });
        dynamic controller = new MockController(request);
        Mvc.Run(controller);
        Assert.IsTrue(controller.All_Called);
        Assert.AreEqual("y", request.UrlData[1]);
        Assert.AreEqual("z", request.UrlData[2]);
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

    [Get("/*")]
    public void All()
    {
        SetupGet("All_Called", true);
    }

    [Post("/")]
    public void Default_Post()
    {
        SetupGet("Default_Post_Called", true);
    }

    [Get("a")]
    public void Get()
    {
        SetupGet("Get_Called", true);
    }

    [Post("a")]
    public void Post()
    {
        SetupGet("Post_Called", true);
    }
}