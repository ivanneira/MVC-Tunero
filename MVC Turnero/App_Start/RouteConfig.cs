using System.Web.Mvc;
using System.Web.Routing;

namespace MVC_Turnero.App_Start
{
    public class RouteConfig
    {
        public static void Configure(RouteCollection routes)
        {
            routes.MapRoute(
                            "Default",     // Route name
                            "{controller}/{action}/{sistema}/{csId}/{consultorioId}",    // URL with parameters
                            new { controller = "Home", action = "Index",sistema = "", csId = "", consultorioId = "", UrlParameter.Optional } // Parameter defaults
                            );
        }
    }
}