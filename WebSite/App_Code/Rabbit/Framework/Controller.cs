using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using System.Reflection;
using System.Dynamic;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Diagnostics;

public class Controller 
{
    internal dynamic WebPage { get; set; }
    internal dynamic Model { get; set; }
    //internal string _viewName { get; set; }

    public Controller(object webPage)
    {
        this.WebPage = (dynamic) webPage;
    }
  
    internal void RenderView(string view, object model)
    {
        var getViewHook = string.Format("GET_{0}_{1}_{2}View", ModuleName, ContentTypeName, view);
        var defaultView = string.Format("~/{0}/_{1}_{2}.cshtml", ModuleName, ContentTypeName, view);
        WebPage.Page.View = SiteEngine.RunHook(getViewHook, defaultView) as string;
        WebPage.Page.Model = model;
        //WebPage.Write(WebPage.RenderPage(view, model));
    }

    /// <summary>
    /// To call this function, paramter cannot be dynamic
    /// It has to be casted to ExpandoObject if it is dynmaic. 
    /// e.g. it should be used like this: RenderView((ExpandoObject) data)
    /// Otherwise the view name will be CallSite.Target
    /// </summary>
    /// <param name="model"></param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    protected void RenderView(ExpandoObject model)
    {
        var st = new StackTrace(1);
        var view = st.GetFrame(0).GetMethod().Name;
        RenderView(view, model);
    }
    
    protected string ModuleName { get; set; }
    protected string ContentTypeName { get; set; }

    public string GET_LIST { get { return string.Format("GET_{0}_{1}_List", ModuleName, ContentTypeName); } }
    public string GET_ITEM { get { return string.Format("GET_{0}_{1}", ModuleName, ContentTypeName); } }
    public string DELETE_ITEM { get { return string.Format("DELETE_{0}_{1}", ModuleName, ContentTypeName); } }
    public string SAVE_ITEM { get { return string.Format("SAVE_{0}_{1}", ModuleName, ContentTypeName); } }
    public string NEW_ITEM { get { return string.Format("NEW_{0}_{1}", ModuleName, ContentTypeName); } }
    public string CREATE_ITEM { get { return string.Format("CREATE_{0}_{1}", ModuleName, ContentTypeName); } }
    public string VALIDATE_ITEM { get { return string.Format("VALIDATE_{0}_{1}", ModuleName, ContentTypeName); } }
/*
    public string GET_LIST_VIEW { get { return string.Format("GET_{0}_{1}_ListView", ModuleName, ContentTypeName); } }
    public string GET_ITEM_VIEW { get { return string.Format("GET_{0}_{1}_ItemView", ModuleName, ContentTypeName); } }
    public string GET_ITEM_EDIT_VIEW { get { return string.Format("GET_{0}_{1}_EditView", ModuleName, ContentTypeName); } }
    public string GET_ITEM_CREATE_VIEW { get { return string.Format("GET_{0}_{1}_CreateView", ModuleName, ContentTypeName); } }

    public string DEFAULT_LIST_VIEW { get { return string.Format("~/{0}/_{1}_List.cshtml", ModuleName, ContentTypeName); } }
    public string DEFAULT_ITEM_VIEW { get { return string.Format("~/{0}/_{1}_View.cshtml", ModuleName, ContentTypeName); } }
    public string DEFAULT_ITEM_EDIT_VIEW { get { return string.Format("~/{0}/_{1}_Edit.cshtml", ModuleName, ContentTypeName); } }
    public string DEFAULT_ITEM_CREATE_VIEW { get { return string.Format("~/{0}/_{1}_Create.cshtml", ModuleName, ContentTypeName); } }
*/    
}