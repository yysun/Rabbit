using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Dynamic;
using System.Collections.Specialized;
using System.Web.WebPages;

/// <summary>
/// Summary description for PagesController
/// </summary>
public class PagesController : Controller
{
    public dynamic Request  { get; set; }
    public PagesController(object webPage)
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
            SiteEngine.RunHook(GET_ITEM_VIEW, DEFAULT_ITEM_VIEW) as string,
            GetItemById(id));
    }

    [Get("Edit")]
    public virtual void Edit()
    {
        RenderView(
            SiteEngine.RunHook(GET_ITEM_EDIT_VIEW, DEFAULT_ITEM_EDIT_VIEW) as string,
            GetItemById(string.Join("/", UrlData.ToArray(), 1, UrlData.Count() - 1)));
    }

    [Get("New")]
    public virtual void New()
    {
        //check access
        RenderView(
            SiteEngine.RunHook(GET_ITEM_CREATE_VIEW, DEFAULT_ITEM_CREATE_VIEW) as string,
            SiteEngine.RunHook(NEW_ITEM, new ExpandoObject()));
    }

    //[Post("Create")]
    //public virtual void Create()
    //{
    //    //check access
    //    Page.Model = SiteEngine.RunHook(NEW_ITEM, new ExpandoObject());
    //    Page.View = SiteEngine.RunHook(GET_ITEM_CREATE_VIEW, DEFAULT_ITEM_CREATE_VIEW) as string;
    //}

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
        dynamic item = GetItemFromRequest(WebPage.Request.Form);
        item = SiteEngine.RunHook(SAVE_ITEM, item);
        string view;

        if (((IDictionary<string, object>)item).ContainsKey("HasError") && item.HasError)
        {
            view = SiteEngine.RunHook(GET_ITEM_EDIT_VIEW, DEFAULT_ITEM_EDIT_VIEW) as string;
        }
        else
        {
            view = SiteEngine.RunHook(GET_ITEM_VIEW, DEFAULT_ITEM_VIEW) as string;
        }

        RenderView(view, item);
    }

    protected virtual dynamic GetItemById(string id)
    {
        dynamic item = new ExpandoObject();
        item.Id = id;
        return SiteEngine.RunHook(GET_ITEM, item);
    }

    //protected virtual dynamic GetItemFromRequest(NameValueCollection form)
    //{
    //    return form.ToDynamic();
    //}

    [Get("EditMenu")]
    public void EditMenu()
    {
        WebPage.Page.Model = new System.Dynamic.ExpandoObject();
        WebPage.Page.Model.Menu = SiteEngine.RunHook("get_menu", "");
        WebPage.Page.View = SiteEngine.RunHook("get_menu_view", "~/Pages/_Menu_Edit.cshtml") as string;
    }

    [Post("SaveMenu")]
    public void SaveMenu()
    {
        WebPage.Page.Model = new System.Dynamic.ExpandoObject();
        WebPage.Page.Model.Menu = WebPage.Context.Request.Form["Menu"];
        WebPage.Page.Model = SiteEngine.RunHook("save_menu", WebPage.Page.Model);
        WebPage.Page.View = SiteEngine.RunHook("get_menu_view", "~/Pages/_Menu_Edit.cshtml") as string;
    }

    protected dynamic GetItemFromRequest(System.Collections.Specialized.NameValueCollection form)
    {
        dynamic item = new System.Dynamic.ExpandoObject();
        item.Id = Request["Id"];
        item.Title = Request["Title"];
        item.Content = Request.Unvalidated("Content");

        //if (string.IsNullOrWhiteSpace(item.Id))
        //{
        //    item.Id = item.Title.Replace(" ", "-");
        //}
        return item;
    }
}