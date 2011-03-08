using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;
using System.Web.Script.Serialization;
using System.Collections;
using System.Text;

//[TestClass]
public class PageModelTest
{
    [TestMethod]
    public void GetPageTest()
    {
        dynamic data = new ExpandoObject();
        data.Id = "123"; 

        var mock = new Mock();
        mock.Setup("LoadContent", new object[]{"Pages", "123"}, null);
        
        var model = new PageModel();
        model.Store = mock;        
        model.Load(data);
        
        mock.Verify();
    }

    [TestMethod]
    public void GetSafeIdTest()
    {
        dynamic data = new ExpandoObject();
        data.Id = null;
        data.Title = @"~`!@#$%^&*()_+=-{}[]|\?><,./;:'""";

        var mock = new Mock();
        mock.Setup("SaveContent", new object[] { "Pages", "----------()_---{}[]------.---'-", data }, null);

        var model = new PageModel(data);
        model.Store = mock;
        model.Save();
        
        mock.Verify();

    }

}

