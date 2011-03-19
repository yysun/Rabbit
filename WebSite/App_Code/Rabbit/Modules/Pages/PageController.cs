using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Dynamic;
using System.Collections.Specialized;
using System.Web.WebPages;
using System.Web.Helpers;

/// <summary>
/// Summary description for PageController
/// </summary>
public class PageController : Controller
{
    private string GetModelHook = "Get_PageModel";

    public PageController(object webPage)
        : base(webPage)
    {
        this.ModuleName = "Pages";
        this.ContentTypeName = "Page";
    }

    [Get("List")]
    public virtual void List()
    {
        dynamic data = new ExpandoObject();
        data.List = new dynamic[] { };
        data.Start = 1;
        data = SiteEngine.RunHook(GET_LIST, data);

        RenderView(SiteEngine.RunHook(GET_LIST_VIEW, DEFAULT_LIST_VIEW) as string, data);
    }

    [Get("/")]
    public virtual void Default()
    {
        ViewItem("Default");
    }

    [Post("/")]
    public virtual void Default_Post()
    {
        ViewItem("Default");
    }

    [Get("/*")]
    public virtual void Get()
    {
        ViewItem(string.Join("/", UrlData.ToArray()));
    }

    protected virtual void ViewItem(string id)
    {
        //check access

        RenderView(
            SiteEngine.RunHook(GET_ITEM_VIEW, DEFAULT_ITEM_VIEW),
            GetItemById(id));
    }

    [Get("Edit")]
    public virtual void Edit()
    {
        RenderView(
            SiteEngine.RunHook(GET_ITEM_EDIT_VIEW, DEFAULT_ITEM_EDIT_VIEW),
            GetItemById(string.Join("/", UrlData.ToArray(), 1, UrlData.Count() - 1)));
    }

    [Get("New")]
    public virtual void New()
    {
        //check access
        RenderView(
            SiteEngine.RunHook(GET_ITEM_CREATE_VIEW, DEFAULT_ITEM_CREATE_VIEW),
            SiteEngine.RunHook(NEW_ITEM, new ExpandoObject()));
    }

    [Get("Delete")]
    public virtual void Delete()
    {
        //check access
        var item = GetItemById(string.Join("/", WebPage.UrlData.ToArray(), 1, WebPage.UrlData.Count() - 1));
        SiteEngine.RunHook(DELETE_ITEM, item);

        List();
    }

    [Post("Save")]
    public virtual void Save()
    {
        dynamic item = GetItemFromRequest();
        item = SiteEngine.RunHook(SAVE_ITEM, item);
        string view;

        if (((IDictionary<string, object>)item).ContainsKey("HasError") && item.HasError)
        {
            view = SiteEngine.RunHook(GET_ITEM_EDIT_VIEW, DEFAULT_ITEM_EDIT_VIEW);
        }
        else
        {
            view = SiteEngine.RunHook(GET_ITEM_VIEW, DEFAULT_ITEM_VIEW);
        }

        RenderView(view, item);
    }

    protected dynamic GetItemById(string id)
    {
        dynamic item = new ExpandoObject();
        item.Id = id;
        //return SiteEngine.RunHook(GET_ITEM, item);

        dynamic model = SiteEngine.RunHook(GetModelHook, new PageModel());
        return model.Load(item).Value;
    }

    protected dynamic GetItemFromRequest()
    {
        dynamic item = new ExpandoObject();
        item.Id = Request.Form["Id"];
        item.Title = Request.Form["Title"];
        item.Content = Validation.Unvalidated(Request, "Content");
        return item;
    }

    [Get("EditMenu")]
    public void EditMenu()
    {
        RenderView(
            SiteEngine.RunHook("get_menu_view", "~/Pages/_Menu_Edit.cshtml"),
            SiteEngine.RunHook("get_menu", ""));
    }

    [Post("SaveMenu")]
    public void SaveMenu()
    {
        dynamic menu = WebPage.Context.Request.Form["Menu"];
        RenderView(
            SiteEngine.RunHook("get_menu_view", "~/Pages/_Menu_Edit.cshtml"),
            SiteEngine.RunHook("save_menu", WebPage.Page.Model));
    }


}