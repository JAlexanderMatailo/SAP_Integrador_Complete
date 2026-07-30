using IntegradorSAP.Data.Entidades;
using IntegradorSAP.Data.Manager;
using IntegradorSAP.Data.Models;
using IntegradorSAP.Services.Manager;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Timers;
using NLog;
using System.Configuration;
using System.Threading;

namespace IntegradorSAP.WebApi.Controllers
{
    [RoutePrefix("api/FacturacionLote")]

    public class FacturacionLoteController : ApiController
    {

        private Logger _logger = LogManager.GetLogger("DataLog");
     
        //private Logger _logger = LogManager.GetLogger("DataLog");
       // private static Timer aTimer;
        protected FacturacionLoteManager _service => new FacturacionLoteManager();

        [Route("GetPedidosPendientesFactxurar")]
        public List<OrdenVentaXFacturar> GetPedidosPendientesFactxurar(string CompanyDB)
        {
            //Se debe enviar los DocEntry de los pedidos a los que se desea buscar si tienen facturas
            List<OrdenVentaXFacturar> obj = _service.GetOrdenesVentaPendienteXFacturar(CompanyDB);
            return obj;
        }




        [Route("PostActualizaOrdenesFactxuradas")]
        public List<RespuestaGenerica> PostActualizaOrdenesFactxuradas([FromBody] List<OrdenVentaXFacturar> Orden)
        {
            
           
            List<RespuestaGenerica> respuestas = new List<RespuestaGenerica>();
            RespuestaGenerica respuesta = new RespuestaGenerica();
            OrdenesVentaXLoteProcesado value;
            foreach (var orden in Orden)
            {
                value = new OrdenesVentaXLoteProcesado();
                value.U_CTK_FechaHoraGeneracion = DateTime.Now.ToString();
                value.U_CTK_Generado = "S";
                value.U_CTK_Lote = "S";
                value.U_CTK_Observacion = "S";
                value.DocEntry = orden.DocEntry;
                var CompanyDB = orden.CompanyDB;
                respuesta = _service.ActualizaEstadoOrdenes(value, CompanyDB);
                respuestas.Add(respuesta);
            }
            return respuestas;
        }
        [Route("ProcesarLoteFacturasSinPago")]
        public List<RespuestaGenericaLote> ProcesarLoteFacturasSinPago(string CompanyDB)
        {
           return  _service.ProcesarLoteFacturasSinPago(CompanyDB, 0);

        }

        [Route("ProcesarFacturasEnLoteOld")]
        public List<RespuestaGenericaLote> ProcesarFacturasEnLoteOld(string CompanyDB)
        {
            string IVA_SAP = ConfigurationManager.AppSettings["IVA"].ToString();//http://192.168.238.2:9097/";//

            List<RespuestaGenericaLote> respuestas = new List<RespuestaGenericaLote>();
            RespuestaGenericaLote respuesta = new RespuestaGenericaLote();

            if (!_service.Login(CompanyDB   ))
            {
                respuesta = new RespuestaGenericaLote();
                respuesta.DocEntryRel = 0;
                respuesta.DocNumRel = 0;
                respuesta.TipoDocumento = "Facturas";
                respuesta.Cliente = "";
                respuesta.DocEntry = 0;
                respuesta.DocNum = 0;
                respuesta.Success = false;
                respuesta.ErrMensaje = $"No se pudo establecer conexion con  [{CompanyDB}] {DateTime.Now}";
                respuestas.Add(respuesta);

                _logger.Error($" {respuesta.ErrMensaje}  {DateTime.Now}");
                return respuestas;
            }else 
            {
                _logger.Info($"Conexión exitosa con {CompanyDB} a las   {DateTime.Now}");

            }
        
            List<OrdenVentaXFacturar> Ordenes = new List<OrdenVentaXFacturar>();           
            OrdenesVentaXLoteProcesado value;
            _logger.Info("========================================================== ");
            _logger.Info($"Iniciando Proceso de Generacion de Facturas a las  {DateTime.Now}");
            //obtengo todas las ordenes pendientes de procesar
            try
            {

                Ordenes = _service.GetOrdenesVentaPendienteXFacturar(CompanyDB);

                if (Ordenes != null)
                {
                    respuesta = new RespuestaGenericaLote();
                    respuesta.DocEntryRel = 0;
                    respuesta.DocNumRel = 0;
                    respuesta.TipoDocumento = "Facturas";
                    respuesta.Cliente = "";
                    respuesta.DocEntry = 0;
                    respuesta.DocNum = 0;
                    respuesta.Success = false;
                    respuesta.ErrMensaje = $"Se encontraron {Ordenes.Count()} listas para ser Procesadas. Inició a las {DateTime.Now}";
                    respuestas.Add(respuesta);
                }
            }
            catch (Exception ex)
            {
               
                respuesta = new RespuestaGenericaLote();
                respuesta.DocEntryRel = 0;
                respuesta.DocNumRel = 0;
                respuesta.TipoDocumento = "";
                respuesta.Cliente = "";
                respuesta.DocEntry = 0;
                respuesta.DocNum = 0;
                respuesta.Success = false;
                respuesta.ErrMensaje = "Ocurrió un error al consultar las Ordenes pendientes de facturar" + ex.Message.ToString();

                respuestas.Add(respuesta);
                return respuestas;
            }

            long Lote = _service.GenerarSecuenciaLote(CompanyDB);

            if (Ordenes.Count > 0)
            {
                Lote = Lote + 1;

                _logger.Info($"LOTE: {Lote} ");

                var Error = Ordenes.FirstOrDefault();


                if (Error.MensajeError != null)
                {
                    respuesta.ErrMensaje = Error.MensajeError;
                    respuestas.Add(respuesta);
                }
                else
                {
                    foreach (var orden in Ordenes)
                    {
                        if (orden == null || orden.DocEntry == 0)
                        {
                            respuesta = new RespuestaGenericaLote();
                            respuesta.DocEntryRel = 0;
                            respuesta.DocNumRel = 0;
                            respuesta.TipoDocumento = "";
                            respuesta.Cliente = "";
                            respuesta.DocEntry = 0;
                            respuesta.DocNum = 0;
                            respuesta.ErrMensaje = "Ocurrió un error al consultar las Ordenes pendientes de Facturas.";

                            respuestas.Add(respuesta);
                            _logger.Error(respuesta.ErrMensaje);
                        }
                        var FechaDocumento = orden.DocDate.AddHours(orden.HoraDocumento);
                        var FechaGeneracion = DateTime.Now;

                        FechaDocumento = FechaDocumento.AddMinutes(orden.MinutoDocumento + orden.MINUTOSAGREGADOS);

                        if (FechaDocumento.Month != FechaGeneracion.Month)
                        {
                            FechaDocumento = FechaGeneracion;
                        }

                        value = new OrdenesVentaXLoteProcesado();
                        value.U_CTK_Generado = "N";
                        value.U_CTK_Lote = "";
                        value.DocEntry = orden.DocEntry;
                        value.DocNum = orden.DocNum;

                        //************************************************************
                        //VALIDAR SI LA ORDEN DE VENTA YA TIENE FACTURA ASOCIADA.
                        //************************************************************
                        var existefactura = _service.GetFacturaByOrdenVentaLote(orden.DocEntry, CompanyDB);
                        if (existefactura != null && existefactura.DocNum != 0)
                        {
                            value.U_CTK_Lote = Lote.ToString();
                            value.U_CTK_Generado = "S";
                            value.U_CTK_DocEntryRel = existefactura.DocEntry.ToString();
                            value.U_CTK_DocNumRel = existefactura.DocNum.ToString();
                            value.U_CTK_FechaHoraGeneracion = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                            value.U_CTK_Observacion = $"Factura de Venta ya tiene asociada esta orden de Venta.";
                            var respuest = _service.ActualizaEstadoOrdenes(value, CompanyDB);


                            //CERRAR ORDEN DE VENTA
                            if (respuest.Success && existefactura.FolioNumber != null)
                            {
                                var OrdenCerrada = _service.GetOrdenFacturarLote(orden.DocEntry, CompanyDB);
                                if (OrdenCerrada.DocumentStatus == "bost_Open" || OrdenCerrada.DocumentStatus == "O")
                                {
                                    var respCierreDocVta = _service.CerrarOrdenVenta(value.DocEntry, CompanyDB);

                                    respuesta = new RespuestaGenericaLote();
                                    respuesta.DocEntryRel = value.DocEntry;
                                    respuesta.DocNumRel = value.DocNum;
                                    respuesta.NumeroDocumento = $"{existefactura.U_SER_EST}-{existefactura.U_SER_PE}-{existefactura.FolioNumber}";
                                    respuesta.TipoDocumento = orden.U_EXX_DOC_GEN;
                                    respuesta.Cliente = orden.CardCode + "-" + orden.CardName;
                                    respuesta.DocEntry = existefactura.DocEntry;
                                    respuesta.DocNum = existefactura.DocNum;
                                    //respuesta.ErrMensaje = value.U_CTK_Observacion;

                                    respuesta.ErrMensaje = $"Se forzó cierre de OV No.{value.DocNum}" + value.U_CTK_Observacion;
                                    respuestas.Add(respuesta);
                                    continue;

                                }
                            }
                        }

                        #region Validaciones de Parametrizaciones
                        if (orden.CardCode is null || orden.CardCode == "")
                        {
                            value.U_CTK_Observacion = orden.DocNum + "  ADVERTENCIA: No se ha parametrizado el cliente, por favor acceda a la pantalla de parametrizaciones. Documento OV:" + orden.DocNum;
                        }
                        else if (orden.U_tipo_comprob == null || orden.U_tipo_comprob == "")
                        {
                            value.U_CTK_Observacion = orden.DocNum + "  ADVERTENCIA: No se ha configurado la serie de Factura de Venta";
                        }
                        else if (orden.U_TIPO_ID == null || orden.U_TIPO_ID == "")
                        {
                            value.U_CTK_Observacion = orden.DocNum + "  ADVERTENCIA: No se ha configurado el tipo de comprobante para el cliente";
                        }

                        //else if (orden.U_TIPO_ID == "C" && orden.U_SerieFV != "FAE006")
                        //{
                        //    value.U_CTK_Observacion = value.DocEntry + "  ADVERTENCIA: El cliente con tipo de comprobante Pasaporte no tiene configurado FAE006";
                        //}
                        //else if (orden.U_TIPO_ID == "R" && orden.U_SerieFV != "FAE006")
                        //{
                        //    value.U_CTK_Observacion = value.DocEntry + "  ADVERTENCIA: El cliente  no tiene configurado FAE006";
                        //}
                        //else if (orden.U_TIPO_ID == "P" && orden.U_SerieFV != "FAE002")
                        //{
                        //    value.U_CTK_Observacion = value.DocEntry + "  ADVERTENCIA: El cliente con tipo de comprobante Pasaporte no tiene configurado FAE002";
                        //}
                        else if ((orden.CardName.Contains("seaboard") && orden.U_SerieFV != "FAE002"))
                        {
                            value.U_CTK_Observacion = orden.DocNum + "  ADVERTENCIA: El cliente Seaboard no tiene configurado FAE002";
                        }
                        else if (orden.U_tipo_comprob != "01" && orden.U_SerieFV == "FAE002")
                        {
                            value.U_CTK_Observacion = orden.DocNum + "  ADVERTENCIA: El tipo de comprobante debe ser igual a 01 para Seabord o Clientes del Exterior";
                        }
                        else if (orden.U_tipo_comprob != "18" && orden.U_SerieFV == "FAE006")
                        {
                            value.U_CTK_Observacion = orden.DocNum + "  ADVERTENCIA: El tipo de comprobante debe ser igual a 18 para facturas locales";
                        }
                        else if (orden.U_SerieFV == "FAE002" && orden.U_tipo_comprob != "01")
                        {
                            value.U_CTK_Observacion = orden.DocNum + "  ADVERTENCIA: El tipo de comprobante debe ser 01  para facturas del exterior";
                        }
                        else if (orden.U_SerieFV == "FAE002" && (orden.U_tipo_export == null))
                        {
                            value.U_CTK_Observacion = orden.DocNum + "  ADVERTENCIA: El tipo de comprobante de referendo no puede estar vacío para facturas de exportación";
                        }
                        else if (orden.U_TipoFacturacion == "A")
                        {
                            value.U_CTK_Observacion = orden.DocNum + "  ADVERTENCIA: El cliente esta configurado con facturacion agrupada (U_TipoFacturacion = A), que este proceso no genera.";
                        }

                        else if (FechaDocumento.Day < orden.U_FechaApertura || FechaDocumento.Day > orden.U_FechaCierre)
                        {
                            value.U_CTK_Observacion = orden.DocNum + "  ADVERTENCIA: El día del documento a generar está fuera del tiempo de Generación.      Dia Inicio para el cliente: " + orden.U_FechaApertura.ToString()
                                + "      Dia Fin: " + orden.U_FechaCierre.ToString() + "    Dia Documento: " + orden.DiaFactura.ToString();
                        }
                        else if (FechaDocumento > FechaGeneracion)
                        {
                            value.U_CTK_Observacion = orden.DocNum + "  ADVERTENCIA: El comprobante debe generarse después de " + FechaDocumento.ToString();
                        }
                        else if (orden.DocTotal <= 0)
                        {
                            value.U_CTK_Observacion = orden.DocNum + "  ADVERTENCIA: El valor total de la factura no puede serss igual a 0 ";
                        }
                        else if (orden.U_Exx_FE_Paisdestin == "")
                        {
                            value.U_CTK_Observacion = orden.DocNum + "  ADVERTENCIA: No se ha asignado el codigo de pais en el maestro PAÍSES, campo ReportCode";
                        }

                        else if (!(orden.U_EXX_FPAGO_VENTAS is null) == true && orden.U_EXX_FPAGO_VENTAS != "0" && orden.U_EXX_FPAGO_VENTAS != "")
                        {
                            value.U_CTK_Observacion = orden.DocNum + "  ADVERTENCIA: Este documento ya tiene generado un Id de Pago, por favor verificar antes de realizar el envio";
                        }


                        ///***********************************************************
                        ///*ACTUALIZAR ORDEN CON MENSAJERÍA Y CONTINUA A LA SIGUIENTE
                        ///***********************************************************
                        if (value.U_CTK_Observacion != null)
                        {
                            var respuest = _service.ActualizaEstadoOrdenes(value, CompanyDB);

                            respuesta = new RespuestaGenericaLote();
                            respuesta.DocEntryRel = value.DocEntry;
                            respuesta.DocNumRel = value.DocNum;
                            respuesta.TipoDocumento = orden.U_EXX_DOC_GEN;
                            respuesta.Cliente = orden.CardCode + "-" + orden.CardName;
                            respuesta.DocEntry = 0;
                            respuesta.DocNum = 0;
                            respuesta.Success = false;
                            respuesta.ErrMensaje = value.U_CTK_Observacion;
                            respuestas.Add(respuesta);
                            continue;
                        }

                        #endregion 

                        else
                        {
                            long DocNum = 0;
                            //En caso de no encontrar novedades, se procede a armar el documento de factuxracion
                            var ov = _service.GetOrdenFacturarLote(orden.DocEntry, CompanyDB);

                            if (ov == null)
                            {
                                respuesta = new RespuestaGenericaLote();
                                respuesta.DocEntryRel = value.DocEntry;
                                respuesta.DocNumRel = value.DocNum;
                                respuesta.TipoDocumento = orden.U_EXX_DOC_GEN;
                                respuesta.Cliente = orden.CardCode + "-" + orden.CardName;
                                respuesta.DocEntry = 0;
                                respuesta.DocNum = 0;
                                respuesta.Success = false;
                                respuesta.ErrMensaje = $"El método GetOrdenFacturarLote no devolvió datos para la orden {orden.DocNum} ";
                                respuestas.Add(respuesta);
                                continue;
                            }

                            _logger.Info($"OrdenVenta: {orden.DocNum}");

                            ov.U_REFRENDO_ANIO = DateTime.Now.Year.ToString();
                            ov.U_SER_EST = orden.U_SER_EST;
                            ov.U_SER_PE = orden.U_SER_PE;
                            ov.U_tipo_export = orden.U_tipo_export;
                            ov.U_FECHA_EMBARQUE = DateTime.Now;
                            ov.DocDate = DateTime.Now;
                            ov.DocDueDate = FechaDocumento.AddDays(orden.ExtraDays).AddMonths(orden.ExtraMonth);
                            //var secuencial = _service.GetSerieXFacturar(CompanyDB, orden.U_tipo_comprob.ToString());
                            var _json = JsonConvert.SerializeObject(ov);

                            FacturaVentaCreateModel factura = JsonConvert.DeserializeObject<FacturaVentaCreateModel>(_json);
                            // factura.Series = secuencial;
                            factura.Series = orden.Series;
                            factura.DocumentSubType = ov.DocumentSubType;
                            factura.DiscountPercent = ov.DiscountPercent;
                            factura.PaymentMethod = ov.PaymentMethod;
                            factura.DocumentsOwner = ov.DocumentsOwner;
                            factura.PickRemark = ov.PickRemark;
                            factura.TotalDiscount = ov.TotalDiscount;
                            factura.TotalDiscountFC = ov.TotalDiscountFC;
                            factura.TotalDiscountSC = ov.TotalDiscountSC;
                            factura.U_COD_ST = ov.U_COD_ST;
                            factura.VatSum = ov.VatSum;
                            factura.VatSumSys = ov.VatSumSys;
                            factura.VatSumFc = ov.VatSumFc;
                            factura.SalesPersonCode = orden.SalesPersonCode;
                            factura.U_tipo_export = orden.U_tipo_export;
                            factura.TrnspCode = orden.TransportationCode;
                            //factura.AuthorizationCode=orden.AuthorizationCode;
                            //factura.U_NUM_AUTO_FAC = orden.AuthorizationCode;
                            //factura.U_NUM_AUT_FR = orden.AuthorizationCode;
                            //factura.U_NUM_AUT_RET = orden.AuthorizationCode;
                            factura.NumAtCard = ov.NumAtCard;
                            factura.DocDate = DateTime.Now;
                            factura.TaxDate = DateTime.Now;
                            factura.DocDueDate = ov.DocDueDate;
                            //factura.DocDueDate = DateTime.Now.AddMonths(orden.ExtraMonth);
                            factura.U_NUM_AUTOR = orden.U_NUM_AUTOR;
                            factura.U_tipo_comprob = orden.U_tipo_comprob;
                            factura.OpeningRemarks = ov.OpeningRemarks;
                            factura.ClosingRemarks = ov.ClosingRemarks;
                            factura.U_Exx_FE_Paisdestin = ov.U_Exx_FE_Paisdestin;
                            if (ov.CardName.Contains("Seab") && factura.U_Exx_FE_Paisdestin == null)
                            {
                                factura.U_Exx_FE_Paisdestin = "110";
                            }
                            else
                            {
                                factura.U_Exx_FE_Paisdestin = orden.U_Exx_FE_Paisdestin;
                            }

                            factura.U_EXX_MAN_AG = ov.U_EXX_MAN_AG;
                            factura.U_EXX_TIPO_TRANSACC = ov.U_EXX_TIPO_TRANSACC;
                            factura.U_EXX_DOC_GEN = ov.U_EXX_DOC_GEN;
                            if (orden.U_SerieFV == "FAE002")
                            {
                                factura.U_Exx_ingFueGra_IR = "NO";
                            }
                            if (orden.U_SerieFV == "FAE002")
                            {
                                factura.U_Exx_TipRegFis = "01";
                            }
                            if (orden.U_SerieFV == "FAE002")
                            {
                                factura.U_Exx_TipIngExt = "439";
                            }

                            //if (factura.U_EXX_DOC_GEN.Contains("exte") && orden.U_SerieFV != "FAE002")
                            //{

                            //    value.U_CTK_Observacion = value.DocEntry + "  ADVERTENCIA: El tipo de comprobante exterior no coincide con FAE002";
                            //}

                            //if (factura.U_EXX_DOC_GEN.Contains("local") && orden.U_SerieFV != "FAE006")
                            //{

                            //    value.U_CTK_Observacion = value.DocEntry + "  ADVERTENCIA: El tipo de comprobante local no coincide con FAE006";
                            //}
                            if (orden.U_SerieFV == "FAE002")
                            {
                                factura.U_DOC_DECLARABLE = "S";
                            }
                            factura.U_HRH_Lote = ov.U_HRH_Lote;
                            factura.U_HRH_Modo_Fact = ov.U_HRH_Modo_Fact;
                            factura.U_LocalCtaContab = ov.U_LocalCtaContab;
                            factura.U_CTK_FechaHoraGeneracion = DateTime.Now.ToString();
                            factura.JournalMemo = ov.JournalMemo;
                            factura.U_VALOR_FOB = ov.DocTotal;
                            factura.U_FECHA_EMBARQUE = DateTime.Now;
                            factura.Comments = ov.Comments;
                            if (ov.Comments.Length > 50)
                            {
                                factura.JournalMemo = ov.Comments.Substring(0, 50);
                            }
                            else
                            {
                                factura.JournalMemo = ov.Comments;
                            }

                            factura.U_VALOR_FOB = ov.DocTotal;
                            List<DocumentlineVentaModel> DocumentLines = new List<DocumentlineVentaModel>();
                            DocumentlineVentaModel documentLine;

                            List<DocumentlineCreateVentaModel> DocumentLinesCreate = new List<DocumentlineCreateVentaModel>();
                            DocumentlineCreateVentaModel documentLineCreate;


                            List<DocumentlineVtaLoteProcesado> LstLine = new List<DocumentlineVtaLoteProcesado>();
                            DocumentlineVtaLoteProcesado line;
                            int lineid = 0;
                            foreach (var ItemDet in ov.DocumentLines)
                            {
                                //aTimer = new System.Timers.Timer();
                                //aTimer.Interval = 2000000;

                                if (ItemDet.LineTotal > 0)
                                {
                                    line = new DocumentlineVtaLoteProcesado();
                                    documentLine = new DocumentlineVentaModel();
                                    documentLineCreate = new DocumentlineCreateVentaModel();
                                    documentLine.AccountCode = ItemDet.AccountCode;
                                    documentLine.CostingCode = ItemDet.CostingCode;
                                    documentLine.ProjectCode = ItemDet.ProjectCode;
                                    if (ItemDet.Rate == null || ItemDet.Rate == 0)
                                    {
                                        documentLine.Rate = 1;
                                    }
                                    else
                                    {
                                        documentLine.Rate = ItemDet.Rate.Value;
                                    }

                                    documentLine.VendorNum = ItemDet.VendorNum;
                                    documentLine.TreeType = ItemDet.TreeType;
                                    documentLine.SupplierCatNum = ItemDet.SupplierCatNum;
                                    documentLine.BackOrder = ItemDet.BackOrder;
                                    documentLine.POTargetEntry = ItemDet.POTargetEntry;
                                    documentLine.NetTaxAmount = ItemDet.NetTaxAmount;
                                    documentLine.NetTaxAmountFC = ItemDet.NetTaxAmountFC;
                                    documentLine.NetTaxAmountSC = ItemDet.NetTaxAmountSC;
                                    documentLine.MeasureUnit = ItemDet.MeasureUnit;
                                    documentLine.TaxPercentagePerRow = ItemDet.TaxPercentagePerRow;
                                    documentLine.RowTotalSC = ItemDet.RowTotalSC;

                                    try
                                    {
                                        documentLine.U_DetGlosaMrk = ItemDet.U_DetGlosaMrk;
                                        documentLine.FreeText = ItemDet.FreeText;                                        
                                    }
                                    catch { }
                                    //  documentLine.Incoterms = ItemDet.Incoterms;
                                    line.BaseEntry = ov.DocEntry.ToString();
                                    line.BaseType = "17";
                                    line.BaseLine = ItemDet.LineNum.ToString();
                                    if ((documentLine.AccountCode == null || documentLine.AccountCode == "") && value.U_CTK_Observacion == null)
                                    {
                                        value.U_CTK_Observacion = value.DocEntry + "  Advertencia: El item " + ItemDet.ItemCode + " - No tiene asignado una cuenta";
                                    }

                                    if (value.U_CTK_Observacion == null)
                                    {
                                        // if (documentLine.)

                                        documentLine.LineTotal = ItemDet.LineTotal;
                                        documentLine.ItemCode = ItemDet.ItemCode;
                                        documentLine.LineNum = lineid;
                                        documentLine.Quantity = ItemDet.Quantity;
                                        documentLine.ShipDate = ItemDet.ShipDate;
                                        documentLine.BarCode = ItemDet.BarCode;
                                        documentLine.VatGroup = ItemDet.VatGroup;
                                        documentLine.PriceAfterVAT = ItemDet.PriceAfterVAT;
                                        documentLine.Currency = ItemDet.Currency;
                                        //documentLine.BaseType = "15";
                                        //documentLine.BaseEntry = ov.DocEntry.ToString();
                                        //documentLine.BaseLine = ItemDet.LineNum.ToString();
                                        documentLine.ShippingMethod = ItemDet.ShippingMethod;
                                        documentLine.Text = ItemDet.Text;
                                        documentLine.COGSCostingCode = ItemDet.COGSCostingCode;
                                        documentLine.CostingCode2 = ItemDet.CostingCode2;
                                        documentLine.CostingCode3 = ItemDet.CostingCode3;
                                        documentLine.CostingCode4 = ItemDet.CostingCode4;
                                        documentLine.CostingCode5 = ItemDet.CostingCode5;
                                        documentLine.DiscountPercent = ItemDet.DiscountPercent;
                                        documentLine.CommisionPercent = ItemDet.CommisionPercent;
                                        documentLine.TaxType = ItemDet.TaxType;
                                        documentLine.TaxLiable = ItemDet.TaxLiable;
                                        // documentLine.TransportMode = ItemDet.TransportMode;
                                        if (orden.U_SerieFV == "FAE002")
                                        {
                                            documentLine.TaxCode = "IVA_EXE";
                                        }
                                        else
                                        {
                                            //documentLine.TaxCode = "IVA";
                                            documentLine.TaxCode = IVA_SAP;
                                        }
                                        documentLine.Price = ItemDet.Price;
                                        documentLine.UnitPrice = ItemDet.UnitPrice;
                                        documentLine.WarehouseCode = ItemDet.WarehouseCode;
                                        line.BaseQty = ItemDet.Quantity;

                                        line.TargetType = "13";
                                        line.TrgetEntry = ov.DocEntry.ToString();



                                        //   line.OwnerCode = ItemDet.OwnerCode;
                                        DocumentLines.Add(documentLine);
                                        LstLine.Add(line);
                                    }
                                    lineid = lineid + 1;

                                }



                            }
                            value.Line = LstLine;
                            value.BaseDocNum = ov.DocNum.ToString();
                            value.U_CTK_FechaHoraGeneracion = DateTime.Now.ToString();

                            _logger.Info($"OrdenVenta [{ov.DocNum}] Tiene Observaciones {value.U_CTK_Observacion}");

                            FacturaResultModel facturagenerada = new FacturaResultModel();
                            RespuestaGenerica resp = new RespuestaGenerica();

                            if (value.U_CTK_Observacion == null)
                            {
                                Thread.Sleep(2000);//Tiempo de espera para que se asienten los cambios del Folio y de la factura.

                                _logger.Info($"OrdenVenta [{ov.DocNum}] Está Lista para Procesarse...");
                                factura.DocumentLines = DocumentLines;
                                factura.U_CTK_DocEntryRel = ov.DocEntry.ToString();
                                factura.U_CTK_DocNumRel = ov.DocNum.ToString();
                                factura.U_CTK_FechaHoraGeneracion = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                                factura.U_CTK_Generado = "S";
                                factura.U_CTK_Lote = Lote;

                                var folio = _service.GetOrdenesMaxSerieFactSSL(CompanyDB, orden.U_SerieFV);
                                _logger.Info($"OrdenVenta [{ov.DocNum}] se le asignará el FOLIO [{folio}]...");

                                factura.FolioPrefixString = "FCE";
                                var folioOINV = _service.GetMaxSerieOINV(CompanyDB, factura.FolioPrefixString, factura.U_SER_EST, factura.U_SER_PE);
                                if (folioOINV > folio)
                                {
                                    folio = folioOINV;
                                }
                                factura.FolioNumber = folio;

                                var duplicado = _service.GetSerieDuplicadaSSL(CompanyDB, folio.ToString(), factura.FolioPrefixString, factura.U_SER_EST, factura.U_SER_PE);
                                if (duplicado == 0)
                                {
                                    var validafactura = _service.GetFacturaByOrdenVentaLote(orden.DocEntry, CompanyDB);

                                    if (validafactura == null)
                                    {
                                        _logger.Info($"OrdenVenta [{ov.DocNum}]. Está disponible se enviará a grabar.");
                                        //*********************************************
                                        //GUARDA LA FACTURA
                                        //*********************************************
                                        factura.U_CTK_Generado = "S";
                                        factura.U_CTK_Lote = Lote;
                                        factura.U_CTK_DocEntryRel = ov.DocEntry.ToString();
                                        factura.U_CTK_DocNumRel = ov.DocNum.ToString();
                                        factura.U_CTK_FechaHoraGeneracion = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                                        resp = _service.GuardarFactura(factura, CompanyDB);

                                        if (resp.Success)
                                        {
                                            facturagenerada = JsonConvert.DeserializeObject<FacturaResultModel>(resp.RespuestaJson);
                                            _logger.Info($"OrdenVenta [{ov.DocNum}]. Factura Grabada Existosamente DocNum:{facturagenerada.DocNum} No.{facturagenerada.U_SER_EST}-{facturagenerada.U_SER_PE}-{facturagenerada.FolioNumber} DocEntry:{facturagenerada.DocEntry} -{DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}");


                                            value.U_CTK_Observacion = $"Se generó  la Factura {facturagenerada.DocNum}. NO. {facturagenerada.U_SER_EST}-{facturagenerada.U_SER_PE}-{facturagenerada.FolioNumber} Pago:{facturagenerada.U_EXX_FPAGO_VENTAS}";

                                            if (facturagenerada.FolioNumber == 0)
                                            {
                                                value.U_CTK_Observacion += ". No se creó Folio.";
                                            }
                                            if (string.IsNullOrEmpty(facturagenerada.U_EXX_FPAGO_VENTAS))
                                            {
                                                value.U_CTK_Observacion += ". No se creó la Forma de Pago.";
                                            }

                                            //************************************************************************************
                                            //Si la factura se guarda exitosamente, se actualiza la secuencia en Documentos Legales
                                            //************************************************************************************

                                            value.U_CTK_Generado = "S";
                                            value.CTK_DocEntryRel = facturagenerada.DocEntry.ToString();
                                            value.CTK_DocNumRel = facturagenerada.DocNum.ToString();
                                            value.U_CTK_Lote = Lote.ToString();// ov.U_SER_EST + "-" + ov.U_SER_PE;                                       
                                            value.U_CTK_FechaHoraGeneracion = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                                            value.U_CTK_Observacion = $"Factura # {facturagenerada.U_SER_EST}-{facturagenerada.U_SER_PE}-{facturagenerada.FolioNumber } DocNum:{facturagenerada.DocNum} DocEntry: {facturagenerada.DocEntry} OV REL: {value.DocNum} -Procesada con Exito { DateTime.Now.ToString("dd - MM - yyyy HH: mm: ss")}";
                                            value.DocNum = ov.DocNum;

                                            //******ACTUALIZAR ORDEN DE VENTA Y CERRAR******/
                                            var respu = _service.ActualizaEstadoOrdenes(value, CompanyDB);

                                            if (respu.Success)
                                            {
                                                var respCierreDocVta = _service.CerrarOrdenVenta(value.DocEntry, CompanyDB);
                                            }

                                            respuesta = new RespuestaGenericaLote();
                                            respuesta.DocEntryRel = ov.DocEntry;
                                            respuesta.DocNumRel = ov.DocNum;
                                            respuesta.TipoDocumento = orden.U_EXX_DOC_GEN;
                                            respuesta.Cliente = orden.CardCode + "-" + orden.CardName;
                                            respuesta.DocEntry = facturagenerada.DocEntry;
                                            respuesta.DocNum = facturagenerada.DocNum;
                                            respuesta.Success = true;
                                            respuesta.ErrMensaje = value.U_CTK_Observacion;
                                            respuestas.Add(respuesta);

                                            /*Verificar 2 puntos
                                           1) Folio no se duplicó
                                           2) Se grabó Forma de Pago
                                           3) Actualizar el Estado de la Orden de Venta                                           
                                            */
                                        }
                                        else if (!resp.ErrMensaje.Contains("-1116"))
                                        {
                                            facturagenerada = _service.GetFacturaByOrdenVentaLote(value.DocEntry, CompanyDB);
                                            if (facturagenerada != null && facturagenerada.DocEntry != 0)
                                            {

                                                // value.U_CTK_Observacion = "Ocurrió un error al procesar el pedido.";
                                            }
                                            else
                                            {
                                                value.U_CTK_Observacion = $"Ocurrió un error al procesar el pedido. {resp.ErrCodigo} -{resp.ErrMensaje}";

                                                respuesta = new RespuestaGenericaLote();
                                                respuesta.DocEntryRel = orden.DocEntry;
                                                respuesta.DocNumRel = orden.DocNum;
                                                respuesta.TipoDocumento = orden.U_EXX_DOC_GEN;
                                                respuesta.Cliente = orden.CardCode + "-" + orden.CardName;
                                                respuesta.DocEntry = 0;
                                                respuesta.DocNum = 0;
                                                respuesta.Success = false;
                                                respuesta.RespuestaJson = "";
                                                respuesta.ErrMensaje = value.U_CTK_Observacion;
                                                respuestas.Add(respuesta);
                                            }
                                        }
                                        else if (resp.ErrMensaje.Contains("-1116"))
                                        {
                                            /*Si cae en este error es porque si se grabó la factura 
                                           1) Verificar si se grabó forma de pago
                                           2) Si no se grabó forma de pago, llamar SP InsertFormaPago
                                           3) Actualizar Secuencial de Facturacion Electrónica.
                                           3) Actualizar el Estado de la Orden de Venta                                   
                                            */
                                            facturagenerada = _service.GetFacturaByOrdenVentaLote(value.DocEntry, CompanyDB);
                                            if (facturagenerada != null && facturagenerada.DocEntry != 0)
                                            {
                                                value.U_CTK_Observacion = $"Se Forzó Guardar la Factura {facturagenerada.DocNum}. NO. {facturagenerada.U_SER_EST}-{facturagenerada.U_SER_PE}-{facturagenerada.FolioNumber} Pago:{facturagenerada.U_EXX_FPAGO_VENTAS}";

                                                value.U_CTK_Generado = "S";
                                                value.U_CTK_Lote = ov.U_SER_EST + "-" + ov.U_SER_PE;
                                                //value.U_CTK_Observacion = $"Factura # {facturagenerada.U_SER_EST}-{facturagenerada.U_SER_PE}-{ facturaGenerada.FolioNumber }  DocEntry {facturaGenerada.DocEntry} Procesada con Exito. Se Forzó actualización de la orden de venta.";
                                                value.U_CTK_DocEntryRel = facturagenerada.DocEntry.ToString();
                                                value.U_CTK_DocNumRel = facturagenerada.DocNum.ToString();
                                                value.U_CTK_FechaHoraGeneracion = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                                                value.DocNum = ov.DocNum;


                                                if (string.IsNullOrEmpty(factura.U_EXX_FPAGO_VENTAS))
                                                {
                                                    if (_service.GuardarFormaPagoBySP(CompanyDB, orden))
                                                        resp.Success = true;
                                                    else
                                                        resp.Success = false;
                                                }

                                                //******ACTUALIZAR ORDEN DE VENTA Y CERRAR******/
                                                var respu = _service.ActualizaEstadoOrdenes(value, CompanyDB);

                                                if (value.U_CTK_Generado == "S")
                                                {
                                                    var respCierreDocVta = _service.CerrarOrdenVenta(value.DocEntry, CompanyDB);
                                                }


                                                respuesta = new RespuestaGenericaLote();
                                                respuesta.DocEntryRel = value.DocEntry;
                                                respuesta.DocNumRel = value.DocNum;
                                                respuesta.TipoDocumento = orden.U_EXX_DOC_GEN;
                                                respuesta.Cliente = orden.CardCode + "-" + orden.CardName;
                                                respuesta.DocEntry = facturagenerada.DocEntry;
                                                respuesta.DocNum = facturagenerada.DocNum;
                                                respuesta.Success = true;
                                                respuesta.ErrMensaje = value.U_CTK_Observacion;
                                                respuestas.Add(respuesta);

                                            }
                                        }
                                    }                                   

                                    if (resp.Success)
                                    {

                                        _logger.Info($"OrdenVenta [{ov.DocNum}] Actualizando secuencial de Facturación electronica de Documentos Legales Internos.");


                                        //************************************************************************************
                                        //Si la factura se guarda exitosamente, se actualiza la secuencia en Documentos Legales
                                        //************************************************************************************

                                        value.U_CTK_Generado = "S";
                                        value.CTK_DocEntryRel = facturagenerada.DocEntry.ToString();
                                        value.CTK_DocNumRel = facturagenerada.DocNum.ToString();
                                        value.U_CTK_Lote = Lote.ToString();// ov.U_SER_EST + "-" + ov.U_SER_PE;                                       
                                        value.U_CTK_FechaHoraGeneracion = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                                        value.U_CTK_Observacion = $"Factura # {facturagenerada.U_SER_EST}-{facturagenerada.U_SER_PE}-{facturagenerada.FolioNumber } DocNum:{facturagenerada.DocNum} DocEntry: {facturagenerada.DocEntry} OV REL: {value.DocNum} -Procesada con Exito { DateTime.Now.ToString("dd - MM - yyyy HH: mm: ss")}";
                                        value.DocNum = ov.DocNum;
                                        //value.FolioNumber = factura.FolioNumber;

                                        var Ultimofolio = _service.GetOrdenesMaxSerieFactSSL(CompanyDB, orden.U_SerieFV);

                                        if (Ultimofolio <= folio)
                                        {
                                            EXX_DOCUM_LEG_INTER docserie = new EXX_DOCUM_LEG_INTER();
                                            docserie.U_ULT_SECUEN = folio;

                                            var respActSecuencia = _service.ActualizaSecuenciaDocumentoElectronico(docserie, orden.U_CodeSerieId, CompanyDB);

                                            if (respActSecuencia.ErrMensaje.Contains("Unknown error"))
                                            {
                                                var SecDocInterno = _service.GetOrdenesMaxSerieFactSSL(CompanyDB, orden.U_SerieFV);
                                                if (SecDocInterno < folio)
                                                {
                                                    var respActSecuencia2Intento = _service.ActualizaSecuenciaDocumentoElectronico(docserie, orden.U_CodeSerieId, CompanyDB);

                                                    if (respActSecuencia2Intento.ErrMensaje.Contains("Unknown error"))
                                                    {

                                                        value.U_CTK_Observacion = "Se detiene el proceso por error desconocido";
                                                        respuesta.ErrMensaje = ov.DocNum + " -" + value.U_CTK_Observacion;
                                                        break;
                                                    }
                                                }
                                            }

                                            if (!respActSecuencia.Success)
                                            {

                                                value.U_CTK_Observacion = value.U_CTK_Observacion + " - La serie del documento no pudo ser actualizada en la pantalla Documentos Internos";
                                                break;
                                            }
                                            else
                                            {
                                                _logger.Info($"OrdenVenta [{ov.DocNum}] actualizado Secuencial de Facturación electronica de Documentos Legales Internos.");
                                            }


                                        }
                                    }
                                    else
                                    {
                                        if (resp != null && !resp.Success && !string.IsNullOrEmpty(resp.ErrMensaje) && resp.ErrMensaje.Contains("Unknown error"))
                                        {

                                            value.U_CTK_Observacion = "Se detiene el proceso por error desconocido";
                                            respuesta.ErrMensaje = ov.DocNum + " -" + value.U_CTK_Observacion;

                                            //  break;
                                        }
                                        //else
                                        //{
                                        //    if (resp != null && !resp.Success && !string.IsNullOrEmpty(resp.ErrMensaje) && resp.ErrMensaje.Contains("Se ha terminado la conexión"))
                                        //    {
                                        //        respuesta.ErrMensaje = ov.DocNum + " -" + value.U_CTK_Observacion + " El servidor Cerro la conexion.";
                                        //        break;
                                        //    }
                                        //    else
                                        //    if (resp != null && !resp.Success && !string.IsNullOrEmpty(resp.ErrMensaje) && resp.ErrMensaje.Contains("No se pudo establecer conexión con SSL"))
                                        //    {
                                        //        respuesta.ErrMensaje = ov.DocNum + " -" + value.U_CTK_Observacion + " No se pudo establecer conexión con SSL";
                                        //        break;
                                        //    }
                                        //    else
                                        //    {
                                        //        if (resp != null && !resp.Success && !string.IsNullOrEmpty(resp.ErrMensaje))
                                        //        {
                                        //            value.U_CTK_Observacion = resp.ErrMensaje.Split(':')[7].Replace("}", "");
                                        //            respuesta.ErrMensaje = ov.DocNum + " -" + value.U_CTK_Observacion;
                                        //        }
                                        //    }

                                        //}

                                    }
                                }
                                else
                                {
                                    _logger.Info($"OrdenVenta [{ov.DocNum}] Se intentó generar secuencia duplicada [{folio}], Verifique la parametrización de las series...");


                                    respuesta = new RespuestaGenericaLote();
                                    value.FolioNumber = null;
                                    value.U_CTK_Observacion = "Se intento generar secuencia duplicada";
                                    value.U_CTK_Generado = "N";
                                }

                            }

                            else
                            {

                                _logger.Info($"OrdenVenta [{ov.DocNum}] Tiene Observaciones {value.U_CTK_Observacion}");

                            }
                        }
                        _logger.Info($"OrdenVenta {orden.DocNum} Finalizó proceso.");


                        //respuesta = new RespuestaGenericaLote();
                        //if (value.DocNum == 0)
                        //{
                        //    value.DocNum = orden.DocNum;
                        //}                        

                        // var respu = _service.ActualizaEstadoOrdenes(value, CompanyDB);                        

                        //if (value.U_CTK_Generado == "S" )
                        //{
                        //    var OrdenCerrada = _service.GetOrdenFacturarLote(orden.DocEntry, CompanyDB);
                        //    if (OrdenCerrada.DocumentStatus == "bost_Open" || OrdenCerrada.DocumentStatus == "O")
                        //    {                               
                        //        var respCierreDocVta = _service.CerrarOrdenVenta(value.DocEntry, CompanyDB);                              
                        //    }

                        //}

                        // respuestas.Add(respuesta);

                    }
                    
                }
            }


            respuesta = new RespuestaGenericaLote();
            respuesta.DocEntryRel = 0;
            respuesta.DocNumRel = 0;
            respuesta.TipoDocumento = "Facturas";
            respuesta.Cliente = "";
            respuesta.DocEntry = 0;
            respuesta.DocNum = 0;
            respuesta.Success = false;
            respuesta.ErrMensaje = $"Se Procesaron [{(respuestas.Count(p => p.Success == true) - 1)}] de {Ordenes.Count()}, Ordenes Sin Procesar.[{(respuestas.Count(p => p.Success != true) - 1) } Finalizó a las {DateTime.Now}]";
            respuestas.Add(respuesta);


            _logger.Info($"Finalizando el Proceso de Generacion de Facturas a las {DateTime.Now}");
            return respuestas;

        }

        [Route("ProcesarFacturasEnLote")]
        public List<RespuestaGenericaLote> ProcesarFacturasEnLote(string CompanyDB)
        {
            string IVA_SAP = ConfigurationManager.AppSettings["IVA"].ToString();//http://192.168.238.2:9097/";//

            List<RespuestaGenericaLote> source = new List<RespuestaGenericaLote>();
            RespuestaGenericaLote respuestaGenericaLote1 = new RespuestaGenericaLote();
            if (!this._service.Login(CompanyDB))
            {
                RespuestaGenericaLote respuestaGenericaLote2 = new RespuestaGenericaLote();
                respuestaGenericaLote2.DocEntryRel = 0L;
                respuestaGenericaLote2.DocNumRel = 0L;
                respuestaGenericaLote2.TipoDocumento = "Facturas";
                respuestaGenericaLote2.Cliente = "";
                respuestaGenericaLote2.DocEntry = 0L;
                respuestaGenericaLote2.DocNum = 0L;
                respuestaGenericaLote2.Success = false;
                respuestaGenericaLote2.ErrMensaje = string.Format("No se pudo establecer conexion con  [{0}] {1}", (object)CompanyDB, (object)DateTime.Now);
                source.Add(respuestaGenericaLote2);
                this._logger.Error(string.Format(" {0}  {1}", (object)respuestaGenericaLote2.ErrMensaje, (object)DateTime.Now));
                return source;
            }
            this._logger.Info(string.Format("Conexión exitosa con {0} a las   {1}", (object)CompanyDB, (object)DateTime.Now));
            List<OrdenVentaXFacturar> ordenVentaXfacturarList = new List<OrdenVentaXFacturar>();
            this._logger.Info("========================================================== ");
            this._logger.Info(string.Format("Iniciando Proceso de Generacion de Facturas a las  {0}", (object)DateTime.Now));
            List<OrdenVentaXFacturar> pendienteXfacturar;
            try
            {
                pendienteXfacturar = this._service.GetOrdenesVentaPendienteXFacturar(CompanyDB);
                if (pendienteXfacturar != null)
                {
                    respuestaGenericaLote1 = new RespuestaGenericaLote();
                    respuestaGenericaLote1.DocEntryRel = 0L;
                    respuestaGenericaLote1.DocNumRel = 0L;
                    respuestaGenericaLote1.TipoDocumento = "Facturas";
                    respuestaGenericaLote1.Cliente = "";
                    respuestaGenericaLote1.DocEntry = 0L;
                    respuestaGenericaLote1.DocNum = 0L;
                    respuestaGenericaLote1.Success = false;
                    respuestaGenericaLote1.ErrMensaje = string.Format("Se encontraron {0} listas para ser Procesadas. Inició a las {1}", (object)pendienteXfacturar.Count<OrdenVentaXFacturar>(), (object)DateTime.Now);
                    source.Add(respuestaGenericaLote1);
                }
            }
            catch (Exception ex)
            {
                RespuestaGenericaLote respuestaGenericaLote3 = new RespuestaGenericaLote();
                respuestaGenericaLote3.DocEntryRel = 0L;
                respuestaGenericaLote3.DocNumRel = 0L;
                respuestaGenericaLote3.TipoDocumento = "";
                respuestaGenericaLote3.Cliente = "";
                respuestaGenericaLote3.DocEntry = 0L;
                respuestaGenericaLote3.DocNum = 0L;
                respuestaGenericaLote3.Success = false;
                respuestaGenericaLote3.ErrMensaje = "Ocurrió un error al consultar las Ordenes pendientes de facturar" + ex.Message.ToString();
                source.Add(respuestaGenericaLote3);
                return source;
            }
            long num1 = this._service.GenerarSecuenciaLote(CompanyDB);
            if (pendienteXfacturar.Count > 0)
            {
                long num2 = num1 + 1L;
                this._logger.Info(string.Format("LOTE: {0} ", (object)num2));
                OrdenVentaXFacturar ordenVentaXfacturar = pendienteXfacturar.FirstOrDefault<OrdenVentaXFacturar>();
                if (ordenVentaXfacturar.MensajeError != null)
                {
                    respuestaGenericaLote1.ErrMensaje = ordenVentaXfacturar.MensajeError;
                    source.Add(respuestaGenericaLote1);
                }
                else
                {
                    foreach (OrdenVentaXFacturar orden in pendienteXfacturar)
                    {
                        if (orden == null || orden.DocEntry == 0L)
                        {
                            respuestaGenericaLote1 = new RespuestaGenericaLote();
                            respuestaGenericaLote1.DocEntryRel = 0L;
                            respuestaGenericaLote1.DocNumRel = 0L;
                            respuestaGenericaLote1.TipoDocumento = "";
                            respuestaGenericaLote1.Cliente = "";
                            respuestaGenericaLote1.DocEntry = 0L;
                            respuestaGenericaLote1.DocNum = 0L;
                            respuestaGenericaLote1.ErrMensaje = "Ocurrió un error al consultar las Ordenes pendientes de Facturas.";
                            source.Add(respuestaGenericaLote1);
                            this._logger.Error(respuestaGenericaLote1.ErrMensaje);
                        }
                        DateTime dateTime1 = orden.DocDate;
                        DateTime dateTime2 = dateTime1.AddHours((double)orden.HoraDocumento);
                        DateTime now = DateTime.Now;
                        DateTime dateTime3 = dateTime2.AddMinutes((double)(orden.MinutoDocumento + orden.MINUTOSAGREGADOS));
                        if (dateTime3.Month != now.Month)
                            dateTime3 = now;
                        OrdenesVentaXLoteProcesado ventaXloteProcesado1 = new OrdenesVentaXLoteProcesado();
                        ventaXloteProcesado1.U_CTK_Generado = "N";
                        ventaXloteProcesado1.U_CTK_Lote = "";
                        ventaXloteProcesado1.DocEntry = orden.DocEntry;
                        ventaXloteProcesado1.DocNum = orden.DocNum;
                        FacturaResultModel byOrdenVentaLote = this._service.GetFacturaByOrdenVentaLote(orden.DocEntry, CompanyDB);
                        long num3;
                        if (byOrdenVentaLote != null && byOrdenVentaLote.DocNum != 0L)
                        {
                            ventaXloteProcesado1.U_CTK_Lote = num2.ToString();
                            ventaXloteProcesado1.U_CTK_Generado = "S";
                            OrdenesVentaXLoteProcesado ventaXloteProcesado2 = ventaXloteProcesado1;
                            num3 = byOrdenVentaLote.DocEntry;
                            string str1 = num3.ToString();
                            ventaXloteProcesado2.U_CTK_DocEntryRel = str1;
                            OrdenesVentaXLoteProcesado ventaXloteProcesado3 = ventaXloteProcesado1;
                            num3 = byOrdenVentaLote.DocNum;
                            string str2 = num3.ToString();
                            ventaXloteProcesado3.U_CTK_DocNumRel = str2;
                            OrdenesVentaXLoteProcesado ventaXloteProcesado4 = ventaXloteProcesado1;
                            dateTime1 = DateTime.Now;
                            string str3 = dateTime1.ToString("dd-MM-yyyy HH:mm:ss");
                            ventaXloteProcesado4.U_CTK_FechaHoraGeneracion = str3;
                            ventaXloteProcesado1.U_CTK_Observacion = "Factura de Venta ya tiene asociada esta orden de Venta.";
                            if (this._service.ActualizaEstadoOrdenes(ventaXloteProcesado1, CompanyDB).Success)
                            {
                                long folioNumber = byOrdenVentaLote.FolioNumber;
                                OrdenVentaFacturaLote ordenFacturarLote = this._service.GetOrdenFacturarLote(orden.DocEntry, CompanyDB);
                                if (ordenFacturarLote.DocumentStatus == "bost_Open" || ordenFacturarLote.DocumentStatus == "O")
                                {
                                    this._service.CerrarOrdenVenta(ventaXloteProcesado1.DocEntry, CompanyDB);
                                    respuestaGenericaLote1 = new RespuestaGenericaLote();
                                    respuestaGenericaLote1.DocEntryRel = ventaXloteProcesado1.DocEntry;
                                    respuestaGenericaLote1.DocNumRel = ventaXloteProcesado1.DocNum;
                                    respuestaGenericaLote1.NumeroDocumento = string.Format("{0}-{1}-{2}", (object)byOrdenVentaLote.U_SER_EST, (object)byOrdenVentaLote.U_SER_PE, (object)byOrdenVentaLote.FolioNumber);
                                    respuestaGenericaLote1.TipoDocumento = orden.U_EXX_DOC_GEN;
                                    respuestaGenericaLote1.Cliente = orden.CardCode + "-" + orden.CardName;
                                    respuestaGenericaLote1.DocEntry = byOrdenVentaLote.DocEntry;
                                    respuestaGenericaLote1.DocNum = byOrdenVentaLote.DocNum;
                                    respuestaGenericaLote1.ErrMensaje = string.Format("Se forzó cierre de OV No.{0}", (object)ventaXloteProcesado1.DocNum) + ventaXloteProcesado1.U_CTK_Observacion;
                                    source.Add(respuestaGenericaLote1);
                                    continue;
                                }
                            }
                        }
                        int num4;
                        if (orden.CardCode == null || orden.CardCode == "")
                            ventaXloteProcesado1.U_CTK_Observacion = orden.DocNum.ToString() + "  ADVERTENCIA: No se ha parametrizado el cliente, por favor acceda a la pantalla de parametrizaciones. Documento OV:" + (object)orden.DocNum;
                        else if (orden.U_tipo_comprob == null || orden.U_tipo_comprob == "")
                            ventaXloteProcesado1.U_CTK_Observacion = orden.DocNum.ToString() + "  ADVERTENCIA: No se ha configurado la serie de Factura de Venta";
                        else if (orden.U_TIPO_ID == null || orden.U_TIPO_ID == "")
                            ventaXloteProcesado1.U_CTK_Observacion = orden.DocNum.ToString() + "  ADVERTENCIA: No se ha configurado el tipo de comprobante para el cliente";
                        else if (orden.CardName.Contains("seaboard") && orden.U_SerieFV != "FAE002")
                            ventaXloteProcesado1.U_CTK_Observacion = orden.DocNum.ToString() + "  ADVERTENCIA: El cliente Seaboard no tiene configurado FAE002";
                        else if (orden.U_tipo_comprob != "01" && orden.U_SerieFV == "FAE002")
                            ventaXloteProcesado1.U_CTK_Observacion = orden.DocNum.ToString() + "  ADVERTENCIA: El tipo de comprobante debe ser igual a 01 para Seabord o Clientes del Exterior";
                        else if (orden.U_tipo_comprob != "18" && orden.U_SerieFV == "FAE006")
                            ventaXloteProcesado1.U_CTK_Observacion = orden.DocNum.ToString() + "  ADVERTENCIA: El tipo de comprobante debe ser igual a 18 para facturas locales";
                        else if (orden.U_SerieFV == "FAE002" && orden.U_tipo_comprob != "01")
                            ventaXloteProcesado1.U_CTK_Observacion = orden.DocNum.ToString() + "  ADVERTENCIA: El tipo de comprobante debe ser 01  para facturas del exterior";
                        else if (orden.U_SerieFV == "FAE002" && orden.U_tipo_export == null)
                            ventaXloteProcesado1.U_CTK_Observacion = orden.DocNum.ToString() + "  ADVERTENCIA: El tipo de comprobante de referendo no puede estar vacío para facturas de exportación";
                        else if (orden.U_TipoFacturacion == "A")
                            ventaXloteProcesado1.U_CTK_Observacion = orden.DocNum.ToString() + "  ADVERTENCIA: El cliente esta configurado con facturacion agrupada (U_TipoFacturacion = A), que este proceso no genera.";
                        else if (dateTime3.Day < orden.U_FechaApertura || dateTime3.Day > orden.U_FechaCierre)
                        {
                            OrdenesVentaXLoteProcesado ventaXloteProcesado5 = ventaXloteProcesado1;
                            object[] objArray = new object[7];
                            objArray[0] = (object)orden.DocNum;
                            objArray[1] = (object)"  ADVERTENCIA: El día del documento a generar está fuera del tiempo de Generación.      Dia Inicio para el cliente: ";
                            num4 = orden.U_FechaApertura;
                            objArray[2] = (object)num4.ToString();
                            objArray[3] = (object)"      Dia Fin: ";
                            num4 = orden.U_FechaCierre;
                            objArray[4] = (object)num4.ToString();
                            objArray[5] = (object)"    Dia Documento: ";
                            num4 = orden.DiaFactura;
                            objArray[6] = (object)num4.ToString();
                            string str = string.Concat(objArray);
                            ventaXloteProcesado5.U_CTK_Observacion = str;
                        }
                        else if (dateTime3 > now)
                            ventaXloteProcesado1.U_CTK_Observacion = orden.DocNum.ToString() + "  ADVERTENCIA: El comprobante debe generarse después de " + dateTime3.ToString();
                        else if (orden.DocTotal <= 0.0)
                            ventaXloteProcesado1.U_CTK_Observacion = orden.DocNum.ToString() + "  ADVERTENCIA: El valor total de la factura no puede serss igual a 0 ";
                        else if (orden.U_Exx_FE_Paisdestin == "" )
                            ventaXloteProcesado1.U_CTK_Observacion = orden.DocNum.ToString() + "  ADVERTENCIA: No se ha asignado el codigo de pais en el maestro PAÍSES, campo ReportCode";
                        else if (orden.U_EXX_FPAGO_VENTAS != null && orden.U_EXX_FPAGO_VENTAS != "0" && orden.U_EXX_FPAGO_VENTAS != "")
                            ventaXloteProcesado1.U_CTK_Observacion = orden.DocNum.ToString() + "  ADVERTENCIA: Este documento ya tiene generado un Id de Pago, por favor verificar antes de realizar el envio";
                        if (ventaXloteProcesado1.U_CTK_Observacion != null)
                        {
                            this._service.ActualizaEstadoOrdenes(ventaXloteProcesado1, CompanyDB);
                            respuestaGenericaLote1 = new RespuestaGenericaLote();
                            respuestaGenericaLote1.DocEntryRel = ventaXloteProcesado1.DocEntry;
                            respuestaGenericaLote1.DocNumRel = ventaXloteProcesado1.DocNum;
                            respuestaGenericaLote1.TipoDocumento = orden.U_EXX_DOC_GEN;
                            respuestaGenericaLote1.Cliente = orden.CardCode + "-" + orden.CardName;
                            respuestaGenericaLote1.DocEntry = 0L;
                            respuestaGenericaLote1.DocNum = 0L;
                            respuestaGenericaLote1.Success = false;
                            respuestaGenericaLote1.ErrMensaje = ventaXloteProcesado1.U_CTK_Observacion;
                            source.Add(respuestaGenericaLote1);
                        }
                        else
                        {
                            OrdenVentaFacturaLote ordenFacturarLote = this._service.GetOrdenFacturarLote(orden.DocEntry, CompanyDB);
                            if (ordenFacturarLote == null)
                            {
                                respuestaGenericaLote1 = new RespuestaGenericaLote();
                                respuestaGenericaLote1.DocEntryRel = ventaXloteProcesado1.DocEntry;
                                respuestaGenericaLote1.DocNumRel = ventaXloteProcesado1.DocNum;
                                respuestaGenericaLote1.TipoDocumento = orden.U_EXX_DOC_GEN;
                                respuestaGenericaLote1.Cliente = orden.CardCode + "-" + orden.CardName;
                                respuestaGenericaLote1.DocEntry = 0L;
                                respuestaGenericaLote1.DocNum = 0L;
                                respuestaGenericaLote1.Success = false;
                                respuestaGenericaLote1.ErrMensaje = string.Format("El método GetOrdenFacturarLote no devolvió datos para la orden {0} ", (object)orden.DocNum);
                                source.Add(respuestaGenericaLote1);
                            }
                            else
                            {
                                Thread.Sleep(2000);

                                this._logger.Info(string.Format("OrdenVenta: {0}", (object)orden.DocNum));
                                OrdenVentaFacturaLote ventaFacturaLote1 = ordenFacturarLote;
                                dateTime1 = DateTime.Now;
                                num4 = dateTime1.Year;
                                string str4 = num4.ToString();
                                ventaFacturaLote1.U_REFRENDO_ANIO = str4;
                                ordenFacturarLote.U_SER_EST = orden.U_SER_EST;
                                ordenFacturarLote.U_SER_PE = orden.U_SER_PE;
                                ordenFacturarLote.U_tipo_export = orden.U_tipo_export;
                                ordenFacturarLote.U_FECHA_EMBARQUE = new DateTime?(DateTime.Now);
                                ordenFacturarLote.DocDate = DateTime.Now;
                                OrdenVentaFacturaLote ventaFacturaLote2 = ordenFacturarLote;
                                dateTime1 = dateTime3.AddDays((double)orden.ExtraDays);
                                DateTime dateTime4 = dateTime1.AddMonths(orden.ExtraMonth);
                                ventaFacturaLote2.DocDueDate = dateTime4;
                                FacturaVentaCreateModel ventaCreateModel1 = JsonConvert.DeserializeObject<FacturaVentaCreateModel>(JsonConvert.SerializeObject((object)ordenFacturarLote));
                                ventaCreateModel1.Series = orden.Series;
                                ventaCreateModel1.DocumentSubType = ordenFacturarLote.DocumentSubType;
                                ventaCreateModel1.DiscountPercent = ordenFacturarLote.DiscountPercent;
                                ventaCreateModel1.PaymentMethod = ordenFacturarLote.PaymentMethod;
                                ventaCreateModel1.DocumentsOwner = ordenFacturarLote.DocumentsOwner;
                                ventaCreateModel1.PickRemark = ordenFacturarLote.PickRemark;
                                ventaCreateModel1.TotalDiscount = ordenFacturarLote.TotalDiscount;
                                ventaCreateModel1.TotalDiscountFC = ordenFacturarLote.TotalDiscountFC;
                                ventaCreateModel1.TotalDiscountSC = ordenFacturarLote.TotalDiscountSC;
                                ventaCreateModel1.U_COD_ST = ordenFacturarLote.U_COD_ST;
                                ventaCreateModel1.VatSum = ordenFacturarLote.VatSum;
                                ventaCreateModel1.VatSumSys = ordenFacturarLote.VatSumSys;
                                ventaCreateModel1.VatSumFc = ordenFacturarLote.VatSumFc;
                                ventaCreateModel1.SalesPersonCode = orden.SalesPersonCode;
                                ventaCreateModel1.U_tipo_export = orden.U_tipo_export;
                                ventaCreateModel1.TrnspCode = orden.TransportationCode;
                                ventaCreateModel1.NumAtCard = ordenFacturarLote.NumAtCard;
                                ventaCreateModel1.DocDate = DateTime.Now;
                                ventaCreateModel1.TaxDate = DateTime.Now;
                                ventaCreateModel1.DocDueDate = ordenFacturarLote.DocDueDate;
                                ventaCreateModel1.U_NUM_AUTOR = orden.U_NUM_AUTOR;
                                ventaCreateModel1.U_tipo_comprob = orden.U_tipo_comprob;
                                ventaCreateModel1.OpeningRemarks = ordenFacturarLote.OpeningRemarks;
                                ventaCreateModel1.ClosingRemarks = ordenFacturarLote.ClosingRemarks;
                                ventaCreateModel1.U_Exx_FE_Paisdestin = ordenFacturarLote.U_Exx_FE_Paisdestin;
                                
                                ventaCreateModel1.U_Exx_FE_Paisdestin = !ordenFacturarLote.CardName.Contains("Seab") || ventaCreateModel1.U_Exx_FE_Paisdestin != null ? orden.U_Exx_FE_Paisdestin : "110";

                                //Campo nuevo agregado en mayo 2024 Para facturas de Exportacion
                                ventaCreateModel1.U_Exx_FE_PuertoDest = !ordenFacturarLote.CardName.Contains("Seab") || ventaCreateModel1.U_Exx_FE_PuertoDest != null ? orden.U_Exx_FE_PuertoDest : "ESTADOS UNIDOS";

                                ventaCreateModel1.U_EXX_MAN_AG = ordenFacturarLote.U_EXX_MAN_AG;
                                ventaCreateModel1.U_EXX_TIPO_TRANSACC = ordenFacturarLote.U_EXX_TIPO_TRANSACC;
                                ventaCreateModel1.U_EXX_DOC_GEN = ordenFacturarLote.U_EXX_DOC_GEN;
                                if (orden.U_SerieFV == "FAE002")
                                    ventaCreateModel1.U_Exx_ingFueGra_IR = "NO";
                                if (orden.U_SerieFV == "FAE002")
                                    ventaCreateModel1.U_Exx_TipRegFis = "01";
                                if (orden.U_SerieFV == "FAE002")
                                    ventaCreateModel1.U_Exx_TipIngExt = "439";//TIpo de ingresos del Exterior:Otros servicios
                                if (orden.U_SerieFV == "FAE002")
                                    ventaCreateModel1.U_DOC_DECLARABLE = "S";
                                ventaCreateModel1.U_HRH_Lote = ordenFacturarLote.U_HRH_Lote;
                                ventaCreateModel1.U_HRH_Modo_Fact = ordenFacturarLote.U_HRH_Modo_Fact;
                                ventaCreateModel1.U_LocalCtaContab = ordenFacturarLote.U_LocalCtaContab;
                                FacturaVentaCreateModel ventaCreateModel2 = ventaCreateModel1;
                                dateTime1 = DateTime.Now;
                                string str5 = dateTime1.ToString();
                                ventaCreateModel2.U_CTK_FechaHoraGeneracion = str5;
                                ventaCreateModel1.JournalMemo = ordenFacturarLote.JournalMemo;
                                ventaCreateModel1.U_VALOR_FOB = ordenFacturarLote.DocTotal;
                                ventaCreateModel1.U_FECHA_EMBARQUE = DateTime.Now;
                                ventaCreateModel1.Comments = ordenFacturarLote.Comments;
                                ventaCreateModel1.JournalMemo = ordenFacturarLote.Comments.Length <= 50 ? ordenFacturarLote.Comments : ordenFacturarLote.Comments.Substring(0, 50);

                                //Campos nuevos agregado en mayo 2024 Para facturas de Exportacion
                                ventaCreateModel1.U_Exx_FE_ComExt = "EXPORTADOR";
                                ventaCreateModel1.U_Exx_FE_IncoTermFac = "GUAYAQUIL";
                                ventaCreateModel1.U_Exx_FE_LugIncoTerm = "CIF";
                                ventaCreateModel1.U_Exx_FE_PaisOrigen = "593";
                                ventaCreateModel1.U_Exx_FE_PuertoEmb = "GUAYAQUIL";
                                //ventaCreateModel1.U_Exx_FE_PuertoDest = ""; //aqui debe ir el nombre pais destino DEL CLIENTE
                                ventaCreateModel1.U_Exx_FE_Paisadquis = ventaCreateModel1.U_Exx_FE_Paisdestin;//"593";
                                ventaCreateModel1.U_Exx_FE_Incotermto = "FOB";
                                ////////////////////////////////////////
                                

                                List<DocumentlineVentaModel> documentlineVentaModelList = new List<DocumentlineVentaModel>();
                                List<DocumentlineCreateVentaModel> createVentaModelList = new List<DocumentlineCreateVentaModel>();
                                List<DocumentlineVtaLoteProcesado> vtaLoteProcesadoList = new List<DocumentlineVtaLoteProcesado>();
                                int num5 = 0;
                                foreach (DocumentlineVtaFac documentLine in ordenFacturarLote.DocumentLines)
                                {
                                    if ((double)documentLine.LineTotal > 0.0)
                                    {
                                        DocumentlineVtaLoteProcesado vtaLoteProcesado1 = new DocumentlineVtaLoteProcesado();
                                        DocumentlineVentaModel documentlineVentaModel1 = new DocumentlineVentaModel();
                                        DocumentlineCreateVentaModel createVentaModel = new DocumentlineCreateVentaModel();
                                        documentlineVentaModel1.AccountCode = documentLine.AccountCode;
                                        documentlineVentaModel1.CostingCode = documentLine.CostingCode;
                                        documentlineVentaModel1.ProjectCode = documentLine.ProjectCode;
                                        Decimal? rate = documentLine.Rate;
                                        if (rate.HasValue)
                                        {
                                            rate = documentLine.Rate;
                                            Decimal num6 = 0M;
                                            if (!(rate.GetValueOrDefault() == num6 & rate.HasValue))
                                            {
                                                DocumentlineVentaModel documentlineVentaModel2 = documentlineVentaModel1;
                                                rate = documentLine.Rate;
                                                Decimal? nullable = new Decimal?(rate.Value);
                                                documentlineVentaModel2.Rate = nullable;
                                                goto label_66;
                                            }
                                        }
                                        documentlineVentaModel1.Rate = new Decimal?((Decimal)1);
                                          label_66:
                                        documentlineVentaModel1.VendorNum = documentLine.VendorNum;
                                        documentlineVentaModel1.TreeType = documentLine.TreeType;
                                        documentlineVentaModel1.SupplierCatNum = documentLine.SupplierCatNum;
                                        documentlineVentaModel1.BackOrder = documentLine.BackOrder;
                                        documentlineVentaModel1.POTargetEntry = documentLine.POTargetEntry;
                                        documentlineVentaModel1.NetTaxAmount = documentLine.NetTaxAmount;
                                        documentlineVentaModel1.NetTaxAmountFC = documentLine.NetTaxAmountFC;
                                        documentlineVentaModel1.NetTaxAmountSC = documentLine.NetTaxAmountSC;
                                        documentlineVentaModel1.MeasureUnit = documentLine.MeasureUnit;
                                        documentlineVentaModel1.TaxPercentagePerRow = documentLine.TaxPercentagePerRow;
                                        documentlineVentaModel1.RowTotalSC = documentLine.RowTotalSC;
                                        try
                                        {
                                            documentlineVentaModel1.U_DetGlosaMrk = documentLine.U_DetGlosaMrk;
                                            documentlineVentaModel1.FreeText = documentLine.FreeText;
                                        }
                                        catch
                                        {
                                        }
                                        vtaLoteProcesado1.BaseEntry = ordenFacturarLote.DocEntry.ToString();
                                        vtaLoteProcesado1.BaseType = "17";
                                        DocumentlineVtaLoteProcesado vtaLoteProcesado2 = vtaLoteProcesado1;
                                        int num7 = documentLine.LineNum;
                                        string str6 = num7.ToString();
                                        vtaLoteProcesado2.BaseLine = str6;
                                        if ((documentlineVentaModel1.AccountCode == null || documentlineVentaModel1.AccountCode == "") && ventaXloteProcesado1.U_CTK_Observacion == null)
                                            ventaXloteProcesado1.U_CTK_Observacion = ventaXloteProcesado1.DocEntry.ToString() + "  Advertencia: El item " + documentLine.ItemCode + " - No tiene asignado una cuenta";
                                        if (ventaXloteProcesado1.U_CTK_Observacion == null)
                                        {
                                            documentlineVentaModel1.LineTotal = documentLine.LineTotal;
                                            documentlineVentaModel1.ItemCode = documentLine.ItemCode;
                                            documentlineVentaModel1.LineNum = num5;
                                            documentlineVentaModel1.Quantity = documentLine.Quantity;
                                            documentlineVentaModel1.ShipDate = documentLine.ShipDate;
                                            documentlineVentaModel1.BarCode = documentLine.BarCode;
                                            documentlineVentaModel1.VatGroup = documentLine.VatGroup;
                                            documentlineVentaModel1.PriceAfterVAT = documentLine.PriceAfterVAT;
                                            documentlineVentaModel1.Currency = documentLine.Currency;
                                            documentlineVentaModel1.ShippingMethod = documentLine.ShippingMethod;
                                            documentlineVentaModel1.Text = documentLine.Text;
                                            documentlineVentaModel1.COGSCostingCode = documentLine.COGSCostingCode;
                                            documentlineVentaModel1.CostingCode2 = documentLine.CostingCode2;
                                            documentlineVentaModel1.CostingCode3 = documentLine.CostingCode3;
                                            documentlineVentaModel1.CostingCode4 = documentLine.CostingCode4;
                                            documentlineVentaModel1.CostingCode5 = documentLine.CostingCode5;
                                            documentlineVentaModel1.DiscountPercent = documentLine.DiscountPercent;
                                            documentlineVentaModel1.CommisionPercent = documentLine.CommisionPercent;
                                            documentlineVentaModel1.TaxType = documentLine.TaxType;
                                            documentlineVentaModel1.TaxLiable = documentLine.TaxLiable;
                                            documentlineVentaModel1.TaxCode = !(orden.U_SerieFV == "FAE002") ? IVA_SAP : "IVA_EXE";
                                            documentlineVentaModel1.Price = documentLine.Price;
                                            documentlineVentaModel1.UnitPrice = documentLine.UnitPrice;
                                            documentlineVentaModel1.WarehouseCode = documentLine.WarehouseCode;
                                            vtaLoteProcesado1.BaseQty = documentLine.Quantity;
                                            vtaLoteProcesado1.TargetType = "13";
                                            DocumentlineVtaLoteProcesado vtaLoteProcesado3 = vtaLoteProcesado1;
                                            num7 = ordenFacturarLote.DocEntry;
                                            string str7 = num7.ToString();
                                            vtaLoteProcesado3.TrgetEntry = str7;
                                            documentlineVentaModelList.Add(documentlineVentaModel1);
                                            vtaLoteProcesadoList.Add(vtaLoteProcesado1);
                                        }
                                        ++num5;
                                    }
                                }
                                ventaXloteProcesado1.Line = vtaLoteProcesadoList;
                                ventaXloteProcesado1.BaseDocNum = ordenFacturarLote.DocNum.ToString();
                                ventaXloteProcesado1.U_CTK_FechaHoraGeneracion = DateTime.Now.ToString();
                                this._logger.Info(string.Format("OrdenVenta [{0}] Tiene Observaciones {1}", (object)ordenFacturarLote.DocNum, (object)ventaXloteProcesado1.U_CTK_Observacion));
                                FacturaResultModel facturaResultModel = new FacturaResultModel();
                                RespuestaGenerica respuestaGenerica1 = new RespuestaGenerica();
                                if (ventaXloteProcesado1.U_CTK_Observacion == null)
                                {
                                  

                                    this._logger.Info(string.Format("OrdenVenta [{0}] Está Lista para Procesarse...", (object)ordenFacturarLote.DocNum));
                                    ventaCreateModel1.DocumentLines = documentlineVentaModelList;
                                    ventaCreateModel1.U_CTK_DocEntryRel = ordenFacturarLote.DocEntry.ToString();
                                    ventaCreateModel1.U_CTK_DocNumRel = ordenFacturarLote.DocNum.ToString();

                                    FacturaVentaCreateModel ventaCreateModel3 = ventaCreateModel1;
                                    dateTime1 = DateTime.Now;
                                    string str8 = dateTime1.ToString("dd-MM-yyyy HH:mm:ss");
                                    ventaCreateModel3.U_CTK_FechaHoraGeneracion = str8;
                                    ventaCreateModel1.U_CTK_Generado = (object)"S";
                                    ventaCreateModel1.U_CTK_Lote = (object)num2;
                                    long num8 = this._service.GetOrdenesMaxSerieFactSSL(CompanyDB, orden.U_SerieFV);
                                    this._logger.Info(string.Format("OrdenVenta [{0}] se le asignará el FOLIO [{1}]...", (object)ordenFacturarLote.DocNum, (object)num8));
                                    ventaCreateModel1.FolioPrefixString = "FCE";
                                    long maxSerieOinv = this._service.GetMaxSerieOINV(CompanyDB, ventaCreateModel1.FolioPrefixString, ventaCreateModel1.U_SER_EST, ventaCreateModel1.U_SER_PE);
                                    if (maxSerieOinv > num8)
                                        num8 = maxSerieOinv;
                                    ventaCreateModel1.FolioNumber = num8;
                                    if (this._service.GetSerieDuplicadaSSL(CompanyDB, num8.ToString(), ventaCreateModel1.FolioPrefixString, ventaCreateModel1.U_SER_EST, ventaCreateModel1.U_SER_PE) == 0L)
                                    {
                                        if (this._service.GetFacturaByOrdenVentaLote(orden.DocEntry, CompanyDB) == null)
                                        {
                                            this._logger.Info(string.Format("OrdenVenta [{0}]. Está disponible se enviará a grabar.", (object)ordenFacturarLote.DocNum));
                                            ventaCreateModel1.U_CTK_Generado = (object)"S";
                                            ventaCreateModel1.U_CTK_Lote = (object)num2;
                                            ventaCreateModel1.U_CTK_DocEntryRel = ordenFacturarLote.DocEntry.ToString();
                                            ventaCreateModel1.U_CTK_DocNumRel = ordenFacturarLote.DocNum.ToString();
                                            FacturaVentaCreateModel ventaCreateModel4 = ventaCreateModel1;
                                            dateTime1 = DateTime.Now;
                                            string str9 = dateTime1.ToString("dd-MM-yyyy HH:mm:ss");
                                            ventaCreateModel4.U_CTK_FechaHoraGeneracion = str9;
                                            respuestaGenerica1 = this._service.GuardarFactura(ventaCreateModel1, CompanyDB);
                                            if (respuestaGenerica1.Success)
                                            {
                                                facturaResultModel = JsonConvert.DeserializeObject<FacturaResultModel>(respuestaGenerica1.RespuestaJson);
                                                Logger logger = this._logger;
                                                object[] objArray1 = new object[7]
                                                {
                                                      (object) ordenFacturarLote.DocNum,
                                                      (object) facturaResultModel.DocNum,
                                                      (object) facturaResultModel.U_SER_EST,
                                                      (object) facturaResultModel.U_SER_PE,
                                                      (object) facturaResultModel.FolioNumber,
                                                      (object) facturaResultModel.DocEntry,
                                                      null
                                                };
                                                dateTime1 = DateTime.Now;
                                                objArray1[6] = (object)dateTime1.ToString("dd-MM-yyyy HH:mm:ss");
                                                string message = string.Format("OrdenVenta [{0}]. Factura Grabada Existosamente DocNum:{1} No.{2}-{3}-{4} DocEntry:{5} -{6}", objArray1);
                                                logger.Info(message);
                                                ventaXloteProcesado1.U_CTK_Observacion = string.Format("Se generó  la Factura {0}. NO. {1}-{2}-{3} Pago:{4}", (object)facturaResultModel.DocNum, (object)facturaResultModel.U_SER_EST, (object)facturaResultModel.U_SER_PE, (object)facturaResultModel.FolioNumber, (object)facturaResultModel.U_EXX_FPAGO_VENTAS);
                                                if (facturaResultModel.FolioNumber == 0L)
                                                    ventaXloteProcesado1.U_CTK_Observacion += ". No se creó Folio.";
                                                if (string.IsNullOrEmpty(facturaResultModel.U_EXX_FPAGO_VENTAS))
                                                    ventaXloteProcesado1.U_CTK_Observacion += ". No se creó la Forma de Pago.";
                                                ventaXloteProcesado1.U_CTK_Generado = "S";
                                                ventaXloteProcesado1.CTK_DocEntryRel = facturaResultModel.DocEntry.ToString();
                                                OrdenesVentaXLoteProcesado ventaXloteProcesado6 = ventaXloteProcesado1;
                                                num3 = facturaResultModel.DocNum;
                                                string str10 = num3.ToString();
                                                ventaXloteProcesado6.CTK_DocNumRel = str10;
                                                ventaXloteProcesado1.U_CTK_Lote = num2.ToString();
                                                OrdenesVentaXLoteProcesado ventaXloteProcesado7 = ventaXloteProcesado1;
                                                dateTime1 = DateTime.Now;
                                                string str11 = dateTime1.ToString("dd-MM-yyyy HH:mm:ss");
                                                ventaXloteProcesado7.U_CTK_FechaHoraGeneracion = str11;
                                                OrdenesVentaXLoteProcesado ventaXloteProcesado8 = ventaXloteProcesado1;
                                                object[] objArray2 = new object[7]
                                                {
                                                      (object) facturaResultModel.U_SER_EST,
                                                      (object) facturaResultModel.U_SER_PE,
                                                      (object) facturaResultModel.FolioNumber,
                                                      (object) facturaResultModel.DocNum,
                                                      (object) facturaResultModel.DocEntry,
                                                      (object) ventaXloteProcesado1.DocNum,
                                                      null
                                                };
                                                dateTime1 = DateTime.Now;
                                                objArray2[6] = (object)dateTime1.ToString("dd - MM - yyyy HH: mm: ss");
                                                string str12 = string.Format("Factura # {0}-{1}-{2} DocNum:{3} DocEntry: {4} OV REL: {5} -Procesada con Exito {6}", objArray2);
                                                ventaXloteProcesado8.U_CTK_Observacion = str12;
                                                ventaXloteProcesado1.DocNum = (long)ordenFacturarLote.DocNum;
                                                if (this._service.ActualizaEstadoOrdenes(ventaXloteProcesado1, CompanyDB).Success)
                                                    this._service.CerrarOrdenVenta(ventaXloteProcesado1.DocEntry, CompanyDB);
                                                respuestaGenericaLote1 = new RespuestaGenericaLote();
                                                respuestaGenericaLote1.DocEntryRel = (long)ordenFacturarLote.DocEntry;
                                                respuestaGenericaLote1.DocNumRel = (long)ordenFacturarLote.DocNum;
                                                respuestaGenericaLote1.TipoDocumento = orden.U_EXX_DOC_GEN;
                                                respuestaGenericaLote1.Cliente = orden.CardCode + "-" + orden.CardName;
                                                respuestaGenericaLote1.DocEntry = facturaResultModel.DocEntry;
                                                respuestaGenericaLote1.DocNum = facturaResultModel.DocNum;
                                                respuestaGenericaLote1.Success = true;
                                                respuestaGenericaLote1.ErrMensaje = ventaXloteProcesado1.U_CTK_Observacion;
                                                source.Add(respuestaGenericaLote1);
                                            }
                                            else if (!respuestaGenerica1.ErrMensaje.Contains("-1116"))
                                            {
                                                facturaResultModel = this._service.GetFacturaByOrdenVentaLote(ventaXloteProcesado1.DocEntry, CompanyDB);
                                                if (facturaResultModel == null || facturaResultModel.DocEntry == 0L)
                                                {
                                                    ventaXloteProcesado1.U_CTK_Observacion = string.Format("Ocurrió un error al procesar el pedido. {0} -{1}", (object)respuestaGenerica1.ErrCodigo, (object)respuestaGenerica1.ErrMensaje);
                                                    respuestaGenericaLote1 = new RespuestaGenericaLote();
                                                    respuestaGenericaLote1.DocEntryRel = orden.DocEntry;
                                                    respuestaGenericaLote1.DocNumRel = orden.DocNum;
                                                    respuestaGenericaLote1.TipoDocumento = orden.U_EXX_DOC_GEN;
                                                    respuestaGenericaLote1.Cliente = orden.CardCode + "-" + orden.CardName;
                                                    respuestaGenericaLote1.DocEntry = 0L;
                                                    respuestaGenericaLote1.DocNum = 0L;
                                                    respuestaGenericaLote1.Success = false;
                                                    respuestaGenericaLote1.RespuestaJson = "";
                                                    respuestaGenericaLote1.ErrMensaje = ventaXloteProcesado1.U_CTK_Observacion;
                                                    source.Add(respuestaGenericaLote1);
                                                }
                                            }
                                            else if (respuestaGenerica1.ErrMensaje.Contains("-1116"))
                                            {
                                                facturaResultModel = this._service.GetFacturaByOrdenVentaLote(ventaXloteProcesado1.DocEntry, CompanyDB);
                                                if (facturaResultModel != null && facturaResultModel.DocEntry != 0L)
                                                {
                                                    ventaXloteProcesado1.U_CTK_Observacion = string.Format("Se Forzó Guardar la Factura {0}. NO. {1}-{2}-{3} Pago:{4}", (object)facturaResultModel.DocNum, (object)facturaResultModel.U_SER_EST, (object)facturaResultModel.U_SER_PE, (object)facturaResultModel.FolioNumber, (object)facturaResultModel.U_EXX_FPAGO_VENTAS);
                                                    ventaXloteProcesado1.U_CTK_Generado = "S";
                                                    ventaXloteProcesado1.U_CTK_Lote = ordenFacturarLote.U_SER_EST + "-" + ordenFacturarLote.U_SER_PE;
                                                    OrdenesVentaXLoteProcesado ventaXloteProcesado9 = ventaXloteProcesado1;
                                                    long num9 = facturaResultModel.DocEntry;
                                                    string str13 = num9.ToString();
                                                    ventaXloteProcesado9.U_CTK_DocEntryRel = str13;
                                                    OrdenesVentaXLoteProcesado ventaXloteProcesado10 = ventaXloteProcesado1;
                                                    num9 = facturaResultModel.DocNum;
                                                    string str14 = num9.ToString();
                                                    ventaXloteProcesado10.U_CTK_DocNumRel = str14;
                                                    OrdenesVentaXLoteProcesado ventaXloteProcesado11 = ventaXloteProcesado1;
                                                    dateTime1 = DateTime.Now;
                                                    string str15 = dateTime1.ToString("dd-MM-yyyy HH:mm:ss");
                                                    ventaXloteProcesado11.U_CTK_FechaHoraGeneracion = str15;
                                                    ventaXloteProcesado1.DocNum = (long)ordenFacturarLote.DocNum;
                                                    if (string.IsNullOrEmpty(ventaCreateModel1.U_EXX_FPAGO_VENTAS))
                                                        respuestaGenerica1.Success = this._service.GuardarFormaPagoBySP(CompanyDB, orden);
                                                    this._service.ActualizaEstadoOrdenes(ventaXloteProcesado1, CompanyDB);
                                                    if (ventaXloteProcesado1.U_CTK_Generado == "S")
                                                        this._service.CerrarOrdenVenta(ventaXloteProcesado1.DocEntry, CompanyDB);
                                                    respuestaGenericaLote1 = new RespuestaGenericaLote();
                                                    respuestaGenericaLote1.DocEntryRel = ventaXloteProcesado1.DocEntry;
                                                    respuestaGenericaLote1.DocNumRel = ventaXloteProcesado1.DocNum;
                                                    respuestaGenericaLote1.TipoDocumento = orden.U_EXX_DOC_GEN;
                                                    respuestaGenericaLote1.Cliente = orden.CardCode + "-" + orden.CardName;
                                                    respuestaGenericaLote1.DocEntry = facturaResultModel.DocEntry;
                                                    respuestaGenericaLote1.DocNum = facturaResultModel.DocNum;
                                                    respuestaGenericaLote1.Success = true;
                                                    respuestaGenericaLote1.ErrMensaje = ventaXloteProcesado1.U_CTK_Observacion;
                                                    source.Add(respuestaGenericaLote1);
                                                }
                                            }
                                        }
                                        if (respuestaGenerica1.Success)
                                        {
                                            this._logger.Info(string.Format("OrdenVenta [{0}] Actualizando secuencial de Facturación electronica de Documentos Legales Internos.", (object)ordenFacturarLote.DocNum));
                                            ventaXloteProcesado1.U_CTK_Generado = "S";
                                            OrdenesVentaXLoteProcesado ventaXloteProcesado12 = ventaXloteProcesado1;
                                            num3 = facturaResultModel.DocEntry;
                                            string str16 = num3.ToString();
                                            ventaXloteProcesado12.CTK_DocEntryRel = str16;
                                            OrdenesVentaXLoteProcesado ventaXloteProcesado13 = ventaXloteProcesado1;
                                            num3 = facturaResultModel.DocNum;
                                            string str17 = num3.ToString();
                                            ventaXloteProcesado13.CTK_DocNumRel = str17;
                                            ventaXloteProcesado1.U_CTK_Lote = num2.ToString();
                                            OrdenesVentaXLoteProcesado ventaXloteProcesado14 = ventaXloteProcesado1;
                                            dateTime1 = DateTime.Now;
                                            string str18 = dateTime1.ToString("dd-MM-yyyy HH:mm:ss");
                                            ventaXloteProcesado14.U_CTK_FechaHoraGeneracion = str18;
                                            OrdenesVentaXLoteProcesado ventaXloteProcesado15 = ventaXloteProcesado1;
                                            object[] objArray = new object[7]
                                            {
                                                (object) facturaResultModel.U_SER_EST,
                                                (object) facturaResultModel.U_SER_PE,
                                                (object) facturaResultModel.FolioNumber,
                                                (object) facturaResultModel.DocNum,
                                                (object) facturaResultModel.DocEntry,
                                                (object) ventaXloteProcesado1.DocNum,
                                                null
                                            };
                                            dateTime1 = DateTime.Now;
                                            objArray[6] = (object)dateTime1.ToString("dd - MM - yyyy HH: mm: ss");
                                            string str19 = string.Format("Factura # {0}-{1}-{2} DocNum:{3} DocEntry: {4} OV REL: {5} -Procesada con Exito {6}", objArray);
                                            ventaXloteProcesado15.U_CTK_Observacion = str19;
                                            ventaXloteProcesado1.DocNum = (long)ordenFacturarLote.DocNum;
                                            if (this._service.GetOrdenesMaxSerieFactSSL(CompanyDB, orden.U_SerieFV) <= num8)
                                            {
                                                EXX_DOCUM_LEG_INTER exxDocumLegInter = new EXX_DOCUM_LEG_INTER();
                                                exxDocumLegInter.U_ULT_SECUEN = num8;
                                                RespuestaGenerica respuestaGenerica2 = this._service.ActualizaSecuenciaDocumentoElectronico(exxDocumLegInter, orden.U_CodeSerieId, CompanyDB);
                                                if (respuestaGenerica2.ErrMensaje.Contains("Unknown error") && this._service.GetOrdenesMaxSerieFactSSL(CompanyDB, orden.U_SerieFV) < num8 && this._service.ActualizaSecuenciaDocumentoElectronico(exxDocumLegInter, orden.U_CodeSerieId, CompanyDB).ErrMensaje.Contains("Unknown error"))
                                                {
                                                    ventaXloteProcesado1.U_CTK_Observacion = "Se detiene el proceso por error desconocido";
                                                    respuestaGenericaLote1.ErrMensaje = ordenFacturarLote.DocNum.ToString() + " -" + ventaXloteProcesado1.U_CTK_Observacion;
                                                    break;
                                                }
                                                if (!respuestaGenerica2.Success)
                                                {
                                                    ventaXloteProcesado1.U_CTK_Observacion += " - La serie del documento no pudo ser actualizada en la pantalla Documentos Internos";
                                                    break;
                                                }
                                                this._logger.Info(string.Format("OrdenVenta [{0}] actualizado Secuencial de Facturación electronica de Documentos Legales Internos.", (object)ordenFacturarLote.DocNum));
                                            }
                                        }
                                        else if (respuestaGenerica1 != null && !respuestaGenerica1.Success && !string.IsNullOrEmpty(respuestaGenerica1.ErrMensaje) && respuestaGenerica1.ErrMensaje.Contains("Unknown error"))
                                        {
                                            ventaXloteProcesado1.U_CTK_Observacion = "Se detiene el proceso por error desconocido";
                                            respuestaGenericaLote1.ErrMensaje = ordenFacturarLote.DocNum.ToString() + " -" + ventaXloteProcesado1.U_CTK_Observacion;
                                        }
                                    }
                                    else
                                    {
                                        this._logger.Info(string.Format("OrdenVenta [{0}] Se intentó generar secuencia duplicada [{1}], Verifique la parametrización de las series...", (object)ordenFacturarLote.DocNum, (object)num8));
                                        respuestaGenericaLote1 = new RespuestaGenericaLote();
                                        ventaXloteProcesado1.FolioNumber = (object)null;
                                        ventaXloteProcesado1.U_CTK_Observacion = "Se intento generar secuencia duplicada";
                                        ventaXloteProcesado1.U_CTK_Generado = "N";
                                    }
                                }
                                else
                                    this._logger.Info(string.Format("OrdenVenta [{0}] Tiene Observaciones {1}", (object)ordenFacturarLote.DocNum, (object)ventaXloteProcesado1.U_CTK_Observacion));
                                this._logger.Info(string.Format("OrdenVenta {0} Finalizó proceso.", (object)orden.DocNum));
                            }
                        }
                    }
                }
            }
            RespuestaGenericaLote respuestaGenericaLote4 = new RespuestaGenericaLote();
            respuestaGenericaLote4.DocEntryRel = 0L;
            respuestaGenericaLote4.DocNumRel = 0L;
            respuestaGenericaLote4.TipoDocumento = "Facturas";
            respuestaGenericaLote4.Cliente = "";
            respuestaGenericaLote4.DocEntry = 0L;
            respuestaGenericaLote4.DocNum = 0L;
            respuestaGenericaLote4.Success = false;
            respuestaGenericaLote4.ErrMensaje = string.Format("Se Procesaron [{0}] de {1}, Ordenes Sin Procesar.[{2} Finalizó a las {3}]", (object)(source.Count<RespuestaGenericaLote>((Func<RespuestaGenericaLote, bool>)(p => p.Success)) - 1), (object)pendienteXfacturar.Count<OrdenVentaXFacturar>(), (object)(source.Count<RespuestaGenericaLote>((Func<RespuestaGenericaLote, bool>)(p => !p.Success)) - 1), (object)DateTime.Now);
            source.Add(respuestaGenericaLote4);
            this._logger.Info(string.Format("Finalizando el Proceso de Generacion de Facturas a las {0}", (object)DateTime.Now));
            return source;
        }
    }

}