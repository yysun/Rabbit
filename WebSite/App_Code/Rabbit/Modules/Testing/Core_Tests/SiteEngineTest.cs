using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;
using System.Web.Script.Serialization;
using System.Collections;
using System.Text;
using System.Reflection;

//[TestClass]
public class SiteEngineTest
{
    [TestMethod]
    public void Get_Site_Settings_HookTest()
    {
        dynamic a = SiteEngine.RunHook("get_site_settings", new ExpandoObject());
        Assert.IsTrue(a.Template == "Default");
    }

    [TestMethod]
    public void AssertTest()
    {
        var i = 0;

        //this works only web.config says debug=true 
        //it could blow up IISExpress, if not clicked ignore
        //System.Diagnostics.Debug.Assert(i != 0); 

        //this always works
        Assert.IsTrue(i == 0);
    }
}

