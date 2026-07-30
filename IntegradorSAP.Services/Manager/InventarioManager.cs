using Newtonsoft.Json;
using IntegradorSAP.Data.DataAccess;
using IntegradorSAP.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using IntegradorSAP.Data.Helper;
using IntegradorSAP.Data.Entidades;
using IntegradorSAP.Data.Manager;
using System.Web.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Sap.Data.Hana;

namespace IntegradorSAP.Services.Manager
{
    public class InventarioManager:DAO
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




        public InventarioViewModel Get(int DocEntry, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            if (servicio.IsConected)
            {

                string _recurso = $"InventoryGenExits({DocEntry})";
                var respuesta = servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);
                InventarioViewModel obj = JsonConvert.DeserializeObject<InventarioViewModel>(respuesta.RespuestaJson);


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


        public async Task<RespuestaGenerica> GuardarSalidaInventario(InventarioGuardarViewModel obj, string CompanyDB, long? DocEntryTransfer, long? DocNumTransfer)
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
                string destino = "";
                string _Method = "POST";//PATCH
                string _recurso = $"InventoryGenExits";// Salida de Inventario SAP
                _Status = System.Net.HttpStatusCode.Created;

                obj.GroupNumber = -1;
                obj.SalesPersonCode = -1;
                obj.RequriedDate = obj.DocDate;
                obj.DocDueDate = obj.DocDate;
                obj.DocDate = obj.DocDate;

                foreach (var item in obj.DocumentLines)
                {
                    destino = await GetBodegaDestino(CompanyDB, item.WarehouseCode);//recibe la bodega destino obtengo la origen                    

                    if (string.IsNullOrEmpty(destino))
                    {
                        respuestaPrincipal.Success = false;
                        respuestaPrincipal.ErrCodigo = -002;
                        respuestaPrincipal.ErrMensaje = $"Bodega {item.WarehouseCode} no tiene definida bodega de paso. Definir en sap la bodega relacionada y vuelva a intentar. ";

                        return respuestaPrincipal;
                    }
                   

                    item.WarehouseCode = destino;//Bodega Destino PNORT
                }
               
                var _json = JsonConvert.SerializeObject(obj);

                var obj1 = JsonConvert.DeserializeObject<InventarioSaveViewModel>(_json);
                obj1.WarehouseCode = destino;
                obj1.U_LocalCtaContab = destino;
                _json = JsonConvert.SerializeObject(obj1);


                var respuesta = servicio.SLSendRequestReturnResponse(_recurso, _Method, _json, _Status, false);


                if (respuesta.Success)
                {
                    var objInvTransf = JsonConvert.DeserializeObject<InventarioTransfSaveViewModel>(_json);
                    var respSalida = JsonConvert.DeserializeObject<InventarioSaveViewModel>(respuesta.RespuestaJson);

                    //objInvTransf.DocEntryTransf = DocEntryTransfer;
                    //objInvTransf.DocNumTransf = DocNumTransfer;
                    objInvTransf.DocNum = respSalida.DocNum;
                    objInvTransf.DocEntry = respSalida.DocEntry;

                    string _jsonTrans = JsonConvert.SerializeObject(objInvTransf);

                    this.errMsg = "Creación Exitosa";
                    respuesta.RespuestaJson = _jsonTrans;
                    return respuesta;
                }
                else
                {
                    var respserv = respuesta.ErrMensaje.Split(':')[6].Replace("}", "");

                    //  this.errMsg = $"Error: {respuesta.ErrCodigo}-{respuesta.ErrException}-{respuesta.ErrMensaje}";
                    respuesta.Success = false;
                    respuesta.ErrMensaje = respserv;
                    respuesta.RespuestaJson = null;
                    respuesta.ErrException = null;
                    this.errMsg = null;
                    return respuesta;
                }
            }
            else
            {
                this.errMsg = $"Error: No se pudo establecer conexión con SSL";
                respuestaPrincipal.Success = false;
                respuestaPrincipal.ErrMensaje = this.errMsg;
                respuestaPrincipal.RespuestaJson = null;
                respuestaPrincipal.ErrException = null;
                return respuestaPrincipal;
            }
        }


        public async Task<RespuestaGenerica> GuardarTransferenciaStock(InventarioGuardarViewModel obj, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(obj.CompanyDB);
            System.Net.HttpStatusCode _Status = System.Net.HttpStatusCode.Created;
            // string FromWarehouse = "";
            RespuestaGenerica respuestaPrincipal = new RespuestaGenerica();

            if (servicio.IsConected)
            {
                string destino = "";
                CatalogosManager catalogos = new CatalogosManager(ref servicio);

                #region Valida Contenedor, Items y Centros de costos
                var contenedor = catalogos.GetUDO(obj.U_EXX_CONTENEDOR, CompanyDB, "EXX_CONTENEDOR");


                StockInventarioGuardarViewModel doctransf = new StockInventarioGuardarViewModel();
                List<StockInventarioline> ListitemTranfer = new List<StockInventarioline>();


                string cc = "";
                foreach (var item in obj.DocumentLines)
                {
                    destino = await GetBodegaDestino(CompanyDB, item.WarehouseCode);//recibe la bodega destino obtengo la origen

                    StockInventarioline itemTranfer = new StockInventarioline();
                    itemTranfer.ItemCode = item.ItemCode;
                    itemTranfer.Quantity = item.Quantity;
                    itemTranfer.WarehouseCode = item.WarehouseCode;//Bodega Destino PNORT
                    itemTranfer.FromWarehouseCode = destino;//Bodega Desde  PATLIMP                 
                    itemTranfer.UnitsOfMeasurment = 1;// item.UnitsOfMeasurment;
                    itemTranfer.BaseType = "Default";
                    itemTranfer.UoMCode = item.UoMCode;
                    itemTranfer.ItemDescription = item.ItemDescription;

                    itemTranfer.LineNum = item.LineNum;
                    //Valida si esxiste Item
                    ItemsViewModel itemsap = catalogos.GetItem(item.ItemCode, CompanyDB);

                    if (itemsap == null)
                    {
                        respuestaPrincipal.Success = false;
                        respuestaPrincipal.ErrCodigo = -002;
                        respuestaPrincipal.ErrMensaje = $"Artículo o Item  con [{item.ItemCode}] no existe en SAP.";

                        return respuestaPrincipal;

                    }


                    itemTranfer.Price = item.Price;

                    //Valida si esxiste Item
                    for (int i = 1; i <= 5; i++)
                    {
                        cc = string.Empty;
                        //  FromWarehouse = item.WarehouseCode;
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

                    ListitemTranfer.Add(itemTranfer);
                }

                #endregion

                // doctransf.FromWarehouse = item.WarehouseCode;
                doctransf.ToWarehouse = obj.DocumentLines.First().WarehouseCode;// Bodega Origen
                doctransf.FromWarehouse = destino;// Bodega desde;
                doctransf.PriceList = -1;
                doctransf.DocDate = obj.DocDate;
                doctransf.Series = 27;
                doctransf.DueDate = obj.DocDate;
                doctransf.DocumentStatus = "bost_Open";
                doctransf.DocObjectCode = 67;
                doctransf.AuthorizationStatus = "sasWithout";
                doctransf.U_Exx_IP_Pais = "593";
                doctransf.U_Exx_IP_DobleTrib = "N";
                doctransf.U_Exx_IP_SujetRet_NL = "N";
                doctransf.U_DOC_DECLARABLE = "N";
                doctransf.U_VALOR_FOB = 0;
                doctransf.U_EXX_CONTENEDOR = obj.U_EXX_CONTENEDOR;
                string _Method = "POST";//PATCH
                string _recurso = $"StockTransfers";// Salida de Inventario SAP
                _Status = System.Net.HttpStatusCode.Created;

                //  doctransf.GroupNumber = -1;
                doctransf.SalesPersonCode = -1;
                doctransf.CreationDate = DateTime.Now;
                if (obj.Reference1 != null)
                {
                    if (obj.Reference1.Length >= 10)
                    {
                        obj.Reference1 = obj.Reference1.Substring(0, 10);

                    }
                    else
                    {
                        obj.Reference1 = obj.Reference1;
                    }


                }
                doctransf.Reference1 = obj.Reference1;
                doctransf.Comments = obj.Comments;
                doctransf.JournalMemo = obj.JournalMemo;
                doctransf.TaxDate = DateTime.Now;
                doctransf.U_Exx_IP_Pago = "01";
                //doctransf.RequriedDate = obj.DocDate;
                //doctransf.CancelDate = obj.DocDate;
                //doctransf.DocDueDate = obj.DocDate;
                doctransf.StockTransferLines = ListitemTranfer;

                var _json = JsonConvert.SerializeObject(doctransf);

                var obj1 = JsonConvert.DeserializeObject<StockInventarioGuardarViewModel>(_json);

                _json = JsonConvert.SerializeObject(obj1);


                var respuesta = servicio.SLSendRequestReturnResponse(_recurso, _Method, _json, _Status, false);

                if (respuesta.Success)
                {
                    this.errMsg = "Creación Exitosa";

                    return respuesta;
                }
                else
                {
                    var respserv = respuesta.ErrMensaje.Split(':')[6].Replace("}", "");
                    this.errMsg = null;// $"Error: {respuesta.ErrCodigo}-{respuesta.ErrException}-{respuesta.ErrMensaje}";
                    respuesta.Success = false;
                    respuesta.ErrMensaje = respserv;
                    respuesta.RespuestaJson = null;
                    respuesta.ErrException = null;
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
                var contenedor = catalogos.GetUDO(obj.U_EXX_CONTENEDOR, CompanyDB, "EXX_CONTENEDOR");

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

                //var obj1 = JsonConvert.DeserializeObject<SalidasInventarioSaveViewModel>(_json);

                //_json = JsonConvert.SerializeObject(obj1);


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
        public async Task<string> GetBodegaDestino(string CompanyDB, string BodOrigen)
        {
            string destino = "";
            HanaCommand comm = null;
            HanaDataReader reader = null;
            try
            {
                conectarHana(CompanyDB);
                string StrQty = "SELECT T0.\"U_BODDESTINO\"  as \"BODDESTINO\"  FROM  \"" + CompanyDB + "\".\"@RFS_PARAMETRIZABODE\" T0";
                StrQty += @" WHERE ""U_BODORIGEN"" in ('" + BodOrigen + "')";

                comm = new HanaCommand(StrQty, Connection);

                reader = await comm.ExecuteReaderAsync();

                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {

                            destino = reader.GetValue(0).ToString();
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

            return destino;
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


        public List<ArticuloViewModel> GetItemsServiciosSeaboard(string CompanyDB)
        {
            HanaCommand comm = null;
            HanaDataReader reader = null;
            List<ArticuloViewModel> articulos = new List<ArticuloViewModel>();
            ArticuloViewModel articulo = new ArticuloViewModel();

            try
            {
                conectarHana(CompanyDB);
                string StrQty = "SELECT * FROM  \"" + CompanyDB + "\".\"CTKItemsServiciosSeaboardView\" ";
                //  StrQty += @" WHERE ""SapPedidoDocEntry"" in (" + Ids + ")";

                comm = new HanaCommand(StrQty, Connection);

                reader = comm.ExecuteReader();

                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            articulo = new ArticuloViewModel();
                            articulo.ItemCode = Convert.ToString(reader.GetValue(0));
                            articulo.ItemName = Convert.ToString(reader.GetValue(1));
                            articulo.Price = Convert.ToDecimal(reader.GetValue(2));
                            articulos.Add(articulo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //List<OrdenVentaXFacturar> ordenesError = new List<OrdenVentaXFacturar>();
                //OrdenVentaXFacturar ordenError = new OrdenVentaXFacturar();
                ErrorMensaje = ex.Message;
                //ordenError.MensajeError = ErrorMensaje;
                //ordenesError.Add(ordenError);
                return articulos;
            }
            finally
            {


                LiberarVariables(ref Connection, ref comm, ref reader);
            }

            return articulos;
        }

        public async Task<RespuestaGenerica> GetMaterialesSap(string CompanyDB)
        {
            RespuestaGenerica respuesta = new RespuestaGenerica();
            HanaCommand comm = null;
            HanaDataReader reader = null;
            List<MaterialesSapViewModel> ordenes = new List<MaterialesSapViewModel>();
            MaterialesSapViewModel orden = new MaterialesSapViewModel();
 
            try
            {
                conectarHana(CompanyDB);
                string StrQty = "SELECT * FROM  \"" + CompanyDB + "\".\"CTK_STOCKMATERIALESVIEW\" ";
               
                comm = new HanaCommand(StrQty, Connection);

                reader = await comm.ExecuteReaderAsync();

                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                orden = new MaterialesSapViewModel();
                                orden.Codigo = reader.GetValue(0).ToString();
                                orden.Producto = reader.GetValue(1).ToString();
                                orden.Unidad = reader.GetValue(2).ToString();
                                orden.BodegaCodigo = reader.GetValue(3).ToString();
                                orden.Bodega = reader.GetValue(4).ToString();
                                orden.Grupo = Convert.ToString(reader.GetValue(5));
                                orden.CatalogoFabricante = reader.GetValue(6).ToString();
                                orden.Fabricante = Convert.ToString(reader.GetValue(7));
                                orden.UnidadInventario = Convert.ToString(reader.GetValue(8));
                                orden.UnidadCompra = Convert.ToString(reader.GetValue(9));                                
                                orden.UltimoPrecioCmp = Convert.ToDecimal(reader.GetValue(10));
                                orden.Stock = Convert.ToDecimal(reader.GetValue(11));                                
                                orden.PMP = Convert.ToDecimal(reader.GetValue(12));
                                orden.Valorizado = Convert.ToDecimal(reader.GetValue(13));
                                ordenes.Add(orden);
                            }
                            catch { }


                        }
                    }
                }



                var _json = JsonConvert.SerializeObject(ordenes);


                respuesta = new RespuestaGenerica();
                respuesta.Success = true;
                respuesta.ErrMensaje="OK";
                respuesta.RespuestaJson = _json;
                
            }
            catch (Exception ex)
            {
                respuesta = new RespuestaGenerica();
               
                this.errMsg = $"Error: {ex.Message??""}-{ex.InnerException?.Message??""}";
                respuesta.Success = false;
                respuesta.ErrMensaje = errMsg;
                respuesta.RespuestaJson = "Error";
            }
            finally
            {


                LiberarVariables(ref Connection, ref comm, ref reader);
            }

            return respuesta;
        }



    }

}

