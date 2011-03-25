using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

public class Mock: DynamicObject
{
    List<Expectation> expectations = new List<Expectation>();
    
    [System.Diagnostics.DebuggerStepThrough]
    private bool GetExpectation(string name, object[] args, out object result)
    {
        var exp = expectations.Where(e => e.Name == name && !e.HasCalled).FirstOrDefault();
        if (exp != null)
        {
            exp.HasCalled = true;
            exp.CalledArgs = args;
            result = exp.Return;
            return true;
        }
        else
        {
            if (expectations.Where(e => e.Name == name).Count() > 0)
            {
                throw new UnitTestException(
                    string.Format("{0} has been called already. Add more setup.", name));
            }
            else
            {
                throw new UnitTestException(
                    string.Format("{0} has not been defined.", name));
            }
        }
    }

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        var name = "get_" + binder.Name;
        return GetExpectation(name, null, out result);
    }

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
        var name = "set_" + binder.Name;
        object result;
        return GetExpectation(name, new object[] { value }, out result);
    }

    public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
    {
        var name = binder.Name;
        return GetExpectation(name, args, out result);
    }

    public void SetupGet(string name, object value)
    {
        expectations.Add(new Expectation("get_" + name, null, value));
    }

    public void SetupSet(string name, object value)
    {
        expectations.Add(new Expectation("set_" + name, new object[] { value }, null));
    }

    public void Setup(string name, object[] parameters, object returnValue = null)
    {
        expectations.Add(new Expectation(name, parameters, returnValue));
    }

    [System.Diagnostics.DebuggerStepThrough]
    public virtual void Verify()
    {
        foreach (var exp in expectations)
        {
            Assert.IsTrue(exp.HasCalled, exp.Name + " is not called.");
            if (exp.Args != null)
            {
                Assert.IsTrue(exp.CalledArgs != null, 
                    string.Format("{0} is called w/o parameters, expected: {1}.", exp.Name, exp.Args.Length));
                
                Assert.AreEqual(exp.Args.Length, exp.CalledArgs.Length,
                    string.Format("{0} is called w/ {1} parameters, expected: {2}.", 
                        exp.Name, exp.CalledArgs.Length, exp.Args.Length));

                for (int i = 0; i < exp.Args.Length; i++)
                {
                    if (exp.Args[i] is Func<object, bool>)
                    {
                        var pass = ((Func<object, bool>)exp.Args[i])(exp.CalledArgs[i]);
                        if (pass) continue;

                        MethodInfo method = ((MulticastDelegate)exp.Args[i]).Method;
                        if (method.IsGenericMethod)
                        {
                            throw new UnitTestException(
                                string.Format("{0} is called, parameter #{1} type failed, expected: {2}, actual {3}.",
                                exp.Name, i + 1, method.GetGenericArguments()[0], exp.CalledArgs[i].GetType()));
                        }
                        else
                        {
                            var mname = method.Name.Substring(1, method.Name.IndexOf(">") - 1);
                            throw new UnitTestException(
                                string.Format("{0} is called, parameter #{1} type failed, expected: {2}, actual {3}.",
                                exp.Name, i + 1, mname, exp.CalledArgs[i] ?? "[null]"));
                        }
                    }
                    var message = string.Format("{0} is called, parameter #{1} value failed, expected: ", exp.Name, i + 1);
                    message += exp.Args[i] == null ? "[null]" : exp.Args[i].ToString() + ":" + exp.Args[i].GetType().ToString();
                    message += ", actual: ";
                    message += exp.CalledArgs[i] == null ? "[null]" : exp.CalledArgs[i].ToString() + ":" + exp.CalledArgs[i].GetType().ToString();
                    Assert.AreEqual(exp.Args[i], exp.CalledArgs[i], message);
                }
            }
        }
    }

    internal class Expectation
    {
        public string Name { get; set; }
        public object[] Args { get; set; }
        public object Return { get; set; }
        public bool HasCalled { get; set; }
        public object[] CalledArgs { get; set; }

        public Expectation(string name, object[] args, object _return)
        {
            Name = name;
            Args = args;
            Return = _return;
            HasCalled = false;
        }
    }
}

public static class It
{
    public static Func<object, bool> IsAny()
    {
        return (input) => true;
    }

    public static Func<object, bool> IsNull()
    {
        return (input) => input == null;
    }

    public static Func<object, bool> IsNotNull()
    {
        return (input) => input != null;
    }

    public static Func<object, bool> IsAny<T>()
    {
        return (input) => input is T;
    }

    public static Func<object, bool> Is<T>(Func<T, bool> condition)
    {
        return (input) => condition((T) input);
    }
}
