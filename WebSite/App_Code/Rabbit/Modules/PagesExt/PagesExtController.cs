using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for PagesExtController
/// </summary>
public class PagesExtController : Controller
{
	public PagesExtController(object webPage)
        : base(webPage)
    {

    }

    [Get("EditHomePageLayout")]
    public void EditHomePageLayout()
    {
    }

    [Get("EditSectionPageLayout")]
    public void EditSectionPageLayout()
    {
    }

    [Get("EditArticlePageLayout")]
    public void EditArticlePageLayout()
    {
    }

    [Get("EditCategories")]
    public void EditCategories()
    {
    }

    [Get("EditMenuTree")]
    public void EditMenu()
    {
        //Page.Model = new System.Dynamic.ExpandoObject();
        //Page.Model.Menu = SiteEngine.RunHook("get_menu", "");
        //Page.View = SiteEngine.RunHook("get_menu_view", "~/PagesExt/_MenuTree_Edit.cshtml") as string;
    }

}