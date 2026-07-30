using Newtonsoft.Json;
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
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Sap.Data.Hana;

namespace IntegradorSAP.Services.Manager
{
    public class OrdenesVentaManager : DAO
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

        public OrdenVentaViewModel GetOrden(int DocEntry, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            if (servicio.IsConected)
            {

                string _recurso = $"Orders({DocEntry})";
                var respuesta = servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);
                OrdenVentaViewModel obj = JsonConvert.DeserializeObject<OrdenVentaViewModel>(respuesta.RespuestaJson);


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


        public RespuestaGenerica GuardarOrdenVenta(OrdenVentaGuardar obj, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(obj.CompanyDB);
            System.Net.HttpStatusCode _Status = System.Net.HttpStatusCode.Created;

            RespuestaGenerica respuestaPrincipal = new RespuestaGenerica();

            if (servicio.IsConected)
            { //OBTENER CARDCODE

                #region DocumentoAsociado
                
                var docasoc = this.ExisteDocumentoAsociado(obj.U_EXX_TIPO_TRANSACC, CompanyDB);


                if (!docasoc.HasValue || docasoc.Value==0)
                {

                    //var respuestadocasoc = GuardarDocumentoAsociado(obj.DocumentoAsociado, CompanyDB);
                    //if (!respuestadocasoc.Success)
                    //{
                        respuestaPrincipal.Success = false;
                        respuestaPrincipal.ErrCodigo = -001;
                        respuestaPrincipal.ErrMensaje = $"Documento Asociado  {obj.U_EXX_TIPO_TRANSACC} no pudo ser creado.";
                        return respuestaPrincipal;

                    //}

                   

                }            
                #endregion

                //VALIDAR SOCIO NEGOCIOS/PROVEEDOR
                #region SocioNegocios
                CatalogosManager catalogos = new CatalogosManager(ref servicio);
                BusinessPartnerViewModel bp = catalogos.GetCliente(obj.CardCode, CompanyDB);


                if (bp == null)
                {
                    respuestaPrincipal.Success = false;
                    respuestaPrincipal.ErrCodigo = -001;
                    respuestaPrincipal.ErrMensaje = $"Cliente con {obj.CardCode} no existe en SAP.";

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
                                respuestaPrincipal.Success = false;
                                respuestaPrincipal.ErrCodigo = -002;
                                respuestaPrincipal.ErrMensaje = $"Centro de costos [{i}] [{item.CostingCode}] no existe en SAP. ";

                                return respuestaPrincipal;

                            }
                        }
                    }
                }

                #endregion

                ////////GUARDAR ORDEN DE VENTA
                string _Method = "POST";//PATCH
                string _recurso = $"Orders";

                obj.GroupNumber = -1;
                obj.SalesPersonCode = -1;
                obj.RequriedDate = obj.DocDate;
                obj.CancelDate = obj.DocDate;
                obj.DocDueDate = obj.DocDate;
                
                foreach (var d in obj.DocumentLines)
                {
                   d.ShipDate = obj.DocDate;

                    if (obj.U_EXX_DOC_GEN.Contains("NotaDebito"))
                    {
                        d.TaxCode = "IVA_EXE";
                      
                    }
                    else
                    {
                        //d.TaxCode = "IVA";
                        d.TaxCode = "IVA_15";

                    }
                }
                //if (obj.DocEntry == 0)//CREACIÓN
                //{
                    _Method = "POST";
                    _Status = System.Net.HttpStatusCode.Created;
                    

                //}
                //else //ACTUALIZACION
                //{
                //    _Method = "PATCH";
                //    _recurso += $"({obj.DocEntry})";
                //    _Status = System.Net.HttpStatusCode.NoContent;

                //}


                var _json = JsonConvert.SerializeObject(obj);

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

        public RespuestaGenerica GuardarOrdenCostosLocalesVenta(OrdenVentaCLGuardar obj, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(obj.CompanyDB);
            System.Net.HttpStatusCode _Status = System.Net.HttpStatusCode.Created;

            RespuestaGenerica respuestaPrincipal = new RespuestaGenerica();

            if (servicio.IsConected)
            { //OBTENER CARDCODE

                #region DocumentoAsociado

                var docasoc = this.ExisteDocumentoAsociado(obj.U_EXX_TIPO_TRANSACC, CompanyDB);


                if (!docasoc.HasValue || docasoc.Value == 0)
                {

                    //var respuestadocasoc = GuardarDocumentoAsociado(obj.DocumentoAsociado, CompanyDB);
                    //if (!respuestadocasoc.Success)
                    //{
                    respuestaPrincipal.Success = false;
                    respuestaPrincipal.ErrCodigo = -001;
                    respuestaPrincipal.ErrMensaje = $"Documento Asociado  {obj.U_EXX_TIPO_TRANSACC} no pudo ser creado.";
                    return respuestaPrincipal;

                    //}



                }
                #endregion

                //VALIDAR SOCIO NEGOCIOS/PROVEEDOR
                #region SocioNegocios
                CatalogosManager catalogos = new CatalogosManager(ref servicio);
                BusinessPartnerViewModel bp = catalogos.GetCliente(obj.CardCode, CompanyDB);


                if (bp == null)
                {
                    respuestaPrincipal.Success = false;
                    respuestaPrincipal.ErrCodigo = -001;
                    respuestaPrincipal.ErrMensaje = $"Cliente con {obj.CardCode} no existe en SAP.";

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
                                respuestaPrincipal.Success = false;
                                respuestaPrincipal.ErrCodigo = -002;
                                respuestaPrincipal.ErrMensaje = $"Centro de costos [{i}] [{item.CostingCode}] no existe en SAP. ";

                                return respuestaPrincipal;

                            }
                        }
                    }
                }

                #endregion

                ////////GUARDAR ORDEN DE VENTA
                string _Method = "POST";//PATCH
                string _recurso = $"Orders";

                obj.GroupNumber = -1;
                obj.SalesPersonCode = -1;
                obj.RequriedDate = obj.DocDate;
                obj.CancelDate = obj.DocDate;
                obj.DocDueDate = obj.DocDate;
                obj.SalesPersonCode = bp.SalesPersonCode;

                if (bp.U_TIPO_ID.Contains("P"))
                {
                    obj.U_EXX_DOC_GEN = "FacturaExportacion";
                    obj.U_HRH_Serie = "FAE002";
                }

                foreach (var d in obj.DocumentLines)
                {
                    d.ShipDate = obj.DocDate;

                    if (obj.U_EXX_DOC_GEN.Contains("NotaDebito") || bp.U_TIPO_ID.Contains("P"))
                    {
                        d.TaxCode = "IVA_EXE";
                       

                    }
                    else
                    {
                        //d.TaxCode = "IVA";
                        d.TaxCode = "IVA_15";

                    }

                 
                    
                }
                //if (obj.DocEntry == 0)//CREACIÓN
                //{
                _Method = "POST";
                _Status = System.Net.HttpStatusCode.Created;


                //}
                //else //ACTUALIZACION
                //{
                //    _Method = "PATCH";
                //    _recurso += $"({obj.DocEntry})";
                //    _Status = System.Net.HttpStatusCode.NoContent;

                //}


                var _json = JsonConvert.SerializeObject(obj);

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

        public RespuestaGenerica GuardarOrdenVentaBasica(OrdenVentaBasicaGuardar obj, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(obj.CompanyDB);
            System.Net.HttpStatusCode _Status = System.Net.HttpStatusCode.Created;

            RespuestaGenerica respuestaPrincipal = new RespuestaGenerica();

            if (servicio.IsConected)
            { //OBTENER CARDCODE

                #region DocumentoAsociado

                //var docasoc = this.ExisteDocumentoAsociado(obj.U_EXX_TIPO_TRANSACC, CompanyDB);


                //if (!docasoc.HasValue || docasoc.Value == 0)
                //{

                //    //var respuestadocasoc = GuardarDocumentoAsociado(obj.DocumentoAsociado, CompanyDB);
                //    //if (!respuestadocasoc.Success)
                //    //{
                //    respuestaPrincipal.Success = false;
                //    respuestaPrincipal.ErrCodigo = -001;
                //    respuestaPrincipal.ErrMensaje = $"Documento Asociado  {obj.U_EXX_TIPO_TRANSACC} no pudo ser creado.";
                //    return respuestaPrincipal;

                //    //}



                //}
                #endregion

                //VALIDAR SOCIO NEGOCIOS/PROVEEDOR
                #region SocioNegocios
                CatalogosManager catalogos = new CatalogosManager(ref servicio);
                BusinessPartnerViewModel bp = catalogos.GetCliente(obj.CardCode, CompanyDB);


                if (bp == null)
                {
                    respuestaPrincipal.Success = false;
                    respuestaPrincipal.ErrCodigo = -001;
                    respuestaPrincipal.ErrMensaje = $"Cliente con {obj.CardCode} no existe en SAP.";

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
                                respuestaPrincipal.Success = false;
                                respuestaPrincipal.ErrCodigo = -002;
                                respuestaPrincipal.ErrMensaje = $"Centro de costos [{i}] [{item.CostingCode}] no existe en SAP. ";

                                return respuestaPrincipal;

                            }
                        }
                    }
                }

                #endregion

                ////////GUARDAR ORDEN DE VENTA
                string _Method = "POST";//PATCH
                string _recurso = $"Orders";

                obj.GroupNumber = -1;
                obj.SalesPersonCode = -1;
                obj.RequriedDate = obj.DocDate;
                obj.CancelDate = obj.DocDate;
                obj.DocDueDate = obj.DocDate;

                foreach (var d in obj.DocumentLines)
                {
                    d.ShipDate = obj.DocDate;

                    //if (obj.U_EXX_DOC_GEN.Contains("NotaDebito"))
                    //{
                    //    d.TaxCode = "IVA_EXE";

                    //}
                    //else
                    //{
                    //    d.TaxCode = "IVA";

                    //}
                }
                //if (obj.DocEntry == 0)//CREACIÓN
                //{
                _Method = "POST";
                _Status = System.Net.HttpStatusCode.Created;


                //}
                //else //ACTUALIZACION
                //{
                //    _Method = "PATCH";
                //    _recurso += $"({obj.DocEntry})";
                //    _Status = System.Net.HttpStatusCode.NoContent;

                //}


                var _json = JsonConvert.SerializeObject(obj);

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

        public RespuestaGenerica GuardarOrdenVentaAtcontrans(OrdenVentaAtcontransGuardar obj, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(obj.CompanyDB);
            System.Net.HttpStatusCode _Status = System.Net.HttpStatusCode.Created;

            RespuestaGenerica respuestaPrincipal = new RespuestaGenerica();

            if (servicio.IsConected)
            { //OBTENER CARDCODE

                #region DocumentoAsociado

                //var docasoc = this.ExisteDocumentoAsociado(obj.U_EXX_TIPO_TRANSACC, CompanyDB);


                //if (!docasoc.HasValue || docasoc.Value == 0)
                //{

                //    //var respuestadocasoc = GuardarDocumentoAsociado(obj.DocumentoAsociado, CompanyDB);
                //    //if (!respuestadocasoc.Success)
                //    //{
                //    respuestaPrincipal.Success = false;
                //    respuestaPrincipal.ErrCodigo = -001;
                //    respuestaPrincipal.ErrMensaje = $"Documento Asociado  {obj.U_EXX_TIPO_TRANSACC} no pudo ser creado.";
                //    return respuestaPrincipal;

                //    //}



                //}
                #endregion

                //VALIDAR SOCIO NEGOCIOS/PROVEEDOR
                #region SocioNegocios
                CatalogosManager catalogos = new CatalogosManager(ref servicio);
                BusinessPartnerViewModel bp = catalogos.GetCliente(obj.CardCode, CompanyDB);


                if (bp == null)
                {
                    respuestaPrincipal.Success = false;
                    respuestaPrincipal.ErrCodigo = -001;
                    respuestaPrincipal.ErrMensaje = $"Cliente con {obj.CardCode} no existe en SAP.";

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
                                respuestaPrincipal.Success = false;
                                respuestaPrincipal.ErrCodigo = -002;
                                respuestaPrincipal.ErrMensaje = $"Centro de costos [{i}] [{item.CostingCode}] no existe en SAP. ";

                                return respuestaPrincipal;

                            }
                        }
                    }
                }

                #endregion

                ////////GUARDAR ORDEN DE VENTA
                string _Method = "POST";//PATCH
                string _recurso = $"Orders";

                obj.GroupNumber = -1;
                obj.SalesPersonCode = -1;
                obj.RequriedDate = obj.DocDate;
                obj.CancelDate = obj.DocDate;
                obj.DocDueDate = obj.DocDate;

                foreach (var d in obj.DocumentLines)
                {
                    d.ShipDate = obj.DocDate;
                }
               
                _Method = "POST";
                _Status = System.Net.HttpStatusCode.Created;



                var _json = JsonConvert.SerializeObject(obj);

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

        public RespuestaGenerica GuardarOrdenVentaRFS(OrdenVentaGuardarRFSViewModel obj, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(obj.CompanyDB);
            System.Net.HttpStatusCode _Status = System.Net.HttpStatusCode.Created;

            RespuestaGenerica respuestaPrincipal = new RespuestaGenerica();

            if (servicio.IsConected)
            { //OBTENER CARDCODE

                #region DocumentoAsociado

                //var docasoc = this.ExisteDocumentoAsociado(obj.U_EXX_TIPO_TRANSACC, CompanyDB);


                //if (!docasoc.HasValue || docasoc.Value == 0)
                //{

                //    //var respuestadocasoc = GuardarDocumentoAsociado(obj.DocumentoAsociado, CompanyDB);
                //    //if (!respuestadocasoc.Success)
                //    //{
                //    respuestaPrincipal.Success = false;
                //    respuestaPrincipal.ErrCodigo = -001;
                //    respuestaPrincipal.ErrMensaje = $"Documento Asociado  {obj.U_EXX_TIPO_TRANSACC} no pudo ser creado.";
                //    return respuestaPrincipal;

                //    //}



                //}
                #endregion

                //VALIDAR SOCIO NEGOCIOS/PROVEEDOR
                #region SocioNegocios
                CatalogosManager catalogos = new CatalogosManager(ref servicio);
                BusinessPartnerViewModel bp = catalogos.GetCliente(obj.CardCode, CompanyDB);


                if (bp == null)
                {
                    respuestaPrincipal.Success = false;
                    respuestaPrincipal.ErrCodigo = -001;
                    respuestaPrincipal.ErrMensaje = $"Cliente con {obj.CardCode} no existe en SAP.";

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
                                respuestaPrincipal.Success = false;
                                respuestaPrincipal.ErrCodigo = -002;
                                respuestaPrincipal.ErrMensaje = $"Centro de costos [{i}] [{item.CostingCode}] no existe en SAP. ";

                                return respuestaPrincipal;

                            }
                        }
                    }
                }

                #endregion

                ////////GUARDAR ORDEN DE VENTA
                string _Method = "POST";//PATCH
                string _recurso = $"Orders";

                obj.GroupNumber = -1;
                obj.SalesPersonCode = -1;
                obj.RequriedDate = obj.DocDate;
                obj.CancelDate = obj.DocDate;
                obj.DocDueDate = obj.DocDate;

                foreach (var d in obj.DocumentLines)
                {
                    d.ShipDate = obj.DocDate;
                }

                _Method = "POST";
                _Status = System.Net.HttpStatusCode.Created;



                var _json = JsonConvert.SerializeObject(obj);

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


        public RespuestaGenerica GuardarDocumentoAsociado(DocumentoAsociadoGuardar obj, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB/*obj.CompanyDB*/);
            System.Net.HttpStatusCode _Status = System.Net.HttpStatusCode.Created;

            RespuestaGenerica respuestaPrincipal = new RespuestaGenerica();

            if (servicio.IsConected)
            {
                var existe = this.ExisteDocumentoAsociado(obj.Code, CompanyDB);
                if (existe.HasValue)
                {
                    obj.DocEntry = (long)existe.Value;
                    //this.errMsg = $"Documento Asociado ya existe.";
                    //respuestaPrincipal.Success = false;
                    //respuestaPrincipal.ErrMensaje = this.errMsg;
                    //return respuestaPrincipal;
                }
            

                CatalogosManager catalogos = new CatalogosManager(ref servicio);
                if (!string.IsNullOrEmpty(obj.U_EXX_NAVE))
                {
                    TablaGenericaSapViewModel nave = new TablaGenericaSapViewModel();
                    var resp = catalogos.GetTableGenerica(obj.U_EXX_NAVE, CompanyDB, "EXX_NAVES");
                    if (resp.Success)
                    {
                        nave = JsonConvert.DeserializeObject<TablaGenericaSapViewModel>(resp.RespuestaJson);
                    }
                    else
                    {
                        TablaGenericaGuardar tabla = new TablaGenericaGuardar();
                        tabla.Code = obj.U_EXX_NAVE;
                        tabla.Name = obj.NAVE_NOMBRE;
                        var resp_table = catalogos.GuardarObjetoTablaGenerica(tabla, CompanyDB, "EXX_NAVES");
                        if (!resp_table.Success)
                        {
                            nave = JsonConvert.DeserializeObject<TablaGenericaSapViewModel>(resp.RespuestaJson);

                            this.errMsg = $"Error al crear EXX_NAVE: {obj.U_EXX_NAVE}";
                            respuestaPrincipal.Success = false;
                            respuestaPrincipal.ErrMensaje = this.errMsg;
                            return respuestaPrincipal;

                        }
                    }

                }
                if (!string.IsNullOrEmpty(obj.U_EXX_PORI))
                {
                    TablaGenericaSapViewModel nave = new TablaGenericaSapViewModel();
                    var resp = catalogos.GetTableGenerica(obj.U_EXX_PORI, CompanyDB, "EXX_PUERTOS");
                    if (resp.Success)
                    {
                        nave = JsonConvert.DeserializeObject<TablaGenericaSapViewModel>(resp.RespuestaJson);
                    }
                    else
                    {
                        TablaGenericaGuardar tabla = new TablaGenericaGuardar();
                        tabla.Code = obj.U_EXX_PORI;
                        tabla.Name = obj.PUERTOORI_NOMBRE;
                        var resp_table = catalogos.GuardarObjetoTablaGenerica(tabla, CompanyDB, "EXX_PUERTOS");
                        if (!resp_table.Success)
                        {
                            //nave = JsonConvert.DeserializeObject<TablaGenericaGuardar>(resp.RespuestaJson);

                            this.errMsg = $"Error al crear EXX_PUERTOS ORIGEN: {obj.U_EXX_PORI}";
                            respuestaPrincipal.Success = false;
                            respuestaPrincipal.ErrMensaje = this.errMsg;
                            return respuestaPrincipal;

                        }
                    }

                }
                if (!string.IsNullOrEmpty(obj.U_EXX_PDEST))
                {
                    TablaGenericaSapViewModel nave = new TablaGenericaSapViewModel();
                    var resp = catalogos.GetTableGenerica(obj.U_EXX_PDEST, CompanyDB, "EXX_PUERTOS");
                    if (resp.Success)
                    {
                        nave = JsonConvert.DeserializeObject<TablaGenericaSapViewModel>(resp.RespuestaJson);
                    }
                    else
                    {
                        TablaGenericaGuardar tabla = new TablaGenericaGuardar();
                        tabla.Code = obj.U_EXX_PDEST;
                        tabla.Name = obj.PUERTODES_NOMBRE;
                        var resp_table = catalogos.GuardarObjetoTablaGenerica(tabla, CompanyDB, "EXX_PUERTOS");
                        if (!resp_table.Success)
                        {
                            nave = JsonConvert.DeserializeObject<TablaGenericaSapViewModel>(resp.RespuestaJson);
                            string errorMessage = ObtenerMensajeError(resp_table.ErrMensaje);

                            this.errMsg = $"Error al crear EXX_PUERTOS DESTINO: {obj.U_EXX_PDEST} -" + nave.error.message.value + "-" + errorMessage;
                            respuestaPrincipal.Success = false;
                            respuestaPrincipal.ErrMensaje = this.errMsg;
                            return respuestaPrincipal;
                        }
                    }
                }


                ////////GUARDAR ORDEN DE VENTA
                string _Method = "POST";//PATCH
                string _recurso = $"EXX_TIPO_TRANSACCION";

                if (obj.DocEntry == 0)//CREACIÓN
                {
                    _Method = "POST";
                    _Status = System.Net.HttpStatusCode.Created;
                }
                else //ACTUALIZACION
                {
                    _Method = "PATCH";
                    _recurso += $"('{obj.Code}')";
                    _Status = System.Net.HttpStatusCode.NoContent;

                }

                var docasoc = new DocumentoAsociadoSave()
                {
                    DocEntry = obj.DocEntry,
                    Code = obj.Code, //Códido del documento asociado campo clave con el UDO Tipo Transacción.
                    Name = obj.Name, //Nombre asignado al documento            
                    U_EXX_CAMCLAVE = obj.U_EXX_CAMCLAVE,
                    U_EXX_TDOC = obj.U_EXX_TDOC, //***Tipo de documento RA,BL,PO,RO  
                    U_EXX_FEC_EMI = obj.U_EXX_FEC_EMI, //Fecha de emision del documento asociado BL,RA,PO,RO
                    U_EXX_OBSERV = obj.U_EXX_OBSERV,
                    U_EXX_LINEA = obj.U_EXX_LINEA,
                    U_EXX_DES_LIN = obj.U_EXX_DES_LIN, //Nombre de la linea naviera
                    U_EXX_NAVE = obj.U_EXX_NAVE.Trim(), //*** Siglas del barco, OTROS CODIGO DE NAVE FUSIO|ORCA1|PAOLA|WDL VER MAESTRO DE NAVES /
                    U_EXX_VIAJE = obj.U_EXX_VIAJE, // Numero del viaje
                    U_EXX_SEM = obj.U_EXX_SEM.ToString().PadLeft(2,'0'), //Indica el numero de la semana  del año que zarpe el buque, ejemplo 10
                    U_EXX_NUM_GUIA = "",//Numero de Guia de remisión o nota de embarque se usa en proyecto Galapagos
                    U_EXX_TO_FLE = obj.U_EXX_TO_FLE, //Total Flete 
                    U_EXX_SER_LOG = obj.U_EXX_SER_LOG,//40% del Flete                   
                    U_EXX_COS_TOT = obj.U_EXX_COS_TOT,//Total de costos locales se usará en naviera y comercial
                    U_EXX_TO_AG = obj.U_EXX_TO_AG,//Total de valores de agente se usará en el flujo comercial
                    U_EXX_TO_OPRO = obj.U_EXX_TO_OPRO,//Total de valores de comisiones a tercero se usará en el flujo comercial
                    U_EXX_TO_PRO = obj.U_EXX_TO_PRO,//Total de valores de otras liquidaciones se usará en el flujo comercial              
                    U_EXX_COD_CLI = obj.U_EXX_COD_CLI, // Enviamos el RUC del cliente grabar en Sap con su respectivo Código
                    U_EXX_NOMB_CLI = obj.U_EXX_NOMB_CLI,
                    U_EXX_COD_AG = obj.U_EXX_COD_AG, //RUC de un proveedor tipo agente se usará en el flujo comercial,Enviamos el RUC del proveedor grabar en Sap con su respectivo Código
                    U_EXX_DES_AG = obj.U_EXX_DES_AG, // Razón social del proveedor se usará en el flujo comercial
                    U_EXX_CVEND = obj.U_EXX_CVEND, //RUC del Vendedor se usará en el flujo comercial
                    U_EXX_DVEND = obj.U_EXX_DVEND, //Nombre del Vendedor se usará en el flujo comercial
                    U_EXX_EMB = obj.U_EXX_EMB,// "",//Razón social del Embarcador 
                    U_EXX_CONS = "",//Razón social del Embarcador 
                    U_EXX_NUM_COT = obj.U_EXX_NUM_COT, //Código secuencial indica cual es numero de cotización se usará en el flujo comercial CT-00000021-17
                    U_EXX_ROPOPA = obj.U_EXX_ROPOPA, //Debe ser el mismo del campo Código.
                    U_EXX_MBL = obj.U_EXX_MBL,//Numero de MBL (Master BL) asociado al BL
                    U_EXX_HBL = obj.U_EXX_HBL,//Numero de HBL (Bl Hijo) asociado al BL
                    U_EXX_TIP_OP = obj.U_EXX_TIP_OP,//***I: Import E: Export
                    U_EXX_TIP_EMB = "M",//***M: Maritimo A: Aéreo
                    U_EXX_PORI = obj.U_EXX_PORI,
                    U_EXX_PDEST = obj.U_EXX_PDEST,
                    U_EXX_BOOK = "",//Campo alfanumerico opcional
                    U_EXX_CON_CAR = "NA", //*** NA: No aplica, Campo indica la condicion de la carga: FCL/FCL | FCL/LCL | LCL/FCL | VACIO                         
                    U_EXX_TIP_CONT = "NA", //Indica las siglas del tipo de contenedor Eje: 40DV, 40RF
                    U_EXX_FE_INI = obj.U_EXX_FE_INI,
                    U_EXX_FE_FIN = obj.U_EXX_FE_FIN,// Desconozco funcionalidad
                    U_EXX_TI_ORD = obj.U_EXX_TI_ORD,//***F: Full V:Vacios  
                    U_EXX_MAQ = "",// Nombre de la maquina uso en RFS
                    U_EXX_CONT = obj.U_EXX_CONT,//NUMERO CONTENEDORES
                };


                var _json = JsonConvert.SerializeObject(docasoc);

                RespuestaGenerica respuesta = servicio.SLSendRequestReturnResponse(_recurso, _Method, _json, _Status, false);

                if (respuesta.Success)
                {
                    this.errMsg = "Creación Exitosa";
                    if(_Method == "PATCH")
                    {
                        respuesta.RespuestaJson = _json;
                    }
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


        public RespuestaGenerica CancelarOrden(OrdenVentaCancelar obj, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);
            System.Net.HttpStatusCode _Status = System.Net.HttpStatusCode.Created;


            RespuestaGenerica respuestaPrincipal = new RespuestaGenerica();

            if (servicio.IsConected)
            { //OBTENER CARDCODE



                string _Method = "POST";

                string _recurso = $"Orders({obj.DocEntry})/Cancel";

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

        public RespuestaGenerica CanceledOrderByDocEntry(string CompanyDB, long DocEntry)
        {
            string mensaje = string.Empty;
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            string _Method = "POST";

            string _recurso = $"Orders({DocEntry})/Cancel";
            System.Net.HttpStatusCode _Status = System.Net.HttpStatusCode.NoContent;
            var respuesta = servicio.SLSendRequestReturnResponse(_recurso, _Method, null, _Status, false);

            if (respuesta.Success)
            {
                mensaje += $">> Orden de venta DocEntry: {DocEntry} fue CANCELADO  en SAP Exitosamente." + Environment.NewLine;
                this.errMsg = "Ok";

               
                respuesta.Success = true;
                respuesta.ErrMensaje = mensaje;
                respuesta.RespuestaJson = "";//JsonConvert.SerializeObject(clientes);

                return respuesta;
            }
            else
            {
                this.errMsg = $">> Error Cancelación DocEntry:{DocEntry} Orden Venta, {respuesta.ErrCodigo}-{respuesta.ErrException}-{respuesta.ErrMensaje}";
                mensaje += this.errMsg;

                if (mensaje.Contains("-5002"))
                {
                    this.errMsg = $">> Error Cancelación DocEntry:{DocEntry} de Orden Venta, Orden de Venta ya tiene una Factura o Nota de Débito asociada";
                   
                }
               
                respuesta.Success = false;
                respuesta.ErrMensaje = this.errMsg;
                respuesta.RespuestaJson = "";

                return respuesta;
            }
        }

        public string CancelarOrdenPorDocAsociado(DocumentoAsociadoAnulacion  DocumentoAsociado)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(DocumentoAsociado.CompanyDB);
            System.Net.HttpStatusCode _Status = System.Net.HttpStatusCode.Created;
            string mensaje = "";


            RespuestaGenerica respuestaPrincipal = new RespuestaGenerica();


            if (servicio.IsConected)
            {
                //Valida que no existan facturas o notas de debito asociadas al documento asociado. (BL)

                List<DocumentosAsociadosSapVtaViewModel> docsasoc = this.GetDocumentosAsociados(DocumentoAsociado.Code, DocumentoAsociado.CompanyDB);

                var res = docsasoc.Where(p => p.CancelStatus == "NO").ToList();

                if (res.Count() > 0)
                {
                    this.errMsg = $"Error!! No se pueden Cancelar las ordenes de Venta asociado al Documento {DocumentoAsociado.Code} porque ya tiene Facturas de Venta, Notas debito/Notas de Credito Emitidas.";
                    mensaje += this.errMsg;
                    return mensaje;
                }

                List<DocumentosAsociadosSapVtaViewModel> ordenesacancelar = this.GetOrdenesPorDocAsociados(DocumentoAsociado.Code, DocumentoAsociado.CompanyDB, "");

                if (ordenesacancelar.Count() > 0)
                {
                    var ordenescerradas = ordenesacancelar.Where(p => p.DocumentStatus.Contains("Cerrad") && p.CancelStatus == "NO").ToList();
                    var ordenesparacancel = ordenesacancelar.Where(p => p.DocumentStatus.Contains("Abier") && p.CancelStatus == "NO").ToList();


                    if (ordenescerradas.Count() > 0)
                    {
                        this.errMsg = $"Error!! No se pueden Cancelar las ordenes CERRADAS asociado al Documento {DocumentoAsociado.Code} porque ya tiene Facturas de Venta, Notas debito/Notas de Credito Emitidas.";
                        mensaje += this.errMsg;
                        return mensaje;
                    }

                    string _Method = "POST";
                    string _recurso = "";
                    foreach (var orden in ordenesparacancel)
                    {
                        if (orden.CancelStatus.Contains("N"))
                        {
                            if (!servicio.IsConected)
                                servicio.ConectarSL(DocumentoAsociado.CompanyDB);

                            _recurso = $"Orders({orden.DocEntry})/Cancel";
                            _Status = System.Net.HttpStatusCode.NoContent;
                            var respuesta = servicio.SLSendRequestReturnResponse(_recurso, _Method, null, _Status, false);

                            if (respuesta.Success)
                            {
                                mensaje += $">> Orden de venta DocEntry: {orden.DocEntry} DocNum: {orden.DocNum} fue CANCELADO  del {DocumentoAsociado.Code} en SAP Exitosamente." + Environment.NewLine;
                                this.errMsg = "Ok";


                            }
                            else
                            {
                                this.errMsg = $">> Error Cancelación {DocumentoAsociado.Code} Orden Venta: {respuesta.ErrCodigo}-{respuesta.ErrException}-{respuesta.ErrMensaje}";
                                mensaje += this.errMsg;
                            }
                        }
                        else
                        {
                            this.errMsg = $" >> No existen órdenes Venta para este documento asociado {DocumentoAsociado.Code}.";
                            mensaje += this.errMsg;
                        }
                    }
                }
                else
                {
                    this.errMsg = $" >> No existen órdenes Venta para este documento asociado {DocumentoAsociado.Code}.";
                    mensaje += this.errMsg;
                }
            }
            else
            {
                this.errMsg = $"Error: No se pudo establecer conexión con SSL";

            }

            this.errMsg = mensaje;
            return mensaje;
        }

        


        /// <summary>
        /// Obtiene las facturas y notas de crédito asociadas al BL
        /// </summary>
        /// <param name="DocumentoAsociado"></param>
        /// <param name="CompanyDB"></param>
        /// <returns></returns>
        public List<DocumentosAsociadosSapVtaViewModel> GetDocumentosAsociados(string DocumentoAsociado, string CompanyDB)
        {

            RespuestaGenerica respuesta = new RespuestaGenerica();
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            List<DocumentosAsociadosSapVtaViewModel> objgroup = new List<DocumentosAsociadosSapVtaViewModel>();
            List<DocumentosAsociadosSapVtaViewModel> obj = new List<DocumentosAsociadosSapVtaViewModel>();
            if (servicio.IsConected)
            {
                //Obtener los datos de facturas y notas de debito
                string _recurso = $"Invoices?$select=DocEntry,DocNum,DocType,DocumentSubType,DocDate,CardCode,CardName,DocTotal,Comments,CancelStatus,DocumentStatus,DocumentStatus,U_EXX_TIPO_TRANSACC,U_EXX_DOC_GEN&$filter=U_EXX_TIPO_TRANSACC eq '{DocumentoAsociado}'";
                respuesta = servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);

                if (respuesta.Success)
                {
                    DocumentosAsociadosVta docu = JsonConvert.DeserializeObject<DocumentosAsociadosVta>(respuesta.RespuestaJson);
                    if (docu != null)
                    {
                        if (docu.value != null)
                        {
                            foreach (var d in docu.value)
                            {
                                d.DocumentSubType = d.DocumentSubType == "bod_None" ? "FacturaVenta" : "NotaDebito";
                                d.CancelStatus = d.CancelStatus == "csNo" ? "NO" : "SI";
                                d.DocumentStatus = d.DocumentStatus == "bost_Close" ? "Cerrado" : "Abierto";
                            }
                            objgroup.AddRange(docu.value);
                        }
                    }
                }

                //Obtener los datos de Notas de crédito
                _recurso = $"CreditNotes?$select=DocEntry,DocNum,DocType,DocumentSubType,DocDate,CardCode,CardName,DocTotal,Comments,CancelStatus,DocumentStatus,U_EXX_TIPO_TRANSACC,U_EXX_DOC_GEN&$filter=U_EXX_TIPO_TRANSACC eq '{DocumentoAsociado}'";
                respuesta = servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);

                if (respuesta.Success)
                {

                    DocumentosAsociadosVta docu = JsonConvert.DeserializeObject<DocumentosAsociadosVta>(respuesta.RespuestaJson);
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


  
        public RespuestaGenerica GetDescuentosEspecialesClientes(string ruc, string CompanyDB)
        {
            RespuestaGenerica respuesta = new RespuestaGenerica();
            HanaCommand comm = null;
            HanaDataReader reader = null;
            List<ClienteDescuento> clientes = new List<ClienteDescuento>();
            ClienteDescuento cliente = new ClienteDescuento();

            

            try
            {
                conectarHana(CompanyDB);
            

                string StrQty = "SELECT T0.* FROM  " + CompanyDB + ".ViewDescuentosEspecialesClientes  T0 ";
                StrQty += @" WHERE T0.""Ruc"" like '%" + ruc.Trim() + "%'";

                comm = new HanaCommand(StrQty, Connection);

                reader = comm.ExecuteReader();

                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            cliente = new ClienteDescuento();
                            cliente.ItemCode = Convert.ToString(reader.GetValue(0));
                            cliente.CardCode = Convert.ToString(reader.GetValue(1));
                            cliente.Price = Convert.ToDecimal( reader.GetValue(2).ToString());
                            cliente.Currency = Convert.ToString(reader.GetValue(3));
                            cliente.Ruc = Convert.ToString(reader.GetValue(4));
                            cliente.DiscountPercent = Convert.ToDecimal(reader.GetValue(5).ToString());


                            clientes.Add(cliente);
                        }
                    }
                }

                this.errMsg = $"OK";
                respuesta.Success = true;
                respuesta.ErrMensaje = this.errMsg;
                respuesta.RespuestaJson = JsonConvert.SerializeObject(clientes);

                return respuesta;

            }
            catch (Exception ex)
            {

                this.errMsg = $"Error: No se pudo establecer conexión con SSL";
                respuesta.Success = false;
                respuesta.ErrMensaje = this.errMsg;
                
            }
            finally
            {
               LiberarVariables(ref Connection, ref comm, ref reader);
            }

            return respuesta;
        }

        public long? ExisteDocumentoAsociado(string DocumentoAsociado, string CompanyDB)
        {

            RespuestaGenerica respuesta = new RespuestaGenerica();
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            List<DocumentosAsociadosSapVtaViewModel> objgroup = new List<DocumentosAsociadosSapVtaViewModel>();
            List<DocumentosAsociadosSapVtaViewModel> obj = new List<DocumentosAsociadosSapVtaViewModel>();
            if (servicio.IsConected)
            {
                //Obtener los datos de facturas y notas de debito
                string _recurso = $"EXX_TIPO_TRANSACCION('{DocumentoAsociado}')";
                respuesta = servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);

                if (respuesta.Success)
                {
                    DocumentoAsociadoSave docu = JsonConvert.DeserializeObject<DocumentoAsociadoSave>(respuesta.RespuestaJson);
                    if (docu != null)
                    {
                        if (docu.Code != null)
                        {
                            return docu.DocEntry;
                        }
                        else
                            return 0;
                    }
                }
                else
                {
                    this.errMsg = respuesta.ErrMensaje;

                    if (this.errMsg.ToLower().Contains("no matching "))
                    {
                        return 0;
                    }
                    //var respuestagn= JsonConvert.DeserializeObject<RespuestaGenerica>(respuesta.ErrMensaje); 


                }

               
            }
            return null;
        }
        /// <summary>
        /// Ob
        /// </summary>
        /// <param name="DocumentoAsociado"></param>
        /// <param name="CompanyDB"></param>
        /// <param name="TipoDocumentoGenerado"></param>
        /// <returns></returns>
        public List<DocumentosAsociadosSapVtaViewModel> GetOrdenesPorDocAsociados(string DocumentoAsociado, string CompanyDB, string TipoDocumentoGenerado)
        {

            RespuestaGenerica respuesta = new RespuestaGenerica();
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            List<DocumentosAsociadosSapVtaViewModel> objgroup = new List<DocumentosAsociadosSapVtaViewModel>();
            List<DocumentosAsociadosSapVtaViewModel> obj = new List<DocumentosAsociadosSapVtaViewModel>();
            if (servicio.IsConected)
            {
                //Obtener los datos de facturas y notas de debito
                var tiposdoc = TipoDocumentoGenerado.Split('|');
                foreach (var t in tiposdoc)
                {
                    TipoDocumentoGenerado += $"'{t}',";
                }


                string filtertipodoc =string.IsNullOrEmpty(TipoDocumentoGenerado)?"": $"&U_EXX_DOC_GEN='{TipoDocumentoGenerado}'";
                string _recurso = $"Orders?$select=DocEntry,DocNum,DocType,DocumentSubType,DocDate,CardCode,CardName,DocTotal,Comments,CancelStatus,DocumentStatus,U_EXX_TIPO_TRANSACC,U_EXX_DOC_GEN&$filter=U_EXX_TIPO_TRANSACC eq '{DocumentoAsociado}'{filtertipodoc}";
                respuesta = servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);

                if (respuesta.Success)
                {
                    DocumentosAsociadosVta docu = JsonConvert.DeserializeObject<DocumentosAsociadosVta>(respuesta.RespuestaJson);
                    if (docu != null)
                    {
                        if (docu.value != null)
                        {
                            foreach (var d in docu.value)
                            {
                                d.DocumentSubType = d.DocumentSubType == "bod_None" ? "OrdenVenta" : d.DocumentSubType;
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


        public string CancelarOrdenPorDocAsociadoyTipoMovimiento(DocumentoAsociadoAnulacionPorTipoMov DocumentoAsociado)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(DocumentoAsociado.CompanyDB);
            System.Net.HttpStatusCode _Status = System.Net.HttpStatusCode.Created;
            string mensaje = "";


            RespuestaGenerica respuestaPrincipal = new RespuestaGenerica();


            if (servicio.IsConected)
            {
                //Valida que no existan facturas o notas de debito asociadas al documento asociado. (BL)

                List<DocumentosAsociadosSapVtaViewModel> docsasoc = this.GetDocumentosAsociadosPorTipoMovimiento(DocumentoAsociado.CodeBL, DocumentoAsociado.CompanyDB, DocumentoAsociado.DocGen);

                var res = docsasoc.Where(p => p.CancelStatus == "NO").ToList();

                if (res.Count() > 0)
                {
                    this.errMsg = $"Error!! No se pueden Cancelar las ordenes de Venta asociado al Documento {DocumentoAsociado.CodeBL} de {DocumentoAsociado.DocGen} porque ya tiene Facturas de Venta, Notas debito/Notas de Credito Emitidas.";
                    mensaje += this.errMsg;
                    return mensaje;
                }

                List<DocumentosAsociadosSapVtaViewModel> ordenesacancelar = this.GetDocumentosAsociadosPorTipoMovimiento(DocumentoAsociado.CodeBL, DocumentoAsociado.CompanyDB, DocumentoAsociado.DocGen);

                if (ordenesacancelar.Count() > 0)
                {
                    var ordenescerradas = ordenesacancelar.Where(p => p.DocumentStatus.Contains("Cerrad") && p.CancelStatus == "NO").ToList();
                    var ordenesparacancel = ordenesacancelar.Where(p => p.DocumentStatus.Contains("Abier") && p.CancelStatus == "NO").ToList();


                    if (ordenescerradas.Count() > 0)
                    {
                        this.errMsg = $"Error!! No se pueden Cancelar las ordenes CERRADAS asociado al Documento {DocumentoAsociado.CodeBL} de {DocumentoAsociado.DocGen} porque ya tiene Facturas de Venta, Notas debito/Notas de Credito Emitidas.";
                        mensaje += this.errMsg;
                        return mensaje;
                    }

                    string _Method = "POST";
                    string _recurso = "";
                    foreach (var orden in ordenesparacancel)
                    {
                        if (orden.CancelStatus.Contains("N"))
                        {
                            if (!servicio.IsConected)
                                servicio.ConectarSL(DocumentoAsociado.CompanyDB);

                            _recurso = $"Orders({orden.DocEntry})/Cancel";
                            _Status = System.Net.HttpStatusCode.NoContent;
                            var respuesta = servicio.SLSendRequestReturnResponse(_recurso, _Method, null, _Status, false);

                            if (respuesta.Success)
                            {
                                mensaje += $">> Orden de venta DocEntry: {orden.DocEntry} DocNum: {orden.DocNum} fue CANCELADO  del {DocumentoAsociado.CodeBL} en SAP Exitosamente." + Environment.NewLine;
                                this.errMsg = "Ok";


                            }
                            else
                            {
                                this.errMsg = $">> Error Cancelación {DocumentoAsociado.CodeBL} de {DocumentoAsociado.DocGen} Orden Venta: {respuesta.ErrCodigo}-{respuesta.ErrException}-{respuesta.ErrMensaje}";
                                mensaje += this.errMsg;
                            }
                        }
                        else
                        {
                            this.errMsg = $" >> No existen órdenes Venta para este documento asociado {DocumentoAsociado.CodeBL} de {DocumentoAsociado.DocGen}. ";
                            mensaje += this.errMsg;
                        }
                    }
                }
                else
                {
                    this.errMsg = $" >> No existen órdenes Venta para este documento asociado {DocumentoAsociado.CodeBL} de {DocumentoAsociado.DocGen}.";
                    mensaje += this.errMsg;
                }
            }
            else
            {
                this.errMsg = $"Error: No se pudo establecer conexión con SSL";

            }

            this.errMsg = mensaje;
            return mensaje;
        }


        public List<DocumentosAsociadosSapVtaViewModel> GetOrdenesPorDocAsociadosPorTipoMovimiento(string DocumentoAsociado, string CompanyDB, string TipoDocumentoGenerado)
        {

            RespuestaGenerica respuesta = new RespuestaGenerica();
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            List<DocumentosAsociadosSapVtaViewModel> objgroup = new List<DocumentosAsociadosSapVtaViewModel>();
            List<DocumentosAsociadosSapVtaViewModel> obj = new List<DocumentosAsociadosSapVtaViewModel>();
            if (servicio.IsConected)
            {
                //Obtener los datos de facturas y notas de debito
                //var tiposdoc = TipoDocumentoGenerado.Split('|');
                //foreach (var t in tiposdoc)
                //{
                //    TipoDocumentoGenerado += $"'{t}',";
                //}


                string filtertipodoc = string.IsNullOrEmpty(TipoDocumentoGenerado) ? "" : $"&U_EXX_DOC_GEN='{TipoDocumentoGenerado}'";
                string _recurso = $"Orders?$select=DocEntry,DocNum,DocType,DocumentSubType,DocDate,CardCode,CardName,DocTotal,Comments,CancelStatus,DocumentStatus,U_EXX_TIPO_TRANSACC,U_EXX_DOC_GEN&$filter=U_EXX_TIPO_TRANSACC eq '{DocumentoAsociado} and U_EXX_DOC_GEN eq  '{TipoDocumentoGenerado}' '{filtertipodoc}";
                respuesta = servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);

                if (respuesta.Success)
                {
                    DocumentosAsociadosVta docu = JsonConvert.DeserializeObject<DocumentosAsociadosVta>(respuesta.RespuestaJson);
                    if (docu != null)
                    {
                        if (docu.value != null)
                        {
                            foreach (var d in docu.value)
                            {
                                d.DocumentSubType = d.DocumentSubType == "bod_None" ? "OrdenVenta" : d.DocumentSubType;
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

        public List<DocumentosAsociadosSapVtaViewModel> GetDocumentosAsociadosPorTipoMovimiento(string DocumentoAsociado, string CompanyDB, string TipoMovimiento)
        {

            RespuestaGenerica respuesta = new RespuestaGenerica();
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            List<DocumentosAsociadosSapVtaViewModel> objgroup = new List<DocumentosAsociadosSapVtaViewModel>();
            List<DocumentosAsociadosSapVtaViewModel> obj = new List<DocumentosAsociadosSapVtaViewModel>();
            if (servicio.IsConected)
            {
                //Obtener los datos de facturas y notas de debito
                string _recurso = $"Invoices?$select=DocEntry,DocNum,DocType,DocumentSubType,DocDate,CardCode,CardName,DocTotal,Comments,CancelStatus,DocumentStatus,DocumentStatus,U_EXX_TIPO_TRANSACC,U_EXX_DOC_GEN&$filter=U_EXX_TIPO_TRANSACC eq '{DocumentoAsociado}' and U_EXX_DOC_GEN eq '{TipoMovimiento}' ";
                respuesta = servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);

                if (respuesta.Success)
                {
                    DocumentosAsociadosVta docu = JsonConvert.DeserializeObject<DocumentosAsociadosVta>(respuesta.RespuestaJson);
                    if (docu != null)
                    {
                        if (docu.value != null)
                        {
                            foreach (var d in docu.value)
                            {
                                d.DocumentSubType = d.DocumentSubType == "bod_None" ? "FacturaVenta" : "NotaDebito";
                                d.CancelStatus = d.CancelStatus == "csNo" ? "NO" : "SI";
                                d.DocumentStatus = d.DocumentStatus == "bost_Close" ? "Cerrado" : "Abierto";
                            }
                            objgroup.AddRange(docu.value);
                        }
                    }
                }

                //Obtener los datos de Notas de crédito
                _recurso = $"CreditNotes?$select=DocEntry,DocNum,DocType,DocumentSubType,DocDate,CardCode,CardName,DocTotal,Comments,CancelStatus,DocumentStatus,U_EXX_TIPO_TRANSACC,U_EXX_DOC_GEN&$filter=U_EXX_TIPO_TRANSACC eq '{DocumentoAsociado}' and U_EXX_DOC_GEN eq '{TipoMovimiento}'";
                respuesta = servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);

                if (respuesta.Success)
                {

                    DocumentosAsociadosVta docu = JsonConvert.DeserializeObject<DocumentosAsociadosVta>(respuesta.RespuestaJson);
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


        public List<OrdenesFacturadas> GetOrdenesVentaFacturadas( string CompanyDB, string Ids)
        {
            HanaCommand comm = null;
            HanaDataReader reader = null;
            List<OrdenesFacturadas> ordenes = new List<OrdenesFacturadas>();
            OrdenesFacturadas orden = new OrdenesFacturadas();
            try
            {
                conectarHana(CompanyDB);
                string StrQty = "SELECT * FROM  \"" + CompanyDB + "\".\"CTK_GET_PEDIDOS_FACTURADOS_VIEW\" ";
                StrQty += @" WHERE ""SapPedidoDocEntry"" in ("  + Ids + ")";
                
                comm = new HanaCommand(StrQty, Connection);

                reader = comm.ExecuteReader();

                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {

                            orden = new OrdenesFacturadas();
                            orden.SapPedidoDocEntry = Convert.ToInt64(reader.GetValue(0));
                            orden.SapFacturaDocEntry = Convert.ToInt64(reader.GetValue(1));
                            orden.SapFacturaDocNum = Convert.ToInt64(reader.GetValue(2));
                            orden.SapNumeroFactura = reader.GetValue(4).ToString();
                            orden.SapFacturaDocDate = Convert.ToDateTime(reader.GetValue(3));
                            orden.SapFacturaValor = Convert.ToDecimal(reader.GetValue(5));


                            ordenes.Add(orden);

                        }
                    }
                }



            }
            catch (Exception ex)
            {
                ErrorMensaje = ex.Message;
            }
            finally
            {


                LiberarVariables(ref Connection, ref comm, ref reader);
            }

            return ordenes;
        }


        public async Task<List<OrdenesFacturadas>> GetOrdenesVentaFacturadasAsync(string CompanyDB, string Ids)
        {
            HanaCommand comm = null;
            HanaDataReader reader = null;
            List<OrdenesFacturadas> ordenes = new List<OrdenesFacturadas>();
            OrdenesFacturadas orden = new OrdenesFacturadas();
            try
            {
                conectarHana(CompanyDB);
                string StrQty = "SELECT * FROM  \"" + CompanyDB + "\".\"CTK_GET_PEDIDOS_FACTURADOS_VIEW\" ";
                StrQty += @" WHERE ""SapPedidoDocEntry"" in (" + Ids + ")";

                comm = new HanaCommand(StrQty, Connection);

                reader = await comm.ExecuteReaderAsync();

                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {

                            orden = new OrdenesFacturadas();
                            orden.SapPedidoDocEntry = Convert.ToInt64(reader.GetValue(0));
                            orden.SapFacturaDocEntry = Convert.ToInt64(reader.GetValue(1));
                            orden.SapFacturaDocNum = Convert.ToInt64(reader.GetValue(2));
                            orden.SapNumeroFactura = reader.GetValue(4).ToString();
                            orden.SapFacturaDocDate = Convert.ToDateTime(reader.GetValue(3));
                           //orden.SapFacturaValor = Convert.ToDecimal(reader.GetValue(5));


                            ordenes.Add(orden);

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorMensaje = ex.Message;
            }
            finally
            {


                LiberarVariables(ref Connection, ref comm, ref reader);
            }

            return ordenes;
        }

        public string ObtenerMensajeError(string respuestaJson)
        {
            // Parseamos la respuesta JSON
            var obj = JObject.Parse(respuestaJson);

            // Accedemos al valor de "value" dentro del objeto "message"
            string errorMessage = obj["error"]?["message"]?["value"]?.ToString();

            return errorMessage;
        }


    }



}