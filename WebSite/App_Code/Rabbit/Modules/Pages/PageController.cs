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
    public PageController(object webPage)
        : base(webPage)
    {
        this.ModuleName = "Pages";
        this.ContentTypeName = "Page";
        this.Model = new PageModel();
    }

    [Get("/")]
    public virtual void Default()
    {
        View("Default");
    }

    [Post("/")]
    public virtual void Default(object form)
    {
        View("Default");
    }

    [Get("/*")]
    public virtual void Get(string[] urlData)
    {
        var id = string.Join("/", urlData.ToArray());
        View(id);
    }

    protected virtual void View(string id)
    {
        RenderView(GetItemById(id));
    }

    [Get("List")]
    public virtual void List(string[] urlData)
    {
        int pageNo   = urlData.Length > 1 && int.TryParse(urlData[1], out pageNo) ? pageNo : 1;
        int pageSize = urlData.Length > 2 && int.TryParse(urlData[2], out pageSize) ? pageSize : 20;

        dynamic data = this.Model.List(pageNo, pageSize).Value;
        data.Title = this.ModuleName;
        data = SiteEngine.RunHook(GET_LIST, data);
        RenderView((ExpandoObject) data);
    }

    [Get("Edit")]
    public virtual void Edit(string id)
    {
        var item = GetItemById(id);

        if (item == null)
        {
            View("Default"); //Reponse.Redirect("~/");
        }
        else
        {
            RenderView(item);
        }
    }

    [Post("Edit")]
    public virtual void Edit(string id, dynamic request)
    {
        dynamic item = new ExpandoObject();
        item.Id = request.Form["Id"];
        item.Title = request.Form["Title"];
        item.Content = request is HttpRequestBase ? Validation.Unvalidated(request, "Content")
            : request.Form["Content"];

        item = this.Model.Update(item).Value;
        item = SiteEngine.RunHook(SAVE_ITEM, item);

        if (this.Model.HasError)
        {
            item.Errors = Model.Errors;
            RenderView((ExpandoObject) item);
        }
        else
        {
            View(id);
        }
    }

    [Post("Delete")]
    public virtual void Delete(NameValueCollection form)
    {
        dynamic item = new ExpandoObject();
        item.Id = form["Id"];
        SiteEngine.RunHook(DELETE_ITEM, item);
        this.Model.Delete(item);
        View("Default");
    }

    [Get("Create")]
    public virtual void Create()
    {
        //check access
        dynamic newitem = new ExpandoObject();
        newitem.Id = null as string;
        newitem.Title = string.Format("[New {0}]", this.ContentTypeName);
        newitem = SiteEngine.RunHook(NEW_ITEM, newitem);
        RenderView((ExpandoObject)newitem);
    }

    [Post("Create")]
    public virtual void Create(dynamic request)
    {
        dynamic item = new ExpandoObject();
        item.Title = request.Form["Title"];
        item.Content = request is HttpRequestBase ? Validation.Unvalidated(request, "Content")
            : request.Form["Content"];
        
        item = this.Model.Create(item).Value;
        item = SiteEngine.RunHook(SAVE_ITEM, item);

        if (this.Model.HasError)
        {
            item.Errors = Model.Errors;
            RenderView((ExpandoObject)item);
        }
        else
        {
            View(item.Id);
        }
    }

    protected ExpandoObject GetItemById(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return null;

        dynamic item = new ExpandoObject();
        item.Id = id;
        item = Model.Load(item).Value;
        return SiteEngine.RunHook(GET_ITEM, item);        
    }


    [Get("EditMenu")]
    public void EditMenu()
    {
        RenderView(
            SiteEngine.RunHook("get_menu_view", "~/Pages/_Menu_Edit.cshtml") as string,
            SiteEngine.RunHook("get_menu", ""));
    }

    [Post("EditMenu")]
    public void EditMenu(NameValueCollection form)
    {
        dynamic menu = form["Menu"];
        RenderView(SiteEngine.RunHook("save_menu", menu));
    }
}