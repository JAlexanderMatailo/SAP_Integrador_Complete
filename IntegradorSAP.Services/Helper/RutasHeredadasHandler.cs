using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IntegradorSAP.Data.Helper
{
    /// <summary>
    /// Mantiene vivas las rutas antiguas con marca reescribiendo la URL antes de
    /// que Web API enrute: /api/CTK/... pasa a /api/sap/...
    ///
    /// Por qué un handler y no un segundo atributo: [RoutePrefix] tiene
    /// AllowMultiple = false, así que un controller no puede declarar dos
    /// prefijos. Y duplicar [Route] en las 16 acciones sería repetir 16 veces lo
    /// que aquí se resuelve en un sitio.
    ///
    /// Se comprobó que el consumidor actual (CTK.Marlon.Api) invoca /api/CTK en 6
    /// lugares, así que sin esto el renombrado lo rompería.
    ///
    /// PARA RETIRARLO: cuando todos los consumidores apunten a /api/sap, quitar
    /// el registro en WebApiConfig y borrar este archivo. El log deja constancia
    /// de quién sigue usando la ruta vieja.
    /// </summary>
    public class RutasHeredadasHandler : DelegatingHandler
    {
        private const string PrefijoViejo = "/api/CTK/";
        private const string PrefijoNuevo = "/api/sap/";

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string ruta = request.RequestUri.AbsolutePath;

            if (ruta.StartsWith(PrefijoViejo, StringComparison.OrdinalIgnoreCase))
            {
                var nueva = new UriBuilder(request.RequestUri);
                nueva.Path = PrefijoNuevo + ruta.Substring(PrefijoViejo.Length);
                request.RequestUri = nueva.Uri;

                LogGeneral.EscribirLog(
                    "Ruta heredada en uso: " + ruta + " reescrita a " + nueva.Path +
                    ". Actualizar el consumidor a " + PrefijoNuevo);
            }
            else if (ruta.Equals("/api/CTK", StringComparison.OrdinalIgnoreCase))
            {
                // Sin barra final: mismo trato, para no dejar un 404 raro.
                var nueva = new UriBuilder(request.RequestUri);
                nueva.Path = "/api/sap";
                request.RequestUri = nueva.Uri;
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
