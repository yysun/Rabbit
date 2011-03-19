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

        var repository = new Mock();
        repository.Setup("Load", new object[]{"123"}, null);
        
        var model = new PageModel();
        model.Repository = repository;        
        model.Load(data);
        
        repository.Verify();
    }

    [TestMethod]
    public void SavePage_Should_Use_SafeId()
    {
        dynamic data = new ExpandoObject();
        data.Id = null;
        data.Title = @"~`!@#$%^&*()_+=-{}[]|\?><,./;:'""";

        var repository = new Mock();
        repository.Setup("Save", new object[] { "----------()_---{}[]------.---'-", data }, null);

        var model = new PageModel(data);
        model.Repository = repository;
        model.Save();
        
        repository.Verify();

    }

}

