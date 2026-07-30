using IntegradorSAP.Data.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegradorSAP.Data.Models
{
    /// <summary>
    /// Cuerpo de la petición POST /Login del Service Layer de SAP.
    /// Los nombres de las propiedades son el contrato con SAP: no renombrar.
    /// </summary>
    public class LoginSap
    {
        public string CompanyDB { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public LoginSap()
        {
        }

        /// <summary>
        /// Resuelve usuario y clave del CompanyDB desde la configuración
        /// (claves Sap.Login.*). Antes esta lógica era una cadena de if/else con
        /// 15 nombres de empresa y sus contraseñas escritos en el código.
        /// </summary>
        public LoginSap(string CompanyDB)
        {
            // Punto único por el que pasa todo login al Service Layer.
            SapCompanyCredentials.Validar(CompanyDB);

            this.CompanyDB = CompanyDB;

            SapCredencial credencial = SapCompanyCredentials.Obtener(CompanyDB);
            this.UserName = credencial.UserName;
            this.Password = credencial.Password;
        }
    }
}
