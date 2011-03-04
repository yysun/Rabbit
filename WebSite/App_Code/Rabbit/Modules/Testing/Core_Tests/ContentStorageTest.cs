using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;

/// <summary>
/// Summary description for ContentStorageTest
/// </summary>
[TestClass]
public class ContentStorageTest
{
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
        
        ContentStore.SaveContent("", "D1", d1);
        dynamic d3 = ContentStore.LoadContent("", "D1");
        Assert.AreEqual(d1.Name, d3.Name);
        Assert.AreEqual(d1.Index, d3.Index);
        Assert.AreEqual(d1.Date.ToString("u"), d3.Date.ToString("u"));
        Assert.AreEqual(d1.Number, d3.Number);
        Assert.AreEqual(d1.Yes, d3.Yes);
        Assert.AreEqual(d1.No, d3.No);

        ContentStore.DeleteContent("", "D1");
    }

    [TestMethod]
    public void TestSaveDynamicWithChild()
    {
        dynamic d1 = new ExpandoObject();
        d1.Name = "d1";

        dynamic d2 = new ExpandoObject();
        d2.Name = "d2";
        d1.Child = d2;

        ContentStore.SaveContent("", "D1", d1);

        dynamic d3 = ContentStore.LoadContent("", "D1");
        Assert.AreEqual(d1.Name, d3.Name);
        Assert.AreEqual(d1.Child.Name, d3.Child.Name);
        ContentStore.DeleteContent("", "D1");
    }

}