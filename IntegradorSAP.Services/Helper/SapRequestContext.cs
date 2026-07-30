using System;
using System.Runtime.Remoting.Messaging;
using System.Web;

namespace IntegradorSAP.Data.Helper
{
    /// <summary>
    /// Credenciales de SAP Service Layer que la API llamadora envía en las
    /// cabeceras de cada petición. Viven solo durante esa petición.
    ///
    /// Se guardan en dos sitios a propósito:
    ///  - HttpContext.Current.Items, que es lo idiomático alojando en System.Web;
    ///  - CallContext lógico, que sí fluye a través de 'await'.
    /// Varios managers usan métodos async (SLSendRequestReturnResponseAsync), y
    /// HttpContext.Current puede quedar en null en una continuación si el
    /// SynchronizationContext no se restaura. Con los dos, la resolución no
    /// depende de ese detalle.
    ///
    /// La contraseña NO se registra en ningún log ni se devuelve en ninguna
    /// respuesta: solo se usa para armar el cuerpo del POST /Login hacia SAP.
    /// </summary>
    public static class SapRequestContext
    {
        private const string Clave = "IntegradorSAP.CredencialesSap";
        private const string ClaveSesion = "IntegradorSAP.SesionSap";

        public static void Establecer(SapCredencial credencial)
        {
            if (credencial == null) { return; }

            if (HttpContext.Current != null)
            {
                HttpContext.Current.Items[Clave] = credencial;
            }
            CallContext.LogicalSetData(Clave, credencial);
        }

        /// <summary>
        /// Cookie de sesión del Service Layer, compartida por toda la petición.
        ///
        /// Antes vivía como campo de instancia de ServiceLayer_Web, y como cada
        /// manager construye el suyo (12 managers) y los controllers exponen
        /// '_service => new XManager()' —instancia nueva en CADA acceso— una sola
        /// petición abría varias sesiones en SAP. En los bucles era peor: un
        /// foreach con N elementos hacía N logins.
        ///
        /// Al vivir en el contexto de la petición, todos los ServiceLayer_Web de
        /// esa petición reutilizan la misma sesión: un login, y un logout al
        /// final (ver SapCredentialsHandler).
        /// </summary>
        public static string Sesion
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    return HttpContext.Current.Items[ClaveSesion] as string;
                }
                return CallContext.LogicalGetData(ClaveSesion) as string;
            }
            set
            {
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Items[ClaveSesion] = value;
                }
                CallContext.LogicalSetData(ClaveSesion, value);
            }
        }

        /// <summary>True si esta petición ya tiene una sesión abierta en SAP.</summary>
        public static bool HaySesion()
        {
            return !string.IsNullOrEmpty(Sesion);
        }

        /// <summary>
        /// Credenciales de la petición en curso, o null si la API llamadora no
        /// las envió (entonces se recurre a la configuración).
        /// </summary>
        public static SapCredencial Actual()
        {
            if (HttpContext.Current != null)
            {
                SapCredencial desdeHttp = HttpContext.Current.Items[Clave] as SapCredencial;
                if (desdeHttp != null) { return desdeHttp; }
            }
            return CallContext.LogicalGetData(Clave) as SapCredencial;
        }

        public static void Limpiar()
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Items.Remove(Clave);
                HttpContext.Current.Items.Remove(ClaveSesion);
            }
            CallContext.FreeNamedDataSlot(Clave);
            CallContext.FreeNamedDataSlot(ClaveSesion);
        }
    }
}
