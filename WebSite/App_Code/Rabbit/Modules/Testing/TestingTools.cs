using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

[AttributeUsage(AttributeTargets.Class)]
public class TestClassAttribute : Attribute
{

}

[AttributeUsage(AttributeTargets.Method)]
public class TestMethodAttribute : Attribute
{

}
public class TestException : Exception
{
    public TestException(string message) : base (message)
    {}
}

public static class Assert
{
    internal static void IsTrue(bool p, string message = null)
    {
        if (!p) throw new TestException(message ?? "IsTrue failed");
    }

    internal static void IsFalse(bool p, string message = null)
    {
        if (p) throw new TestException(message ?? "IsFalse failed");
    }

    internal static void AreEqual(object obj1, object obj2, string message = null)
    {
        if((obj1==null && obj2 !=null) || !obj1.Equals(obj2)) throw new TestException(
            string.Format(message ?? "AreEqual failed, expected <{0}>, actual <{1}>", obj1, obj2));
    }

    internal static void AreNotEqual(object obj1, object obj2, string message = null)
    {
        if ((obj1 == null && obj2 == null) || obj1.Equals(obj2)) throw new TestException(
                string.Format(message ?? "AreNotEqual failed <{0}> = <{1}>", obj1, obj2));
    }

}