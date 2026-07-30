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
    public static class LogGeneral
    {

        public static void EscribirLog(string mensaje)
        {
            string rutaLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
            try
            {
                using (StreamWriter sw = new StreamWriter(rutaLog, true))
                {
                    sw.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {mensaje}");
                }
            }
            catch
            {
                // Si el log falla, no queremos que se caiga todo
            }
        }


    }
}
