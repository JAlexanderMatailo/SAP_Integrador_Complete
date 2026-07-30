using Newtonsoft.Json;
//using Sap.Data.Hana;
using IntegradorSAP.Data.DataAccess;
using IntegradorSAP.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IntegradorSAP.Data.Helper;
using IntegradorSAP.Data.Entidades;
using IntegradorSAP.Data.Manager;
using System.Text.RegularExpressions;
using static IntegradorSAP.Data.Models.OrdenesCompraATCOViewModel;

namespace IntegradorSAP.Services.Manager
{
    public class OrdenesCompraManager
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

        public OrdenCompraViewModel GetOrdenCompra(int DocEntry, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            if (servicio.IsConected)
            {

                string _recurso = $"PurchaseOrders({DocEntry})";
                var respuesta = servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);
                OrdenCompraViewModel obj = JsonConvert.DeserializeObject<OrdenCompraViewModel>(respuesta.RespuestaJson);


                if (respuesta.Success)
                {
                    this.errMsg = "Consulta Exitosa";

                    return obj;
                }
                else
                {

                    this.errMsg = $"Error: {respuesta.ErrCodigo}-{respuesta.ErrException}-{respuesta.ErrMensaje}";
                    return null;
                }
            }
            else
            {
                this.errMsg = $"Error: No se pudo establecer conexión";
                return null;
            }
        }


        public RespuestaGenerica GuardarOrdenCompra(OrdenCompraGuardar obj, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(obj.CompanyDB);
            System.Net.HttpStatusCode _Status = System.Net.HttpStatusCode.Created;

            RespuestaGenerica respuestaPrincipal = new RespuestaGenerica();

            if (servicio.IsConected)
            { //OBTENER CARDCODE


                //VALIDAR SOCIO NEGOCIOS/PROVEEDOR
                #region SocioNegocios
                CatalogosManager catalogos = new CatalogosManager(ref servicio);
                BusinessPartnerViewModel bp = catalogos.GetProveedor(obj.CardCode, CompanyDB);


                if (bp == null)
                {
                    respuestaPrincipal.Success = false;
                    respuestaPrincipal.ErrCodigo = -001;
                    respuestaPrincipal.ErrMensaje = $"Proveedor con {obj.CardCode} no existe en SAP.";

                    return respuestaPrincipal;

                }
                //Se actualiza el CardCode del Cliente
                obj.CardCode = bp.CardCode;
                #endregion

                #region ValidaItems

                string cc = "";
                foreach (var item in obj.DocumentLines)
                {
                    //Valida si esxiste Item
                    ItemsViewModel itemsap = catalogos.GetItem(item.ItemCode, CompanyDB);

                    if (itemsap == null)
                    {
                        respuestaPrincipal.Success = false;
                        respuestaPrincipal.ErrCodigo = -002;
                        respuestaPrincipal.ErrMensaje = $"Artículo o Item  con [{item.ItemCode}] no existe en SAP.";

                        return respuestaPrincipal;

                    }

                    //Valida si esxiste Item
                    for (int i = 1; i <= 5; i++)
                    {
                        cc = string.Empty;
                        cc = i == 1 ? item.CostingCode : i == 2 ? item.CostingCode2 : i == 3 ? item.CostingCode3 : i == 4 ? item.CostingCode4 : i == 5 ? item.CostingCode5 : "";
                        if (!string.IsNullOrEmpty(cc))
                        {
                            ProfitCenterViewModel centrocosto = catalogos.GetCentroCostos(cc, CompanyDB);

                            if (centrocosto == null)
                            {
                                if (i == 1)
                                {
                                    ProfitCenterGuardarViewModel profit = new ProfitCenterGuardarViewModel();
                                    profit.CenterCode = cc;
                                    profit.CenterName = $"{ cc}/{obj.U_EXX_TIPO_TRANSACC}";
                                    profit.GroupCode = "1";
                                    profit.InWhichDimension = 1;
                                    //profit.EffectiveTo = DateTime.Now.AddDays(-30);
                                    profit.EffectiveFrom = DateTime.Now.AddDays(-30);
                                    var respuestaprof = catalogos.GuardarCentroCostos(profit, CompanyDB);
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

                string _recurso = $"PurchaseOrders";


                if (obj.U_EXX_DOC_GEN == EnumDocGenerarse.NotaDebitoFlete.ToString())//Caso Especial si es Flete viaja como Nota de debito en Firme
                {
                    _recurso = $"PurchaseInvoices";// _recurso = $"PurchaseInvoices";
                    obj.DocumentSubType = "DM";//Nota de Débito S
                }
                else
                {
                    _recurso = $"PurchaseOrders";
                    obj.DocumentSubType = "--";

                }

                OrdenCompraSave obj1 = new OrdenCompraSave()
                {
                    CardCode = obj.CardCode,
                    DocDate = obj.DocDate,
                    U_EXX_TIPO_TRANSACC = obj.U_EXX_TIPO_TRANSACC,
                    U_EXX_DOC_GEN = obj.U_EXX_DOC_GEN,
                    Reference1 = obj.Reference1,
                    Reference2 = obj.Reference2,
                    NumAtCard = obj.NumAtCard,
                    Comments = obj.Comments,
                    JournalMemo = obj.JournalMemo,
                    U_DOC_DECLARABLE = obj.U_DOC_DECLARABLE,
                    DocumentSubType = obj.DocumentSubType,
                    DocumentLines = obj.DocumentLines,
                };

                if (obj.U_EXX_DOC_GEN == EnumDocGenerarse.NotaDebitoFlete.ToString())//Caso Especial si es Flete viaja como Nota de debito en Firme
                {
                    //Match m = Regex.Match(obj.U_EXX_TIPO_TRANSACC, "(\\d+)");
                    //string num = string.Empty;

                    //if (m.Success)
                    //{
                    //    num = m.Value;
                    //}

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

               // obj1.GroupNumber = -1;
               // obj.SalesPersonCode = -1;
                obj1.RequriedDate = obj.DocDate;
                obj1.CancelDate = obj.DocDate;
                obj1.DocDueDate = obj.DocDate;

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

        public RespuestaGenerica CancelarOrdenCompra(OrdenCompraCancelar obj, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);
            System.Net.HttpStatusCode _Status = System.Net.HttpStatusCode.Created;


            RespuestaGenerica respuestaPrincipal = new RespuestaGenerica();

            if (servicio.IsConected)
            { //OBTENER CARDCODE



                string _Method = "POST";

                string _recurso = $"PurchaseOrders({obj.DocEntry})/Cancel";
              
                _Status = System.Net.HttpStatusCode.OK;
                obj.Cancelled = "Y";

               // var _json = JsonConvert.SerializeObject(obj);

                var respuesta = servicio.SLSendRequestReturnResponse(_recurso, _Method, null, _Status, false);

                if (respuesta.Success)
                {
                    this.errMsg = "Ok";

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

        public string CancelarOrdenCompraPorDocAsociado(string DocumentoAsociado, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);
            System.Net.HttpStatusCode _Status = System.Net.HttpStatusCode.Created;
            string mensaje = "";


            RespuestaGenerica respuestaPrincipal = new RespuestaGenerica();

           
            if (servicio.IsConected)
            { 
                //Valida que no existan facturas o notas de debito asociadas al documento asociado. (BL)

                List<DocumentosAsociadosSapViewModel> docsasoc = this.GetDocumentosAsociados(DocumentoAsociado, CompanyDB);

                var res = docsasoc.Where(p => p.CancelStatus == "csNo");

                if (res.Count() > 0)
                {
                    this.errMsg = $"Error!! No se pueden Cancelar las ordendes de Compra asociado al Documento {DocumentoAsociado} porque ya tiene Facturas de Compra, Notas debito/Notas de Credito Emitidas.";
                    mensaje += this.errMsg;
                    return mensaje;
                }

                List<DocumentosAsociadosSapViewModel> ordenesacancelar = this.GetOrdenesCompraPorDocAsociados(DocumentoAsociado, CompanyDB);

                var ordenescerradas = ordenesacancelar.Where(p => p.DocumentStatus.Contains("Close") || p.CancelStatus == "csYes");


                if (ordenescerradas.Count() > 0)
                {
                    this.errMsg = $"Error!! No se pueden Cancelar las ordendes de Compra CERRADAS asociado al Documento {DocumentoAsociado} porque ya tiene Facturas de Compra, Notas debito/Notas de Credito Emitidas.";
                    mensaje += this.errMsg;
                    return mensaje;
                }

                string _Method = "POST";
                string _recurso = "";
                foreach (var orden in ordenesacancelar)
                {
                    if (!servicio.IsConected)
                        servicio.ConectarSL(CompanyDB);

                    _recurso = $"PurchaseOrders({orden.DocEntry})/Cancel";
                    _Status = System.Net.HttpStatusCode.NoContent;                    
                    var respuesta = servicio.SLSendRequestReturnResponse(_recurso, _Method, null, _Status, false);

                    if (respuesta.Success)
                    {
                        mensaje += $"Orden de Compra DocEntry: {orden.DocEntry} DocNum: {orden.DocNum} fue CANCELADO en SAP Exitosamente."+Environment.NewLine;
                        this.errMsg = "Ok";

                        
                    }
                    else
                    {
                        this.errMsg = $"Error Cancelacion Orden Compra: {respuesta.ErrCodigo}-{respuesta.ErrException}-{respuesta.ErrMensaje}";
                        mensaje += this.errMsg;
                    }
                }
            }
            else
            {
                this.errMsg = $"Error: No se pudo establecer conexión con SSL";
               
            }

            this.errMsg = mensaje;
            return mensaje;
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

        public List<DocumentosAsociadosSapViewModel> GetOrdenesCompraPorDocAsociados(string DocumentoAsociado, string CompanyDB)
        {

            RespuestaGenerica respuesta = new RespuestaGenerica();
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            List<DocumentosAsociadosSapViewModel> objgroup = new List<DocumentosAsociadosSapViewModel>();
            List<DocumentosAsociadosSapViewModel> obj = new List<DocumentosAsociadosSapViewModel>();
            if (servicio.IsConected)
            {
                //Obtener los datos de facturas y notas de debito
                string _recurso = $"PurchaseOrders?$select=DocEntry,DocNum,DocType,DocumentSubType,DocDate,CardCode,CardName,DocTotal,Comments,CancelStatus,DocumentStatus,U_EXX_TIPO_TRANSACC,U_EXX_DOC_GEN&$filter=U_EXX_TIPO_TRANSACC eq '{DocumentoAsociado}'";
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
                                d.DocumentSubType = d.DocumentSubType == "bod_None" ? "OrdenCompra" : d.DocumentSubType;
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


        #region ATCOTRANS

        public OrdenCompraATCOViewModel GetOrdenCompraATCO(int DocEntry, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            if (servicio.IsConected)
            {

                string _recurso = $"PurchaseOrders({DocEntry})";
                var respuesta = servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);
                OrdenCompraATCOViewModel obj = JsonConvert.DeserializeObject<OrdenCompraATCOViewModel>(respuesta.RespuestaJson);


                if (respuesta.Success)
                {
                    this.errMsg = "Consulta Exitosa";

                    return obj;
                }
                else
                {

                    this.errMsg = $"Error: {respuesta.ErrCodigo}-{respuesta.ErrException}-{respuesta.ErrMensaje}";
                    return null;
                }
            }
            else
            {
                this.errMsg = $"Error: No se pudo establecer conexión";
                return null;
            }
        }

        public RespuestaGenerica GuardarOrdenCompraATCO(OrdenCompraATCOGuardar obj, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(obj.CompanyDB);
            System.Net.HttpStatusCode _Status = System.Net.HttpStatusCode.Created;

            RespuestaGenerica respuestaPrincipal = new RespuestaGenerica();

            if (servicio.IsConected)
            { //OBTENER CARDCODE


                //VALIDAR SOCIO NEGOCIOS/PROVEEDOR
                #region SocioNegocios
                CatalogosManager catalogos = new CatalogosManager(ref servicio);
                BusinessPartnerViewModel bp = catalogos.GetProveedor(obj.CardCode, CompanyDB);


                if (bp == null)
                {
                    respuestaPrincipal.Success = false;
                    respuestaPrincipal.ErrCodigo = -001;
                    respuestaPrincipal.ErrMensaje = $"Proveedor con {obj.CardCode} no existe en SAP.";

                    return respuestaPrincipal;

                }
                //Se actualiza el CardCode del Cliente
                obj.CardCode = bp.CardCode;
                #endregion

                #region ValidaItems

                string cc = "";
                foreach (var item in obj.DocumentLines)
                {
                    //Valida si esxiste Item
                    ItemsViewModel itemsap = catalogos.GetItem(item.ItemCode, CompanyDB);

                    if (itemsap == null)
                    {
                        respuestaPrincipal.Success = false;
                        respuestaPrincipal.ErrCodigo = -002;
                        respuestaPrincipal.ErrMensaje = $"Artículo o Item  con [{item.ItemCode}] no existe en SAP.";

                        return respuestaPrincipal;

                    }

                    //Valida si esxiste Item
                    for (int i = 1; i <= 5; i++)
                    {
                        cc = string.Empty;
                        cc = i == 1 ? item.CostingCode : i == 2 ? item.CostingCode2 : i == 3 ? item.CostingCode3 : i == 4 ? item.CostingCode4 : i == 5 ? item.CostingCode5 : "";
                        if (!string.IsNullOrEmpty(cc))
                        {
                            ProfitCenterViewModel centrocosto = catalogos.GetCentroCostos(cc, CompanyDB);

                            if (centrocosto == null)
                            {
                                if (i == 1)
                                {
                                    ProfitCenterGuardarViewModel profit = new ProfitCenterGuardarViewModel();
                                    profit.CenterCode = cc;
                                    profit.CenterName = $"{ cc}/{obj.U_EXX_TIPO_TRANSACC}";
                                    profit.GroupCode = "1";
                                    profit.InWhichDimension = 1;
                                    //profit.EffectiveTo = DateTime.Now.AddDays(-30);
                                    profit.EffectiveFrom = DateTime.Now.AddDays(-30);
                                    var respuestaprof = catalogos.GuardarCentroCostos(profit, CompanyDB);
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

                string _recurso = $"PurchaseOrders";
               
                _recurso = $"PurchaseOrders";
                obj.DocumentSubType = "--";

                OrdenCompraATCOSave obj1 = new OrdenCompraATCOSave()
                {
                    CardCode = obj.CardCode,
                    DocDate = obj.DocDate,
                    U_EXX_TIPO_TRANSACC = obj.U_EXX_TIPO_TRANSACC,
                    U_EXX_DOC_GEN = obj.U_EXX_DOC_GEN,
                    Reference1 = obj.Reference1,
                    Reference2 = obj.Reference2,
                    NumAtCard = obj.NumAtCard,
                    Comments = obj.Comments,
                    JournalMemo = obj.JournalMemo,
                    U_DOC_DECLARABLE = obj.U_DOC_DECLARABLE,
                    DocumentSubType = obj.DocumentSubType,


                    DocumentLines = obj.DocumentLines,

                };
                
                _Method = "POST";
                _Status = System.Net.HttpStatusCode.Created;
                
                // obj1.GroupNumber = -1;
                // obj.SalesPersonCode = -1;
                obj1.RequriedDate = obj.DocDate;
                obj1.CancelDate = obj.DocDate;
                obj1.DocDueDate = obj.DocDate;

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

        #endregion 

    }

}