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

            //SiteEngine.Start();
        }
    }
}
