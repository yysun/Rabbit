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

[AttributeUsage(AttributeTargets.Method)]
public class ExpectedException : Attribute
{
    public Type Type { get; private set; }
    public ExpectedException(Type type)
    {
        this.Type = type;
    }
}

public class UnitTestException : Exception
{
    public UnitTestException(string message) : base (message)
    {}
}

public static class Assert
{
    [System.Diagnostics.DebuggerStepThrough]
    public static void IsTrue(bool p, string message = null)
    {
        if (!p) throw new UnitTestException(message ?? "IsTrue failed");
    }

    [System.Diagnostics.DebuggerStepThrough]
    public static void IsFalse(bool p, string message = null)
    {
        if (p) throw new UnitTestException(message ?? "IsFalse failed");
    }

    [System.Diagnostics.DebuggerStepThrough]
    public static void AreEqual(object obj1, object obj2, string message = null)
    {
        if ((obj1 == null && obj2 != null) || !obj1.Equals(obj2))
        {
            if (message == null)
            {
                message = "AreEqual failed, expected: ";
                message += obj1 == null ? "[null]" : obj1.ToString() + ":" + obj1.GetType().ToString();
                message += ", actual: ";
                message += obj2 == null ? "[null]" : obj2.ToString() + ":" + obj2.GetType().ToString();
            }
            throw new UnitTestException(message);
        }
    }

    [System.Diagnostics.DebuggerStepThrough]
    public static void AreNotEqual(object obj1, object obj2, string message = null)
    {
        if ((obj1 == null && obj2 == null) || obj1.Equals(obj2)) 
        {
            if (message == null)
            {
                message = "AreEqual failed, expected: ";
                message += obj1 == null ? "[null]" : obj1.ToString() + ":" + obj1.GetType().ToString();
                message += ", actual: ";
                message += obj2 == null ? "[null]" : obj2.ToString() + ":" + obj2.GetType().ToString();
            }

        }
        throw new UnitTestException(message);
    }

}