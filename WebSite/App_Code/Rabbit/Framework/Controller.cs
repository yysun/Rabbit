using System.Web;
using System.Diagnostics;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Web.WebPages;

public class Controller 
{
    internal dynamic WebPage { get; set; }
    internal dynamic Model { get; set; }

    public Controller(object webPage)
    {
        this.WebPage = (dynamic) webPage;
    }
  
    internal void RenderView(string view, object model)
    {
        var getViewHook = string.Format("GET_{0}_{1}_{2}_View", ModuleName, ContentTypeName, view);
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

    protected void Redirect(string path)
    {
        this.WebPage.Page.Redirect = path;

        if (!(this.WebPage is DynamicObject))
        {
            this.WebPage.WriteLiteral(string.Format("<meta http-equiv='refresh' content='0;url=/{0}/{1}'>", 
                this.ModuleName, path));
        }
    }

    protected string ModuleName { get; set; }
    protected string ContentTypeName { get; set; }

    public string GET_LIST { get { return string.Format("GET_{0}_{1}_List", ModuleName, ContentTypeName); } }
    public string GET_ITEM { get { return string.Format("GET_{0}_{1}", ModuleName, ContentTypeName); } }
    public string DELETE_ITEM { get { return string.Format("DELETE_{0}_{1}", ModuleName, ContentTypeName); } }
    public string UPDATE_ITEM { get { return string.Format("UPDATE_{0}_{1}", ModuleName, ContentTypeName); } }
    public string NEW_ITEM { get { return string.Format("NEW_{0}_{1}", ModuleName, ContentTypeName); } }
    public string CREATE_ITEM { get { return string.Format("CREATE_{0}_{1}", ModuleName, ContentTypeName); } }
    public string VALIDATE_ITEM { get { return string.Format("VALIDATE_{0}_{1}", ModuleName, ContentTypeName); } }
}