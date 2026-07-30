using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace IntegradorSAP.Data.Helper
{
    /// <summary>
    /// Credenciales del Service Layer de SAP para la petición en curso.
    ///
    /// Vía normal: la API llamadora las envía en las cabeceras de cada petición
    /// (ver <see cref="SapCredentialsHandler"/>). El integrador no custodia
    /// credenciales de SAP: solo las usa para armar el POST /Login.
    ///
    /// Vía de respaldo: appSettings con el prefijo "Sap.Login." y formato
    /// usuario|clave, útil en pruebas o mientras un consumidor todavía no envía
    /// las cabeceras.
    ///
    ///   &lt;add key="Sap.Login.SAP_IKO"  value="usuario|clave" /&gt;
    ///   &lt;add key="Sap.Login.Default"  value="usuario|clave" /&gt;
    ///
    /// Las bases habilitadas se declaran por separado en
    /// "Sap.CompanyDbPermitidas", porque la lista blanca de empresas es una
    /// decisión distinta de dónde salen las credenciales.
    /// </summary>
    public static class SapCompanyCredentials
    {
        private const string Prefijo = "Sap.Login.";
        private const string NombreDefecto = "Default";
        private const string ClavePermitidas = "Sap.CompanyDbPermitidas";

        /// <summary>
        /// Credenciales a usar: primero las que llegaron por cabecera en esta
        /// petición, y si no vinieron, las de configuración. Lanza
        /// ConfigurationErrorsException si no hay ninguna, en vez de intentar el
        /// login con credenciales vacías.
        /// </summary>
        public static SapCredencial Obtener(string companyDB)
        {
            if (string.IsNullOrWhiteSpace(companyDB))
            {
                throw new ArgumentException("CompanyDB es obligatorio.", "companyDB");
            }

            // 1) Lo que envió la API llamadora en las cabeceras de esta petición.
            SapCredencial deLaPeticion = SapRequestContext.Actual();
            if (deLaPeticion != null) { return deLaPeticion; }

            // 2) Respaldo por configuración.
            SapCredencial credencial = Leer(companyDB);
            if (credencial != null) { return credencial; }

            credencial = Leer(NombreDefecto);
            if (credencial != null) { return credencial; }

            throw new ConfigurationErrorsException(
                "No llegaron credenciales de SAP para el CompanyDB '" + companyDB + "'. " +
                "Envíelas en las cabeceras '" + SapCredentialsHandler.CabeceraUsuario + "' y '" +
                SapCredentialsHandler.CabeceraClave + "', o declare '" + Prefijo + companyDB +
                "' en appSettings como respaldo.");
        }

        /// <summary>
        /// Valida el CompanyDB que llegó por parámetro ANTES de usarlo.
        ///
        /// Hace falta porque el CompanyDB no solo elige credenciales: se inyecta
        /// como nombre de esquema en la cadena de conexión de HANA y se concatena
        /// al SQL ("SELECT ... FROM \"" + CompanyDB + "\".\"OCRD\""). Un nombre de
        /// esquema no se puede pasar como HanaParameter —SQL no admite
        /// identificadores parametrizados—, así que la defensa es esta:
        ///
        ///  1. Rechaza vacío.
        ///  2. Rechaza cualquier carácter que no sea válido en un identificador
        ///     de HANA (letras, dígitos y '_'), lo que corta la inyección.
        ///  3. Si hay bases declaradas y no hay Sap.Login.Default, exige que el
        ///     CompanyDB sea una de ellas: lista blanca.
        ///
        /// Lanza ArgumentException con un mensaje claro en vez de dejar que el
        /// error aparezca como un fallo de SQL o de conexión más adelante.
        /// </summary>
        public static void Validar(string companyDB)
        {
            if (string.IsNullOrWhiteSpace(companyDB))
            {
                throw new ArgumentException(
                    "CompanyDB es obligatorio: indique la base de SAP en la ruta o en el cuerpo de la petición.",
                    "companyDB");
            }

            foreach (char c in companyDB)
            {
                if (!char.IsLetterOrDigit(c) && c != '_')
                {
                    throw new ArgumentException(
                        "CompanyDB '" + companyDB + "' contiene caracteres no válidos para un " +
                        "nombre de esquema de SAP. Solo se admiten letras, dígitos y '_'.",
                        "companyDB");
                }
            }

            // La lista blanca es su propia clave de configuración, no se deduce
            // de dónde haya credenciales: ahora las credenciales llegan por
            // cabecera y puede no haber ninguna entrada Sap.Login.*, pero las
            // empresas habilitadas siguen siendo un conjunto cerrado.
            List<string> permitidas = CompanyDbPermitidas();
            if (permitidas.Count == 0) { return; }

            bool habilitada = permitidas.Any(n => n.Equals(companyDB, StringComparison.OrdinalIgnoreCase));
            if (!habilitada)
            {
                throw new ArgumentException(
                    "CompanyDB '" + companyDB + "' no está habilitada en este integrador. " +
                    "Bases permitidas: " + string.Join(", ", permitidas.ToArray()) + ".",
                    "companyDB");
            }
        }

        /// <summary>
        /// Bases de SAP habilitadas, de la clave "Sap.CompanyDbPermitidas"
        /// (separadas por coma). Lista vacía = sin restricción por nombre; la
        /// validación de caracteres se aplica siempre.
        /// </summary>
        public static List<string> CompanyDbPermitidas()
        {
            string valor = ConfigurationManager.AppSettings[ClavePermitidas];
            if (string.IsNullOrWhiteSpace(valor)) { return new List<string>(); }

            return valor.Split(',')
                .Select(n => n.Trim())
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .ToList();
        }

        /// <summary>
        /// Nombres de CompanyDB con credenciales de respaldo en configuración,
        /// sin incluir Default.
        /// </summary>
        public static List<string> CompanyDbConfiguradas()
        {
            return ConfigurationManager.AppSettings.AllKeys
                .Where(k => k != null && k.StartsWith(Prefijo, StringComparison.OrdinalIgnoreCase))
                .Select(k => k.Substring(Prefijo.Length))
                .Where(n => !string.IsNullOrWhiteSpace(n)
                            && !NombreDefecto.Equals(n, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        private static SapCredencial Leer(string nombre)
        {
            string valor = ConfigurationManager.AppSettings[Prefijo + nombre];
            if (string.IsNullOrWhiteSpace(valor)) { return null; }

            // Se parte en el PRIMER separador, así la clave puede contener '|'.
            int i = valor.IndexOf('|');
            if (i <= 0 || i == valor.Length - 1)
            {
                throw new ConfigurationErrorsException(
                    "La clave '" + Prefijo + nombre + "' debe tener el formato usuario|clave.");
            }

            return new SapCredencial(valor.Substring(0, i).Trim(), valor.Substring(i + 1));
        }
    }

    /// <summary>
    /// Serializable porque se guarda en el CallContext lógico, para que fluya a
    /// través de los 'await' de los métodos async del integrador.
    /// </summary>
    [Serializable]
    public class SapCredencial
    {
        public SapCredencial(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        public string UserName { get; private set; }
        public string Password { get; private set; }
    }
}
