using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegradorSAP.Data.Helper
{
    public class SrvHana
    {
        public bool Connected { get; set; }
        public string ErrorMensaje { get; set; }

        private string ServerHana { get; set; }
        private string PortHana { get; set; }
        private string UserHana { get; set; }
        private string PwdHana { get; set; }

        // Aquí había una sobrecarga conectar() sin parámetros que fijaba
        // "Current Schema" al esquema de una empresa concreta. Se eliminó: no
        // tenía ninguna referencia (solo la llamaba DAO.conectarHana(), también
        // sin usar) y en un integrador multiempresa una conexión que ignora el
        // CompanyDB recibido y apunta siempre a la misma base es un riesgo:
        // acabaría leyendo o escribiendo en la empresa equivocada.
        // Use siempre conectar(DataBaseName).

        public HanaConnection conectar(string DataBaseName)
        {
            HanaConnection conn = null;

            try
            {

                ServerHana = ConfigurationManager.AppSettings["ServerHana"];
                PortHana = ConfigurationManager.AppSettings["PortHana"];
                UserHana = ConfigurationManager.AppSettings["UserHana"];
                PwdHana = ConfigurationManager.AppSettings["PwdHana"];

                string cadenaConexion = $"Server={ServerHana}:{PortHana};UserName={UserHana};Current Schema={DataBaseName};Password={PwdHana};";

                // Nunca se registra la cadena completa: llevaba la contraseña de
                // HANA en claro a log.txt. Solo servidor, usuario y esquema.
                LogGeneral.EscribirLog($"Conectando a HANA {ServerHana}:{PortHana} usuario={UserHana} esquema={DataBaseName}");

                Connected = false;
                conn = new HanaConnection(cadenaConexion);
                conn.Open();
                int contador = 0;

                //if (ConnectionState.Closed.Equals(conn.State)) { conn.Open(); };
                if (ConnectionState.Connecting.Equals(conn.State) || ConnectionState.Fetching.Equals(conn.State))
                {
                    while (true)
                    {
                        if (ConnectionState.Open.Equals(conn.State)) { break; }
                        if (ConnectionState.Broken.Equals(conn.State))
                        {
                            conn.Close();
                            conn.Open();
                        }
                        if (contador > 100000) { break; }
                        contador += 1;
                    }
                }

                if (ConnectionState.Open.Equals(conn.State))
                {
                    Connected = true;
                }

            }
            catch (ConfigurationErrorsException ex)
            {
                Connected = false;
                ErrorMensaje = "Error en configuración: " + ex.Message;
                LogGeneral.EscribirLog($"[Configuración] Error al conectar a la base de datos: {ex.Message} - {ex.StackTrace}");
            }
            catch (HanaException ex)
            {
                Connected = false;
                ErrorMensaje = "Error HANA: " + ex.Message;
                LogGeneral.EscribirLog($"[HANA] Error al conectar a la base de datos: {ex.Message} - {ex.StackTrace}");
            }
            catch (InvalidOperationException ex)
            {
                Connected = false;
                ErrorMensaje = "Operación inválida: " + ex.Message;
                LogGeneral.EscribirLog($"[Operación inválida] Error al conectar a la base de datos: {ex.Message} - {ex.StackTrace}");
            }
            catch (System.IO.IOException ex)
            {
                Connected = false;
                ErrorMensaje = "Error de IO: " + ex.Message;
                LogGeneral.EscribirLog($"[IO] Error al conectar a la base de datos: {ex.Message} - {ex.StackTrace}");
            }
            catch (Exception ex)
            {
                Connected = false;
                ErrorMensaje = "Error desconocido: " + ex.Message;
                LogGeneral.EscribirLog($"[General] Error al conectar a la base de datos: {ex.Message} - {ex.StackTrace}");

                // Si hay InnerException, la escribimos también
                if (ex.InnerException != null)
                {
                    LogGeneral.EscribirLog($"[General] InnerException: {ex.InnerException.Message} - {ex.InnerException.StackTrace}");
                }
                conn = null;
            }
            return conn;
        }

       


    }
}
