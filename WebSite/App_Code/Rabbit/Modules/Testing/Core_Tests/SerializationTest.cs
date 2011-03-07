using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;
using System.Web.Script.Serialization;
using System.Collections;
using System.Text;

/// <summary>
/// Summary description for ContentStorageTest
/// </summary>
//[TestClass]
public class SerializationTest
{
    //[TestMethod]
    //public void TestStringArray()
    //{
    //    //var s1 = new string[] { "1", "2", "3", "4", "5" };

    //    ContentStore.SaveContent("", "D2", s1);
    //    var s2 = ContentStore.LoadContent("", "D2") as string[];

    //    for (int i = 0; i < s1.Length; i++)
    //    {
    //        Assert.AreEqual(s1[i], s2[1]);
    //    }
    //    ContentStore.DeleteContent("", "D2");
    //}

    //[TestMethod]
    //public void TestAnonymousObject()
    //{
    //    var s1 = new { name = "a", id = 5, b = true, d = DateTime.Now, m = 0.25M };
    //    dynamic s2 = s1.ToDynamic();

    //    Assert.AreEqual(s1.name, s2.name);
    //    Assert.AreEqual(s1.id, s2.id);
    //    Assert.AreEqual(s1.b, s2.b);
    //    Assert.AreEqual(s1.d, s2.d);
    //    Assert.AreEqual(s1.m, s2.m);
    //}

    [TestMethod]
    public void TestStringToExpando()
    {
        var json = "{\"id\":1,\"name\":\"test\"}";
        dynamic expando = json.ToDynamic();
        Assert.AreEqual(expando.id, 1);
        Assert.AreEqual(expando.name, "test");
    }

    [TestMethod]
    public void TestExpandoToString()
    {
        var json = "{\"id\":1,\"name\":\"test\"}";
        dynamic expando = new ExpandoObject();
        expando.id = 1;
        expando.name = "test";
        Assert.AreEqual(json, ((ExpandoObject)expando).ToJson());
    }

    [TestMethod]
    public void TestExpandoWithChild()
    {
        dynamic d1 = new ExpandoObject();
        d1.Name = "d1";

        dynamic d2 = new ExpandoObject();
        d2.Name = "d2";
        d1.Child = d2;

        dynamic d4 = new ExpandoObject();
        d4.Names = new string[] { "1", "2", "3" };
        d2.Child = d4;

        var text = ((ExpandoObject)d1).ToJson();

        dynamic d3 = text.ToDynamic();

        Assert.AreEqual(d1.Name, d3.Name);
        Assert.AreEqual(d1.Child.Name, d3.Child.Name);
        Assert.AreEqual(d1.Child.Child.Names[1], d3.Child.Child.Names[1]);

    }
}
