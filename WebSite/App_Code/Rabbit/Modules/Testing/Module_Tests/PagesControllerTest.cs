using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;
using System.Web.Script.Serialization;
using System.Collections;
using System.Text;

//[TestClass]
public class PagesControllerTest
{
    [TestMethod]
    public void PagesController_Default_Get()
    {
        dynamic page = new MockGet(new string[] { "/" });
        Mvc.Run(new PagesController(page));

        Assert.AreEqual("~/Pages/_Page_View.cshtml", page.Page.View);
        Assert.AreEqual("Default", page.Page.Model.Id);

        page.Verify();
    }

    [TestMethod]
    public void PagesController_Default_Post()
    {
        dynamic page = new MockGet(new string[] { "/" });
        Mvc.Run(new PagesController(page));

        Assert.AreEqual("~/Pages/_Page_View.cshtml", page.Page.View);
        Assert.AreEqual("Default", page.Page.Model.Id);

        page.Verify();
    }

    [TestMethod]
    public void PagesController_Default_Edit()
    {
        dynamic page = new MockGet(new string[] { "Edit" });
        Mvc.Run(new PagesController(page));

        //Assert.AreEqual("~/Pages/_Page_Edit.cshtml", page.Page.View);
        Assert.AreEqual("", page.Page.Model.Id);

        page.Verify();
    }
}

