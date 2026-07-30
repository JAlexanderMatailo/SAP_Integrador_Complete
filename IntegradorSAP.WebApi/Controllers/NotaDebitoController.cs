using IntegradorSAP.Data.Entidades;
using IntegradorSAP.Data.Manager;
using IntegradorSAP.Data.Models;
using IntegradorSAP.Services.Manager;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IntegradorSAP.WebApi.Controllers
{
    [RoutePrefix("api/NotaDebitoLote")]

    public class NotaDebitoLoteController : ApiController
    {
        private Logger _logger = LogManager.GetLogger("DataLog");
        protected NotaDebitoLoteManager _service => new NotaDebitoLoteManager();
        protected FacturacionLoteManager _serviceFactLote => new FacturacionLoteManager();
        // protected CatalogosManager _serviceBP => new CatalogosManager();    

        //   [HttpGet]
        //   [Route("GetPedidosPendientesFactxurar")]
        [Route("GetPedidosPendientesFactxurar")]
        public List<OrdenVentaXNotaDebito> GetPedidosPendientesFactxurar(string CompanyDB)
        {
            //Se debe enviar los DocEntry de los pedidos a los que se desea buscar si tienen notaDebitos
            List<OrdenVentaXNotaDebito> obj = _service.GetOrdenesVentaPendienteXNotaDebito(CompanyDB);
            return obj;
        }

   
        [Route("PostActualizaOrdenesFactxuradas")]
        public List<RespuestaGenerica> PostActualizaOrdenesFactxuradas([FromBody] List<OrdenVentaXNotaDebito> Orden)
        {
            List<RespuestaGenerica> respuestas = new List<RespuestaGenerica>();
            RespuestaGenerica respuesta = new RespuestaGenerica();
            OrdenesVentaXLoteProcesadoNotaDebito value;
            foreach (var orden in Orden)
            {

                value = new OrdenesVentaXLoteProcesadoNotaDebito();
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


        [Route("ProcesaNotasDebitoEnLote")]
        public List<RespuestaGenericaLote> ProcesaNotasDebitoEnLote(string CompanyDB)
        {
            List<OrdenVentaXNotaDebito> Ordenes = new List<OrdenVentaXNotaDebito>();
            List<RespuestaGenericaLote> respuestas = new List<RespuestaGenericaLote>();
            RespuestaGenericaLote respuesta = new RespuestaGenericaLote();
            if (!_service.Login(CompanyDB))
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
            }
            else
            {
                _logger.Info($"Conexión exitosa con {CompanyDB} a las   {DateTime.Now}");

            }
            
            OrdenesVentaXLoteProcesadoNotaDebito value;

            _logger.Info("========================================================== ");
            _logger.Info($"Iniciando Proceso de Generacion de Notas de Debito {DateTime.Now}");
            try
            {
                Ordenes = _service.GetOrdenesVentaPendienteXNotaDebito(CompanyDB);

                if (Ordenes != null)
                {
                    respuesta = new RespuestaGenericaLote();
                    respuesta.DocEntryRel = 0;
                    respuesta.DocNumRel = 0;
                    respuesta.TipoDocumento = "NotasDebito";
                    respuesta.Cliente = "";
                    respuesta.DocEntry = 0;
                    respuesta.DocNum = 0;
                    respuesta.Success = false;
                    respuesta.ErrMensaje = $"Se encontraron {Ordenes.Count()} listas para ser Procesadas.Inicio a las  {DateTime.Now}";
                    respuestas.Add(respuesta);

                    _logger.Info(respuesta.ErrMensaje);
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
                respuesta.ErrMensaje = "Ocurrió un error al consultar las Ordenes pendientes de Notas de Débito,"+ex.Message.ToString();
               
                respuestas.Add(respuesta);
                _logger.Error(ex,respuesta.ErrMensaje);
                return respuestas;
            }
                       
            if (Ordenes.Count > 0)
            {

               
                    var Error = Ordenes.FirstOrDefault();

                if (Error.MensajeError != null)
                {
                    _logger.Error(Error.MensajeError);
                    respuesta.ErrMensaje = Error.MensajeError;
                    respuestas.Add(respuesta);
                }
                else
                {
                    long Lote = _serviceFactLote.GenerarSecuenciaLote(CompanyDB);
                    Lote = Lote + 1;
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
                            respuesta.ErrMensaje = "Ocurrió un error al consultar las Ordenes pendientes de Notas de Débito." ;

                            respuestas.Add(respuesta);
                            _logger.Error(respuesta.ErrMensaje);
                        }
                        try
                        {
                            _logger.Info($"=================Lote: {Lote}===============================================");

                            _logger.Info($"Procesando OV # {orden.DocNum}, DocEntrey {orden.DocEntry}");
                            Boolean NotaOk = false;

                            var FechaDocumento = orden.DocDate.AddHours(orden.HoraDocumento);
                            var FechaGeneracion = DateTime.Now;

                            FechaDocumento = FechaDocumento.AddMinutes(orden.MinutoDocumento + orden.MINUTOSAGREGADOS);


                            if (FechaDocumento.Month != FechaGeneracion.Month)
                            {
                                FechaDocumento = FechaGeneracion;
                            }

                            //Convert.ToDateTime(orden.DocDate.Substring(5,4)+"-"+ orden.DocDate.Substring(2, 2)+"-"+ orden.DocDate.Substring(0, 2)+" "+orden.HoraDocumento+":"+ orden.MinutoDocumento+":00");
                            value = new OrdenesVentaXLoteProcesadoNotaDebito();
                            value.U_CTK_Generado = "N";
                            value.U_CTK_Lote = "";
                            value.DocEntry = orden.DocEntry;


                            var existefactura = _service.GetNotaDebitoByOrdenVentaLote(orden.DocEntry, CompanyDB);
                            if (existefactura != null && existefactura.DocNum != 0)
                            {
                                value.U_CTK_Lote = Lote.ToString();
                                value.U_CTK_Generado = "S";
                                value.U_CTK_DocEntryRel = existefactura.DocEntry.ToString();
                                value.U_CTK_DocNumRel = existefactura.DocNum.ToString();
                                value.U_CTK_FechaHoraGeneracion = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                                value.U_CTK_Observacion = $"Nota debito de Venta ya tiene asociada esta orden de Venta {orden.DocNum}.";
                                var respues = _service.ActualizaEstadoOrdenes(value, CompanyDB);


                                _logger.Info(value.U_CTK_Observacion);
                                //CERRAR ORDEN DE VENTA
                                if (respues.Success && existefactura.DocNum != null)
                                {
                                    var OrdenCerrada = _service.GetOrdenNotaDebitoLote(orden.DocEntry, CompanyDB);
                                    if (OrdenCerrada != null && (OrdenCerrada.DocumentStatus == "bost_Open" || OrdenCerrada.DocumentStatus == "O"))
                                    {
                                        var respCierreDocVta = _service.CierraOrdenProcesada(value.DocEntry, CompanyDB);

                                        respuesta = new RespuestaGenericaLote();
                                        respuesta.DocEntryRel = value.DocEntry;
                                        respuesta.DocNumRel = value.DocNum;
                                        respuesta.TipoDocumento = orden.U_EXX_DOC_GEN;
                                        respuesta.Cliente = orden.CardCode + "-" + orden.CardName;
                                        respuesta.DocEntry = existefactura.DocEntry;
                                        respuesta.DocNum = existefactura.DocNum;
                                        respuesta.ErrMensaje = value.U_CTK_Observacion;

                                        respuesta.ErrMensaje = $"Se forzó cierre de OV No.{value.DocNum}" + value.U_CTK_Observacion;
                                        respuestas.Add(respuesta);

                                        _logger.Info(respuesta.ErrMensaje);
                                        continue;

                                    }
                                }
                            }
                            if (orden.CardCode is null || orden.CardCode == "")
                            {
                                value.U_CTK_Observacion = orden.DocNum + "  ADVERTENCIA: No se ha parametrizado el cliente, por favor acceda a la pantalla de parametrizaciones. Documento OV:" + orden.DocNum;
                            }
                            else if (orden.U_tipo_comprob == null || orden.U_tipo_comprob == "")
                            {
                                value.U_CTK_Observacion = orden.DocNum + "  ADVERTENCIA: No se ha configurado la serie de NotaDebito de Venta";
                            }
                            else if (orden.U_TIPO_ID == null || orden.U_TIPO_ID == "")
                            {
                                value.U_CTK_Observacion = orden.DocNum + "  ADVERTENCIA: No se ha configurado el tipo de comprobante para el cliente";
                            }
                            else if ((orden.CardName.Contains("seaboard") && orden.U_SerieFV != "FAE002"))
                            {
                                value.U_CTK_Observacion = orden.DocNum + "  ADVERTENCIA: El cliente Seaboard no tiene configurado FAE002";
                            }
                            else if (orden.U_SerieFV == null || orden.U_SerieFV == "")
                            {
                                value.U_CTK_Observacion = orden.DocNum + "  ADVERTENCIA: SerieND no esta definida para el cliente. Por favor ir a la pantala CTKPARAMFACTURAR";
                            }
                            else if (orden.U_tipo_comprob != "01" && orden.U_SerieFV == "FAE002")
                            {
                                value.U_CTK_Observacion = orden.DocNum + "  ADVERTENCIA: El tipo de comprobante debe ser igual a 01 para Seabord o Clientes del Exterior";
                            }
                            else if (orden.U_tipo_comprob != "18" && orden.U_SerieFV == "FAE006")
                            {
                                value.U_CTK_Observacion = orden.DocNum + "  ADVERTENCIA: El tipo de comprobante debe ser igual a 18 para notaDebitos locales";
                            }
                            else if (orden.U_TipoNotaDebito == "A")
                            {
                                value.U_CTK_Observacion = orden.DocNum + "  ADVERTENCIA: El cliente esta configurado con facturacion agrupada (U_TipoFacturacion = A): este proceso no genera notas de debito agrupadas.";
                            }
                            else if (FechaDocumento.Day < orden.U_FechaApertura || FechaDocumento.Day > orden.U_FechaCierre)
                            {
                                value.U_CTK_Observacion = orden.DocNum + "  ADVERTENCIA: El día del documento a generar está fuera del tiempo de Generación. Dia Inicio para el cliente: " + orden.U_FechaApertura.ToString()
                                    + " Dia Fin: " + orden.U_FechaCierre.ToString() + "    Dia Documento: " + orden.DiaNotaDebito.ToString();
                            }
                            else if (FechaDocumento > FechaGeneracion)
                            {
                                value.U_CTK_Observacion = orden.DocNum + "  ADVERTENCIA: El comprobante debe generarse después de " + FechaDocumento.ToString();
                            }
                            else if (orden.DocTotal <= 0)
                            {
                                value.U_CTK_Observacion = orden.DocNum + "  ADVERTENCIA: El valor total de la notaDebito no puede serss igual a 0 ";
                            }
                            else if (orden.U_Exx_FE_Paisdestin == "")
                            {
                                value.U_CTK_Observacion = orden.DocNum + "  ADVERTENCIA: No se ha asignado el codigo de pais en el maestro PAÍSES, campo ReportCode";
                            }


                            ///***********************************************************
                            ///*ACTUALIZAR ORDEN CON MENSAJERÍA Y CONTINUA A LA SIGUIENTE
                            ///***********************************************************
                            if (!string.IsNullOrEmpty(value.U_CTK_Observacion))
                            {
                                var respues = _service.ActualizaEstadoOrdenes(value, CompanyDB);

                                respuesta = new RespuestaGenericaLote();
                                respuesta.DocEntryRel = value.DocEntry;
                                respuesta.DocNumRel = value.DocNum;
                                respuesta.TipoDocumento = orden.U_EXX_DOC_GEN;
                                respuesta.Cliente = orden.CardCode + "-" + orden.CardName;
                                respuesta.DocEntry = 0;
                                respuesta.DocNum = 0;
                                respuesta.ErrMensaje = value.U_CTK_Observacion;
                                respuestas.Add(respuesta);

                                _logger.Info(value.U_CTK_Observacion);

                                continue;
                            }

                            if (string.IsNullOrEmpty(value.U_CTK_Observacion))
                            {
                                long DocNum = 0;

                                _logger.Info($"Obteniendo datos de OV {orden.DocNum}");
                                //En caso de no encontrar novedades, se procede a armar el documento de factuxracion
                                var ov = _service.GetOrdenNotaDebitoLote(orden.DocEntry, CompanyDB);
                                if (ov == null)
                                {
                                    respuesta = new RespuestaGenericaLote();
                                    respuesta.DocEntryRel = value.DocEntry;
                                    respuesta.DocNumRel = orden.DocNum;
                                    respuesta.TipoDocumento = orden.U_EXX_DOC_GEN;
                                    respuesta.Cliente = orden.CardCode + "-" + orden.CardName;
                                    respuesta.DocEntry = 0;
                                    respuesta.DocNum = 0;
                                    respuesta.ErrMensaje = $"Error! no se encontró la OV {orden.DocNum}.";
                                    respuestas.Add(respuesta);
                                    continue;
                                }
                                ov.U_REFRENDO_ANIO = DateTime.Now.Year.ToString();// ov.U_REFRENDO_ANIO;
                                ov.U_SER_EST = orden.U_SER_EST;
                                ov.U_SER_PE = orden.U_SER_PE;
                                ov.U_tipo_export = orden.U_tipo_export;
                                ov.U_FECHA_EMBARQUE = DateTime.Now;

                                var _json = JsonConvert.SerializeObject(ov);

                                NotaDebitoCreateModel notaDebito = JsonConvert.DeserializeObject<NotaDebitoCreateModel>(_json);
                                notaDebito.Series = orden.Series;
                                notaDebito.SummeryType = "dNoSummary";
                                notaDebito.TransportationCode = 9;
                                notaDebito.DiscountPercent = ov.DiscountPercent;
                                notaDebito.PaymentMethod = ov.PaymentMethod;
                                notaDebito.ContactPersonCode = orden.ContactPersonCode;
                                notaDebito.DocumentsOwner = ov.DocumentsOwner;
                                notaDebito.PickRemark = ov.PickRemark;
                                notaDebito.TotalDiscount = ov.TotalDiscount;
                                notaDebito.TotalDiscountFC = ov.TotalDiscountFC;
                                notaDebito.TotalDiscountSC = ov.TotalDiscountSC;
                                notaDebito.U_COD_ST = ov.U_COD_ST;
                                notaDebito.VatSum = ov.VatSum;
                                notaDebito.VatSumSys = ov.VatSumSys;
                                notaDebito.VatSumFc = ov.VatSumFc;
                                notaDebito.SalesPersonCode = orden.SalesPersonCode;
                                notaDebito.U_tipo_export = orden.U_tipo_export;
                                notaDebito.TrnspCode = ov.TrnspCode;
                                notaDebito.NumAtCard = ov.NumAtCard;
                                notaDebito.DocDate = DateTime.Now;
                                notaDebito.TaxDate = DateTime.Now;
                                notaDebito.DocDueDate = DateTime.Now.AddDays(orden.ExtraDays).AddMonths(orden.ExtraMonth);
                                notaDebito.U_NUM_AUTOR = orden.U_NUM_AUTOR;
                                notaDebito.U_TIP_DOC_APLIC = "01";
                                notaDebito.U_tipo_comprob = orden.U_tipo_comprob;
                                notaDebito.OpeningRemarks = ov.OpeningRemarks;
                                notaDebito.ClosingRemarks = ov.ClosingRemarks;
                                notaDebito.U_Exx_FE_Paisdestin = ov.U_Exx_FE_Paisdestin;
                                //notaDebito.U_SER_EST_FR = "001";
                                //notaDebito.U_SER_PEFR = "001";
                                notaDebito.U_NUM_FAC_REL = "123456789";
                                notaDebito.U_NUM_AUT_FR = "0123456789";
                                notaDebito.U_COD_ST = "--";
                                if (ov.CardName.Contains("Seab") && notaDebito.U_Exx_FE_Paisdestin == null)
                                {
                                    notaDebito.U_Exx_FE_Paisdestin = "110";
                                }
                                else
                                {
                                    notaDebito.U_Exx_FE_Paisdestin = orden.U_Exx_FE_Paisdestin;
                                }

                                notaDebito.U_EXX_MAN_AG = ov.U_EXX_MAN_AG;
                                notaDebito.U_EXX_TIPO_TRANSACC = ov.U_EXX_TIPO_TRANSACC;
                                notaDebito.U_EXX_DOC_GEN = ov.U_EXX_DOC_GEN;
                                notaDebito.U_Exx_ingFueGra_IR = "NO";
                                notaDebito.U_Exx_TipRegFis = "01";
                                notaDebito.U_Exx_TipIngExt = "439";
                                notaDebito.U_DOC_DECLARABLE = "N";
                                notaDebito.U_HRH_Lote = ov.U_HRH_Lote;
                                notaDebito.U_HRH_Modo_Fact = ov.U_HRH_Modo_Fact;
                                notaDebito.U_LocalCtaContab = ov.U_LocalCtaContab;
                                notaDebito.U_CTK_FechaHoraGeneracion = DateTime.Now.ToString();
                                notaDebito.JournalMemo = ov.JournalMemo;
                                notaDebito.U_VALOR_FOB = ov.DocTotal;
                                notaDebito.U_FECHA_EMBARQUE = DateTime.Now;
                                notaDebito.DocumentSubType = "bod_DebitMemo";
                                notaDebito.Comments = ov.Comments;
                                if (ov.Comments.Length > 50)
                                {
                                    notaDebito.JournalMemo = ov.Comments.Substring(0, 50);
                                }
                                else
                                {
                                    notaDebito.JournalMemo = ov.Comments;
                                }
                                notaDebito.TrnspCode = ov.TrnspCode;
                                notaDebito.U_VALOR_FOB = ov.DocTotal;
                                List<DocumentlineNotaVentaModel> DocumentLines = new List<DocumentlineNotaVentaModel>();
                                DocumentlineNotaVentaModel documentLine;
                                List<DocumentlineCreateVentaModel> DocumentLinesCreate = new List<DocumentlineCreateVentaModel>();
                                DocumentlineNotaVentaCreateModel documentLineCreate;
                                List<DocumentlineVtaLoteProcesadoNotaDebito> LstLine = new List<DocumentlineVtaLoteProcesadoNotaDebito>();
                                DocumentlineVtaLoteProcesadoNotaDebito line;
                                int lineid = 0;
                                foreach (var ItemDet in ov.DocumentLines)
                                {
                                    if (ItemDet.LineTotal > 0)
                                    {
                                        line = new DocumentlineVtaLoteProcesadoNotaDebito();
                                        documentLine = new DocumentlineNotaVentaModel();
                                        documentLineCreate = new DocumentlineNotaVentaCreateModel();
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

                                        line.BaseEntry = ov.DocEntry.ToString();
                                        line.BaseType = "17";
                                        line.BaseLine = ItemDet.LineNum.ToString();
                                        if ((documentLine.AccountCode == null || documentLine.AccountCode == "") && value.U_CTK_Observacion == null)
                                        {
                                            value.U_CTK_Observacion = value.DocEntry + "  Advertencia: El item " + ItemDet.ItemCode + " - No tiene asignado una cuenta";
                                        }
                                        if (value.U_CTK_Observacion == null)
                                        {
                                            documentLine.LineTotal = ItemDet.LineTotal;
                                            documentLine.ItemCode = ItemDet.ItemCode;
                                            documentLine.LineNum = lineid;
                                            documentLine.Quantity = ItemDet.Quantity;
                                            documentLine.ShipDate = ItemDet.ShipDate;
                                            documentLine.BarCode = ItemDet.BarCode;
                                            documentLine.VatGroup = ItemDet.VatGroup;
                                            documentLine.PriceAfterVAT = ItemDet.PriceAfterVAT;
                                            documentLine.Currency = ItemDet.Currency;
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
                                            documentLine.TaxCode = "IVA_EXE";
                                            documentLine.Price = ItemDet.Price;
                                            documentLine.UnitPrice = ItemDet.UnitPrice;
                                            documentLine.WarehouseCode = ItemDet.WarehouseCode;
                                            //documentLine.BaseType = "15";
                                            //documentLine.BaseEntry = ov.DocEntry.ToString();
                                            //documentLine.BaseLine = ItemDet.LineNum.ToString();

                                            line.BaseQty = ItemDet.Quantity;
                                            line.TargetType = "13";
                                            line.TrgetEntry = ov.DocEntry.ToString();

                                            DocumentLines.Add(documentLine);
                                            LstLine.Add(line);
                                        }
                                        lineid = lineid + 1;
                                    }
                                }
                                value.Line = LstLine;
                                value.BaseDocNum = ov.DocNum.ToString();
                                value.U_CTK_FechaHoraGeneracion = DateTime.Now.ToString();
                                value.DocNum = ov.DocNum;

                                respuesta = new RespuestaGenericaLote();
                                NotaOk = false;
                                if (value.U_CTK_Observacion == null)
                                {
                                    notaDebito.DocumentLines = DocumentLines;

                                    notaDebito.U_CTK_DocEntryRel = ov.DocEntry.ToString();
                                    notaDebito.U_CTK_DocNumRel = ov.DocNum.ToString();
                                    notaDebito.U_CTK_FechaHoraGeneracion = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

                                    var validaNotaDebito = _service.GetNotaDebitoByOrdenVentaLote(orden.DocEntry, CompanyDB);

                                    if (validaNotaDebito != null)
                                    {
                                        value.U_CTK_Generado = "S";
                                        value.U_CTK_Lote = Lote.ToString();
                                        value.U_CTK_Observacion = $"Se forzó cierre de OV {orden.DocNum} con  NotaDebito # { validaNotaDebito.DocNum }  DocEntry {validaNotaDebito.DocEntry} Procesada con Exito";
                                        value.U_CTK_DocEntryRel = validaNotaDebito.DocEntry.ToString();
                                        value.U_CTK_DocNumRel = validaNotaDebito.DocNum.ToString();
                                        value.U_CTK_FechaHoraGeneracion = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                                        value.DocNum = ov.DocNum;


                                        var respue = _service.ActualizaEstadoOrdenes(value, CompanyDB);
                                        if (respue.Success)
                                        {
                                            var respCierreDocVta = _service.CierraOrdenProcesada(value.DocEntry, CompanyDB);
                                        }


                                        respuesta = new RespuestaGenericaLote();
                                        respuesta.DocEntryRel = value.DocEntry;
                                        respuesta.DocNumRel = orden.DocNum;
                                        respuesta.TipoDocumento = orden.U_EXX_DOC_GEN;
                                        respuesta.Cliente = orden.CardCode + "-" + orden.CardName;
                                        respuesta.DocEntry = 0;
                                        respuesta.DocNum = 0;
                                        respuesta.Success = false;
                                        respuesta.ErrMensaje = $"Esta OV {orden.DocNum} ya fue asignada a una ND # {validaNotaDebito.DocNum} DocEntry: {validaNotaDebito.DocEntry}";
                                        respuestas.Add(respuesta);
                                        continue;
                                    }

                                    var resp = _service.GuardarNotaDebito(notaDebito, CompanyDB);
                                    if (resp.Success)
                                    {

                                        NotaOk = true;
                                        var facturaGenerada = JsonConvert.DeserializeObject<NotaDebitoRespCreateModel>(resp.RespuestaJson);
                                        DocNum = facturaGenerada.DocNum;


                                        _logger.Info(value.U_CTK_Observacion);
                                        //Actualizo los datos de la orden con el numero de notaDebito y pago
                                        value.U_CTK_Generado = "S";
                                        value.U_CTK_Lote = Lote.ToString();
                                        value.U_CTK_Observacion = $"NotaDebito # { facturaGenerada.DocNum }  DocEntry {facturaGenerada.DocEntry} Procesada con Exito";
                                        value.U_CTK_DocEntryRel = facturaGenerada.DocEntry.ToString();
                                        value.U_CTK_DocNumRel = facturaGenerada.DocNum.ToString();
                                        value.U_CTK_FechaHoraGeneracion = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                                        value.DocNum = ov.DocNum;

                                        _logger.Info($"Actualizando status de OV # {value.DocNum}");

                                        var respue = _service.ActualizaEstadoOrdenes(value, CompanyDB);

                                        
                                        var respCierreDocVta = _service.CierraOrdenProcesada(value.DocEntry, CompanyDB);

                                         _logger.Info($"Cerrada OV # {value.DocNum}");


                                        respuesta = new RespuestaGenericaLote();
                                        respuesta.DocEntryRel = ov.DocEntry;
                                        respuesta.DocNumRel = ov.DocNum;
                                        respuesta.TipoDocumento = orden.U_EXX_DOC_GEN;
                                        respuesta.Cliente = orden.CardCode + "-" + orden.CardName;
                                        respuesta.DocEntry = facturaGenerada.DocEntry;
                                        respuesta.DocNum = facturaGenerada.DocNum;
                                        respuesta.Success = true;
                                        respuesta.ErrMensaje = value.U_CTK_Observacion;
                                        respuesta.U_EXX_FPAGO_VENTAS = facturaGenerada.U_EXX_FPAGO_VENTAS;
                                        respuestas.Add(respuesta);
                                    }
                                    else
                                    {
                                        //Se usa esta validación cuando sale un error al Grabar pero de todas maneras se guarda la Nota de débito.
                                        var facturaGenerada = _service.GetNotaDebitoByOrdenVentaLote(value.DocEntry, CompanyDB);
                                        if (facturaGenerada != null && facturaGenerada.DocEntry != 0)
                                        {
                                            NotaOk = true;
                                            value.U_CTK_Generado = "S";
                                            value.U_CTK_Lote = ov.U_SER_EST + "-" + ov.U_SER_PE;
                                            value.U_CTK_Observacion = $"NotaDebito # { facturaGenerada.DocNum }  DocEntry {facturaGenerada.DocEntry} Procesada con Exito. Se Forzó actualización de la orden de venta.";
                                            value.U_CTK_DocEntryRel = facturaGenerada.DocEntry.ToString();
                                            value.U_CTK_DocNumRel = facturaGenerada.DocNum.ToString();
                                            value.U_CTK_FechaHoraGeneracion = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                                            value.DocNum = ov.DocNum;
                                            _logger.Info(value.U_CTK_Observacion);


                                            _logger.Info($"Actualizando status de OV Forzada # {value.DocNum}");

                                            var respue = _service.ActualizaEstadoOrdenes(value, CompanyDB);

                                            if (respue.Success)
                                            {
                                                var respCierreDocVta = _service.CierraOrdenProcesada(value.DocEntry, CompanyDB);

                                                _logger.Info($"Cerrada OV Forzando # {value.DocNum}");
                                            }

                                            respuesta = new RespuestaGenericaLote();
                                            respuesta.DocEntryRel = value.DocEntry;
                                            respuesta.DocNumRel = value.DocNum;
                                            respuesta.TipoDocumento = orden.U_EXX_DOC_GEN;
                                            respuesta.Cliente = orden.CardCode + "-" + orden.CardName;
                                            respuesta.DocEntry = facturaGenerada.DocEntry;
                                            respuesta.DocNum = facturaGenerada.DocNum;
                                            respuesta.Success = true;
                                            respuesta.ErrMensaje = value.U_CTK_Observacion;
                                            respuesta.U_EXX_FPAGO_VENTAS = facturaGenerada.U_EXX_FPAGO_VENTAS;
                                            respuestas.Add(respuesta);

                                        }
                                        else
                                        {
                                            //Dio Error General
                                            
                                            value.U_CTK_Generado = "N";
                                            value.U_CTK_Observacion = resp.ErrMensaje;//.Split(':')[7].Replace("}", "");
                                            respuesta.ErrMensaje = ov.DocNum + " -" + value.U_CTK_Observacion;
                                            _logger.Info(value.U_CTK_Observacion);

                                            var respue = _service.ActualizaEstadoOrdenes(value, CompanyDB);

                                            _logger.Error($"Actualizando status de OV Error al  Generar Factura # {value.DocNum}.  Error {resp.ErrMensaje}");


                                            respuesta = new RespuestaGenericaLote();
                                            respuesta.DocEntryRel = value.DocEntry;
                                            respuesta.DocNumRel = value.DocNum;
                                            respuesta.TipoDocumento = orden.U_EXX_DOC_GEN;
                                            respuesta.Cliente = orden.CardCode + "-" + orden.CardName;
                                            respuesta.DocEntry = 0;
                                            respuesta.DocNum = 0;
                                            respuesta.Success = false;
                                            respuestas.Add(respuesta);
                                        }
                                    }
                                }
                            }

                            //respuesta = new RespuestaGenericaLote();
                            if (value.DocNum == 0)
                            {
                                value.DocNum = orden.DocNum;
                            }
                            _logger.Info($"Terminando Carga de  OV# {value.DocNum}");

                            //if (NotaOk)
                            //{
                            //    _logger.Info($"Actualizando status de OV # {value.DocNum}");

                            //    var respue = _service.ActualizaEstadoOrdenes(value, CompanyDB);

                            //    if (respue.Success && NotaOk)
                            //    {
                            //        var respCierreDocVta = _service.CierraOrdenProcesada(value.DocEntry, CompanyDB);

                            //        _logger.Info($"Cerrada OV # {value.DocNum}");

                            //    }
                            //}

                            //if (value.U_CTK_Observacion != null )
                            //{
                            //    respuesta.ErrMensaje = "Documento OV: " + value.DocNum + " " + value.U_CTK_Observacion;
                            //}

                        }
                        catch (Exception ex)
                        {
                            respuesta = new RespuestaGenericaLote();
                            respuesta.DocEntryRel = 0;
                            respuesta.DocNumRel = 0;
                            respuesta.TipoDocumento = "NotasDebito";
                            respuesta.Cliente = "";
                            respuesta.DocEntry = 0;
                            respuesta.DocNum = 0;
                            respuesta.Success = false;
                            respuesta.ErrMensaje = $"Ocurrió un error al procesar la orden {orden.DocNum} DocEntry {orden.DocEntry}, Exception:{ex.Message}";
                            respuestas.Add(respuesta);
                            _logger.Error(ex, respuesta.ErrMensaje);
                        }

                    }
                }                
            }

            respuesta = new RespuestaGenericaLote();
            respuesta.DocEntryRel = 0;
            respuesta.DocNumRel = 0;
            respuesta.TipoDocumento = "NotasDebito";
            respuesta.Cliente = "";
            respuesta.DocEntry = 0;
            respuesta.DocNum = 0;
            respuesta.Success = false;
            respuesta.ErrMensaje = $"Se Procesaron [{(respuestas.Count(p => p.Success == true && p.DocNumRel != 0) - 1)}] de {Ordenes.Count()} Ordenes Sin Procesar.[{(respuestas.Count(p => p.Success != true) - 1)} Inicio a las  {DateTime.Now}]";
            respuestas.Add(respuesta);

            _logger.Info($"Finalizando el Proceso de Generacion de Notas de Debito  a las {DateTime.Now}");

            return respuestas;

        }
      
    }
}
