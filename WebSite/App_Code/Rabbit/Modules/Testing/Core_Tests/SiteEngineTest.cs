using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;
using System.Web.Script.Serialization;
using System.Collections;
using System.Text;
using System.Reflection;

[TestClass]
public class SiteEngineTest
{
    [TestMethod]
    public void Get_Site_Settings_HookTest()
    {
        dynamic a = SiteEngine.RunHook("get_site_settings", new ExpandoObject());
        Assert.IsTrue(a.Template == "Default");
    }
}

