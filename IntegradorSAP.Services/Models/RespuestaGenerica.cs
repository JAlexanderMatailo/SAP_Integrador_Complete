using IntegradorSAP.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegradorSAP.Data.Entidades
{
    /// <summary>
    /// Respuesta única de todos los endpoints del integrador.
    ///
    /// Contrato con la API llamadora:
    ///  - Success        : true si SAP aceptó la operación.
    ///  - RespuestaJson  : en éxito, el JSON que devolvió SAP.
    ///  - ErrCodigo      : código de error de SAP si lo hubo (p.ej. -10),
    ///                     o -1000 / -2000 para fallos del propio integrador.
    ///  - ErrMensaje     : mensaje legible. Cuando SAP devuelve un error de
    ///                     negocio incluye el JSON crudo, porque el consumidor
    ///                     actual lo extrae de aquí con una expresión regular.
    ///  - ErrMensajeSap  : el texto de SAP ya aislado. **Usar este**, no el regex.
    ///  - Error          : el error de SAP ya deserializado (code + message).
    ///  - HttpStatus     : código HTTP que devolvió el Service Layer.
    ///  - SesionExpirada : true si SAP respondió 401 (sesión caducada).
    /// </summary>
    public class RespuestaGenerica
    {
        public bool Success { get; set; }
        public string RespuestaJson { get; set; }

        public int ErrCodigo { get; set; }
        public string ErrMensaje { get; set; }

        /// <summary>
        /// Excepción original, para diagnóstico interno.
        ///
        /// [JsonIgnore]: antes se serializaba a la respuesta HTTP, así que los
        /// stack traces del integrador salían hacia el consumidor. El detalle
        /// técnico va al log; al consumidor se le devuelve ErrMensajeSap y Error.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public Exception ErrException { get; set; }

        /// <summary>Error de SAP ya deserializado. Antes nunca se llenaba.</summary>
        public ErrorSSL Error { get; set; }

        /// <summary>Mensaje de SAP aislado, sin JSON ni stack trace alrededor.</summary>
        public string ErrMensajeSap { get; set; }

        /// <summary>Código HTTP devuelto por el Service Layer, 0 si no llegó a responder.</summary>
        public int HttpStatus { get; set; }

        /// <summary>SAP respondió 401: la sesión caducó.</summary>
        public bool SesionExpirada { get; set; }
    }

    public class RespuestaJsonGenerica
    {
        public string Code { get; set; }
        public string ValidFrom { get; set; }
        public string ValidTo { get; set; }
        public string Active { get; set; }


    }

    public class RespuestaGenericaLote :RespuestaGenerica
    {        

        public string TipoDocumento { get; set; }

        public long DocEntry { get; set; }

        public long DocNum { get; set; }
        public string NumeroDocumento { get; set; }
        public string Cliente { get; set; }
        public decimal Total { get; set; }
        
        public long DocEntryRel { get; set; }       
       
        public long DocNumRel { get; set; }

        public string U_EXX_FPAGO_VENTAS { get; set; }

    }

    public class ErrorSSL
    {
        public Error error { get; set; }
    }

    public class Error
    {
        public int code { get; set; }
        public Message message { get; set; }
    }

    public class Message
    {
        public string lang { get; set; }
        public string value { get; set; }
    }

    public class CancelaRegistroSap
    {
        public long DocEntry { get; set; }
        public string CompanyDB { get; set; }

    }

    public class ConsultaOVRequest
    {
        public string CompanyDB { get; set; }
        public List<ConsultaOVRequestModel> Items { get; set; }
    }


}
