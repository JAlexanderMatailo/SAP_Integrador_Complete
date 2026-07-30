using IntegradorSAP.Data.Entidades;
using IntegradorSAP.Data.Helper;
using IntegradorSAP.Data.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using IntegradorSAP.Data.DataAccess;
using IntegradorSAP.Data.Manager;
using System.Threading.Tasks;
using Sap.Data.Hana;

namespace IntegradorSAP.Data.Manager
{
    public class CatalogosManager : DAO
    {
        public ServiceLayer_Web servicio = new ServiceLayer_Web();
        string Usuario;
        string Ip;
        string errorCode;
        string errMsg;
        public string Mensaje { get; set; }

        public string ErrMsg { get => ErrMsg; set => errMsg = value; }
        public string ErrorCode { get => ErrorCode; set => errorCode = value; }
        public CatalogosManager(ref ServiceLayer_Web servicio)
        {
            this.servicio = servicio;           

        }

        public CatalogosManager()
        {

        }

        public BusinessPartnerViewModel GetBusinessPartners(string Ruc)
        {
           
                //Obtener los datos del socio de negocios
                string _recurso = _recurso = $"BusinessPartners?$filter=FederalTaxID eq '{Ruc}' and  Valid eq  'Y'   ";


                var respuesta = servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);
                if (respuesta.Success)
                {
                    BusinessPartner bp = JsonConvert.DeserializeObject<BusinessPartner>(respuesta.RespuestaJson);
                if (bp != null)
                {
                    BusinessPartnerViewModel bp2 = bp.value[0];
                    return bp2;
                }
                return null;
            }
            
            return null;
        }

       

        //public List<OrdenVentaXFacturar> GetToWarehouse(string CompanyDB, string FromWarehouse)
        //{
        //    HanaCommand comm = null;
        //    HanaDataReader reader = null;
        //    List<OrdenVentaXFacturar> ordenes = new List<OrdenVentaXFacturar>();
        //    OrdenVentaXFacturar orden = new OrdenVentaXFacturar();

        //    try
        //    {
        //        conectarHana(CompanyDB);
        //        string StrQty = "SELECT * FROM  \"" + CompanyDB + "\".\"CTK_GET_PEDIDOS_PENDIENTES_FACTURAR_VIEW\" ";
        //        //  StrQty += @" WHERE ""SapPedidoDocEntry"" in (" + Ids + ")";

        //        comm = new HanaCommand(StrQty, Connection);

        //        reader = comm.ExecuteReader();

        //        if (reader != null)
        //        {
        //            if (reader.HasRows)
        //            {
        //                while (reader.Read())
        //                {
        //                    orden = new OrdenVentaXFacturar();
        //                    orden.DocEntry = Convert.ToInt64(reader.GetValue(0));
        //                    orden.DocNum = Convert.ToInt64(reader.GetValue(1));
        //                    orden.DocType = Convert.ToString(reader.GetValue(2));
        //                    orden.DocDate = Convert.ToDateTime(reader.GetValue(3));
        //                    orden.U_SerieFV = Convert.ToString(reader.GetValue(4));
        //                    orden.CardCode = Convert.ToString(reader.GetValue(5));
        //                    orden.U_SerieId = Convert.ToString(reader.GetValue(8));
        //                    orden.U_CodeSerieId = Convert.ToString(reader.GetValue(11));
        //                    orden.DiaFactura = Convert.ToInt32(reader.GetValue(15));


        //                    // orden.U_tipo_export = Convert.ToString(reader.GetValue(25));

        //                    if (reader.GetValue(6).ToString() != "")
        //                    {
        //                        orden.U_SER_EST = reader.GetValue(6).ToString();
        //                    }
        //                    if (reader.GetValue(7).ToString() != "")
        //                    {
        //                        orden.U_SER_PE = reader.GetValue(7).ToString();
        //                    }

        //                    if (reader.GetValue(8).ToString() != "")
        //                    {
        //                        orden.Series = Convert.ToInt32(reader.GetValue(8));
        //                    }
        //                    if (reader.GetValue(9).ToString() != "")
        //                    {
        //                        orden.U_tipo_comprob = reader.GetValue(9).ToString();
        //                    }
        //                    if (reader.GetValue(10).ToString() != "")
        //                    {
        //                        orden.U_NUM_AUTOR = reader.GetValue(10).ToString();
        //                    }

        //                    if (reader.GetValue(13).ToString() != "")
        //                    {
        //                        orden.ExtraMonth = Convert.ToInt32(reader.GetValue(13));
        //                    }
        //                    if (reader.GetValue(14).ToString() != "")
        //                    {
        //                        orden.ExtraDays = Convert.ToInt32(reader.GetValue(14));
        //                    }





        //                    if (reader.GetValue(16).ToString() != "")
        //                    {
        //                        orden.U_FechaApertura = Convert.ToInt32(reader.GetValue(16));
        //                    }
        //                    if (reader.GetValue(17).ToString() != "")
        //                    {
        //                        orden.U_FechaCierre = Convert.ToInt32(reader.GetValue(17));
        //                    }
        //                    orden.U_TIPO_ID = Convert.ToString(reader.GetValue(18));
        //                    orden.CardName = Convert.ToString(reader.GetValue(19));

        //                    if (reader.GetValue(20).ToString() != "")
        //                    {
        //                        orden.MINUTOSAGREGADOS = Convert.ToInt32(reader.GetValue(20));
        //                    }


        //                    //  orden.U_FechaApertura = Convert.ToInt32(reader.GetValue(16));
        //                    //  orden.U_FechaCierre = Convert.ToInt32(reader.GetValue(17));
        //                    //    orden.MINUTOSAGREGADOS = Convert.ToInt32(reader.GetValue(20));
        //                    orden.U_TipoFacturacion = reader.GetValue(21).ToString();
        //                    orden.U_EXX_DOC_GEN = reader.GetValue(22).ToString();


        //                    if (reader.GetValue(23).ToString() != "")
        //                    {
        //                        orden.HoraDocumento = Convert.ToInt32(reader.GetValue(23));
        //                    }
        //                    if (reader.GetValue(24).ToString() != "")
        //                    {
        //                        orden.MinutoDocumento = Convert.ToInt32(reader.GetValue(24));
        //                    }

        //                    if (reader.GetValue(25).ToString() != "")
        //                    {
        //                        orden.U_tipo_export = Convert.ToString(reader.GetValue(25));
        //                    }
        //                    if (reader.GetValue(26).ToString() != "")
        //                    {
        //                        orden.DocTotal = Convert.ToDouble(reader.GetValue(26));
        //                    }
        //                    if (reader.GetValue(27).ToString() != "")
        //                    {
        //                        orden.SalesPersonCode = Convert.ToInt16(reader.GetValue(27).ToString());
        //                    }

        //                    ordenes.Add(orden);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        List<OrdenVentaXFacturar> ordenesError = new List<OrdenVentaXFacturar>();
        //        OrdenVentaXFacturar ordenError = new OrdenVentaXFacturar();
        //        ErrorMensaje = ex.Message;
        //        ordenError.MensajeError = ErrorMensaje;
        //        ordenesError.Add(ordenError);
        //        return ordenesError;
        //    }
        //    finally
        //    {


        //        LiberarVariables(ref Connection, ref comm, ref reader);
        //    }

        //    return ordenes;
        //}


        public BusinessPartnerViewModel GetProveedor(string Ruc, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            if (servicio.IsConected)
            {
                //Obtener los datos del socio de negocios
                string _recurso = _recurso = $"BusinessPartners?$filter=FederalTaxID eq '{Ruc}' and CardType eq 'S'  and  Valid eq  'Y'  ";


                var respuesta = servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);
                if (respuesta.Success)
                {
                    BusinessPartner bp = JsonConvert.DeserializeObject<BusinessPartner>(respuesta.RespuestaJson);
                    if (bp != null)
                    {
                        if (bp.value.Length > 0)
                        {
                            BusinessPartnerViewModel bp2 = bp.value[0];
                            return bp2;
                        }
                    }
                    return null;
                }
            }

            return null;
        }

        public BusinessPartnerViewModel GetCliente(string Ruc, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            if (servicio.IsConected)
            {
                //Obtener los datos del socio de negocios
                string _recurso = _recurso = $"BusinessPartners?$filter=FederalTaxID eq '{Ruc}' and CardType eq 'C' and  Valid eq 'Y' ";


                var respuesta = servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);
                if (respuesta.Success)
                {
                    try
                    {
                        BusinessPartner bp = JsonConvert.DeserializeObject<BusinessPartner>(respuesta.RespuestaJson);
                        if (bp != null)
                        {
                            BusinessPartnerViewModel bp2 = bp.value[0];
                            return bp2;
                        }
                        return null;
                    }
                    catch {
                        return null;
                    }
                }
                
            }
            return null;
        }

        public RespuestaGenerica GetClienteResp(string Ruc, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            if (servicio.IsConected)
            {
                //Obtener los datos del socio de negocios
                string _recurso = _recurso = $"BusinessPartners?$filter=FederalTaxID eq '{Ruc}' and CardType eq 'C' and  Valid eq 'Y' ";


                var respuesta = servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);
                return respuesta;

            }
            return null;
        }

        public ItemsViewModel GetItem(string ItemCode, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            if (servicio.IsConected)
            {
                //Obtener los datos del socio de negocios
                string _recurso = _recurso = $"Items('{ItemCode}')";


                var respuesta = servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);
                if (respuesta.Success)
                {
                    ItemsViewModel item = JsonConvert.DeserializeObject<ItemsViewModel>(respuesta.RespuestaJson);
                    return item;
                }
            }
            return null;
        }

       
        public static List<T> GetObject<T>(string response)
        {
            
            //var response = "[{\"firstName\":\"Melanie\",\"lastName\":\"Acevedo\"},
            //    {\"firstName\":\"Rich\",\"lastName\":\"Garrett\"},
            //    {\"firstName\":\"Dominguez\",\"lastName\":\"Rose\"},
            //    {\"firstName\":\"Louisa\",\"lastName\":\"Howell\"},
            //    {\"firstName\":\"Stone\",\"lastName\":\"Bean\"},
            //    {\"firstName\":\"Karen\",\"lastName\":\"Buckley\"}]";

            var obj = JsonConvert.DeserializeObject<List<T>>(response);
            return obj.ToList();
        }
        public ProfitCenterViewModel GetCentroCostos(string CodeCentroCosto, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            if (servicio.IsConected)
            {
                //Obtener los datos del socio de negocios
                string _recurso = _recurso = $"ProfitCenters('{CodeCentroCosto}')";


                var respuesta = servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);
                if (respuesta.Success)
                {
                    ProfitCenterViewModel item = JsonConvert.DeserializeObject<ProfitCenterViewModel>(respuesta.RespuestaJson);
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Valida si existe una Orden de Venta en SAP cuyo UDF U_EXX_TIPO_TRANSACC sea igual al
        /// código de turno enviado. El código del turno se persiste en este campo al transmitir
        /// el turno a SAP (ver Integrador.RFS.Turnos -> EnviarFacturacionTurnosSapRepository.GenerarCabeceraOrdenDeVentaAGuardar).
        /// También se valida que el documento generado corresponda a un turno (U_EXX_DOC_GEN = FacturaGeneral)
        /// para evitar falsos positivos cuando el mismo código se haya reutilizado en otro flujo.
        /// Se incluyen también las órdenes canceladas, de modo que un código de turno ya transmitido
        /// (aunque la OV haya sido anulada en SAP) se considere existente y no se reintente.
        /// </summary>
        public bool ExisteOrdenVentaPorCodigoTurno(string CodTurno, string CompanyDB)
        {
            if (string.IsNullOrWhiteSpace(CodTurno))
                return false;

            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            if (!servicio.IsConected)
                return false;

            string codTurnoEscapado = CodTurno.Replace("'", "''");
            const string tipoDocumentoTurno = "FacturaGeneral";

            string _recurso = $"Orders?$select=DocEntry,DocNum,CancelStatus,Cancelled,U_EXX_TIPO_TRANSACC,U_EXX_DOC_GEN" +
                              $"&$filter=U_EXX_TIPO_TRANSACC eq '{codTurnoEscapado}' and U_EXX_DOC_GEN eq '{tipoDocumentoTurno}'";

            var respuesta = servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);

            if (respuesta == null || !respuesta.Success || string.IsNullOrEmpty(respuesta.RespuestaJson))
                return false;

            try
            {
                JObject jObj = JObject.Parse(respuesta.RespuestaJson);
                JArray value = jObj["value"] as JArray;
                return value != null && value.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        public RespuestaGenerica GetListaCentroCostos(string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            if (servicio.IsConected)
            {
                //Obtener los datos del socio de negocios
                string _recurso = _recurso = $"ProfitCenters";
                return  servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);
               
            }
            return null;
        }

        public RespuestaGenerica GetListaDistributionRules(string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            if (servicio.IsConected)
            {
                //Obtener los datos del socio de negocios
                //string _recurso =  $"DistributionRules";
                string _recurso = "DistributionRules?$select=FactorCode,FactorDescription,TotalFactor,InWhichDimension,Active&$filter=Active eq 'Y'&$skip=500";
                return servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);

            }
            return null;
        }

        public async Task<RespuestaGenerica> GetListaDistributionRulesDB(string CompanyDB)
        {
            HanaCommand comm = null;
            HanaDataReader reader = null;
            List<CentroCostosSapViewModel> lista = new List<CentroCostosSapViewModel>();
            CentroCostosSapViewModel item = new CentroCostosSapViewModel();
            RespuestaGenerica respuesta = new RespuestaGenerica();

            try
            {
                conectarHana(CompanyDB);
                //string StrQty = "SELECT * FROM  \"" + CompanyDB + "\".\"CTKItemsServiciosSeaboardView\" ";
                string StrQty = $"Select \"OcrCode\", \"OcrName\",\"DimCode\",\"Active\"  from OOCR WHERE \"Active\"='Y' ";

                //  StrQty += @" WHERE ""SapPedidoDocEntry"" in (" + Ids + ")";

                comm = new HanaCommand(StrQty, Connection);

                reader =await comm.ExecuteReaderAsync();

                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            item = new CentroCostosSapViewModel();
                            item.FactorCode = Convert.ToString(reader.GetValue(0));
                            item.FactorDescription = Convert.ToString(reader.GetValue(1));                            
                            item.InWhichDimension= Convert.ToInt32(reader.GetValue(2));
                            item.Active = Convert.ToString(reader.GetValue(3));
                            lista.Add(item);
                        }

                        var _json = JsonConvert.SerializeObject(lista);

                        respuesta.Success = true;
                        respuesta.ErrMensaje = "ok";
                        respuesta.RespuestaJson = _json;
                        return respuesta;
                    }


                }
            }
            catch (Exception ex)
            {
                respuesta.Success = false;
                respuesta.ErrMensaje = ex.Message;
                respuesta.RespuestaJson = "";
                return respuesta;
            }
            finally
            {


                LiberarVariables(ref Connection, ref comm, ref reader);
                GC.Collect();
            }

            return respuesta;
        }


        public RespuestaGenerica GetListaProyectos(string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            if (servicio.IsConected)
            {
                //Obtener los datos del socio de negocios
                string _recurso = _recurso = $"Projects";
                return servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);
               
            }
            return null;
        }

        public RespuestaGenerica GetProyectoByCode(string ProjectCode, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            if (servicio.IsConected)
            {
                //Obtener los datos del socio de negocios
                string _recurso = _recurso = $"Projects('{ProjectCode}')";
                RespuestaGenerica respuesta = servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);

                if (!respuesta.Success && respuesta.ErrException.Message.Contains("404"))
                {
                    RespuestaGenerica resp = new RespuestaGenerica();
                    resp.Success = false;
                    resp.ErrException = new Exception("No se encontro el proyecto");
                    resp.RespuestaJson = "";
                    resp.ErrCodigo = 404;
                    return resp;
                }

                if (respuesta.Success)
                {
                    RespuestaJsonGenerica respuestaJsonGenerica = JsonConvert.DeserializeObject<RespuestaJsonGenerica>(respuesta.RespuestaJson);
                    DateTime validToDate = DateTime.Parse(respuestaJsonGenerica.ValidTo?.ToString() ?? DateTime.MinValue.ToString());
                    //DateTime validToDate = DateTime.Parse(respuestaJsonGenerica.ValidTo.ToString() ?? new DateTime().ToString());
                    DateTime valueDate = validToDate.Date.AddDays(1).AddSeconds(-1);
                    if (valueDate < DateTime.Now)
                    {
                        RespuestaGenerica resp = new RespuestaGenerica();
                        resp.Success = false;
                        resp.ErrException = new Exception("No se encontro el proyecto");
                        resp.RespuestaJson = "";
                        resp.ErrCodigo = 400;
                        resp.ErrMensaje = "El proyecto/Asesor se encuentra fuera de la fecha de vigencia";
                        return resp;
                    }
                    RespuestaGenerica item = JsonConvert.DeserializeObject<RespuestaGenerica>(respuesta.RespuestaJson);
                    item.Success = true;
                    item.RespuestaJson = respuesta.RespuestaJson;
                    return item;
                }
            }
            return null;
        }

        public RespuestaGenerica GuardarProyecto(ProjectsGuardarViewModel obj, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);
            System.Net.HttpStatusCode _Status = System.Net.HttpStatusCode.Created;

            RespuestaGenerica respuestaPrincipal = new RespuestaGenerica();
            if (servicio.IsConected)
            {
                ////////GUARDAR REGISTRO
                string _Method = "POST";
                string _recurso = $"Projects";

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

        public RespuestaGenerica GetListaBodegas(string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            if (servicio.IsConected)
            {
                //Obtener los datos del socio de negocios
                string _recurso = _recurso = $"Warehouses";
                return servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);

            }
            return null;
        }

        public RespuestaGenerica GuardarCentroCostos(ProfitCenterGuardarViewModel obj, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);
            System.Net.HttpStatusCode _Status = System.Net.HttpStatusCode.Created;

            RespuestaGenerica respuestaPrincipal = new RespuestaGenerica();

            if (servicio.IsConected)
            {
                ////////GUARDAR REGISTRO
                string _Method = "POST";
                string _recurso = $"ProfitCenters";


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

        /// <summary>
        /// Obtiene el resultado de cualquier UDO o tabla creado por el usuario (registrada en SAP)
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="CompanyDB"></param>
        /// <param name="Table"></param>
        /// <returns></returns>
        public RespuestaGenerica GetUDO(string Code, string CompanyDB,string UDOName)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            if (servicio.IsConected)
            {
                //Obtener los datos del socio de negocios
                //string _recurso = _recurso = $"EXX_TIPO_TRANSACCION?$filter=Code eq '{Code}' ";
                string _recurso = _recurso = $"{UDOName}('{Code}')";
                    

                var respuesta = servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);
                if (respuesta.Success)
                {
                    try
                    {
                        RespuestaGenerica bp = JsonConvert.DeserializeObject<RespuestaGenerica>(respuesta.RespuestaJson);
                        if (bp != null)
                        {                            
                            return bp;
                        }
                        return null;
                    }
                    catch
                    {
                        return null;
                    }
                }

            }
            return null;
        }

        /// <summary>
        /// Obtiene el resultado de cualquier  tabla creado por el usuario (NO registrada en SAP o No es un UDO)
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="CompanyDB"></param>
        /// <param name="Table"></param>
        /// <returns></returns>
        public RespuestaGenerica GetTableGenerica(string Code, string CompanyDB, string Table)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            if (servicio.IsConected)
            {
                //Obtener los datos del socio de negocios
                //string _recurso = _recurso = $"EXX_TIPO_TRANSACCION?$filter=Code eq '{Code}' ";
                string _recurso = _recurso = $"U_{Table}('{Code}')";


                var respuesta = servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);
                if (respuesta.Success)
                {
                    try
                    {
                        //RespuestaGenerica bp = JsonConvert.DeserializeObject<RespuestaGenerica>(respuesta.RespuestaJson);
                        return respuesta;

                    }
                    catch
                    {
                        this.errMsg = respuesta.ErrMensaje;

                        if (this.errMsg.ToLower().Contains("no matching "))
                        {

                            respuesta = new RespuestaGenerica();
                            respuesta.Success = errMsg.Contains("Error") ? false : true;
                            respuesta.RespuestaJson = $"{ errMsg }";
                            respuesta.ErrMensaje = errMsg;

                            return respuesta;
                        }
                    }
                }
                else
                {
                    this.errMsg = respuesta.ErrMensaje;

                    if (this.errMsg.ToLower().Contains("no matching "))
                    {

                        respuesta = new RespuestaGenerica();
                        respuesta.Success = errMsg.Contains("Error") ? false : true;
                        respuesta.RespuestaJson = $"{ errMsg }";
                        respuesta.ErrMensaje = errMsg;

                        return respuesta;
                    }
                }

            }
            return null;
        }

        public RespuestaGenerica GuardarObjetoTablaGenerica(TablaGenericaGuardar obj, string CompanyDB, string TablaGenerica)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);
            System.Net.HttpStatusCode _Status = System.Net.HttpStatusCode.Created;

            RespuestaGenerica respuestaPrincipal = new RespuestaGenerica();

            if (servicio.IsConected)
            {              
                ////////GUARDAR REGISTRO
                string _Method = "POST";
                string _recurso = $"U_{TablaGenerica}";

             
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
                    if (respuesta.ErrMensaje.ToLower().Contains("exist"))
                    {
                        obj.Code = obj.Code ;
                        obj.Name= obj.Name + ".";
                         _json = JsonConvert.SerializeObject(obj);

                         respuesta = servicio.SLSendRequestReturnResponse(_recurso, _Method, _json, _Status, false);

                        if (respuesta.Success)
                        {
                            this.errMsg = "Creación Exitosa";

                            return respuesta;
                        }
                    }
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

        public RespuestaGenerica GuardarContenedor(ContenedorGuardar obj, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);
            System.Net.HttpStatusCode _Status = System.Net.HttpStatusCode.Created;

            RespuestaGenerica respuestaPrincipal = new RespuestaGenerica();

            if (servicio.IsConected)
            {
                ////////GUARDAR REGISTRO
                string _Method = "POST";
                string _recurso = $"EXX_CONTENEDOR";


                _Method = "POST";
                _Status = System.Net.HttpStatusCode.Created;


                var _json = JsonConvert.SerializeObject(obj);

                var respuesta = servicio.SLSendRequestReturnResponse(_recurso, _Method, _json, _Status, false);

                if (respuesta.Success)
                {
                    this.errMsg = "Creación Exitosa.";

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

        public RespuestaGenerica GuardarContenedorTransferenciaStock(ContenedorGuardar obj, string CompanyDB)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);
            System.Net.HttpStatusCode _Status = System.Net.HttpStatusCode.Created;

            RespuestaGenerica respuestaPrincipal = new RespuestaGenerica();

            if (servicio.IsConected)
            {
                ////////GUARDAR REGISTRO
                string _Method = "POST";
                string _recurso = $"StockTransfers";


                _Method = "POST";
                _Status = System.Net.HttpStatusCode.Created;


                var _json = JsonConvert.SerializeObject(obj);

                var respuesta = servicio.SLSendRequestReturnResponse(_recurso, _Method, _json, _Status, false);

                if (respuesta.Success)
                {
                    this.errMsg = "Creación Exitosa.";

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


        /// <summary>
        /// Consulta Service Layer de cualquier objeto
        /// </summary>
        /// <param name="CompanyDB">Nombre de la compañia SAP</param>
        /// <param name="ObjSSL">Nombre de la tabla Ejemplo:BusinessPartners,EXX_TIPO_TRANSACCION, Invoices</param>
        /// <param name="Campos"></param>
        /// <param name="Filtros"></param>
        /// <returns></returns>
        public RespuestaGenerica GetConsultaGenericaSSL(string CompanyDB, string ObjSSL,string Campos, string Filtros)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            if (servicio.IsConected)
            {
                //Obtener los datos del socio de negocios
                //string _recurso = _recurso = $"BusinessPartners?$filter=FederalTaxID eq '{Ruc}' and CardType eq 'C' and  Valid eq 'Y' ";
                //string _recurso = _recurso = $"EXX_TIPO_TRANSACCION?$filter=Code eq '{Code}' ";
                string _recurso = _recurso = $"{ObjSSL}?$filter={Filtros} ";


                var respuesta = servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);
                return respuesta;

            }
            return null;
        }
        public RespuestaGenerica GetMaximoConsultaGenericaSSL(string CompanyDB, string ObjSSL, string Campos, string Filtros)
        {
            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            if (servicio.IsConected)
            {
                //Obtener los datos del socio de negocios
                //string _recurso = _recurso = $"BusinessPartners?$filter=FederalTaxID eq '{Ruc}' and CardType eq 'C' and  Valid eq 'Y' ";
                //string _recurso = _recurso = $"EXX_TIPO_TRANSACCION?$filter=Code eq '{Code}' ";
                string _recurso = _recurso = $"{ObjSSL}?$filter={Filtros} ";


                var respuesta = servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);
                return respuesta;

            }
            return null;
        }

        public async Task<RespuestaGenerica> ConsultarOVSinFacturas(ConsultaOVRequest request)
        {
            HanaCommand comm = null;
            HanaDataReader reader = null;
            List<ConsultaOVRequestModel> lista = new List<ConsultaOVRequestModel>();
            RespuestaGenerica respuesta = new RespuestaGenerica();
            try
            {
                conectarHana(request.CompanyDB);
                string inClause = string.Join(",", request.Items.Select(x => x.DocEntry).ToArray());
                string StrQty = $@"
                                SELECT 
                                    o.""DocEntry"",
                                    o.""DocNum""
                                FROM ORDR o
                                WHERE o.""DocEntry"" IN ({inClause})
                                  AND NOT EXISTS (
                                      SELECT 1
                                      FROM INV1 i
                                      INNER JOIN OINV inv ON i.""DocEntry"" = inv.""DocEntry""
                                      WHERE i.""BaseType"" = 17
                                        AND i.""BaseEntry"" = o.""DocEntry""
                                  )
                                ORDER BY o.""DocEntry"";
                            ";

                comm = new HanaCommand(StrQty, Connection);

                reader = await comm.ExecuteReaderAsync();

                if (reader != null && reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        var item = new ConsultaOVRequestModel();
                        item.DocEntry = reader.GetInt32(0);
                        item.DocNum = reader.GetInt32(1); 
                        lista.Add(item);
                    }
                    var _json = JsonConvert.SerializeObject(lista);
                    respuesta.Success = true;
                    respuesta.ErrMensaje = "ok";
                    respuesta.RespuestaJson = _json;
                    return respuesta;
                }
                else
                {
                    respuesta.Success = true;
                    respuesta.ErrMensaje = "No se encontraron órdenes sin factura para los parámetros indicados.";
                    respuesta.RespuestaJson = JsonConvert.SerializeObject(lista);
                    return respuesta;
                }
            }
            catch (Exception ex)
            {
                respuesta.Success = false;
                respuesta.ErrMensaje = ex.Message;
                respuesta.RespuestaJson = "";
                return respuesta;
            }
            finally
            {
                LiberarVariables(ref Connection, ref comm, ref reader);
                GC.Collect();
            }
        }


        public async Task<RespuestaGenerica> ConsultarOVConFacturas(ConsultaOVRequest request)
        {
            HanaCommand comm = null;
            HanaDataReader reader = null;
            List<ConsultaOVRequestModel> lista = new List<ConsultaOVRequestModel>();
            RespuestaGenerica respuesta = new RespuestaGenerica();
            try
            {
                conectarHana(request.CompanyDB);
                string inClause = string.Join(",", request.Items.Select(x => x.DocEntry).ToArray());
                string StrQty = $@"
                                SELECT 
                                    o.""DocEntry"",
                                    o.""DocNum""
                                FROM ORDR o
                                WHERE o.""DocEntry"" IN ({inClause})
                                  AND EXISTS (
                                      SELECT 1
                                      FROM INV1 i
                                      INNER JOIN OINV inv ON i.""DocEntry"" = inv.""DocEntry""
                                      WHERE i.""BaseType"" = 17
                                        AND i.""BaseEntry"" = o.""DocEntry""
                                  )
                                ORDER BY o.""DocEntry"";
                            ";

                comm = new HanaCommand(StrQty, Connection);

                reader = await comm.ExecuteReaderAsync();

                if (reader != null && reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        var item = new ConsultaOVRequestModel();
                        item.DocEntry = reader.GetInt32(0);
                        item.DocNum = reader.GetInt32(1);
                        lista.Add(item);
                    }
                    var _json = JsonConvert.SerializeObject(lista);
                    respuesta.Success = true;
                    respuesta.ErrMensaje = "ok";
                    respuesta.RespuestaJson = _json;
                    return respuesta;
                }
                else
                {
                    respuesta.Success = true;
                    respuesta.ErrMensaje = "No se encontraron órdenes sin factura para los parámetros indicados.";
                    respuesta.RespuestaJson = JsonConvert.SerializeObject(lista);
                    return respuesta;
                }
            }
            catch (Exception ex)
            {
                respuesta.Success = false;
                respuesta.ErrMensaje = ex.Message;
                respuesta.RespuestaJson = "";
                return respuesta;
            }
            finally
            {
                LiberarVariables(ref Connection, ref comm, ref reader);
                GC.Collect();
            }
        }



    }
}