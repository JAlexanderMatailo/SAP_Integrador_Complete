using Amazon.CloudFront;
using IntegradorSAP.Data.Entidades;
using IntegradorSAP.Data.Helper;
using IntegradorSAP.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegradorSAP.Data.Manager
{
    public class ContabilidadManager
    {
        public ServiceLayer_Web servicio = new ServiceLayer_Web();
        string Usuario;
        string Ip;
        string errorCode;
        string errMsg;
        public string Mensaje { get; set; }

        public string ErrMsg { get => ErrMsg; set => errMsg = value; }
        public string ErrorCode { get => ErrorCode; set => errorCode = value; }
        public ContabilidadManager(ref ServiceLayer_Web servicio)
        {
            this.servicio = servicio;
        }

        public ContabilidadManager()
        {
            
        }
        public async Task<RespuestaGenerica> GuardarAsientoContableNomina( JournalEntriesViewModel journalEntries)
        {
            System.Net.HttpStatusCode _Status = System.Net.HttpStatusCode.Created;
            RespuestaGenerica respuesta = new RespuestaGenerica();

            if (journalEntries == null)
            {
                respuesta.Success = false;
                respuesta.ErrMensaje = "Objeto a enviar no puede ser nulo";
                return respuesta;
            }
            if (string.IsNullOrEmpty(journalEntries.CompanyDB))
            {
                respuesta.Success = false;
                respuesta.ErrMensaje = "El nombre de la companyDB Sap no puede ser nulo o vacio";
                return respuesta;
            }

            if (!servicio.IsConected)
                respuesta = await servicio.ConectarSLV2Async(journalEntries.CompanyDB);     

            if (respuesta.Success)
            {

                string _Method = "POST";
                string _recurso = $"JournalEntries";

                _Method = "POST";
                _Status = System.Net.HttpStatusCode.Created;


                string _json = JsonConvert.SerializeObject(journalEntries);
                var obj1 = JsonConvert.DeserializeObject<JournalEntriesCreateViewModel>(_json);
                _json = JsonConvert.SerializeObject(obj1);

                respuesta = await servicio.SLSendRequestReturnResponseAsync(_recurso, _Method, _json, _Status, false);

                if (respuesta.Success)
                {
                    this.errMsg = "Creación Exitosa.";

                    return respuesta;
                }
                else
                {
                    return respuesta;
                }
            }
            else
            {
                respuesta.Success = false;
                respuesta.ErrMensaje= $"Error al establecer conexión con el SSL de [{journalEntries.CompanyDB}]";
                return respuesta;
            }
            return null;
        }
    }
}
