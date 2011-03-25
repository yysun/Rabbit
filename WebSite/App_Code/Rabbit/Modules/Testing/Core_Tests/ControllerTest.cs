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
        var webPage = new MockGet(new string[] { "List" });

        var controller = new TestController();
        Mvc.Run(webPage, controller);

        Assert.AreEqual("~//__List.cshtml", webPage.Page.View);

        webPage.Verify();
    }
}

public class TestController : Controller
{
    
    [Get("List")]
    public virtual object List(string[] urlData)
    {
        dynamic list = new ExpandoObject();
        return RenderView((ExpandoObject) list); // it fails if not casting
    }
}

