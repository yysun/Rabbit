using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;

/// <summary>
/// Summary description for ContentStorageTest
/// </summary>
//[TestClass]
public class ContentStorageTest
{
    ContentStore Store = new ContentStore();

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

        Store.SaveContent("", "D1", d1);
        dynamic d3 = Store.LoadContent("", "D1");
        Assert.AreEqual(d1.Name, d3.Name);
        Assert.AreEqual(d1.Index, d3.Index);
        Assert.AreEqual(d1.Date.ToString("u"), d3.Date.ToString("u"));
        Assert.AreEqual(d1.Number, d3.Number);
        Assert.AreEqual(d1.Yes, d3.Yes);
        Assert.AreEqual(d1.No, d3.No);

        Store.DeleteContent("", "D1");
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

        Store.SaveContent("", "D1", d1);

        dynamic d3 = Store.LoadContent("", "D1");
        Assert.AreEqual(d1.Name, d3.Name);
        Assert.AreEqual(d1.Child.Name, d3.Child.Name);
        Assert.AreEqual(d1.Child.Child.Names, d3.Child.Child.Names);
        Store.DeleteContent("", "D1");
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

        Store.SaveContent("", "D1", d1);

        dynamic d3 = Store.LoadContent("", "D1");
        Assert.AreEqual(d1.Name, d3.Name);
        Assert.AreEqual(d1.Child.Name, d3.Child.Name);
        Assert.AreEqual(d1.Child.Child.Names[2], d3.Child.Child.Names[2]);
        Store.DeleteContent("", "D1");
    }
}