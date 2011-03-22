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
    public void Update_Should_Validate_Title()
    {
        dynamic data = new ExpandoObject();
        data.Id = "id";
        data.Title = null; // <-- it should catch this
        var repository = new Mock();
        var model = new PageModel();
        model.Repository = repository;
        model.Update(data);
        Assert.IsTrue(model.HasError);
    }

    [TestMethod]
    public void SaveAs_Should_Validate_New_Id_Exists()
    {
        dynamic data = new ExpandoObject();
        data.Id = "old-id";
        data.Title = "new title";
        var repository = new Mock();
        repository.Setup("Exists", new object[] { "new-id" }, true); // <-- it should catch this
        var model = new PageModel();
        model.Repository = repository;
        model.SaveAs(data, "new-id");
        Assert.IsTrue(model.HasError);
        repository.Verify();
    }

    [TestMethod]
    public void SaveAs_Should_Delete_Old_Id()
    {
        dynamic data = new ExpandoObject();
        data.Id = "old-id";
        data.Title = "new title";
        var repository = new Mock();
        repository.Setup("Exists", new object[] { "new-id" }, false);
        repository.Setup("Delete", new object[] { "old-id" });
        repository.Setup("Save", new object[] { "new-id", It.IsAny() });
        var model = new PageModel();
        model.Repository = repository;
        model.SaveAs(data,"new-id");
        Assert.IsFalse(model.HasError); //no validation error
        repository.Verify(); // all repository functions called
    }

    [TestMethod]
    public void Create_Should_Validate_Title()
    {
        dynamic data = new ExpandoObject();
        data.Id = "id";
        data.Title = null; // <-- it should catch this
        var repository = new Mock();
        var model = new PageModel();
        model.Repository = repository;

        model.Create(data);
        Assert.IsTrue(model.HasError);
    }

    [TestMethod]
    public void Create_Should_Validate_New_Title_Exists()
    {
        dynamic data = new ExpandoObject();
        data.Id = null;
        data.Title = "new title";
        var repository = new Mock();
        repository.Setup("Exists", new object[] { "new title" }, true); // <-- it should catch this
        var model = new PageModel();
        model.Repository = repository;
        model.Create(data);
        Assert.IsTrue(model.HasError);
        repository.Verify();
    }

    [TestMethod]
    public void List_Should_Use_Paging()
    {
        var repository = new Mock();
        repository.Setup("List", null);

        var model = new PageModel();
        model.Repository = repository;
        model.List(2, 20);

        Assert.AreEqual(2, model.Value.PageNo);
        Assert.AreEqual(20,model.Value.PageSize);

        repository.Verify(); 
    }

    [TestMethod]
    public void List_Should_Use_Paging_Filter()
    {
        var list = new List<ExpandoObject>();
        for (int i = 0; i < 50; i++)
        {
            dynamic item = new ExpandoObject();
            item.Title = i.ToString();
            list.Add(item);
        }

        var repository = new Mock();
        repository.Setup("List", null, list);

        var model = new PageModel();
        model.Repository = repository;
        model.List(1, 20, (p)=>((dynamic)p).Title.StartsWith("2"));

        Assert.AreEqual(1, model.Value.PageNo);
        Assert.AreEqual(20, model.Value.PageSize);

        IEnumerable<ExpandoObject> ret = model.Value.List;
        Assert.AreEqual(11, ret.Count());

        repository.Verify();
    }

    [TestMethod]
    public void Delete_Should_Call_Delete_To_Repository()
    {
        dynamic data = new ExpandoObject();
        data.Id = "id";

        var repository = new Mock();
        repository.Setup("Delete", new object[] { It.Is<string>(item => item == "id") }, null);

        var model = new PageModel();
        model.Repository = repository;
        model.Delete(data);

        Assert.IsTrue(model.Value == null);
        repository.Verify(); 
    }
}

