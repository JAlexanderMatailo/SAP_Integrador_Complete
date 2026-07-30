using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Web;
using IntegradorSAP.Data.Entidades;
using IntegradorSAP.Data.Models;
using Newtonsoft.Json;
using System.Configuration;

namespace IntegradorSAP.Data.Helper
{
      public class ServiceLayer_Web
    {
        private string RUTA_RAIZ;

        // Respaldo para cuando no hay petición HTTP (pruebas, consola). Dentro de
        // una petición manda SapRequestContext.
        private string _cookiesSinContexto;
        private bool _conectadoSinContexto;

        /// <summary>
        /// Cookie de sesión del Service Layer. Vive en el contexto de la petición
        /// para que TODOS los ServiceLayer_Web de esa petición compartan una sola
        /// sesión de SAP. Antes era un campo de instancia y cada manager abría la
        /// suya: una petición con un bucle de N elementos hacía N logins, y
        /// ninguno se cerraba.
        /// </summary>
        string HANA_SL_COOKIES
        {
            get
            {
                if (HttpContext.Current != null) { return SapRequestContext.Sesion; }
                return _cookiesSinContexto;
            }
            set
            {
                if (HttpContext.Current != null) { SapRequestContext.Sesion = value; }
                else { _cookiesSinContexto = value; }
            }
        }

        /// <summary>
        /// Hay sesión abierta. Se deriva de la cookie compartida, así que si otro
        /// manager de la misma petición ya hizo login, este no lo repite.
        /// </summary>
        public bool IsConected
        {
            get
            {
                if (HttpContext.Current != null) { return SapRequestContext.HaySesion(); }
                return _conectadoSinContexto;
            }
            set
            {
                // El estado real es "hay cookie o no". Al marcar false se descarta
                // la sesión para que el siguiente uso vuelva a autenticar.
                if (HttpContext.Current != null)
                {
                    if (!value) { SapRequestContext.Sesion = null; }
                }
                else { _conectadoSinContexto = value; }
            }
        }

        public string ErrMessage { get; set; }

        /// <summary>
        /// Timeout de las llamadas al Service Layer, en milisegundos.
        /// Configurable con la clave "Sap.TimeoutMs"; por defecto 100 s.
        /// </summary>
        private static int TimeoutMs
        {
            get
            {
                int ms;
                if (int.TryParse(ConfigurationManager.AppSettings["Sap.TimeoutMs"], out ms) && ms > 0)
                {
                    return ms;
                }
                return 100 * 1000;
            }
        }

        public ServiceLayer_Web()
        {
            // Leer la ruta base desde el archivo de configuración web.config
            RUTA_RAIZ = ConfigurationManager.AppSettings["ServerSSL"];

            // Validar que no venga vacío
            if (string.IsNullOrWhiteSpace(RUTA_RAIZ))
            {
                throw new ConfigurationErrorsException("La clave 'ServerSSL' no está configurada en el web.config o está vacía.");
            }

            // Asegurar que termine en '/' para concatenar bien las URLs
            if (!RUTA_RAIZ.EndsWith("/"))
            {
                RUTA_RAIZ += "/";
            }
        }

        //ServiceLayer_Web SSL = new ServiceLayer_Web();


        /// <summary>
        /// Traduce una WebException del Service Layer a RespuestaGenerica.
        ///
        /// El Service Layer devuelve los errores de negocio como HTTP 4xx con un
        /// cuerpo JSON de forma fija:
        ///
        ///   { "error": { "code": -10, "message": { "lang": "en-us", "value": "..." } } }
        ///
        /// Aquí se deserializa a <c>respuesta.Error</c>, que es la estructura
        /// ErrorSSL que ya existía en el proyecto y que **nunca se llenaba**. Así
        /// el consumidor obtiene el código y el mensaje de SAP ya separados en
        /// lugar de tener que sacarlos con una expresión regular.
        ///
        /// Se mantiene además el JSON crudo dentro de ErrMensaje porque el
        /// consumidor actual lo extrae con Regex(@"\{.*\}") sobre ese campo:
        /// cambiarlo rompería su GetErrorDetail(). Lo que sí se quita de ahí es
        /// el stack trace: iba dentro del mismo texto y, al ser un regex
        /// codicioso, unas llaves en el stack trace podían arruinar la
        /// extracción del JSON.
        /// </summary>
        private static void InterpretarWebException(
            RespuestaGenerica respuesta, System.Net.WebException ex, string _Url, string _Method)
        {
            respuesta.Success = false;
            respuesta.ErrCodigo = -2000;
            respuesta.ErrException = ex;
            respuesta.ErrMensaje = ex.Message;
            respuesta.RespuestaJson = null;

            string cuerpo = null;
            var httpResp = ex.Response as HttpWebResponse;

            if (ex.Response != null)
            {
                try
                {
                    using (var streamReader = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        cuerpo = streamReader.ReadToEnd();
                    }
                }
                catch (Exception exLectura)
                {
                    LogGeneral.EscribirLog($"[SL] {_Method} {_Url} - no se pudo leer el cuerpo del error: {exLectura.Message}");
                }
            }

            if (httpResp != null)
            {
                respuesta.HttpStatus = (int)httpResp.StatusCode;

                // 401 con sesión abierta = sesión caducada en SAP. Se marca para
                // que la capa superior pueda reautenticar en vez de dar un error
                // opaco. El Service Layer expira la sesión a los ~30 min.
                if (httpResp.StatusCode == HttpStatusCode.Unauthorized)
                {
                    respuesta.SesionExpirada = true;
                }
            }

            if (!string.IsNullOrWhiteSpace(cuerpo))
            {
                // Se conserva el JSON crudo en ErrMensaje: es el contrato que hoy
                // consume la API llamadora con su regex.
                respuesta.ErrMensaje = ex.Message + "\n\n\n" + cuerpo;

                try
                {
                    var errorSap = JsonConvert.DeserializeObject<ErrorSSL>(cuerpo);
                    if (errorSap != null && errorSap.error != null)
                    {
                        respuesta.Error = errorSap;
                        respuesta.ErrCodigo = errorSap.error.code;
                        if (errorSap.error.message != null &&
                            !string.IsNullOrWhiteSpace(errorSap.error.message.value))
                        {
                            respuesta.ErrMensajeSap = errorSap.error.message.value;
                        }
                    }
                }
                catch (Exception)
                {
                    // El cuerpo no tenía la forma esperada (puede ser HTML de un
                    // proxy o del balanceador). Queda el texto crudo en ErrMensaje.
                }
            }

            LogGeneral.EscribirLog(
                $"[SL] {_Method} {_Url} - HTTP {respuesta.HttpStatus} - " +
                $"codigoSap={respuesta.ErrCodigo} - {respuesta.ErrMensajeSap ?? ex.Message}");
        }

        public RespuestaGenerica SLSendRequestReturnResponse(string _Url, string _Method, string _BodyJson, HttpStatusCode _Status, bool _CapturarCookie)
        {
            Entidades.RespuestaGenerica respuesta = new Entidades.RespuestaGenerica();

            try
            {
                // CONFIGURACION DE ENVIO DE PETICION
                var httpWebRequest = WebRequest.Create(RUTA_RAIZ + _Url) as HttpWebRequest;
                httpWebRequest.Accept = "application/json;odata=minimalmetadata";
                httpWebRequest.ContentType = "application/json;odata=minimalmetadata;charset=utf8";
                httpWebRequest.KeepAlive = true; //keep alive
                httpWebRequest.Method = _Method;
                httpWebRequest.AllowAutoRedirect = false;
                // Antes: 100 * 10000 = 1.000.000 ms, casi 17 minutos, mientras la
                // versión async usaba 100 s. Un timeout de 17 min mantiene
                // ocupado un hilo de IIS. Se unifica y se hace configurable.
                httpWebRequest.Timeout = TimeoutMs;
                httpWebRequest.ServicePoint.Expect100Continue = false;
                // El callback de certificado se asigna una sola vez en el
                // constructor estático: aquí ya no se toca.

                // CONFIGURACION DEL HEADER - REQUEST
                //httpWebRequest.Headers.Add("B1S-PageSize", "5");


                // CONFIGURACION DE COOKIES
                httpWebRequest.CookieContainer = new CookieContainer();
                if (!string.IsNullOrEmpty(HANA_SL_COOKIES)) {
                    string[] cookieItems = HANA_SL_COOKIES.Split(';');
                    foreach (var cookieItem in cookieItems) {
                        string[] parts = cookieItem.Split('=');
                        if (parts.Length == 2) {
                            httpWebRequest.CookieContainer.Add(httpWebRequest.RequestUri, new Cookie(parts[0].Trim(), parts[1].Trim()));
                        }
                    }
                }

                // CONFIGURACION EL BODY AL REQUEST
                if (!string.IsNullOrWhiteSpace(_BodyJson)) {
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream())) {
                        streamWriter.Write(_BodyJson);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }

                // 'using': antes la respuesta no se liberaba nunca, y en la rama
                // de StatusCode inesperado su stream tampoco se leía ni cerraba,
                // así que la conexión quedaba retenida hasta que el GC la
                // finalizara. Con el límite de conexiones por endpoint, eso
                // estrangula el rendimiento bajo carga.
                using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    string responseContent = null;
                    if (httpResponse.StatusCode == _Status) {
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                            responseContent = streamReader.ReadToEnd();

                            //CAPTURAR COKKIE DE SESION. TOKEN DE SERVICE LAYER.
                            if (_CapturarCookie) {
                                string strMessage = httpResponse.GetResponseHeader("Set-Cookie");
                                if (!string.IsNullOrEmpty(strMessage)) {
                                    HANA_SL_COOKIES = strMessage.Replace(',', ';');
                                }
                            }
                        }

                        respuesta.Success = true;
                        respuesta.ErrCodigo = 0;
                        respuesta.ErrMensaje = "";
                        respuesta.RespuestaJson = responseContent;

                        if ("Logout".Equals(_Url)) { HANA_SL_COOKIES = ""; }

                    } else {
                        // Se drena el cuerpo aunque no sirva: si no, la conexión
                        // no vuelve al pool.
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                            responseContent = streamReader.ReadToEnd();
                        }

                        respuesta.Success = false;
                        respuesta.ErrCodigo = -1000;
                        // OJO: la errata "StatudCode" se mantiene a propósito.
                        // ComercialController compara este texto literal para
                        // decidir si una anulación fue correcta; corregirlo
                        // rompería esa lógica en silencio.
                        respuesta.ErrMensaje = $"Error por StatudCode: {httpResponse.StatusDescription} {httpResponse.StatusCode}. ";
                        respuesta.RespuestaJson = responseContent;
                        respuesta.HttpStatus = (int)httpResponse.StatusCode;
                    }
                }

            } catch (System.Net.WebException ex) {
                InterpretarWebException(respuesta, ex, _Url, _Method);

            } catch (Exception ex) {
                respuesta.Success = false;
                respuesta.ErrCodigo = -2000;
                respuesta.ErrMensaje = ex.Message;
                respuesta.ErrException = ex;
                respuesta.RespuestaJson = null;

                LogGeneral.EscribirLog($"[SL] {_Method} {_Url} - excepcion no HTTP: {ex}");
            }

            return respuesta;
        }

        public async Task<RespuestaGenerica> SLSendRequestReturnResponseAsync(string _Url, string _Method, string _BodyJson, HttpStatusCode _Status, bool _CapturarCookie)
        {
            Entidades.RespuestaGenerica respuesta = new Entidades.RespuestaGenerica();

            try
            {
                // CONFIGURACION DE ENVIO DE PETICION
                var httpWebRequest = WebRequest.Create(RUTA_RAIZ + _Url) as HttpWebRequest;
                httpWebRequest.Accept = "application/json;odata=minimalmetadata";
                httpWebRequest.ContentType = "application/json;odata=minimalmetadata;charset=utf8";
                httpWebRequest.KeepAlive = true; //keep alive
                httpWebRequest.Method = _Method;
                httpWebRequest.AllowAutoRedirect = false;
                httpWebRequest.Timeout = TimeoutMs;
                httpWebRequest.ServicePoint.Expect100Continue = false;
                // El callback de certificado se asigna una sola vez en el
                // constructor estático: aquí ya no se toca.

                // CONFIGURACION DEL HEADER - REQUEST
                //httpWebRequest.Headers.Add("B1S-PageSize", "5");


                // CONFIGURACION DE COOKIES
                httpWebRequest.CookieContainer = new CookieContainer();
                if (!string.IsNullOrEmpty(HANA_SL_COOKIES))
                {
                    string[] cookieItems = HANA_SL_COOKIES.Split(';');
                    foreach (var cookieItem in cookieItems)
                    {
                        string[] parts = cookieItem.Split('=');
                        if (parts.Length == 2)
                        {
                            httpWebRequest.CookieContainer.Add(httpWebRequest.RequestUri, new Cookie(parts[0].Trim(), parts[1].Trim()));
                        }
                    }
                }

                // CONFIGURACION EL BODY AL REQUEST
                if (!string.IsNullOrWhiteSpace(_BodyJson))
                {
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        streamWriter.Write(_BodyJson);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }

                using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    string responseContent = null;
                    if (httpResponse.StatusCode == _Status)
                    {
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            responseContent = await streamReader.ReadToEndAsync();

                            //CAPTURAR COKKIE DE SESION. TOKEN DE SERVICE LAYER.
                            if (_CapturarCookie)
                            {
                                string strMessage = httpResponse.GetResponseHeader("Set-Cookie");
                                if (!string.IsNullOrEmpty(strMessage))
                                {
                                    HANA_SL_COOKIES = strMessage.Replace(',', ';');
                                }
                            }
                        }

                        respuesta.Success = true;
                        respuesta.ErrCodigo = 0;
                        respuesta.ErrMensaje = "";
                        respuesta.RespuestaJson = responseContent;

                        if ("Logout".Equals(_Url)) { HANA_SL_COOKIES = ""; }

                    }
                    else
                    {
                        // Drenar el cuerpo para devolver la conexión al pool.
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            responseContent = await streamReader.ReadToEndAsync();
                        }

                        respuesta.Success = false;
                        respuesta.ErrCodigo = -1000;
                        // OJO: la errata "StatudCode" se mantiene a propósito.
                        // ComercialController compara este texto literal para
                        // decidir si una anulación fue correcta; corregirlo
                        // rompería esa lógica en silencio.
                        respuesta.ErrMensaje = $"Error por StatudCode: {httpResponse.StatusDescription} {httpResponse.StatusCode}. ";
                        respuesta.RespuestaJson = responseContent;
                        respuesta.HttpStatus = (int)httpResponse.StatusCode;
                    }
                }

            }
            catch (System.Net.WebException ex)
            {
                InterpretarWebException(respuesta, ex, _Url, _Method);
            }
            catch (Exception ex)
            {
                respuesta.Success = false;
                respuesta.ErrCodigo = -2000;
                respuesta.ErrMensaje = ex.Message;
                respuesta.ErrException = ex;
                respuesta.RespuestaJson = null;

                LogGeneral.EscribirLog($"[SL] {_Method} {_Url} - excepcion no HTTP: {ex}");
            }

            return respuesta;
        }

        /// <summary>
        /// Acepta el certificado del Service Layer.
        ///
        /// Antes se enganchaba con '+=' DENTRO de cada petición, sobre un delegado
        /// que es estático y global al proceso. Cada llamada HTTP añadía otro
        /// handler a la cadena y nunca se quitaba: fuga de memoria y, tras miles
        /// de peticiones, miles de handlers ejecutándose por cada validación.
        /// Ahora se asigna UNA vez desde el constructor estático.
        ///
        /// Sigue devolviendo true porque el Service Layer suele usar un
        /// certificado autofirmado. Para endurecerlo, comparar el thumbprint
        /// esperado en vez de aceptar cualquiera.
        /// </summary>
        private static bool RemoteSSLTLSCertificateValidate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        static ServiceLayer_Web()
        {
            ServicePointManager.ServerCertificateValidationCallback = RemoteSSLTLSCertificateValidate;

            // El Service Layer va por HTTPS: sin esto, .NET puede negociar TLS 1.0.
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            // Por defecto son 2 conexiones por endpoint, que estrangula el
            // rendimiento cuando varias peticiones hablan con SAP a la vez.
            if (ServicePointManager.DefaultConnectionLimit < 64)
            {
                ServicePointManager.DefaultConnectionLimit = 64;
            }
        }

        /// <summary>
        /// Registra el resultado del login y deja el motivo en ErrMessage.
        ///
        /// Antes cada rama hacía Console.WriteLine(respuesta.ErrException.StackTrace),
        /// con dos problemas: en IIS la consola no va a ninguna parte, y
        /// ErrException queda en null cuando el fallo viene por StatusCode
        /// inesperado (camino -1000), así que era un NullReferenceException que
        /// tapaba el error real.
        ///
        /// Importa más ahora que las credenciales las envía la API llamadora: un
        /// login rechazado es un caso corriente y hay que poder distinguir clave
        /// incorrecta de CompanyDB inexistente o de red caída.
        ///
        /// Nunca se registra el cuerpo del login: lleva la contraseña.
        /// </summary>
        private void RegistrarResultadoLogin(RespuestaGenerica respuesta, string CompanyDB, string usuario)
        {
            if (respuesta.Success)
            {
                IsConected = true;
                this.ErrMessage = null;

                // IsConected se deriva de la cookie de sesión compartida. El
                // Service Layer siempre devuelve B1SESSION en un login correcto;
                // si no llegó, hay que verlo explícitamente en vez de dejar que
                // cada manager reintente el login creyendo que no hay sesión.
                if (string.IsNullOrEmpty(HANA_SL_COOKIES))
                {
                    this.ErrMessage = $"SAP aceptó el login pero no devolvió cookie de sesión. " +
                                      $"CompanyDB={CompanyDB} usuario={usuario}";
                    LogGeneral.EscribirLog(this.ErrMessage);
                    return;
                }

                LogGeneral.EscribirLog($"Login SAP correcto. CompanyDB={CompanyDB} usuario={usuario}");
                return;
            }

            IsConected = false;
            this.ErrMessage = $"Login SAP rechazado. CompanyDB={CompanyDB} usuario={usuario}. " +
                              $"Codigo={respuesta.ErrCodigo}. {respuesta.ErrMensaje}";

            LogGeneral.EscribirLog(this.ErrMessage);

            if (respuesta.ErrException != null)
            {
                LogGeneral.EscribirLog($"Login SAP - detalle: {respuesta.ErrException}");
            }
        }

        public bool ConectarSL(string CompanyDB)
        {
            LoginSap login = new LoginSap(CompanyDB);

            var _json = JsonConvert.SerializeObject(login);

            var respuesta = this.SLSendRequestReturnResponse("Login", "POST", _json, System.Net.HttpStatusCode.OK, true);

            RegistrarResultadoLogin(respuesta, CompanyDB, login.UserName);

            return IsConected;

        }
        public async Task<bool> ConectarSLAsync(string CompanyDB)
        {
            LoginSap login = new LoginSap(CompanyDB);

            var _json = JsonConvert.SerializeObject(login);

            RespuestaGenerica respuesta =await this.SLSendRequestReturnResponseAsync("Login", "POST", _json, System.Net.HttpStatusCode.OK, true);

            RegistrarResultadoLogin(respuesta, CompanyDB, login.UserName);

            return IsConected;

        }

        public async Task<RespuestaGenerica> ConectarSLV2Async(string CompanyDB)
        {
            LoginSap login = new LoginSap(CompanyDB);

            var _json = JsonConvert.SerializeObject(login);

            RespuestaGenerica respuesta = await this.SLSendRequestReturnResponseAsync("Login", "POST", _json, System.Net.HttpStatusCode.OK, true);

            RegistrarResultadoLogin(respuesta, CompanyDB, login.UserName);

            return respuesta;

        }

        public bool DesconectarSL()
        {

            var respuesta = this.SLSendRequestReturnResponse("Logout", "POST", "", System.Net.HttpStatusCode.NoContent, false);

            if (respuesta.Success)
            {
                IsConected = false;
                this.ErrMessage = null;
            }
            else
            {
                // Igual que en el login: ErrException puede venir en null cuando
                // el fallo es por StatusCode inesperado, y aquí el Logout espera
                // 204 NoContent, así que un 200 OK entraba justo por ese camino.
                this.ErrMessage = respuesta.ErrMensaje;
                LogGeneral.EscribirLog($"Logout SAP fallido. Codigo={respuesta.ErrCodigo}. {respuesta.ErrMensaje}");

                if (respuesta.ErrException != null)
                {
                    LogGeneral.EscribirLog($"Logout SAP - detalle: {respuesta.ErrException}");
                }
            }

            return IsConected;
        }

    }
}
