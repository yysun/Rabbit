using System.Web;
using System.Web.WebPages;
using System.Web.WebPages.Administration;
using System.Web.Routing;
using System.Configuration;

[assembly: PreApplicationStartMethod(typeof(Rabbit.PreApplicationStartCode), "Start")]
namespace Rabbit
{
    public static class PreApplicationStartCode
    {
        public static void Start()
        {
            SiteAdmin.Register("~/Rabbit/", "Rabbit Framework Admin",
                "Administrative Tools to manage modules, to generate code and to run unit tests.");
            
            var appPart = new ApplicationPart(typeof(PreApplicationStartCode).Assembly, 
                SiteAdmin.GetVirtualPath("~/Rabbit/"));

            ApplicationPart.Register(appPart);

            SiteEngine.Start();

            //to use Rabbit with ASP.ENT MVC project, we can disable Rabbit MVC Routing in web.config
            //var disableRouting = ConfigurationManager.AppSettings["Rabbit_DisableRouting"];
            //if (string.IsNullOrWhiteSpace(disableRouting) || disableRouting.ToLower() != "true")
            //{
            //    RouteTable.Routes.Add(new Route("{*pathInfo}", new MvcRouteHandler()));
            //}
        }
    }
}
