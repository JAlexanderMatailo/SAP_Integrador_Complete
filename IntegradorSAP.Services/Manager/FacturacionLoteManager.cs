

using IntegradorSAP.Data.Entidades;
using IntegradorSAP.Data.Helper;
using IntegradorSAP.Data.Manager;
using IntegradorSAP.Data.Models;
using Newtonsoft.Json;
using NLog;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;


namespace IntegradorSAP.Services.Manager
{
    public class FacturacionLoteManager : BaseManager
    {
        private Logger _logger = LogManager.GetLogger("DataLog");
        private ServiceLayer_Web servicio = new ServiceLayer_Web();
        private string Usuario;
        private string Ip;
        private string errorCode;
        private string errMsg;

        public string Mensaje { get; set; }

        public string ErrMsg
        {
            get => this.ErrMsg;
            set => this.errMsg = value;
        }

        public string ErrorCode
        {
            get => this.ErrorCode;
            set => this.errorCode = value;
        }

        public new bool Login(string CompanyDB)
        {
            if (!this.servicio.IsConected)
                this.servicio.ConectarSL(CompanyDB);
            return this.servicio.IsConected;
        }

        public List<OrdenVentaXFacturar> GetOrdenesVentaPendienteXFacturar(string CompanyDB)
        {
            HanaCommand comm = (HanaCommand)null;
            HanaDataReader reader = (HanaDataReader)null;
            List<OrdenVentaXFacturar> pendienteXfacturar1 = new List<OrdenVentaXFacturar>();
            OrdenVentaXFacturar ordenVentaXfacturar1 = new OrdenVentaXFacturar();
            try
            {
                this.conectarHana(CompanyDB);
                comm = new HanaCommand("SELECT  top 50 * FROM  \"" + CompanyDB + "\".\"CTK_GET_PEDIDOS_PENDIENTES_FACTURAR_VIEW\"  WHERE \"U_SER_PE\" is not null ", this.Connection);
                reader = comm.ExecuteReader();
                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                OrdenVentaXFacturar ordenVentaXfacturar2 = new OrdenVentaXFacturar();
                                ordenVentaXfacturar2.DocEntry = Convert.ToInt64(reader.GetValue(0));
                                ordenVentaXfacturar2.DocNum = Convert.ToInt64(reader.GetValue(1));
                                ordenVentaXfacturar2.DocType = Convert.ToString(reader.GetValue(2));
                                ordenVentaXfacturar2.DocDate = Convert.ToDateTime(reader.GetValue(3));
                                ordenVentaXfacturar2.U_SerieFV = Convert.ToString(reader.GetValue(4));
                                ordenVentaXfacturar2.CardCode = Convert.ToString(reader.GetValue(5));
                                ordenVentaXfacturar2.U_SerieId = Convert.ToString(reader.GetValue(8));
                                ordenVentaXfacturar2.U_CodeSerieId = Convert.ToString(reader.GetValue(11));
                                ordenVentaXfacturar2.DiaFactura = Convert.ToInt32(reader.GetValue(15));
                                if (reader.GetValue(6).ToString() != "")
                                    ordenVentaXfacturar2.U_SER_EST = reader.GetValue(6).ToString();
                                if (reader.GetValue(7).ToString() != "")
                                    ordenVentaXfacturar2.U_SER_PE = reader.GetValue(7).ToString();
                                if (reader.GetValue(8).ToString() != "")
                                    ordenVentaXfacturar2.Series = (long)Convert.ToInt32(reader.GetValue(8));
                                if (reader.GetValue(9).ToString() != "")
                                    ordenVentaXfacturar2.U_tipo_comprob = reader.GetValue(9).ToString();
                                if (reader.GetValue(10).ToString() != "")
                                    ordenVentaXfacturar2.U_NUM_AUTOR = reader.GetValue(10).ToString();
                                if (reader.GetValue(13).ToString() != "")
                                    ordenVentaXfacturar2.ExtraMonth = Convert.ToInt32(reader.GetValue(13));
                                if (reader.GetValue(14).ToString() != "")
                                    ordenVentaXfacturar2.ExtraDays = Convert.ToInt32(reader.GetValue(14));
                                if (reader.GetValue(16).ToString() != "")
                                    ordenVentaXfacturar2.U_FechaApertura = Convert.ToInt32(reader.GetValue(16));
                                if (reader.GetValue(17).ToString() != "")
                                    ordenVentaXfacturar2.U_FechaCierre = Convert.ToInt32(reader.GetValue(17));
                                ordenVentaXfacturar2.U_TIPO_ID = Convert.ToString(reader.GetValue(18));
                                ordenVentaXfacturar2.CardName = Convert.ToString(reader.GetValue(19));
                                if (reader.GetValue(20).ToString() != "")
                                    ordenVentaXfacturar2.MINUTOSAGREGADOS = Convert.ToInt32(reader.GetValue(20));
                                ordenVentaXfacturar2.U_TipoFacturacion = reader.GetValue(21).ToString();
                                ordenVentaXfacturar2.U_EXX_DOC_GEN = reader.GetValue(22).ToString();
                                if (reader.GetValue(23).ToString() != "")
                                    ordenVentaXfacturar2.HoraDocumento = Convert.ToInt32(reader.GetValue(23));
                                if (reader.GetValue(24).ToString() != "")
                                    ordenVentaXfacturar2.MinutoDocumento = Convert.ToInt32(reader.GetValue(24));
                                if (reader.GetValue(25).ToString() != "")
                                    ordenVentaXfacturar2.U_tipo_export = Convert.ToString(reader.GetValue(25));
                                if (reader.GetValue(26).ToString() != "")
                                    ordenVentaXfacturar2.DocTotal = Convert.ToDouble(reader.GetValue(26));
                                if (reader.GetValue(27).ToString() != "")
                                    ordenVentaXfacturar2.SalesPersonCode = (int)Convert.ToInt16(reader.GetValue(27).ToString());
                                if (reader.GetValue(28).ToString() != "")
                                    ordenVentaXfacturar2.U_Exx_FE_Paisdestin = Convert.ToString(reader.GetValue(28).ToString());
                                if (reader.GetValue(29).ToString() != "")
                                    ordenVentaXfacturar2.U_Exx_FE_PuertoDest = Convert.ToString(reader.GetValue(29).ToString());
                                pendienteXfacturar1.Add(ordenVentaXfacturar2);

                                
                            }
                            catch (Exception ex)
                            {
                                this._logger.Error(ex, "GetOrdenesVentaPendienteXFacturar:" + ex.Message + "," + ex.StackTrace);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                List<OrdenVentaXFacturar> pendienteXfacturar2 = new List<OrdenVentaXFacturar>();
                OrdenVentaXFacturar ordenVentaXfacturar3 = new OrdenVentaXFacturar();
                this.ErrorMensaje = ex.Message;
                ordenVentaXfacturar3.MensajeError = this.ErrorMensaje;
                pendienteXfacturar2.Add(ordenVentaXfacturar3);
                this._logger.Error("GetOrdenesVentaPendienteXFacturar:" + this.ErrorMensaje);
                return pendienteXfacturar2;
            }
            finally
            {
                this.LiberarVariables(ref this.Connection, ref comm, ref reader);
            }
            return pendienteXfacturar1;
        }

        public long GetOrdenesMaxCodePago(string CompanyDB)
        {
            long ordenesMaxCodePago = 0;
            HanaCommand comm = (HanaCommand)null;
            HanaDataReader reader = (HanaDataReader)null;
            try
            {
                this.conectarHana(CompanyDB);
                comm = new HanaCommand("SELECT (max(T0.\"DocEntry\" )+1) as \"MaxCode\"  FROM  \"" + CompanyDB + "\".\"@EXX_FPAGO_VENTAS\" T0", this.Connection);
                reader = comm.ExecuteReader();
                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                            ordenesMaxCodePago = Convert.ToInt64(reader.GetValue(0));
                    }
                }
            }
            catch (Exception ex)
            {
                this.ErrorMensaje = ex.Message;
            }
            finally
            {
                this.LiberarVariables(ref this.Connection, ref comm, ref reader);
            }
            return ordenesMaxCodePago;
        }

        public long GetOrdenesMaxCodePagoOINV(string CompanyDB)
        {
            long ordenesMaxCodePagoOinv = 0;
            HanaCommand comm = (HanaCommand)null;
            HanaDataReader reader = (HanaDataReader)null;
            try
            {
                this.conectarHana(CompanyDB);
                comm = new HanaCommand("SELECT (max(cast(T0.\"U_EXX_FPAGO_VENTAS\" as int) )+1) as \"MaxCode\"  FROM  \"" + CompanyDB + "\".\"OINV\" T0 order by 1 desc", this.Connection);
                reader = comm.ExecuteReader();
                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                            ordenesMaxCodePagoOinv = !(reader.GetValue(0).ToString() != "") ? 0L : Convert.ToInt64(reader.GetValue(0));
                    }
                }
            }
            catch (Exception ex)
            {
                this.ErrorMensaje = ex.Message;
            }
            finally
            {
                this.LiberarVariables(ref this.Connection, ref comm, ref reader);
            }
            return ordenesMaxCodePagoOinv;
        }

        public long GetOrdenesMaxSerieFact(string CompanyDB, string Nombre)
        {
            long ordenesMaxSerieFact = 0;
            HanaCommand comm = (HanaCommand)null;
            HanaDataReader reader = (HanaDataReader)null;
            try
            {
                this.conectarHana(CompanyDB);
                comm = new HanaCommand("SELECT (max(T0.\"U_ULT_SECUEN\" )+1) as \"U_ULT_SECUEN\"  FROM  \"" + CompanyDB + "\".\"@EXX_DOCUM_LEG_INTER\" T0" + " WHERE \"U_NOMBRE\" in ('" + Nombre + "')", this.Connection);
                reader = comm.ExecuteReader();
                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                            ordenesMaxSerieFact = Convert.ToInt64(reader.GetValue(0));
                    }
                }
            }
            catch (Exception ex)
            {
                this.ErrorMensaje = ex.Message;
            }
            finally
            {
                this.LiberarVariables(ref this.Connection, ref comm, ref reader);
            }
            return ordenesMaxSerieFact;
        }

        public long GetOrdenesMaxSerieFactSSL(string CompanyDB, string NombreSerie)
        {
            if (!this.servicio.IsConected)
                this.servicio.ConectarSL(CompanyDB);
            if (!this.servicio.IsConected)
                return 0;
            RespuestaGenerica respuestaGenerica = this.servicio.SLSendRequestReturnResponse("EXX_DOCUM_LEG_INTER?$apply=aggregate(U_ULT_SECUEN with max as MaxSecuencial)&$filter = U_NOMBRE eq '" + NombreSerie + "'", "GET", "", HttpStatusCode.OK, false);
            if (respuestaGenerica.Success)
            {
                long ordenesMaxSerieFactSsl = Convert.ToInt64(((IEnumerable<MaxSecuencialViewModel>)JsonConvert.DeserializeObject<MaxSecuencialListViewModel>(respuestaGenerica.RespuestaJson).value).First<MaxSecuencialViewModel>().MaxSecuencial) + 1L;
                this._logger.Info(string.Format("GetOrdenesMaxSerieFactSSL:SecuencialFacturaMax:{0}", (object)ordenesMaxSerieFactSsl));
                return ordenesMaxSerieFactSsl;
            }
            this.ErrMsg = string.Format("Error: {0}", (object)respuestaGenerica.ErrCodigo) + respuestaGenerica.ErrMensaje + "." + (object)respuestaGenerica.ErrException;
            this._logger.Error("GetOrdenesMaxSerieFactSSL:" + this.ErrMsg);
            return 0;
        }

        public long GetMaxSerieOINV(
          string CompanyDB,
          string prefijo,
          string establecimiento,
          string emision)
        {
            long maxSerieOinv = 0;
            HanaCommand comm = (HanaCommand)null;
            HanaDataReader reader = (HanaDataReader)null;
            try
            {
                this.conectarHana(CompanyDB);
                comm = new HanaCommand("SELECT max(T0.\"FolioNum\")+1 as \"Code\"  FROM  \"" + CompanyDB + "\".\"view_FolioFact\" T0" + " WHERE  T0.\"U_SER_EST\" like '" + establecimiento + "'  and \"U_SER_PE\"like '" + emision + "'  ", this.Connection);
                reader = comm.ExecuteReader();
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                        maxSerieOinv = Convert.ToInt64(reader.GetValue(0));
                }
                return maxSerieOinv;
            }
            catch (Exception ex)
            {
                maxSerieOinv = 0L;
                this.ErrorMensaje = ex.Message;
            }
            finally
            {
                this.LiberarVariables(ref this.Connection, ref comm, ref reader);
            }
            return maxSerieOinv;
        }

        public long InsertPago(
          string CompanyDB,
          string codigo,
          string referencia,
          Decimal valor,
          long ov)
        {
            long num = 0;
            HanaCommand comm = (HanaCommand)null;
            HanaDataReader reader = (HanaDataReader)null;
            try
            {
                this.conectarHana(CompanyDB);
                string cmdText = "call \"" + CompanyDB + "\".\"SP_INSERT_PAGO_CTKFact\" (:code,:referencia,:valor,:OV)" + " ";
                List<HanaParameter> hanaParameterList = new List<HanaParameter>();
                HanaParameter hanaParameter1 = new HanaParameter();
                hanaParameter1.ParameterName = ":code";
                hanaParameter1.HanaDbType = HanaDbType.NVarChar;
                hanaParameter1.Value = (object)codigo;
                hanaParameterList.Add(hanaParameter1);
                HanaParameter hanaParameter2 = new HanaParameter();
                hanaParameter2.ParameterName = ":referencia";
                hanaParameter2.HanaDbType = HanaDbType.NVarChar;
                hanaParameter2.Value = (object)referencia;
                hanaParameterList.Add(hanaParameter2);
                HanaParameter hanaParameter3 = new HanaParameter();
                hanaParameter3.ParameterName = ":valor";
                hanaParameter3.HanaDbType = HanaDbType.Decimal;
                hanaParameter3.Value = (object)valor;
                hanaParameterList.Add(hanaParameter3);
                HanaParameter hanaParameter4 = new HanaParameter();
                hanaParameter4.ParameterName = ":OV";
                hanaParameter4.HanaDbType = HanaDbType.BigInt;
                hanaParameter4.Value = (object)ov;
                hanaParameterList.Add(hanaParameter4);
                comm = new HanaCommand(cmdText, this.Connection);
                comm.Parameters.AddRange(hanaParameterList.ToArray());
                reader = comm.ExecuteReader();
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                        num = Convert.ToInt64(reader.GetValue(0));
                }
                return num;
            }
            catch (Exception ex)
            {
                num = 0L;
                this.ErrorMensaje = ex.Message;
            }
            finally
            {
                this.LiberarVariables(ref this.Connection, ref comm, ref reader);
            }
            return num;
        }

        public long GetSerieDuplicadaSSL(
          string CompanyDB,
          string folio,
          string prefijo,
          string establecimiento,
          string emision)
        {
            if (!this.servicio.IsConected)
                this.servicio.ConectarSL(CompanyDB);
            if (!this.servicio.IsConected)
                return 0;
            RespuestaGenerica respuestaGenerica = this.servicio.SLSendRequestReturnResponse("Invoices?$apply=aggregate(DocEntry with countdistinct  as MaxSecuencial )&$filter = FolioNumber eq " + folio + " and U_SER_EST eq '" + establecimiento + "'  and U_SER_PE eq '" + emision + "'  ", "GET", "", HttpStatusCode.OK, false);
            if (respuestaGenerica.Success)
            {
                MaxSecuencialListViewModel secuencialListViewModel = JsonConvert.DeserializeObject<MaxSecuencialListViewModel>(respuestaGenerica.RespuestaJson);
                if (((IEnumerable<MaxSecuencialViewModel>)secuencialListViewModel.value).Count<MaxSecuencialViewModel>() <= 0)
                    return 0;
                long serieDuplicadaSsl = Convert.ToInt64(((IEnumerable<MaxSecuencialViewModel>)secuencialListViewModel.value).First<MaxSecuencialViewModel>().MaxSecuencial) + 1L;
                this._logger.Info(string.Format("GetSerieDuplicadaSSL:CountDistinctFolioNumber:{0}", (object)serieDuplicadaSsl));
                return serieDuplicadaSsl;
            }
            this.ErrMsg = string.Format("Error: {0}", (object)respuestaGenerica.ErrCodigo) + respuestaGenerica.ErrMensaje + "." + (object)respuestaGenerica.ErrException;
            this._logger.Error("GetSerieDuplicadaSSL:" + this.ErrMsg);
            return 0;
        }

        public long GetSerieDuplicada(
          string CompanyDB,
          string folio,
          string prefijo,
          string establecimiento,
          string emision)
        {
            long serieDuplicada = 0;
            HanaCommand comm = (HanaCommand)null;
            HanaDataReader reader = (HanaDataReader)null;
            try
            {
                this.conectarHana(CompanyDB);
                comm = new HanaCommand("SELECT 1  FROM  \"" + CompanyDB + "\".\"view_FolioFact\" T0" + " WHERE T0.\"FolioNum\" like '" + folio + "' and T0.\"U_SER_EST\" like '" + establecimiento + "'  and \"U_SER_PE\"like '" + emision + "'  ", this.Connection);
                reader = comm.ExecuteReader();
                return reader != null && reader.HasRows ? 1L : 0L;
            }
            catch (Exception ex)
            {
                this.ErrorMensaje = ex.Message;
            }
            finally
            {
                this.LiberarVariables(ref this.Connection, ref comm, ref reader);
            }
            return serieDuplicada;
        }

        public FactGenerada GetOINVPreviamenteGenerada(string CompanyDB, string DocEntry)
        {
            HanaCommand comm = (HanaCommand)null;
            HanaDataReader reader = (HanaDataReader)null;
            FactGenerada previamenteGenerada = new FactGenerada();
            previamenteGenerada.DocEntry = 0L;
            previamenteGenerada.DocNum = 0L;
            previamenteGenerada.FolioNum = 0L;
            previamenteGenerada.Pago = 0L;
            try
            {
                this.conectarHana(CompanyDB);
                comm = new HanaCommand("SELECT T0.\"FDocEntry\", T0.\"FDocNum\", T0.\"FFolioNum\", T0.\"U_EXX_FPAGO_VENTAS\"   FROM  \"" + CompanyDB + "\".\"View_VerificaOINVGenerado\" T0" + " WHERE  T0.\"ODocEntry\" = " + DocEntry, this.Connection);
                reader = comm.ExecuteReader();
                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            previamenteGenerada.DocEntry = Convert.ToInt64(reader.GetValue(0));
                            previamenteGenerada.DocNum = Convert.ToInt64(reader.GetValue(1));
                            previamenteGenerada.FolioNum = Convert.ToInt64(reader.GetValue(2));
                            previamenteGenerada.Pago = !(reader.GetValue(3).ToString() == "") ? Convert.ToInt64(reader.GetValue(3)) : 0L;
                        }
                    }
                }
                else
                {
                    previamenteGenerada.DocEntry = 0L;
                    previamenteGenerada.DocNum = 0L;
                    previamenteGenerada.FolioNum = 0L;
                    previamenteGenerada.Pago = 0L;
                }
                return previamenteGenerada;
            }
            catch (Exception ex)
            {
                previamenteGenerada.DocEntry = 0L;
                previamenteGenerada.DocNum = 0L;
                previamenteGenerada.FolioNum = 0L;
                this.ErrorMensaje = ex.Message;
            }
            finally
            {
                this.LiberarVariables(ref this.Connection, ref comm, ref reader);
            }
            return previamenteGenerada;
        }

        public string GetOINVPreviamenteCerrada(string CompanyDB, string DocEntry)
        {
            string previamenteCerrada = "";
            HanaCommand comm = (HanaCommand)null;
            HanaDataReader reader = (HanaDataReader)null;
            try
            {
                this.conectarHana(CompanyDB);
                comm = new HanaCommand("SELECT T0.\"ODocStatus\"  FROM  \"" + CompanyDB + "\".\"View_VerificaOINVGenerado\" T0" + " WHERE  T0.\"ODocEntry\" = " + DocEntry, this.Connection);
                reader = comm.ExecuteReader();
                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                            previamenteCerrada = Convert.ToString(reader.GetValue(0));
                    }
                }
                else
                    previamenteCerrada = "";
                return previamenteCerrada;
            }
            catch (Exception ex)
            {
                previamenteCerrada = "";
                this.ErrorMensaje = ex.Message;
            }
            finally
            {
                this.LiberarVariables(ref this.Connection, ref comm, ref reader);
            }
            return previamenteCerrada;
        }

        public long GetOrdenPreviamenteActualizada(string CompanyDB, string DocEntry)
        {
            long previamenteActualizada = 0;
            HanaCommand comm = (HanaCommand)null;
            HanaDataReader reader = (HanaDataReader)null;
            try
            {
                this.conectarHana(CompanyDB);
                comm = new HanaCommand("SELECT T0.\"ODocEntryRel\"  FROM  \"" + CompanyDB + "\".\"View_VerificaOINVGenerado\" T0" + " WHERE  T0.\"ODocEntry\" = " + DocEntry, this.Connection);
                reader = comm.ExecuteReader();
                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                            previamenteActualizada = !(reader.GetValue(0).ToString() == "") ? Convert.ToInt64(reader.GetValue(0)) : 0L;
                    }
                }
                else
                    previamenteActualizada = 0L;
                return previamenteActualizada;
            }
            catch (Exception ex)
            {
                previamenteActualizada = 0L;
                this.ErrorMensaje = ex.Message;
            }
            finally
            {
                this.LiberarVariables(ref this.Connection, ref comm, ref reader);
            }
            return previamenteActualizada;
        }

        public long GetFacturaSinPago(string CompanyDB, string DocEntry)
        {
            long facturaSinPago = 0;
            HanaCommand comm = (HanaCommand)null;
            HanaDataReader reader = (HanaDataReader)null;
            try
            {
                this.conectarHana(CompanyDB);
                comm = new HanaCommand("SELECT T0.\"U_EXX_FPAGO_VENTAS\"  FROM  \"" + CompanyDB + "\".\"View_VerificaOINVGenerado\" T0" + " WHERE  T0.\"ODocEntry\" = " + DocEntry, this.Connection);
                reader = comm.ExecuteReader();
                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (reader.GetValue(0).ToString() == "")
                            {
                                facturaSinPago = 0L;
                            }
                            else
                            {
                                facturaSinPago = Convert.ToInt64(reader.GetValue(0));
                                this._logger.Info(string.Format("GetFacturaSinPago:IdFP{0}", (object)facturaSinPago));
                            }
                        }
                    }
                }
                else
                    facturaSinPago = 0L;
                return facturaSinPago;
            }
            catch (Exception ex)
            {
                facturaSinPago = 0L;
                this.ErrorMensaje = ex.Message;
                this._logger.Error("GetFacturaSinPago:" + this.ErrorMensaje);
            }
            finally
            {
                this.LiberarVariables(ref this.Connection, ref comm, ref reader);
            }
            return facturaSinPago;
        }

        public OrdenVentaFacturaLote GetOrdenFacturarLote(long DocEntry, string CompanyDB)
        {
            if (!this.servicio.IsConected)
                this.servicio.ConectarSL(CompanyDB);
            if (this.servicio.IsConected)
            {
                RespuestaGenerica respuestaGenerica = this.servicio.SLSendRequestReturnResponse(string.Format("Orders({0})", (object)DocEntry), "GET", "", HttpStatusCode.OK, false);
                OrdenVentaFacturaLote ordenFacturarLote = JsonConvert.DeserializeObject<OrdenVentaFacturaLote>(respuestaGenerica.RespuestaJson);
                if (respuestaGenerica.Success)
                {
                    this.errMsg = "Consulta Exitosa";
                    this._logger.Info(string.Format("GetOrdenFacturarLote:{0}", (object)respuestaGenerica.Success));
                    return ordenFacturarLote;
                }
                this.errMsg = string.Format("Error: {0}-{1}-{2}", (object)respuestaGenerica.ErrCodigo, (object)respuestaGenerica.ErrException, (object)respuestaGenerica.ErrMensaje);
                this._logger.Error("GetOrdenFacturarLote:" + this.errMsg);
                return (OrdenVentaFacturaLote)null;
            }
            this.errMsg = "Error: No se pudo establecer conexión";
            this._logger.Error("GetOrdenFacturarLote:" + this.errMsg);
            return (OrdenVentaFacturaLote)null;
        }

        public FacturaVentaRespCreateModel GetFactura(long DocEntry, string CompanyDB)
        {
            if (!this.servicio.IsConected)
                this.servicio.ConectarSL(CompanyDB);
            if (this.servicio.IsConected)
            {
                RespuestaGenerica respuestaGenerica = this.servicio.SLSendRequestReturnResponse(string.Format("Invoices({0})", (object)DocEntry), "GET", "", HttpStatusCode.OK, false);
                FacturaVentaRespCreateModel factura = JsonConvert.DeserializeObject<FacturaVentaRespCreateModel>(respuestaGenerica.RespuestaJson);
                if (respuestaGenerica.Success)
                {
                    this.errMsg = "Consulta Exitosa";
                    this._logger.Info(string.Format("GetFactura:{0}", (object)respuestaGenerica.Success));
                    return factura;
                }
                this.errMsg = string.Format("Error: {0}-{1}-{2}", (object)respuestaGenerica.ErrCodigo, (object)respuestaGenerica.ErrException, (object)respuestaGenerica.ErrMensaje);
                this._logger.Error("GetFactura:" + this.errMsg);
                return (FacturaVentaRespCreateModel)null;
            }
            this.errMsg = "Error: No se pudo establecer conexión";
            this._logger.Error("GetFactura:" + this.errMsg);
            return (FacturaVentaRespCreateModel)null;
        }

        public FacturaResultModel GetFacturaByOrdenVentaLote(long DocEntryOV, string CompanyDB)
        {
            if (!this.servicio.IsConected)
                this.servicio.ConectarSL(CompanyDB);
            if (this.servicio.IsConected)
            {
                RespuestaGenerica respuestaGenerica = this.servicio.SLSendRequestReturnResponse(string.Format("Invoices?$select=DocEntry,DocNum,DocType,DocTotal,U_EXX_FPAGO_VENTAS,FolioNumber,DocumentStatus,DocTotal,U_SER_EST,U_SER_PE&$filter=U_CTK_DocEntryRel eq '{0}'", (object)DocEntryOV), "GET", "", HttpStatusCode.OK, false);
                if (respuestaGenerica.Success)
                {
                    FacturaListObj facturaListObj = JsonConvert.DeserializeObject<FacturaListObj>(respuestaGenerica.RespuestaJson);
                    this.errMsg = "Consulta Exitosa";
                    this._logger.Info(string.Format("GetFacturaByOrdenVentaLote:{0}", (object)respuestaGenerica.Success));
                    return facturaListObj.value.FirstOrDefault<FacturaResultModel>();
                }
                this.errMsg = string.Format("Error: {0}-{1}-{2}", (object)respuestaGenerica.ErrCodigo, (object)respuestaGenerica.ErrException, (object)respuestaGenerica.ErrMensaje);
                this._logger.Error("GetFacturaByOrdenVentaLote:" + this.errMsg);
                return (FacturaResultModel)null;
            }
            this.errMsg = "Error: No se pudo establecer conexión";
            this._logger.Error("GetFacturaByOrdenVentaLote:" + this.errMsg);
            return (FacturaResultModel)null;
        }

        public RespuestaGenerica ActualizaEstadoOrdenes(
          OrdenesVentaXLoteProcesado obj,
          string CompanyDB)
        {
            if (!this.servicio.IsConected)
                this.servicio.ConectarSL(CompanyDB);
            RespuestaGenerica respuestaGenerica1 = new RespuestaGenerica();
            if (this.servicio.IsConected)
            {
                string str = "Orders";
                string _Method = "PATCH";
                string _Url = str + string.Format("({0})", (object)obj.DocEntry);
                HttpStatusCode _Status = HttpStatusCode.NoContent;
                string _BodyJson = JsonConvert.SerializeObject((object)obj);
                RespuestaGenerica respuestaGenerica2 = this.servicio.SLSendRequestReturnResponse(_Url, _Method, _BodyJson, _Status, false);
                if (respuestaGenerica2.Success)
                {
                    this.errMsg = "Actualización Exitosa";
                    this._logger.Info(string.Format("ActualizaEstadoOrdenesProcesadasXFacturar:{0}", (object)respuestaGenerica2.Success));
                    return respuestaGenerica2;
                }
                this.errMsg = string.Format("Error: {0}-{1}-{2}", (object)respuestaGenerica2.ErrCodigo, (object)respuestaGenerica2.ErrException, (object)respuestaGenerica2.ErrMensaje);
                respuestaGenerica2.Success = false;
                respuestaGenerica2.ErrMensaje = this.errMsg;
                this._logger.Error("ActualizaEstadoOrdenesProcesadasXFacturar:" + respuestaGenerica2.RespuestaJson);
                return respuestaGenerica2;
            }
            this.errMsg = "Error: No se pudo establecer conexión con SSL";
            respuestaGenerica1.Success = false;
            respuestaGenerica1.ErrMensaje = this.errMsg;
            this._logger.Error("ActualizaEstadoOrdenesProcesadasXFacturar:" + respuestaGenerica1.ErrMensaje);
            return respuestaGenerica1;
        }

        public RespuestaGenerica CerrarOrdenVenta(long idOrden, string CompanyDB)
        {
            if (!this.servicio.IsConected)
                this.servicio.ConectarSL(CompanyDB);
            RespuestaGenerica respuestaGenerica1 = new RespuestaGenerica();
            if (this.servicio.IsConected)
            {
                string _Method = "POST";
                string _Url = string.Format("Orders({0})/Close", (object)idOrden);
                HttpStatusCode _Status = HttpStatusCode.NoContent;
                RespuestaGenerica respuestaGenerica2 = this.servicio.SLSendRequestReturnResponse(_Url, _Method, (string)null, _Status, false);
                if (respuestaGenerica2.Success)
                {
                    this.errMsg = "Actualización Exitosa";
                    this._logger.Info(string.Format("CierraOrdenProcesada:{0}", (object)respuestaGenerica2.Success));
                    return respuestaGenerica2;
                }
                this.errMsg = string.Format("Error: {0}-{1}-{2}", (object)respuestaGenerica2.ErrCodigo, (object)respuestaGenerica2.ErrException, (object)respuestaGenerica2.ErrMensaje);
                respuestaGenerica2.Success = false;
                respuestaGenerica2.ErrMensaje = this.errMsg;
                return respuestaGenerica2;
            }
            this.errMsg = "Error: No se pudo establecer conexión con SSL";
            respuestaGenerica1.Success = false;
            respuestaGenerica1.ErrMensaje = this.errMsg;
            this._logger.Error(this.errMsg);
            return respuestaGenerica1;
        }

        public RespuestaGenerica ActualizaSecuenciaDocumentoElectronico(
          EXX_DOCUM_LEG_INTER obj,
          string code,
          string CompanyDB)
        {
            if (!this.servicio.IsConected)
                this.servicio.ConectarSL(CompanyDB);
            RespuestaGenerica respuestaGenerica1 = new RespuestaGenerica();
            if (this.servicio.IsConected)
            {
                string _Method = "PATCH";
                string _Url = "EXX_DOCUM_LEG_INTER" + "('" + code + "')";
                HttpStatusCode _Status = HttpStatusCode.NoContent;
                string _BodyJson = JsonConvert.SerializeObject((object)obj);
                RespuestaGenerica respuestaGenerica2 = this.servicio.SLSendRequestReturnResponse(_Url, _Method, _BodyJson, _Status, false);
                if (respuestaGenerica2.Success)
                {
                    this.errMsg = "Actualización Exitosa";
                    this._logger.Info(string.Format("ActualizaSecuenciaDocumentoElectronico:{0}", (object)respuestaGenerica2.Success));
                    return respuestaGenerica2;
                }
                this.errMsg = string.Format("Error: {0}-{1}-{2}", (object)respuestaGenerica2.ErrCodigo, (object)respuestaGenerica2.ErrException, (object)respuestaGenerica2.ErrMensaje);
                respuestaGenerica2.Success = false;
                respuestaGenerica2.ErrMensaje = this.errMsg;
                this._logger.Error("ActualizaSecuenciaDocumentoElectronico:" + respuestaGenerica2.RespuestaJson);
                return respuestaGenerica2;
            }
            this.errMsg = "Error: No se pudo establecer conexión con SSL";
            respuestaGenerica1.Success = false;
            respuestaGenerica1.ErrMensaje = this.errMsg;
            this._logger.Error("ActualizaSecuenciaDocumentoElectronico:" + respuestaGenerica1.ErrMensaje);
            return respuestaGenerica1;
        }

        public RespuestaGenerica GuardarFactura(FacturaVentaCreateModel obj, string CompanyDB)
        {
            if (!this.servicio.IsConected)
                this.servicio.ConectarSL(CompanyDB);
            RespuestaGenerica respuestaGenerica1 = new RespuestaGenerica();
            if (this.servicio.IsConected)
            {
                CatalogosManager catalogosManager = new CatalogosManager(ref this.servicio);
                string _Method = "POST";
                string _Url = "Invoices";
                HttpStatusCode _Status = HttpStatusCode.Created;
                string _BodyJson = JsonConvert.SerializeObject((object)obj);
                RespuestaGenerica respuestaGenerica2 = this.servicio.SLSendRequestReturnResponse(_Url, _Method, _BodyJson, _Status, false);
                if (respuestaGenerica2.Success)
                {
                    this.errMsg = "Creación Exitosa";
                    this._logger.Info(string.Format("GuardarFactura:{0}", (object)respuestaGenerica2.Success));
                    return respuestaGenerica2;
                }
                this.errMsg = "Error: " + respuestaGenerica2.ErrMensaje;
                respuestaGenerica2.Success = false;
                respuestaGenerica2.ErrMensaje = this.errMsg;
                this._logger.Error("Error al GuardarFactura:" + respuestaGenerica2.RespuestaJson);
                return respuestaGenerica2;
            }
            this.errMsg = "Error: No se pudo establecer conexión con SSL";
            respuestaGenerica1.Success = false;
            respuestaGenerica1.ErrMensaje = this.errMsg + " Detalle:" + this.servicio.ErrMessage;
            this._logger.Error("GuardarFactura:" + respuestaGenerica1.ErrMensaje);
            return respuestaGenerica1;
        }

        public RespuestaGenerica GuardarFormaPago(EXX_FPAGO_VENT obj, string CompanyDB)
        {
            if (!this.servicio.IsConected)
                this.servicio.ConectarSL(CompanyDB);
            RespuestaGenerica respuestaGenerica1 = new RespuestaGenerica();
            if (this.servicio.IsConected)
            {
                string _Url = "EXX_FPAGO_VT";
                string _Method = "POST";
                HttpStatusCode _Status = HttpStatusCode.Created;
                string _BodyJson = JsonConvert.SerializeObject((object)obj);
                RespuestaGenerica respuestaGenerica2 = this.servicio.SLSendRequestReturnResponse(_Url, _Method, _BodyJson, _Status, false);
                if (respuestaGenerica2.Success)
                {
                    this.errMsg = "Creación Exitosa";
                    return respuestaGenerica2;
                }
                this.errMsg = string.Format("Error: {0}-{1}-{2}", (object)respuestaGenerica2.ErrCodigo, (object)respuestaGenerica2.ErrException, (object)respuestaGenerica2.ErrMensaje);
                respuestaGenerica2.Success = false;
                respuestaGenerica2.ErrMensaje = this.errMsg;
                return respuestaGenerica2;
            }
            this.errMsg = "Error: No se pudo establecer conexión con SSL";
            respuestaGenerica1.Success = false;
            respuestaGenerica1.ErrMensaje = this.errMsg;
            return respuestaGenerica1;
        }

        public long GenerarSecuenciaLote(string CompanyDB)
        {
            long num = 0;
            HanaCommand comm = (HanaCommand)null;
            HanaDataReader reader = (HanaDataReader)null;
            try
            {
                this.conectarHana(CompanyDB);
                comm = new HanaCommand("SELECT IFNULL(MAX(CAST(T0.\"U_CTK_Lote\" as bigint)),0)  FROM  \"" + CompanyDB + "\".\"OINV\" T0 WHERE T0.\"U_CTK_Lote\" NOT LIKE '%-%'", this.Connection);
                reader = comm.ExecuteReader();
                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                            num = !(reader.GetValue(0).ToString() == "") ? Convert.ToInt64(reader.GetValue(0)) : 0L;
                    }
                }
                else
                    num = 1L;
                return num;
            }
            catch (Exception ex)
            {
                num = 1L;
                this.ErrorMensaje = ex.Message;
            }
            finally
            {
                this.LiberarVariables(ref this.Connection, ref comm, ref reader);
            }
            return num;
        }

        public bool GuardarFormaPagoBySP(string CompanyDB, OrdenVentaXFacturar orden)
        {
            FactGenerada previamenteGenerada = this.GetOINVPreviamenteGenerada(CompanyDB, orden.DocEntry.ToString());
            if (previamenteGenerada.DocNum <= 0L || previamenteGenerada.Pago != 0L)
                return false;
            FacturaVentaRespCreateModel factura = this.GetFactura(previamenteGenerada.DocEntry, CompanyDB);
            EXX_FPAGO_VENT exxFpagoVent = new EXX_FPAGO_VENT();
            long ordenesMaxCodePago = this.GetOrdenesMaxCodePago(CompanyDB);
            long ordenesMaxCodePagoOinv = this.GetOrdenesMaxCodePagoOINV(CompanyDB);
            exxFpagoVent.Code = ordenesMaxCodePagoOinv < ordenesMaxCodePago ? ordenesMaxCodePago.ToString() : ordenesMaxCodePagoOinv.ToString();
            exxFpagoVent.EXX_FPAGO_VENT_DETCollection = new List<EXX_FPAGO_VENT_DETCollection>()
      {
        new EXX_FPAGO_VENT_DETCollection()
        {
          Code = exxFpagoVent.Code,
          LineId = 1,
          U_Exx_Forma_Pago = "20",
          U_Exx_Total = (float) factura.DocTotal
        }
      };
            exxFpagoVent.U_Exx_referencia = previamenteGenerada.DocNum.ToString();
            this.InsertPago(CompanyDB, exxFpagoVent.Code, previamenteGenerada.DocNum.ToString(), factura.DocTotal, orden.DocEntry);
            return true;
        }

        public List<RespuestaGenericaLote> ProcesarLoteFacturasSinPago(string CompanyDB, long DocEntry)
        {
            List<RespuestaGenericaLote> respuestaGenericaLoteList = new List<RespuestaGenericaLote>();
            RespuestaGenericaLote respuestaGenericaLote1 = new RespuestaGenericaLote();
            if (!this.servicio.IsConected)
                this.servicio.ConectarSL(CompanyDB);
            if (this.servicio.IsConected)
            {
                DateTime dateTime = DateTime.Now;
                dateTime = dateTime.AddDays(-3.0);
                RespuestaGenerica respuestaGenerica = this.servicio.SLSendRequestReturnResponse("Invoices?$select=DocEntry,DocNum,DocType,DocumentSubType,DocTotal,DocDate,U_EXX_DOC_GEN,CardCode,CardName,U_EXX_FPAGO_VENTAS,FolioNumber,DocumentStatus,DocTotal,U_SER_EST,U_SER_PE,U_CTK_DocEntryRel,U_CTK_DocNumRel&$filter=U_EXX_FPAGO_VENTAS eq NULL and DocumentSubType ne 'DN' and DocDate gt '" + dateTime.ToString("yyyy-MM-dd") + "'  ", "GET", "", HttpStatusCode.OK, false);
                if (respuestaGenerica.Success)
                {
                    FacturaListObj facturaListObj = JsonConvert.DeserializeObject<FacturaListObj>(respuestaGenerica.RespuestaJson);
                    if (facturaListObj != null && facturaListObj.value.Count<FacturaResultModel>() > 0)
                    {
                        foreach (FacturaResultModel facturaResultModel in facturaListObj.value)
                        {
                            try
                            {
                                EXX_FPAGO_VENT exxFpagoVent1 = new EXX_FPAGO_VENT();
                                long ordenesMaxCodePago = this.GetOrdenesMaxCodePago(CompanyDB);
                                long ordenesMaxCodePagoOinv = this.GetOrdenesMaxCodePagoOINV(CompanyDB);
                                exxFpagoVent1.Code = ordenesMaxCodePagoOinv < ordenesMaxCodePago ? ordenesMaxCodePago.ToString() : ordenesMaxCodePagoOinv.ToString();
                                exxFpagoVent1.EXX_FPAGO_VENT_DETCollection = new List<EXX_FPAGO_VENT_DETCollection>()
                {
                  new EXX_FPAGO_VENT_DETCollection()
                  {
                    Code = exxFpagoVent1.Code,
                    LineId = 1,
                    U_Exx_Forma_Pago = "20",
                    U_Exx_Total = facturaResultModel.DocTotal
                  }
                };
                                EXX_FPAGO_VENT exxFpagoVent2 = exxFpagoVent1;
                                long docNum = facturaResultModel.DocNum;
                                string str = docNum.ToString();
                                exxFpagoVent2.U_Exx_referencia = str;
                                if (!string.IsNullOrEmpty(facturaResultModel.U_CTK_DocEntryRel))
                                {
                                    if (string.IsNullOrEmpty(facturaResultModel.U_EXX_FPAGO_VENTAS))
                                    {
                                        long num = long.Parse(facturaResultModel.U_CTK_DocEntryRel);
                                        string CompanyDB1 = CompanyDB;
                                        string code = exxFpagoVent1.Code;
                                        docNum = facturaResultModel.DocNum;
                                        string referencia = docNum.ToString();
                                        Decimal docTotal = (Decimal)facturaResultModel.DocTotal;
                                        long ov = num;
                                        this.InsertPago(CompanyDB1, code, referencia, docTotal, ov);
                                        RespuestaGenericaLote respuestaGenericaLote2 = new RespuestaGenericaLote();
                                        respuestaGenericaLote2.DocEntryRel = long.Parse(facturaResultModel.U_CTK_DocEntryRel);
                                        respuestaGenericaLote2.DocNumRel = long.Parse(facturaResultModel.U_CTK_DocNumRel == null ? "0" : facturaResultModel.U_CTK_DocNumRel);
                                        respuestaGenericaLote2.TipoDocumento = facturaResultModel.U_EXX_DOC_GEN;
                                        respuestaGenericaLote2.Cliente = facturaResultModel.CardCode + "-" + facturaResultModel.CardName;
                                        respuestaGenericaLote2.DocEntry = facturaResultModel.DocEntry;
                                        respuestaGenericaLote2.DocNum = facturaResultModel.DocNum;
                                        respuestaGenericaLote2.Success = true;
                                        respuestaGenericaLote2.ErrMensaje = "Forma de Pago Actualizada Existosamente.";
                                        respuestaGenericaLoteList.Add(respuestaGenericaLote2);
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                else
                {
                    this.ErrMsg = string.Format("Error: {0}", (object)respuestaGenericaLote1.ErrCodigo) + respuestaGenericaLote1.ErrMensaje + "." + (object)respuestaGenericaLote1.ErrException;
                    this._logger.Error("GetSerieDuplicadaSSL:" + this.ErrMsg);
                    RespuestaGenericaLote respuestaGenericaLote3 = new RespuestaGenericaLote();
                    respuestaGenericaLote3.DocEntryRel = 0L;
                    respuestaGenericaLote3.DocNumRel = 0L;
                    respuestaGenericaLote3.TipoDocumento = "";
                    respuestaGenericaLote3.Cliente = "";
                    respuestaGenericaLote3.DocEntry = 0L;
                    respuestaGenericaLote3.DocNum = 0L;
                    respuestaGenericaLote3.Success = false;
                    respuestaGenericaLote3.ErrMensaje = "Ocurrió un error al actualizar forma de pago en facturas." + this.ErrMsg;
                    respuestaGenericaLoteList.Add(respuestaGenericaLote3);
                }
            }
            return respuestaGenericaLoteList;
        }
    }
}
