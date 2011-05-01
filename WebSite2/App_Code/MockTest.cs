using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;
using Microsoft.CSharp.RuntimeBinder;
using System.Reflection;
using Rabbit;

//[TestClass]
public class MockTest
{
    [TestMethod]
    public void TestMock()
    {
        dynamic mock = new Mock();
        mock.SetupGet("Prop1", 1);
        mock.SetupSet("Prop1", 5);
        mock.Setup("Method1", new object[] { "a", -2 }, 10);
        Assert.AreEqual(1, mock.Prop1);
        mock.Prop1 = 5;
        Assert.AreEqual(10, mock.Method1("a", -2));
        mock.Verify();
    }

    /// <summary>
    /// Method1 is not called
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(UnitTestException))]
    public void TestMockMethodNotRun()
    {
        dynamic mock = new Mock();
        mock.Setup("Method1", new object[] { "a", It.IsAny<int>() }, 10);
        mock.Verify();
    }

    /// <summary>
    /// Method1 has been called already. Add more setup.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(UnitTestException))]
    public void TestMockMethodCalled()
    {
        dynamic mock = new Mock();
        mock.Setup("Method1", new object[] { "a", It.IsAny<int>() }, 10);
        Assert.AreEqual(10, mock.Method1("a", -2));
        Assert.AreEqual(10, mock.Method1("b", -2m));
        mock.Verify();
    }

    /// <summary>
    /// Method1 is called w/ 0 parameters, expected: 2.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(UnitTestException))]
    public void TestMockParameterCount()
    {
        dynamic mock = new Mock();
        mock.Setup("Method1", new object[] { "a", 2.5m }, 10);
        Assert.AreEqual(10, mock.Method1());
        mock.Verify();
    }

    /// <summary>
    /// Method1 parameter #2 type failed, expected: IsNotNull, actual [null].
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(UnitTestException))]
    public void TestMockParameterNotNull()
    {
        dynamic mock = new Mock();
        mock.Setup("Method1", new object[] { "a", It.IsNotNull() }, 10);
        Assert.AreEqual(10, mock.Method1("a", null));
        mock.Verify();
    }

    /// <summary>
    /// Method1 parameter #2 type failed, expected: IsNull, actual 2.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(UnitTestException))]
    public void TestMockParameterNull()
    {
        dynamic mock = new Mock();
        mock.Setup("Method1", new object[] { "a", It.IsNull() }, 10);
        Assert.AreEqual(10, mock.Method1("a", 2));
        mock.Verify();
    }

    /// <summary>
    /// Method1 parameter #2 type failed, expected: System.Int32, actual System.Decim
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(UnitTestException))]
    public void TestMockParameterType()
    {
        dynamic mock = new Mock();
        mock.Setup("Method1", new object[] { "a", It.IsAny<int>() }, 10);
        Assert.AreEqual(10, mock.Method1("a", -2m));
        mock.Verify();
    }

    /// <summary>
    /// Method1 parameter #1 value failed, a:System.String, actual: b:System.String
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(UnitTestException))]
    public void TestMockParameterValue()
    {
        dynamic mock = new Mock();
        mock.Setup("Method1", new object[] { "a", 2.5m }, 10);
        Assert.AreEqual(10, mock.Method1("b", 2.5));
        mock.Verify();
    }

    /// <summary>
    /// set_Prop1 parameter #1 type failed, expected: System.String[], actual System.Int32
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(UnitTestException))]
    public void TestMockPropertyType()
    {
        dynamic mock = new Mock();
        mock.SetupSet("Prop1", It.IsAny<string[]>());
        mock.Prop1 = 5;
        mock.Verify();
    }

    /// <summary>
    /// set_Prop1 parameter #1 value failed, expected: 5:System.String, actual: 5:System.Int32
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(UnitTestException))]
    public void TestMockPropertyValue()
    {
        dynamic mock = new Mock();
        mock.SetupSet("Prop1", "5");
        mock.Prop1 = 5;
        mock.Verify();
    }
}
