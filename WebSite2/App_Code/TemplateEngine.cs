using System.Web;
using System.Linq;
using Rabbit;

public class TemplateManager
{
    private static string[] HomePages = new string[]{
        "/", 
        "/default", 
        "/default.cshtml"};

    [Hook]
    public static object IsHomePage(object data)
    {
        string path = HttpContext.Current.Request.RawUrl.ToLower();
        return HomePages.Any(s => s == path);
    }

    [Hook]
    public static object Get_Layout(object data)
    {
        var template = "Default";
        return string.Format("~/Templates/{0}/_SiteLayout.cshtml", template ?? "Default");
    }
}