using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for PagesExtController
/// </summary>
public class PagesExtController : PageController
{
	public PagesExtController(object webPage)
        : base(webPage)
    {

    }

    [Get("EditHomePageLayout")]
    public virtual void EditHomePageLayout()
    {
    }

    [Get("EditSectionPageLayout")]
    public virtual void EditSectionPageLayout()
    {
    }

    [Get("EditArticlePageLayout")]
    public virtual void EditArticlePageLayout()
    {
    }

    [Get("EditCategories")]
    public virtual void EditCategories()
    {
    }

    [Get("EditMenuTree")]
    public virtual void EditMenuTree()
    {

    }

}