using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace IntegradorSAP.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Configuración y servicios de API web

            // Lee las credenciales de SAP que envía la API llamadora en las
            // cabeceras X-SAP-User / X-SAP-Password y las deja en el contexto de
            // la petición. Va como message handler para que aplique a todas las
            // rutas sin tocar ninguna firma de acción.
            // Reescribe /api/CTK/... a /api/sap/... Debe ir ANTES de cualquier
            // otro handler que mire la ruta.
            config.MessageHandlers.Add(new Data.Helper.RutasHeredadasHandler());

            config.MessageHandlers.Add(new Data.Helper.SapCredentialsHandler());

            // Toda excepción no controlada sale como RespuestaGenerica en JSON y
            // queda en el log. Sin esto, ASP.NET devolvía HTML de error 500 que
            // el consumidor no puede deserializar.
            config.Filters.Add(new Data.Helper.ErroresNoControladosFilter());

            // Rutas de API web
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
