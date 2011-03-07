using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;
using System.Web.Script.Serialization;
using System.Collections;
using System.Text;

[TestClass]
public class PageModelTest
{
    [TestMethod]
    public void GetPageTest()
    {
        var mock = new Mock();
        PageModel.Store = mock;

        dynamic data = new ExpandoObject();
        data.Id = "123";    
        
        mock.Setup("LoadContent", new object[]{"Pages", "123"}, null);

        SiteEngine.RunHook("GET_PAGES_PAGE", data);

        mock.Verify();
    }

    [TestMethod]
    public void GetSafeIdTest()
    {
        var mock = new Mock();
        PageModel.Store = mock;

        dynamic data = new ExpandoObject();
        data.Id = null;
        data.Title = @"~`!@#$%^&*()_+=-{}[]|\?><,./";

        mock.Setup("SaveContent", new object[] { "Pages", "~`!@#$%^&-()_+=-{}[]-----,.-", data }, null);

        SiteEngine.RunHook("SAVE_PAGES_PAGE", data);

        mock.Verify();

    }

}

