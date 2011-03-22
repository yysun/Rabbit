using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;

/// <summary>
/// Summary description for RepositoryTest
/// </summary>
//[TestClass]
public class RepositoryTest
{
    Repository repository = new Repository("");

    [TestMethod]
    public void TestSaveDynamic()
    {
        dynamic d1 = new ExpandoObject();
        d1.Name = "d1";
        d1.Index = 10;
        d1.Date = DateTime.Now;
        d1.Number = -20.5M;
        d1.No = false;
        d1.Yes = true;

        repository.Save("D1", d1);
        dynamic d3 = repository.Load("D1");
        Assert.AreEqual(d1.Name, d3.Name);
        Assert.AreEqual(d1.Index, d3.Index);
        Assert.AreEqual(d1.Date.ToString("u"), d3.Date.ToString("u"));
        Assert.AreEqual(d1.Number, d3.Number);
        Assert.AreEqual(d1.Yes, d3.Yes);
        Assert.AreEqual(d1.No, d3.No);

        repository.Delete("D1");
    }

    [TestMethod]
    public void TestSaveDynamicWithChild()
    {
        dynamic d1 = new ExpandoObject();
        d1.Name = "d1";

        dynamic d2 = new ExpandoObject();
        d2.Name = "d2";
        d1.Child = d2;

        dynamic d4 = new ExpandoObject();
        d4.Names = "";
        d2.Child = d4;

        repository.Save("D1", d1);

        dynamic d3 = repository.Load("D1");
        Assert.AreEqual(d1.Name, d3.Name);
        Assert.AreEqual(d1.Child.Name, d3.Child.Name);
        Assert.AreEqual(d1.Child.Child.Names, d3.Child.Child.Names);
        repository.Delete("D1");
    }

    [TestMethod]
    public void TestSaveDynamicWithChildArray()
    {
        dynamic d1 = new ExpandoObject();
        d1.Name = "d1";

        dynamic d2 = new ExpandoObject();
        d2.Name = "d2";
        d1.Child = d2;

        dynamic d4 = new ExpandoObject();
        d4.Names = new string[] { "1", "2", "3" };
        d2.Child = d4;

        repository.Save("D1", d1);

        dynamic d3 = repository.Load("D1");
        Assert.AreEqual(d1.Name, d3.Name);
        Assert.AreEqual(d1.Child.Name, d3.Child.Name);
        Assert.AreEqual(d1.Child.Child.Names[2], d3.Child.Child.Names[2]);
        repository.Delete("D1");
    }

    [TestMethod]
    public void Save_Should_UseSafeId()
    {
        var id = "----------()_---{}[]------.---'-";
        dynamic d1 = new ExpandoObject();
        var newid = repository.Save(@"~`!@#$%^&*()_+=-{}[]|\?><,./;:'""", d1);
        Assert.AreEqual(id, newid);
        Assert.IsTrue(repository.Exists(id));
        repository.Delete(id);
        Assert.IsFalse(repository.Exists(id));
    }
}