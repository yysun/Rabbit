using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;
using System.Collections.Specialized;

namespace Rabbit
{
    /// <summary>
    /// Summary description for MockWebPage
    /// </summary>
    public class MockWebPage : Mock
    {
        public dynamic Request { get; set; }
        public dynamic Response { get; set; }
        public dynamic Context { get; set; }
        public dynamic Page { get; set; }
        public bool IsPost { get; set; }
        public IList<string> UrlData { get; set; }

        public MockWebPage(string[] urlData, bool isPost = false)
        {
            Request = new ExpandoObject();
            Response = new Mock();
            Context = new ExpandoObject();
            Page = new ExpandoObject();
            Context.IsDebuggingEnabled = true;
            IsPost = isPost;
            UrlData = urlData;
            //Setup("Write", new object[] { It.IsAny<string>()}, null);
            //Response.Setup("Redirect", new object[] { It.IsAny() });
        }
    }

    public class MockGet : MockWebPage
    {
        public MockGet(string[] UrlData)
            : base(UrlData, false)
        {
        }
    }

    public class MockPost : MockWebPage
    {
        public NameValueCollection Form { get; set; }

        public MockPost(string[] UrlData, NameValueCollection form)
            : base(UrlData, true)
        {
            Request.Form = form;
        }
    }
}