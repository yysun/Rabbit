using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;

/// <summary>
/// Summary description for TagController
/// </summary>
public class TagController : Controller
{
    public TagController()
    {
        this.ModuleName = "Tags";
        this.ContentType = "Tag";
    }

    [Get("/")]
	public object Cloud()
	{
        return RenderView(null);
	}

    [Get("/*")]
    public object PageList(string[] urlData)
    {
        var data = new ExpandoObject();
        ((dynamic)data).List = SiteEngine.RunHook("get_pages_by_tag", urlData[0]);
        return RenderView(data);
    }

    [Post("/")]
    public object Cloud(dynamic form)
    {
        return RenderView(null);
    }

}