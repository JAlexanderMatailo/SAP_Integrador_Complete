using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using IntegradorSAP.Data.Entidades;

namespace IntegradorSAP.Data.Helper
{
    /// <summary>
    /// Convierte cualquier excepción no controlada de un controller en una
    /// RespuestaGenerica, y la registra en el log.
    ///
    /// Por qué hace falta: FilterConfig solo registraba HandleErrorAttribute, que
    /// es de ASP.NET MVC y **no aplica a ApiController**. Sin esto, una excepción
    /// en un endpoint devolvía un HTTP 500 con la pantalla amarilla de ASP.NET
    /// (HTML, no JSON), que la API llamadora no sabe deserializar: su
    /// JsonSerializer.Deserialize revienta y el error real se pierde.
    ///
    /// Además, los controllers hacían
    ///     catch (Exception ex) { throw new Exception("...: " + ex.Message); }
    /// que perdía el stack trace y, en tres endpoints, con el mensaje copiado y
    /// pegado equivocado ("Error al crear Centro de Costo" en ConsultarItemSap).
    /// Con este filtro esos try/catch pueden desaparecer.
    ///
    /// Mapeo de errores a HTTP:
    ///  - ArgumentException            -> 400, es culpa del llamador
    ///    (CompanyDB vacío, no habilitado o con caracteres inválidos)
    ///  - ConfigurationErrorsException -> 500, falta configuración en el servidor
    ///  - TimeoutException / WebException -> 504, SAP no respondió
    ///  - resto                        -> 500
    ///
    /// El detalle técnico va SIEMPRE al log y NUNCA al cuerpo de la respuesta,
    /// salvo que se active "Sap.DetalleErroresEnRespuesta" (solo para depurar).
    /// Al llamador se le da un identificador con el que localizar la traza.
    /// </summary>
    public class ErroresNoControladosFilter : ExceptionFilterAttribute
    {
        private static int _contador;

        public override void OnException(HttpActionExecutedContext context)
        {
            Exception ex = context.Exception;

            // Identificador corto para cruzar la respuesta con el log sin
            // exponer nada del interior.
            string idError = "E" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-" +
                             Interlocked.Increment(ref _contador).ToString("D4");

            string accion = context.ActionContext != null
                ? context.ActionContext.ActionDescriptor.ActionName
                : "(desconocida)";
            string ruta = context.Request != null ? context.Request.RequestUri.AbsolutePath : "(sin ruta)";

            LogGeneral.EscribirLog($"[ERROR {idError}] {ruta} accion={accion} - {ex}");

            HttpStatusCode estado;
            string mensaje;

            if (ex is ArgumentException)
            {
                estado = HttpStatusCode.BadRequest;
                mensaje = ex.Message;   // es informativo y sin datos internos
            }
            else if (ex is ConfigurationErrorsException)
            {
                estado = HttpStatusCode.InternalServerError;
                mensaje = "El integrador no está configurado correctamente. " + ex.Message;
            }
            else if (ex is TimeoutException || ex is WebException)
            {
                estado = HttpStatusCode.GatewayTimeout;
                mensaje = "SAP no respondió a tiempo. Reintente en unos momentos.";
            }
            else
            {
                estado = HttpStatusCode.InternalServerError;
                mensaje = "Error interno del integrador. Referencia: " + idError;
            }

            var respuesta = new RespuestaGenerica
            {
                Success = false,
                ErrCodigo = -3000,
                ErrMensaje = mensaje,
                ErrMensajeSap = null,
                HttpStatus = (int)estado
            };

            if (DetalleEnRespuesta())
            {
                respuesta.ErrMensaje = mensaje + " | Detalle: " + ex;
            }

            // Se responde con el MISMO contrato que el resto de endpoints
            // (RespuestaGenerica en JSON), para que el llamador use siempre el
            // mismo código de deserialización.
            context.Response = context.Request.CreateResponse(estado, respuesta);
        }

        public override Task OnExceptionAsync(HttpActionExecutedContext context, CancellationToken cancellationToken)
        {
            OnException(context);
            return Task.FromResult(0);
        }

        private static bool DetalleEnRespuesta()
        {
            bool v;
            return bool.TryParse(ConfigurationManager.AppSettings["Sap.DetalleErroresEnRespuesta"], out v) && v;
        }
    }
}
