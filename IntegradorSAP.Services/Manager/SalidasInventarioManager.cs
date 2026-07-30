using Newtonsoft.Json;
//using Sap.Data.Hana;
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
using System.Web.Http;

namespace IntegradorSAP.Services.Manager
{
    public class SalidasInventarioManager
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

       

        public SalidasInventarioViewModel Get(int DocEntry, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            if (servicio.IsConected)
            {

                string _recurso = $"InventoryGenExits({DocEntry})";
                var respuesta = servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);
                SalidasInventarioViewModel obj = JsonConvert.DeserializeObject<SalidasInventarioViewModel>(respuesta.RespuestaJson);


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


        public RespuestaGenerica Guardar(SalidasInventarioGuardarViewModel obj, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(obj.CompanyDB);
            System.Net.HttpStatusCode _Status = System.Net.HttpStatusCode.Created;

            RespuestaGenerica respuestaPrincipal = new RespuestaGenerica();

            if (servicio.IsConected)
            {
                CatalogosManager catalogos = new CatalogosManager(ref servicio);

                #region Valida Contenedor, Items y Centros de costos
                var contenedor = catalogos.GetUDO(obj.U_EXX_CONTENEDOR, CompanyDB, "EXX_CONTENEDOR");

                if (contenedor == null)
                {
                    var result = catalogos.GuardarContenedor(obj.ContenedorData, CompanyDB);


                    if (!result.Success)
                    {
                        //Error
                        return result;
                    }
                }

               

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

                
                string _Method = "POST";//PATCH
                string _recurso = $"InventoryGenExits";// Salida de Inventario SAP
                _Status = System.Net.HttpStatusCode.Created;

                obj.GroupNumber = -1;
                obj.SalesPersonCode = -1;
                obj.RequriedDate = obj.DocDate;
                obj.CancelDate = obj.DocDate;
                obj.DocDueDate = obj.DocDate;


                var _json = JsonConvert.SerializeObject(obj);

                var obj1 = JsonConvert.DeserializeObject<SalidasInventarioSaveViewModel>(_json);

                _json = JsonConvert.SerializeObject(obj1);
                                               

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

        public RespuestaGenerica GuardarSolicituTraslado(SolicitudTrasladoViewModel obj, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(obj.CompanyDB);
            System.Net.HttpStatusCode _Status = System.Net.HttpStatusCode.Created;

            RespuestaGenerica respuestaPrincipal = new RespuestaGenerica();

            if (servicio.IsConected)
            {
                CatalogosManager catalogos = new CatalogosManager(ref servicio);

                #region Valida Contenedor, Items y Centros de costos
                //var contenedor = catalogos.GetUDO(obj.U_EXX_CONTENEDOR, CompanyDB, "EXX_CONTENEDOR");

                //if (contenedor == null)
                //{
                //    var result = catalogos.GuardarContenedor(obj.ContenedorData, CompanyDB);


                //    if (!result.Success)
                //    {
                //        //Error
                //        return result;
                //    }
                //}



                string cc = "";
                foreach (var item in obj.StockTransferLines)
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
                        cc = i == 1 ? item.DistributionRule : i == 2 ? item.DistributionRule2 : i == 3 ? item.DistributionRule3 : i == 4 ? item.DistributionRule4 : i == 5 ? item.DistributionRule5 : "";
                        if (!string.IsNullOrEmpty(cc))
                        {
                            ProfitCenterViewModel centrocosto = catalogos.GetCentroCostos(cc, CompanyDB);

                            if (centrocosto == null)
                            {
                                respuestaPrincipal.Success = false;
                                respuestaPrincipal.ErrCodigo = -002;
                                respuestaPrincipal.ErrMensaje = $"Centro de costos [{i}] [{item.DistributionRule}] no existe en SAP. ";

                                return respuestaPrincipal;

                            }
                        }
                    }
                }

                #endregion


                string _Method = "POST";//PATCH
                string _recurso = $"InventoryTransferRequests";// Salida de Inventario SAP
                _Status = System.Net.HttpStatusCode.Created;               


                var _json = JsonConvert.SerializeObject(obj);

                var obj1 = JsonConvert.DeserializeObject<SolicitudTrasladoViewGuardarModel>(_json);

                _json = JsonConvert.SerializeObject(obj1);


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

        public RespuestaGenerica CancelarSolicituTraslado(long DocEntry, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);
            System.Net.HttpStatusCode _Status = System.Net.HttpStatusCode.Created;


            RespuestaGenerica respuestaPrincipal = new RespuestaGenerica();

            if (servicio.IsConected)
            { 
                string _Method = "POST";

                string _recurso = $"InventoryTransferRequests({DocEntry})/Cancel";

                _Status = System.Net.HttpStatusCode.OK;
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

        public RespuestaGenerica Cancelar(OrdenVentaCancelar obj, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);
            System.Net.HttpStatusCode _Status = System.Net.HttpStatusCode.Created;


            RespuestaGenerica respuestaPrincipal = new RespuestaGenerica();

            if (servicio.IsConected)
            { //OBTENER CARDCODE



                string _Method = "POST";

                string _recurso = $"InventoryGenExits({obj.DocEntry})/Cancel";

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

       
    }

}