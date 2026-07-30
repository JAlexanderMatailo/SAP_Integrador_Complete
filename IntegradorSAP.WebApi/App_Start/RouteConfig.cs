using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace IntegradorSAP.WebApi
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // /swagger lleva a la interfaz. Se declara ANTES de la ruta por
            // defecto porque con {controller}/{action} buscaria un controller
            // llamado "swagger", que no existe.
            // No tapa el documento: casa solo con /swagger exacto, mientras que
            // /swagger/docs/v1 lo sigue sirviendo Swashbuckle.
            routes.MapRoute(
                name: "SwaggerUi",
                url: "swagger",
                defaults: new { controller = "Home", action = "Swagger" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
