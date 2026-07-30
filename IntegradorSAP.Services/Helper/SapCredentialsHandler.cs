using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IntegradorSAP.Data.Helper
{
    /// <summary>
    /// Toma las credenciales de SAP Service Layer que la API llamadora envía en
    /// las cabeceras y las deja en el contexto de la petición, para que
    /// <see cref="SapCompanyCredentials"/> las resuelva más adelante.
    ///
    /// Se implementa como DelegatingHandler y no como ActionFilter porque corre
    /// antes del enlace de modelos y cubre las 85 rutas por igual, incluidas las
    /// que reciben el CompanyDB en la URL.
    ///
    /// Cabeceras:
    ///   X-SAP-User        usuario de SAP Business One
    ///   X-SAP-Password    su clave
    ///
    /// El CompanyDB sigue llegando como hasta ahora (en la ruta o en el cuerpo);
    /// no se duplica aquí para no tener dos fuentes de verdad del mismo dato.
    ///
    /// Si las cabeceras no vienen, no se hace nada: SapCompanyCredentials cae al
    /// respaldo por configuración. Así un consumidor que todavía no las envíe
    /// sigue funcionando.
    ///
    /// La clave nunca se registra ni se devuelve: solo se guarda en memoria
    /// durante la petición y se limpia al terminar.
    /// </summary>
    public class SapCredentialsHandler : DelegatingHandler
    {
        public const string CabeceraUsuario = "X-SAP-User";
        public const string CabeceraClave = "X-SAP-Password";

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string usuario = PrimerValor(request, CabeceraUsuario);
            string clave = PrimerValor(request, CabeceraClave);

            bool hayUsuario = !string.IsNullOrWhiteSpace(usuario);
            bool hayClave = !string.IsNullOrWhiteSpace(clave);

            // Media credencial es un error de integración, no un caso a tolerar:
            // si se ignorara en silencio se caería al respaldo de configuración y
            // la petición se ejecutaría con un usuario distinto del pedido.
            // Se responde 400 (culpa del llamador) en vez de lanzar, que daría 500.
            if (hayUsuario != hayClave)
            {
                HttpResponseMessage error = new HttpResponseMessage(HttpStatusCode.BadRequest);
                error.Content = new StringContent(
                    "Credenciales de SAP incompletas: envie '" + CabeceraUsuario + "' y '" +
                    CabeceraClave + "' juntas, o ninguna de las dos.",
                    Encoding.UTF8, "text/plain");
                error.RequestMessage = request;
                return error;
            }

            if (hayUsuario && hayClave)
            {
                SapRequestContext.Establecer(new SapCredencial(usuario.Trim(), clave));
            }

            try
            {
                return await base.SendAsync(request, cancellationToken);
            }
            finally
            {
                CerrarSesionSap();
                SapRequestContext.Limpiar();
            }
        }

        /// <summary>
        /// Cierra la sesión del Service Layer al terminar la petición.
        ///
        /// Antes NO se cerraba nunca: DesconectarSL() y LogOutServiceLayer()
        /// existían pero no tenían ni una llamada en todo el proyecto. Cada
        /// petición abría una sesión y la abandonaba. El Service Layer de SAP B1
        /// tiene un tope de sesiones concurrentes licenciadas y expiran a los ~30
        /// minutos, así que en pruebas no se nota y en producción acaba
        /// rechazando logins por "maximum number of sessions".
        ///
        /// Un fallo al cerrar no debe alterar la respuesta que ya se calculó: la
        /// sesión caducará por su cuenta. Solo se registra.
        /// </summary>
        private static void CerrarSesionSap()
        {
            if (!SapRequestContext.HaySesion()) { return; }

            try
            {
                new ServiceLayer_Web().DesconectarSL();
            }
            catch (Exception ex)
            {
                LogGeneral.EscribirLog("No se pudo cerrar la sesion de SAP: " + ex.Message);
            }
        }

        private static string PrimerValor(HttpRequestMessage request, string nombre)
        {
            IEnumerable<string> valores;
            if (request.Headers.TryGetValues(nombre, out valores))
            {
                return valores.FirstOrDefault();
            }
            return null;
        }
    }
}
