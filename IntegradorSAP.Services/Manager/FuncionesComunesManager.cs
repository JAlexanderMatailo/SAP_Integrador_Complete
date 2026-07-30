using IntegradorSAP.Data.Entidades;
using IntegradorSAP.Data.Helper;
using IntegradorSAP.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace IntegradorSAP.Data.Manager
{
    public class FuncionesComunesManager
    {
        public ServiceLayer_Web servicio = new ServiceLayer_Web();
        public FuncionesComunesManager(ref ServiceLayer_Web servicio)
        {
            this.servicio = servicio;

        }


        /// <summary>
        /// FuncionSelect
        /// </summary>
        /// <param name="Tabla">Nombre de la Tabla de Sap ejemplo: PurchaseOrders</param>
        /// <param name="CamposSelect">Nombres de los campos separados por comas(,) ejemplo: DocEntry,DocNum</param>
        /// <param name="CamposWhere">Campo condición compartivo y valor ejemplo:DocEntry eq '1'</param>
        /// <param name="CompanyDB">Nombre de la Base de Datos SAP</param>
        /// <returns>Devuelve JSON del objeto, se debe deserializar Ejmplo: DocumentosAsociados docu = JsonConvert.DeserializeObject<DocumentosAsociados>(respuesta.RespuestaJson); </returns>
        public string FuncionSelect(string Tabla,string CamposSelect, string CamposWhere, string CompanyDB)
        {

            RespuestaGenerica respuesta = new RespuestaGenerica();
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            List<DocumentosAsociadosSapViewModel> objgroup = new List<DocumentosAsociadosSapViewModel>();
            List<DocumentosAsociadosSapViewModel> obj = new List<DocumentosAsociadosSapViewModel>();
            if (servicio.IsConected)
            {
                //Obtener los datos de facturas y notas de debito
                string _recurso = $"{Tabla}?$select={CamposSelect}&$filter={CamposWhere}";

                //string _recurso = $"PurchaseOrders?$select=DocEntry,DocNum,DocType,DocumentSubType,DocDate,CardCode,CardName,DocTotal,Comments,CancelStatus,DocumentStatus,U_EXX_TIPO_TRANSACC,U_EXX_DOC_GEN&$filter=U_EXX_TIPO_TRANSACC eq '{DocumentoAsociado}'";

                respuesta = servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);

                if (respuesta.Success)
                {
                    return respuesta.RespuestaJson;
                    // DocumentosAsociados docu = JsonConvert.DeserializeObject<DocumentosAsociados>(respuesta.RespuestaJson);

                }

                //Obtener los datos de Notas de crédito

                return string.Empty;
            }
            return string.Empty;
        }
    }
}