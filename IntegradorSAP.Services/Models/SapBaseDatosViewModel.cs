using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegradorSAP.Data.Models
{

    public class TipoTrans
    {
        public Int64 Id { get; set; }

        public string Codigo { get; set; }

        public string Descripcion { get; set; }

        public string Descripcion2 { get; set; }

        public string Descripcion3 { get; set; }
    }
    public class SapBaseDatosViewModel
    {

        public int Id { get; set; }

        public string NombreBaseDatos { get; set; }

        public string NombreSYSBIC { get; set; }

    }

    public class CuentaBancaria
    {

        public int Id { get; set; }

        public string EmpresaSap { get; set; }

        public string CodigoBanco { get; set; }

        public string NombreBanco { get; set; }
        public string NumeroCuenta { get; set; }
        public string GLAccount { get; set; }

    }

    public enum EnumCodigoBanco
    {
        TodosBancos = 0,
        BcoPichincha = 10,
        BcoGuayaquil = 17,
        BcoPacifico = 30,
        BcoInternacional = 32,
        BcoMachala = 25,
        BcoBolivariano = 37,

    }

    // Aquí había un enum EnumCompanyDB que fijaba cinco nombres de empresa en el
    // código. Se eliminó: no tenía ninguna referencia y los CompanyDB ahora se
    // declaran en configuración (claves Sap.Login.*, ver SapCompanyCredentials).
}