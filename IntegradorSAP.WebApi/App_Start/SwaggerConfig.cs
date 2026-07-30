using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Swashbuckle.Application;
using Swashbuckle.Swagger;

namespace IntegradorSAP.WebApi
{
    /// <summary>
    /// Configuración de Swagger / OpenAPI.
    ///
    /// Se usa **Swashbuckle 5.6 (clásico)**, no Swashbuckle.AspNetCore: este
    /// proyecto es ASP.NET Web API 2 sobre .NET Framework 4.8, y el paquete de
    /// AspNetCore solo funciona en .NET Core.
    ///
    /// Se invoca desde WebApiConfig.Register en lugar de usar WebActivatorEx
    /// (que es lo que instala la plantilla del paquete): así queda una sola
    /// tubería explícita y una dependencia menos.
    ///
    /// URLs una vez desplegado:
    ///   /swagger          la interfaz
    ///   /swagger/docs/v1  el documento OpenAPI en JSON
    /// </summary>
    public static class SwaggerConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "Integrador SAP")
                        .Description(
                            "Integrador entre APIs externas y SAP Business One. " +
                            "Escribe por Service Layer y lee por HANA directo.\n\n" +
                            "AUTENTICACIÓN: las credenciales de SAP se envían en las cabeceras " +
                            "X-SAP-User y X-SAP-Password, juntas o ninguna. Si falta una de las " +
                            "dos se responde 400.\n\n" +
                            "EMPRESA: el CompanyDB viaja en cada petición, en la ruta o en el " +
                            "cuerpo, y debe estar habilitado en Sap.CompanyDbPermitidas.");

                    // Las credenciales no son parámetros de acción: las lee
                    // SapCredentialsHandler de las cabeceras. Sin este filtro, la
                    // interfaz no tendría dónde escribirlas y ningún "Try it out"
                    // podría autenticarse contra SAP.
                    c.OperationFilter<CabecerasCredencialesSap>();

                    // Hay 4 rutas declaradas por DOS controllers a la vez
                    // (FacturacionLoteController y FELController comparten el
                    // prefijo api/FacturacionLote y cuatro plantillas). Sin esto
                    // Swashbuckle aborta con "Multiple operations with path".
                    //
                    // OJO: esto solo permite generar el documento; el conflicto es
                    // real y en ejecución esos endpoints devuelven "Multiple
                    // actions were found that match the request". Hay que decidir
                    // cuál de los dos controllers es el bueno.
                    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                    // Descripciones a partir de los comentarios /// del código.
                    foreach (string ruta in RutasDeDocumentacionXml())
                    {
                        c.IncludeXmlComments(ruta);
                    }

                    c.DescribeAllEnumsAsStrings();
                })
                .EnableSwaggerUi(c =>
                {
                    c.DocumentTitle("Integrador SAP - API");
                    c.DocExpansion(DocExpansion.List);
                });
        }

        /// <summary>
        /// Archivos XML de documentación que existan en bin. Se comprueba la
        /// existencia porque si se genera el documento apuntando a un XML ausente
        /// Swashbuckle lanza FileNotFoundException y tumba /swagger entero.
        /// </summary>
        private static IEnumerable<string> RutasDeDocumentacionXml()
        {
            string bin = AppDomain.CurrentDomain.BaseDirectory + "bin\\";
            string[] candidatos =
            {
                bin + "IntegradorSAP.WebApi.xml",
                bin + "IntegradorSAP.Data.xml"
            };

            return candidatos.Where(File.Exists);
        }
    }

    /// <summary>
    /// Añade a cada operación las cabeceras de credenciales de SAP, para que se
    /// puedan escribir desde la interfaz de Swagger.
    /// </summary>
    public class CabecerasCredencialesSap : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.parameters == null)
            {
                operation.parameters = new List<Parameter>();
            }

            operation.parameters.Add(new Parameter
            {
                name = "X-SAP-User",
                @in = "header",
                type = "string",
                required = false,
                description = "Usuario de SAP Business One. Va junto con X-SAP-Password. " +
                              "Si se omiten ambas, se usan las credenciales de respaldo de la configuración."
            });

            operation.parameters.Add(new Parameter
            {
                name = "X-SAP-Password",
                @in = "header",
                type = "string",
                required = false,
                description = "Clave del usuario de SAP. No se registra en ningún log."
            });
        }
    }
}
