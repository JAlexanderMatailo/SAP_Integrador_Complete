using Amazon.Runtime.Internal;
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
using System.Threading.Tasks;
using System.Web.Http;

namespace IntegradorSAP.WebApi.Controllers
{
    // El prefijo pasa a api/sap. La ruta antigua api/CTK sigue funcionando
    // gracias a RutasHeredadasHandler, que reescribe la URL antes del enrutado.
    // Se puede retirar en cuanto los consumidores apunten a api/sap.
    [RoutePrefix("api/sap")]

    public class ComercialController : ApiController
    {

        protected ComercialManager _service => new ComercialManager();
    
    

        [Route("CancelarOrdenesSap")]
        public List<RespuestaGenerica> CancelarOrdenesSap(OrdenVentaCancelarRequest Docs)
        {
           
            List<RespuestaGenerica> Lstrespuesta = new List<RespuestaGenerica>();

            List<OrdenesVentaCancelarItem> LstOV = _service.GetOrdenesVentaBls(Docs.NumeroBL, Docs.TipoMovimiento, Docs.CompanyDB);

            foreach( var item in LstOV)
            {
                RespuestaGenerica respuesta = new RespuestaGenerica();
                string mensaje = "";
                if (item.DocStatus=="C")
                {
                    mensaje = "Este pedido se encuentra cerrado. No se puede anular para evitar duplicados";
                    respuesta.Success = false;// respuesta.Contains("Error") ? false : true;
                    respuesta.RespuestaJson = $"{ mensaje }";
                    respuesta.ErrMensaje = mensaje;

                }

                if (mensaje=="")
                {
                    var result = _service.CancelarOrdenComercial(item, Docs.CompanyDB);
                    if (result.ErrMensaje.Contains("Error: -1000--Error por StatudCode: No Content NoContent."))
                    {
                        respuesta.Success = true;
                        respuesta.ErrMensaje = "";
                        respuesta.RespuestaJson = $"{ "" }";
                    }
                    else
                    {
                        mensaje = result.ErrMensaje;

                        respuesta.Success = mensaje.Contains("Error") ? false : true;
                        respuesta.RespuestaJson = $"{ mensaje }";
                        respuesta.ErrMensaje = mensaje;

                    }
                 

                }

                Lstrespuesta.Add(respuesta);

            }


            return Lstrespuesta;


        }


        [Route("GetAliasSocioNegocio")]
        public RespuestaGenerica GetAliasSocioNegocio(SocioNegocioRequest Cliente)
        {
            RespuestaGenerica respuesta = new RespuestaGenerica();
            // List<RespuestaGenerica> Lstrespuesta = new List<RespuestaGenerica>();

            var socio = _service.GetAliasSocioNegocio(Cliente.RUC, Cliente.CompanyDB);
            var _json = JsonConvert.SerializeObject(socio);

            if (socio.Mensaje!="")
            {
                respuesta.Success = false;
                respuesta.ErrMensaje = socio.Mensaje;
            }
         else
            {
                
                //                           
                respuesta.RespuestaJson = _json;
                respuesta.Success = true;
            }

            return respuesta;


        }


        [Route("GetItemCodeMapeoCostoLocal")]
        public RespuestaGenerica GetItemCodeMapeoCostoLocal(CostoLocalRequest Costolocal)
        {
            RespuestaGenerica respuesta = new RespuestaGenerica();
            // List<RespuestaGenerica> Lstrespuesta = new List<RespuestaGenerica>();

            var costo = _service.GetItemCodeMapeoCostoLocal(Costolocal.CodigoCL, Costolocal.CompanyDB);
            var _json = JsonConvert.SerializeObject(costo);

            if (costo.Mensaje != "")
            {
                respuesta.Success = false;
                respuesta.ErrMensaje = costo.Mensaje;
            }
            else
            {

                //                           
                respuesta.RespuestaJson = _json;
                respuesta.Success = true;
            }

            return respuesta;


        }


        //[Route("GuardarOrdenVentaCTK")]
        //public List<RespuestaGenerica> GuardarOrdenVentaComercial([FromBody] List<OrdenVentaGuardarRequest> ordenes)
        //{

        //    List<RespuestaGenerica> respuestas = new List<RespuestaGenerica>();
        //    RespuestaGenerica respuesta = new RespuestaGenerica();
        //    foreach (var orden in ordenes)
        //    {

        //        respuesta = _service.GuardarOrdenVentaComercial(orden, CompanyDB);
        //        respuestas.Add(respuesta);
        //    }
        //    return respuestas;
        //}

        [Route("GuardarOrdenVentaEmpresa1")]
        public RespuestaGenerica GuardarOrdenVentaComercial([FromBody] OrdenVentaGuardarRequest orden)
        {

            List<RespuestaGenerica> respuestas = new List<RespuestaGenerica>();
            RespuestaGenerica respuesta = new RespuestaGenerica();

            var CompanyDB = orden.CompanyDB;
            respuesta = _service.GuardarOrdenVentaComercial(orden, CompanyDB);

            return respuesta;
        }

        [Route("GuardarOrdenVentaEmpresa3")]
        public RespuestaGenerica GuardarOrdenVentaTurnos([FromBody] OrdenVentaGuardarRequest orden)
        {
            List<RespuestaGenerica> respuestas = new List<RespuestaGenerica>();
            RespuestaGenerica respuesta = new RespuestaGenerica();
            var CompanyDB = orden.CompanyDB;
            respuesta = _service.GuardarOrdenVentaTurnos(orden, CompanyDB);
            return respuesta;
        }

        [Route("GuardarOrdenVentaEmpresa4")]
        public RespuestaGenerica GuardarOrdenVentaRFSTurnos([FromBody] OrdenVentaGuardarRequest orden)
        {
            List<RespuestaGenerica> respuestas = new List<RespuestaGenerica>();
            RespuestaGenerica respuesta = new RespuestaGenerica();
            var CompanyDB = orden.CompanyDB;
            respuesta = _service.GuardarOrdenVentaTurnos(orden, CompanyDB);
            return respuesta;
        }

        [HttpPost]
        [Route("ValidarSocioNegocio")]

        public RespuestaGenerica ValidarSocioNegocio([FromBody] SocioNegocioRequest Cliente)
        {
            RespuestaGenerica respuesta = new RespuestaGenerica();

            var socio = _service.GetAliasSocioNegocio(Cliente.RUC, Cliente.CompanyDB);
            var _json = JsonConvert.SerializeObject(socio);

            if (!string.IsNullOrEmpty(socio.Mensaje))
            {
                respuesta.Success = false;
                respuesta.ErrMensaje = socio.Mensaje;
            }
            else
            {
                respuesta.RespuestaJson = _json;
                respuesta.Success = true;
            }

            return respuesta;
        }

        [HttpGet]
        [Route("ConsultarCentroDeCostoPorCodigo/{CodigoCentro}/{CompanyDB}")]
        public bool ConsultarCentroDeCostoPorCodigo(string CodigoCentro, string CompanyDB)
        {
            try
            {
                CatalogosManager centroCosto = new CatalogosManager();
                var result = centroCosto.GetCentroCostos(CodigoCentro, CompanyDB);
                return result != null ? true : false;
            }catch (Exception ex)
            {
                throw new Exception("Error al consultar Centro de Costo: " + ex.Message);
            }
        }
        
        [HttpGet]
        [Route("ValidarOrdenVentaEmpresa1/{CodTurno}/{database}")]
        public bool ValidarOrdenVentaRFSTurnos(string CodTurno, string database)
        {
            try
            {
                CatalogosManager catalogos = new CatalogosManager();
                return catalogos.ExisteOrdenVentaPorCodigoTurno(CodTurno, database);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al validar Orden de Venta por código de turno: " + ex.Message);
            }
        }
        
        [Route("GuardarCentroDeCosto/{CompanyDB}")]
        public RespuestaGenerica GuardarCentroDeCosto([FromBody] ProfitCenterGuardarViewModel profitCenterGuardarViewModel, string CompanyDB)
        {
            try
            {
                CatalogosManager centroCosto = new CatalogosManager();
                var result = centroCosto.GuardarCentroCostos(profitCenterGuardarViewModel, CompanyDB);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear Centro de Costo: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("ConsultarOVSinFacturas")]
        public async Task<RespuestaGenerica> ConsultarOVSinFacturas([FromBody] ConsultaOVRequest request)
        {
            try
            {
                CatalogosManager centroCosto = new CatalogosManager();
                var result = await centroCosto.ConsultarOVSinFacturas(request);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear Centro de Costo: " + ex.Message);
            }
        }
        [HttpPost]
        [Route("ConsultarOVConFacturas")]
        public async Task<RespuestaGenerica> ConsultarOVConFacturas([FromBody] ConsultaOVRequest request)
        {
            try
            {
                CatalogosManager centroCosto = new CatalogosManager();
                var result = await centroCosto.ConsultarOVConFacturas(request);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear Centro de Costo: " + ex.Message);
            }
        }



        [Route("CancelarOrdenesPorTipoDocumentoSap")]
        public List<RespuestaGenerica> CancelarOrdenesPorTipoDocumentoSap(OrdenVentaCancelarRequest Docs)
        {

            List<RespuestaGenerica> Lstrespuesta = new List<RespuestaGenerica>();

            List<OrdenesVentaCancelarItem> LstOV = _service.GetOrdenesVentaBls(Docs.NumeroBL, Docs.TipoMovimiento, Docs.CompanyDB);

            foreach (var item in LstOV)
            {
                RespuestaGenerica respuesta = new RespuestaGenerica();
                string mensaje = "";
                if (item.DocStatus == "C")
                {
                    mensaje = "Este pedido se encuentra cerrado. No se puede anular para evitar duplicados";
                    respuesta.Success = false;// respuesta.Contains("Error") ? false : true;
                    respuesta.RespuestaJson = $"{ mensaje }";
                    respuesta.ErrMensaje = mensaje;

                }

                if (mensaje == "")
                {
                    var result = _service.CancelarOrdenComercial(item, Docs.CompanyDB);
                    if (result.ErrMensaje.Contains("Error: -1000--Error por StatudCode: No Content NoContent."))
                    {
                        respuesta.Success = true;
                        respuesta.ErrMensaje = "";
                        respuesta.RespuestaJson = $"{ "" }";
                    }
                    else
                    {
                        mensaje = result.ErrMensaje;

                        respuesta.Success = mensaje.Contains("Error") ? false : true;
                        respuesta.RespuestaJson = $"{ mensaje }";
                        respuesta.ErrMensaje = mensaje;

                    }


                }

                Lstrespuesta.Add(respuesta);

            }


            return Lstrespuesta;


        }

        [Route("ConsultarItemSap")]
        public ItemsViewModel ConsultarItemSap([FromBody] ResponseItemSap responseItemSap)
        {
            try
            {
                CatalogosManager item = new CatalogosManager();
                var result = item.GetItem(responseItemSap.CodeItemSap, responseItemSap.CompanyDB);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear Centro de Costo: " + ex.Message);
            }
        }


    }
}
