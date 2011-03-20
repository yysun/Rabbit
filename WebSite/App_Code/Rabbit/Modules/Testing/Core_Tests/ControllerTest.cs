using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;

/// <summary>
/// Summary description for ControllerTest
/// </summary>
//[TestClass]
public class ControllerTest
{
    [TestMethod]
    public void RenderView_Should_Get_Calling_Function_Name_As_View()
    {
        dynamic webPage = new MockGet(new string[] { "List" });
        var controller = new TestController(webPage);
        Mvc.Run(controller);
        Assert.AreEqual("~//__List.cshtml", webPage.Page.View);
    }
}

public class TestController : Controller
{
    public TestController(object webPage) : base(webPage)
    {
        WebPage = webPage;
    }
    
    [Get("List")]
    public virtual void List(string[] urlData)
    {
        dynamic list = new ExpandoObject();
        RenderView((ExpandoObject) list); // it fails if not casting
    }
}

