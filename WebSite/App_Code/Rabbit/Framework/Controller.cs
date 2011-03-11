using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using System.Reflection;
using System.Dynamic;
using System.Collections.Specialized;

public class Controller 
{
    public dynamic WebPage { get; protected set; }
    public dynamic Request { get; set; }
    public IList<string> UrlData { get; set; }

    public Controller(object webPage)
    {
        this.WebPage = (dynamic) webPage;
        this.Request = (dynamic) WebPage.Request;
        this.UrlData = WebPage.UrlData;
    }

    protected void RenderView(string view, object model)
    {
        WebPage.Page.View = view;
        WebPage.Page.Model = model;
        //WebPage.Write(WebPage.RenderPage(view, model));
    }

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
}