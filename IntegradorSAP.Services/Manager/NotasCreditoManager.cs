using Newtonsoft.Json;
using Sap.Data.Hana;
using IntegradorSAP.Data.DataAccess;
using IntegradorSAP.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IntegradorSAP.Data.Models;
using IntegradorSAP.Data.Helper;
using IntegradorSAP.Data.Entidades;
using IntegradorSAP.Data.Manager;
using System.Configuration;
using static IntegradorSAP.Data.Models.OrdenesCompraATCOViewModel;

namespace IntegradorSAP.Services.Manager
{
    public class NotasCreditoManager : BaseManager
    {
        ServiceLayer_Web servicio = new ServiceLayer_Web();
        string Usuario;
        string Ip;
        string errorCode;
        string errMsg;
        public string Mensaje { get; set; }

        public string ErrMsg { get => ErrMsg; set => errMsg = value; }
        public string ErrorCode { get => ErrorCode; set => errorCode = value; }

        public bool Login(string CompanyDB)
        {

            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            return servicio.IsConected;

        }

       
        public RespuestaGenerica GuardarNotaCredito(NotaCreditoModel obj)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(obj.CompanyDB);
            System.Net.HttpStatusCode _Status = System.Net.HttpStatusCode.Created;

            RespuestaGenerica respuestaPrincipal = new RespuestaGenerica();

            if (servicio.IsConected)
            { 
                //VALIDAR SOCIO NEGOCIOS/PROVEEDOR
                #region SocioNegocios
                CatalogosManager catalogos = new CatalogosManager(ref servicio);
                BusinessPartnerViewModel bp = catalogos.GetProveedor(obj.CardCode, obj.CompanyDB);

                if (bp == null)
                {
                    respuestaPrincipal.Success = false;
                    respuestaPrincipal.ErrCodigo = -001;
                    respuestaPrincipal.ErrMensaje = $"Proveedor con {obj.CardCode} no existe en SAP.";
                    return respuestaPrincipal;
                }
                obj.CardCode = bp.CardCode;
                string cc = "";
                foreach (var item in obj.DocumentLines)
                {
                    //Valida si esxiste Item
                    ItemsViewModel itemsap = catalogos.GetItem(item.ItemCode, obj.CompanyDB);
                    if (itemsap == null)
                    {
                        respuestaPrincipal.Success = false;
                        respuestaPrincipal.ErrCodigo = -002;
                        respuestaPrincipal.ErrMensaje = $"Artículo o Item  con [{item.ItemCode}] no existe en SAP.";
                        return respuestaPrincipal;
                    }
                    //Valida si esxiste Item como centro de costo
                    for (int i = 1; i <= 5; i++)
                    {
                        cc = string.Empty;
                        cc = i == 1 ? item.CostingCode : i == 2 ? item.CostingCode2 : i == 3 ? item.CostingCode3 : i == 4 ? item.CostingCode4 : i == 5 ? item.CostingCode5 : "";
                        if (!string.IsNullOrEmpty(cc))
                        {
                            ProfitCenterViewModel centrocosto = catalogos.GetCentroCostos(cc, obj.CompanyDB);

                            if (centrocosto == null)
                            {
                                if (i == 1)
                                {
                                    ProfitCenterGuardarViewModel profit = new ProfitCenterGuardarViewModel();
                                    profit.CenterCode = cc;
                                    profit.CenterName = $"{cc}/{obj.U_EXX_TIPO_TRANSACC}";
                                    profit.GroupCode = "1";
                                    profit.InWhichDimension = 1;
                                    //profit.EffectiveTo = DateTime.Now.AddDays(-30);
                                    profit.EffectiveFrom = DateTime.Now.AddDays(-30);
                                    var respuestaprof = catalogos.GuardarCentroCostos(profit, obj.CompanyDB);
                                    if (!respuestaprof.Success)
                                    {
                                        return respuestaprof;
                                    }
                                }
                                else
                                {
                                    respuestaPrincipal.Success = false;
                                    respuestaPrincipal.ErrCodigo = -002;
                                    respuestaPrincipal.ErrMensaje = $"Centro de costos [{i}] [{cc}] no existe en SAP.";
                                    return respuestaPrincipal;
                                }

                            }
                        }
                    }
                }
                #endregion
                ////////GUARDAR ORDEN DE COMPRA
                string _Method = "POST";//PATCH
                string _recurso = $"PurchaseCreditNotes";

                if (obj.U_EXX_DOC_GEN == EnumDocGenerarse.NotaDebitoFlete.ToString())//Caso Especial si es Flete viaja como Nota de debito en Firme
                {
                    _recurso = $"PurchaseCreditNotes";// _recurso = $"PurchaseInvoices";
                    obj.DocumentSubType = "DM";//Nota de Débito S
                }
                else
                {
                    _recurso = $"PurchaseCreditNotes";
                    obj.DocumentSubType = "--";
                }

                NotaCreditoModel obj1 = new NotaCreditoModel()
                {
                    CompanyDB = obj.CompanyDB,
                    DocDate = obj.DocDate,
                    DocDueDate = obj.DocDueDate,
                    TaxDate = obj.TaxDate,
                    CardCode = obj.CardCode,
                    CardName = obj.CardName,
                    FolioNumber = obj.FolioNumber,
                    FolioPrefixString = obj.FolioPrefixString,
                    U_DOC_DECLARABLE = obj.U_DOC_DECLARABLE,
                    NumAtCard = obj.NumAtCard,
                    U_EXX_TIPO_TRANSACC = obj.U_EXX_TIPO_TRANSACC,
                    U_EXX_DOC_GEN = obj.U_EXX_DOC_GEN,
                    U_SER_EST = obj.U_SER_EST,
                    U_SER_PE = obj.U_SER_PE,
                    U_NUM_AUTOR = obj.U_NUM_AUTOR,
                    U_COD_ST = obj.U_COD_ST,
                    U_tipo_comprob = obj.U_tipo_comprob,
                    U_TIP_DOC_APLIC = obj.U_TIP_DOC_APLIC,
                    Comments = obj.Comments,
                    JournalMemo = obj.JournalMemo,
                    DocumentSubType = obj.DocumentSubType,
                    DocumentLines = obj.DocumentLines,
                };

                if (obj.U_EXX_DOC_GEN == EnumDocGenerarse.NotaDebitoFlete.ToString())//Caso Especial si es Flete viaja como Nota de debito en Firme
                {
                    obj1.FolioNumber = obj.FolioNumber;
                    obj1.FolioPrefixString = "DC";
                }

                if (obj.DocEntry == 0)//CREACIÓN
                {
                    _Method = "POST";
                    _Status = System.Net.HttpStatusCode.Created;
                }
                else //ACTUALIZACION
                {
                    _Method = "PATCH";
                    _recurso += $"({obj.DocEntry})";
                    _Status = System.Net.HttpStatusCode.OK;
                }

                //obj1.RequriedDate = obj.DocDate;
                //obj1.CancelDate = obj.DocDate;
                //obj1.DocDueDate = obj.DocDate;

                var _json = JsonConvert.SerializeObject(obj1);

                var respuesta = servicio.SLSendRequestReturnResponse(_recurso, _Method, _json, _Status, false);

                if (respuesta.Success)
                {
                    this.errMsg = "Creación Exitosa";
                    return respuesta;
                }
                else
                {
                    this.errMsg = $"Error: {respuesta.ErrCodigo}-{respuesta.ErrException}-{respuesta.ErrMensaje}";
                    respuesta.Success = false;
                    respuesta.ErrMensaje = this.errMsg;
                    return respuesta;
                }
            }
            else
            {
                this.errMsg = $"Error: No se pudo establecer conexión con SSL";
                respuestaPrincipal.Success = false;
                respuestaPrincipal.ErrMensaje = this.errMsg;
                return respuestaPrincipal;
            }
        }

      
        public List<DocumentosAsociadosSapViewModel> GetDocumentosAsociados(string DocumentoAsociado, string CompanyDB)
        {

            RespuestaGenerica respuesta = new RespuestaGenerica();
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            List<DocumentosAsociadosSapViewModel> objgroup = new List<DocumentosAsociadosSapViewModel>();
            List<DocumentosAsociadosSapViewModel> obj = new List<DocumentosAsociadosSapViewModel>();
            if (servicio.IsConected)
            {
                //Obtener los datos de facturas y notas de debito
                string _recurso = $"PurchaseInvoices?$select=DocEntry,DocNum,DocType,DocumentSubType,DocDate,CardCode,CardName,DocTotal,Comments,CancelStatus,DocumentStatus,DocumentStatus,U_EXX_TIPO_TRANSACC,U_EXX_DOC_GEN&$filter=U_EXX_TIPO_TRANSACC eq '{DocumentoAsociado}'";
                respuesta = servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);

                if (respuesta.Success)
                {
                    DocumentosAsociados docu = JsonConvert.DeserializeObject<DocumentosAsociados>(respuesta.RespuestaJson);
                    if (docu != null)
                    {
                        if (docu.value != null)
                        {
                            foreach (var d in docu.value)
                            {
                                d.DocumentSubType = d.DocumentSubType == "bod_None" ? "FacturaCompra" : "NotaDebito";
                                d.CancelStatus = d.CancelStatus == "csNo" ? "NO" : "SI";
                                d.DocumentStatus = d.DocumentStatus == "bost_Close" ? "Cerrado" : "Abierto";
                            }
                            objgroup.AddRange(docu.value);
                        }
                    }
                }

                //Obtener los datos de Notas de crédito
                _recurso = $"PurchaseCreditNotes?$select=DocEntry,DocNum,DocType,DocumentSubType,DocDate,CardCode,CardName,DocTotal,Comments,CancelStatus,DocumentStatus,U_EXX_TIPO_TRANSACC,U_EXX_DOC_GEN&$filter=U_EXX_TIPO_TRANSACC eq '{DocumentoAsociado}'";
                respuesta = servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);

                if (respuesta.Success)
                {

                    DocumentosAsociados docu = JsonConvert.DeserializeObject<DocumentosAsociados>(respuesta.RespuestaJson);
                    if (docu != null)
                    {
                        if (docu.value != null && docu.value.Count > 0)
                        {
                            foreach (var d in docu.value)
                            {
                                d.DocumentSubType = d.DocumentSubType == "bod_None" ? "NotaCredito" : d.DocumentSubType;
                                d.CancelStatus = d.CancelStatus == "csNo" ? "NO" : "SI";
                                d.DocumentStatus = d.DocumentStatus == "bost_Close" ? "Cerrado" : "Abierto";
                            }
                            objgroup.AddRange(docu.value);
                        }
                    }

                }

                return objgroup;
            }
            return null;
        }

       
       

    }



}