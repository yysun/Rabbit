using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;
using System.Web.Script.Serialization;
using System.Collections;
using System.Text;

//[TestClass]
public class PagesExtControllerTest
{
    [TestMethod]
    public void PagesExtController_Default_Test()
    {
        dynamic page = new MockWebPage(new string[] { "/" });

        Mvc.Run(page, new PagesExtController());
        //Assert.AreEqual("", page.Page.View);
        
        //Assert.IsTrue(page.Page.View != null, "View has not been set.");
        //Assert.IsTrue(page.Page.Model != null, "Model has not been set.");
    }

    [TestMethod]
    public void PagesExtController_EditMenuTree_Test()
    {
        dynamic page = new MockWebPage(new string[] { "EditMenuTree" });

        Mvc.Run(page, new PagesExtController());
        //Assert.AreEqual("", page.Page.View);

        //Assert.IsTrue(page.Page.View != null, "View has not been set.");
        //Assert.IsTrue(page.Page.Model != null, "Model has not been set.");
    }

}

