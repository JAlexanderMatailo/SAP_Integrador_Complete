using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace IntegradorSAP.Data.DataAccess
{
    public class DAO
    {
        public string ErrorMensaje { get; set; }
        protected HanaConnection Connection;
        protected Helper.SrvHana svrHana;


        // Se eliminó conectarHana() sin parámetros: no tenía referencias y
        // delegaba en SrvHana.conectar(), que fijaba el esquema de una sola
        // empresa. Ver la nota en SrvHana.cs.

        protected void conectarHana(string DataBaseName)
        {
            // Punto único por el que pasa toda consulta a HANA. El nombre llega
            // por parámetro desde la petición y se usa como esquema, tanto en la
            // cadena de conexión como concatenado al SQL, así que se valida aquí.
            Helper.SapCompanyCredentials.Validar(DataBaseName);

            if (svrHana == null) { svrHana = new Helper.SrvHana(); }

            if (Connection == null) {
                Connection = svrHana.conectar(DataBaseName);
            }
            if (Connection == null) { 
                throw new Exception($"Error al conectar a Hana!!! con la base { DataBaseName} con params"); 
            }
        }



        protected void LiberarVariables(ref HanaConnection conn) {
            try { if (conn != null) { conn.Close(); } } catch(Exception ex) { }
            conn = null;
        }

        protected void LiberarVariables(ref HanaConnection conn, ref HanaCommand comm) {
            try { if (comm != null) { comm.Dispose(); } } catch(Exception ex) { }
            try { if (conn != null) { conn.Close(); } } catch(Exception ex) { }
            comm = null;
            conn = null;
        }

        protected void LiberarVariables(ref HanaConnection conn, ref HanaCommand comm, ref HanaDataReader reader) {
            try { if (reader != null) { reader.Close(); } } catch(Exception ex) { }
            try { if (comm != null) { comm.Dispose(); } } catch(Exception ex) { }
            try { if (conn != null) { conn.Close(); } } catch(Exception ex) { }
            reader = null;
            comm = null;
            conn = null;
        }

        protected void LiberarVariables(ref HanaConnection conn, ref HanaDataAdapter adapter) {
            try { if (adapter != null) { adapter.Dispose(); } } catch(Exception ex) { }
            try { if (conn != null) { conn.Close(); } } catch(Exception ex) { }
            adapter = null;
            conn = null;
        }




    }
}
