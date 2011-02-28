using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;
using System.Collections.Specialized;

/// <summary>
/// Summary description for CrudController
/// </summary>
public abstract class CrudController : Controller
{
    public string ModuleName { get; protected set; }
    public string ContentTypeName { get; protected set; }

    public string GET_LIST { get { return string.Format("GET_{0}_{1}_List", ModuleName, ContentTypeName); } }
    public string GET_ITEM { get { return string.Format("GET_{0}_{1}", ModuleName, ContentTypeName); } }
    public string DELETE_ITEM { get { return string.Format("DELETE_{0}_{1}", ModuleName, ContentTypeName); } }
    public string SAVE_ITEM { get { return string.Format("SAVE_{0}_{1}", ModuleName, ContentTypeName); } }
    public string NEW_ITEM { get { return string.Format("NEW_{0}_{1}", ModuleName, ContentTypeName); } }
    public string CREATE_ITEM { get { return string.Format("CREATE_{0}_{1}", ModuleName, ContentTypeName); } }
    public string VALIDATE_ITEM { get { return string.Format("VALIDATE_{0}_{1}", ModuleName, ContentTypeName); } }

    public string GET_LIST_VIEW { get { return string.Format("GET_{0}_{1}_ListView", ModuleName, ContentTypeName); } }
    public string GET_ITEM_VIEW { get { return string.Format("GET_{0}_{1}_ItemView", ModuleName, ContentTypeName); } }
    public string GET_ITEM_EDIT_VIEW { get { return string.Format("GET_{0}_{1}_EditView", ModuleName, ContentTypeName); } }
    public string GET_ITEM_CREATE_VIEW { get { return string.Format("GET_{0}_{1}_CreateView", ModuleName, ContentTypeName); } }

    public string DEFAULT_LIST_VIEW { get { return string.Format("~/{0}/_{1}_List.cshtml", ModuleName, ContentTypeName); } }
    public string DEFAULT_ITEM_VIEW { get { return string.Format("~/{0}/_{1}_View.cshtml", ModuleName, ContentTypeName); } }
    public string DEFAULT_ITEM_EDIT_VIEW { get { return string.Format("~/{0}/_{1}_Edit.cshtml", ModuleName, ContentTypeName); } }
    public string DEFAULT_ITEM_CREATE_VIEW { get { return string.Format("~/{0}/_{1}_Create.cshtml", ModuleName, ContentTypeName); } }

    [Get("List")]
    public virtual void List()
    {
        Page.Model = new ExpandoObject();
        Page.Model.List = new dynamic[] { };
        Page.Model.Start = 1;

        Page.Model = SiteEngine.RunHook(GET_LIST, Page.Model);
        Page.View = SiteEngine.RunHook(GET_LIST_VIEW, DEFAULT_LIST_VIEW) as string;
    }

    [Get("/")]
    public virtual void Default()
    {
        View("Default");
    }

    [Get("/*")]
    public virtual void Get()
    {
        View(string.Join("/", UrlData.ToArray()));
    }

    protected virtual void View(string id)
    {
        //check access
        Page.Model = GetItemById(id);
        Page.View = SiteEngine.RunHook(GET_ITEM_VIEW, DEFAULT_ITEM_VIEW) as string;
    }

    [Get("Edit")]
    public virtual void Edit()
    {
        Page.Model = GetItemById(string.Join("/", UrlData.ToArray(), 1, UrlData.Count() - 1));
        Page.View = SiteEngine.RunHook(GET_ITEM_EDIT_VIEW, DEFAULT_ITEM_EDIT_VIEW) as string;
    }

    [Get("New")]
    public virtual void New()
    {
        //check access
        Page.Model = SiteEngine.RunHook(NEW_ITEM, new ExpandoObject());
        Page.View = SiteEngine.RunHook(GET_ITEM_CREATE_VIEW, DEFAULT_ITEM_CREATE_VIEW) as string;
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

        Page.Model = GetItemById(string.Join("/", UrlData.ToArray(), 1, UrlData.Count() - 1));
        Page.Model = SiteEngine.RunHook(DELETE_ITEM, Page.Model);

        List();
    }

    [Post("Save")]
    public virtual void Save()
    {
        Page.Model = GetItemFromRequest(Request.Form);
        Page.Model = SiteEngine.RunHook(SAVE_ITEM, Page.Model);

        if (((IDictionary<string, object>)Page.Model).ContainsKey("HasError") && Page.Model.HasError)
        {
            Page.View = SiteEngine.RunHook(GET_ITEM_EDIT_VIEW, DEFAULT_ITEM_EDIT_VIEW) as string;
        }
        else
        {
            Page.View = SiteEngine.RunHook(GET_ITEM_VIEW, DEFAULT_ITEM_VIEW) as string;
        }
    }

    protected virtual dynamic GetItemById(string id)
    {
        dynamic item = new ExpandoObject();
        item.Id = id;
        return SiteEngine.RunHook(GET_ITEM, item);
    }

    protected virtual dynamic GetItemFromRequest(NameValueCollection form)
    {
        return form.ToDynamic();
    }

    protected virtual void Run(string moduleName, string contentTypeName)
    {
        this.ModuleName = moduleName;
        this.ContentTypeName = contentTypeName;
        Run();
    }
}