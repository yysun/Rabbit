using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ValidationTest
/// </summary>
[TestClass]
public class ValidationRuleTest
{
    [TestMethod]
    public void TestRequired()
    {
        Assert.IsTrue(Model.RULES["required"]("a", null));
        Assert.IsFalse(Model.RULES["required"]("", null));
        Assert.IsFalse(Model.RULES["required"](null, null));
        Assert.IsFalse(Model.RULES["required"](null, ""));
    }

    [TestMethod]
    public void TestEmail()
    {
        Assert.IsTrue(Model.RULES["email"](null, null));
        Assert.IsFalse(Model.RULES["email"]("", null));
        Assert.IsFalse(Model.RULES["email"]("test-com", null));
        Assert.IsFalse(Model.RULES["email"]("1@test-com", null));
        Assert.IsTrue(Model.RULES["email"]("1@test.com", ""));
    }

    [TestMethod]
    public void TestNumber()
    {
        Assert.IsTrue(Model.RULES["number"](null, null));
        Assert.IsFalse(Model.RULES["number"]("", null));
        Assert.IsFalse(Model.RULES["number"]("1-1", null));
        Assert.IsFalse(Model.RULES["number"]("1 1", null));
        Assert.IsTrue(Model.RULES["number"]("11.1", ""));
        Assert.IsTrue(Model.RULES["number"]("-.1", ""));
    }

    [TestMethod]
    public void TestDigits()
    {
        Assert.IsTrue(Model.RULES["digits"](null, null));
        Assert.IsFalse(Model.RULES["digits"]("", null));
        Assert.IsFalse(Model.RULES["digits"]("1.1", null));
        Assert.IsFalse(Model.RULES["digits"]("1 1", null));
        Assert.IsTrue(Model.RULES["digits"]("11", ""));
    }

    [TestMethod]
    public void TestDate()
    {
        Assert.IsTrue(Model.RULES["date"](null, null));
        Assert.IsFalse(Model.RULES["date"]("", null));
        Assert.IsFalse(Model.RULES["date"]("20 2012", null));
        Assert.IsFalse(Model.RULES["date"]("15-2012", null));
        Assert.IsTrue(Model.RULES["date"]("1/15/2012", ""));
        Assert.IsTrue(Model.RULES["date"]("Jan 15 2012", ""));
        Assert.IsTrue(Model.RULES["date"]("12-15-2012", ""));
    }

    [TestMethod]
    public void TestMinLen()
    {
        Assert.IsTrue(Model.RULES["minlength"](null, "1"));
        Assert.IsFalse(Model.RULES["minlength"]("", "1"));
        Assert.IsFalse(Model.RULES["minlength"]("1.1", "4"));
        Assert.IsTrue(Model.RULES["minlength"]("11", "2"));
        Assert.IsTrue(Model.RULES["minlength"](" a", "2"));
        Assert.IsTrue(Model.RULES["minlength"]("a ", "2"));
    }

    [TestMethod]
    public void TestMaxLen()
    {
        Assert.IsTrue(Model.RULES["maxlength"](null, "1"));
        Assert.IsTrue(Model.RULES["maxlength"]("", "1"));
        Assert.IsFalse(Model.RULES["maxlength"]("1.1", "2"));
        Assert.IsTrue(Model.RULES["maxlength"]("11", "2"));
        Assert.IsTrue(Model.RULES["maxlength"](" a", "2"));
        Assert.IsTrue(Model.RULES["maxlength"]("a ", "2"));
    }

}