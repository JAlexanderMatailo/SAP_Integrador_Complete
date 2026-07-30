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
using NLog;
using Sap.Data.Hana;

namespace IntegradorSAP.Services.Manager
{
    public class NotaDebitoLoteManager : DAO
    {
        private Logger _logger = LogManager.GetLogger("DataLog");
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


        public List<OrdenVentaXNotaDebito> GetOrdenesVentaPendienteXNotaDebito(string CompanyDB)
        {
            HanaCommand comm = null;
            HanaDataReader reader = null;
            List<OrdenVentaXNotaDebito> ordenes = new List<OrdenVentaXNotaDebito>();
            OrdenVentaXNotaDebito orden = new OrdenVentaXNotaDebito();

            try
            {
                conectarHana(CompanyDB);
                string StrQty = "SELECT top 50 * FROM  \"" + CompanyDB + "\".\"CTK_GET_PEDIDOS_PENDIENTES_GENERANOTADEBITO_VIEW\" ";
                //  StrQty += @" WHERE ""SapPedidoDocEntry"" in (" + Ids + ")";

                comm = new HanaCommand(StrQty, Connection);

                reader = comm.ExecuteReader();

                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            orden = new OrdenVentaXNotaDebito();
                            orden.DocEntry = Convert.ToInt64(reader.GetValue(0));
                            orden.DocNum = Convert.ToInt64(reader.GetValue(1));
                            orden.DocType = Convert.ToString(reader.GetValue(2));
                            orden.DocDate = Convert.ToDateTime(reader.GetValue(3));
                            orden.U_SerieFV = Convert.ToString(reader.GetValue(4));
                            orden.CardCode = Convert.ToString(reader.GetValue(5));
                            orden.U_SerieId = Convert.ToString(reader.GetValue(8));
                            orden.U_CodeSerieId = Convert.ToString(reader.GetValue(11));
                            orden.DiaNotaDebito = Convert.ToInt32(reader.GetValue(15));


                            // orden.U_tipo_export = Convert.ToString(reader.GetValue(25));

                            if (reader.GetValue(6).ToString() != "")
                            {
                                orden.U_SER_EST = reader.GetValue(6).ToString();
                            }
                            if (reader.GetValue(7).ToString() != "")
                            {
                                orden.U_SER_PE = reader.GetValue(7).ToString();
                            }

                            if (reader.GetValue(8).ToString() != "")
                            {
                                orden.Series = Convert.ToString(reader.GetValue(8));
                            }
                            if (reader.GetValue(9).ToString() != "")
                            {
                                orden.U_tipo_comprob = reader.GetValue(9).ToString();
                            }
                            if (reader.GetValue(10).ToString() != "")
                            {
                                orden.U_NUM_AUTOR = reader.GetValue(10).ToString();
                            }

                            if (reader.GetValue(13).ToString() != "")
                            {
                                orden.ExtraMonth = Convert.ToInt32(reader.GetValue(13));
                            }
                            if (reader.GetValue(14).ToString() != "")
                            {
                                orden.ExtraDays = Convert.ToInt32(reader.GetValue(14));
                            }





                            if (reader.GetValue(16).ToString() != "")
                            {
                                orden.U_FechaApertura = Convert.ToInt32(reader.GetValue(16));
                            }
                            if (reader.GetValue(17).ToString() != "")
                            {
                                orden.U_FechaCierre = Convert.ToInt32(reader.GetValue(17));
                            }
                            orden.U_TIPO_ID = Convert.ToString(reader.GetValue(18));
                            orden.CardName = Convert.ToString(reader.GetValue(19));

                            if (reader.GetValue(20).ToString() != "")
                            {
                                orden.MINUTOSAGREGADOS = Convert.ToInt32(reader.GetValue(20));
                            }


                            //  orden.U_FechaApertura = Convert.ToInt32(reader.GetValue(16));
                            //  orden.U_FechaCierre = Convert.ToInt32(reader.GetValue(17));
                            //    orden.MINUTOSAGREGADOS = Convert.ToInt32(reader.GetValue(20));
                            orden.U_TipoNotaDebito = reader.GetValue(21).ToString();
                            orden.U_EXX_DOC_GEN = reader.GetValue(22).ToString();


                            if (reader.GetValue(23).ToString() != "")
                            {
                                orden.HoraDocumento = Convert.ToInt32(reader.GetValue(23));
                            }
                            if (reader.GetValue(24).ToString() != "")
                            {
                                orden.MinutoDocumento = Convert.ToInt32(reader.GetValue(24));
                            }

                            if (reader.GetValue(25).ToString() != "")
                            {
                                orden.U_tipo_export = Convert.ToString(reader.GetValue(25));
                            }
                            if (reader.GetValue(26).ToString() != "")
                            {
                                orden.DocTotal = Convert.ToDouble(reader.GetValue(26));
                            }
                            if (reader.GetValue(27).ToString() != "")
                            {
                                orden.SalesPersonCode = Convert.ToInt16(reader.GetValue(27).ToString());
                            }
                            if (reader.GetValue(28).ToString() != "")
                            {
                                orden.U_Exx_FE_Paisdestin = Convert.ToString(reader.GetValue(28).ToString());
                            }

                            ordenes.Add(orden);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                List<OrdenVentaXNotaDebito> ordenesError = new List<OrdenVentaXNotaDebito>();
                OrdenVentaXNotaDebito ordenError = new OrdenVentaXNotaDebito();
                ErrorMensaje = ex.Message;
                ordenError.MensajeError = ErrorMensaje;
                ordenesError.Add(ordenError);
                return ordenesError;
            }
            finally
            {


                LiberarVariables(ref Connection, ref comm, ref reader);
            }

            return ordenes;
        }


        public OrdenVentaFacturaLote GetOrdenNotaDebitoLote(long DocEntry, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            if (servicio.IsConected)
            {

                string _recurso = $"Orders({DocEntry})";
                var respuesta = servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);
                OrdenVentaFacturaLote obj = JsonConvert.DeserializeObject<OrdenVentaFacturaLote>(respuesta.RespuestaJson);


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



        public RespuestaGenerica ActualizaEstadoOrdenes(OrdenesVentaXLoteProcesadoNotaDebito obj, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);
            System.Net.HttpStatusCode _Status = System.Net.HttpStatusCode.Created;

            RespuestaGenerica respuestaPrincipal = new RespuestaGenerica();

            if (servicio.IsConected)
            {
              
                string _Method = "PATCH";//PATCH

                string _recurso = $"Orders";

                _Method = "PATCH";
                _recurso += $"({obj.DocEntry})";
                _Status = System.Net.HttpStatusCode.NoContent;
                
                var _json = JsonConvert.SerializeObject(obj);

                var respuesta = servicio.SLSendRequestReturnResponse(_recurso, _Method, _json, _Status, false);

                if (respuesta.Success)
                {
                    this.errMsg = "Actualización Exitosa";

                    return respuesta;
                }
                else
                {
                    this.errMsg = $"Error: {respuesta.ErrCodigo}-{respuesta.ErrException}-{respuesta.ErrMensaje}";
                    respuesta.Success = false;
                    respuesta.ErrMensaje = this.errMsg;
                    return respuesta;
                }


                return respuestaPrincipal;
            }
            else
            {
                this.errMsg = $"Error: No se pudo establecer conexión con SSL";
                respuestaPrincipal.Success = false;
                respuestaPrincipal.ErrMensaje = this.errMsg;
                return respuestaPrincipal;
            }
        }

        public FacturaResultModel GetNotaDebitoByOrdenVentaLote(long DocEntryOV, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            if (servicio.IsConected)
            {

                string _recurso = $"Invoices?$select=DocEntry,DocNum,DocType,DocDate,DocTotal,U_EXX_FPAGO_VENTAS,FolioNumber,DocumentStatus,DocTotal,U_SER_EST,U_SER_PE&$filter=U_CTK_DocEntryRel eq '{DocEntryOV}'";
                var respuesta = servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);

                if (respuesta.Success)
                {
                    FacturaListObj obj = JsonConvert.DeserializeObject<FacturaListObj>(respuesta.RespuestaJson);


                    this.errMsg = "Consulta Exitosa";
                    _logger.Info($"GetNotaDebitoByOrdenVentaLote:{respuesta.Success}");
                    return obj.value.FirstOrDefault();
                }
                else
                {

                    this.errMsg = $"Error: {respuesta.ErrCodigo}-{respuesta.ErrException}-{respuesta.ErrMensaje}";
                    _logger.Error($"GetFacturaByOrdenVentaLote:{errMsg}");
                    return null;
                }
            }
            else
            {
                this.errMsg = $"Error: No se pudo establecer conexión";
                _logger.Error($"GetFacturaByOrdenVentaLote:{errMsg}");
                return null;
            }
        }


        public RespuestaGenerica CierraOrdenProcesada(long idOrden, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);
            System.Net.HttpStatusCode _Status = System.Net.HttpStatusCode.Created;

            RespuestaGenerica respuestaPrincipal = new RespuestaGenerica();

            if (servicio.IsConected)
            { 
                string _Method = "POST";
                string _recurso = $"Orders";   
                _Method = "POST";
                _recurso += $"({idOrden})";
                _Status = System.Net.HttpStatusCode.NoContent;

                var respuesta = servicio.SLSendRequestReturnResponse(_recurso + "/Close", _Method, null, _Status, false);

                if (respuesta.Success)
                {
                    this.errMsg = "Actualización Exitosa";

                    return respuesta;
                }
                else
                {
                    this.errMsg = $"Error: {respuesta.ErrCodigo}-{respuesta.ErrException}-{respuesta.ErrMensaje}";
                    respuesta.Success = false;
                    respuesta.ErrMensaje = this.errMsg;
                    return respuesta;
                }


                return respuestaPrincipal;
            }
            else
            {
                this.errMsg = $"Error: No se pudo establecer conexión con SSL";
                respuestaPrincipal.Success = false;
                respuestaPrincipal.ErrMensaje = this.errMsg;
                return respuestaPrincipal;
            }
        }

        public RespuestaGenerica GuardarNotaDebito(NotaDebitoCreateModel obj, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);
            System.Net.HttpStatusCode _Status = System.Net.HttpStatusCode.Created;

            RespuestaGenerica respuestaPrincipal = new RespuestaGenerica();

            if (servicio.IsConected)
            {
               
                ////////GUARDAR ORDEN DE VENTA
                string _Method = "POST";//PATCH
                string _recurso = $"Invoices";


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
                    this.errMsg = $"Error: {respuesta.ErrMensaje}";
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