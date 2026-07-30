using IntegradorSAP.Data.Entidades;
using IntegradorSAP.Data.Manager;
using IntegradorSAP.Data.Models;
using IntegradorSAP.Services.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IntegradorSAP.WebApi.Controllers
{
    [RoutePrefix("api/OrdenesCompra")]

    public class OrdenesCompraController : ApiController
    {
        
        protected OrdenesCompraManager _service => new OrdenesCompraManager();
        protected CatalogosManager _serviceBP => new CatalogosManager();

    

        [HttpGet]
        [Route("Get/{DocEntry}/{CompanyDB}")]
        public  OrdenCompraViewModel Get(int DocEntry, string CompanyDB)
        {
            
           
            OrdenCompraViewModel obj = _service.GetOrdenCompra(DocEntry, CompanyDB);

            return obj;
        }

        [Route("Login/{CompanyDB}")]
        public RespuestaGenerica Login(string CompanyDB)
        {

            bool r = _service.Login( CompanyDB);
            RespuestaGenerica respuesta = new RespuestaGenerica();
            respuesta.Success = r;
            respuesta.RespuestaJson = r ? "{true}":"{false}";
            respuesta.ErrMensaje = r ? "Conexión Exitosa" : "Conexion Fallida ";
            return respuesta;
        }

        [Route("Post")]
        public RespuestaGenerica Post([FromBody]OrdenCompraGuardar value)
        {
            OrdenCompraViewModel oc = new OrdenCompraViewModel() ;



            var CompanyDB = value.CompanyDB;
            var respuesta = _service.GuardarOrdenCompra(value, CompanyDB);

           
            return respuesta;
        }

       


        [Route("PostGuardarOrdenes")]
        public List<RespuestaGenerica> PostGuardarOrdenes([FromBody]List<OrdenCompraGuardar> ordenes)
        {
           
            List<RespuestaGenerica> respuestas = new List<RespuestaGenerica>();
            RespuestaGenerica respuesta = new RespuestaGenerica();
            foreach (var orden in ordenes)
            {

                var CompanyDB = orden.CompanyDB;
                 respuesta = _service.GuardarOrdenCompra(orden, CompanyDB);
                respuestas.Add(respuesta);
            }
            return respuestas;
        }


        [Route("CancelarOrdenesPorDocAsociado/{DocumentoAsociado}/{CompanyDB}")]
        public RespuestaGenerica CancelarOrdenesPorDocAsociado(string DocumentoAsociado, string CompanyDB)
        {
          
            string mensaje = _service.CancelarOrdenCompraPorDocAsociado(DocumentoAsociado, CompanyDB);

            RespuestaGenerica respuesta = new RespuestaGenerica();
            respuesta.Success = mensaje.Contains("Error") ? false : true;
            respuesta.RespuestaJson=$"{ mensaje }";
            respuesta.ErrMensaje = mensaje;


            return respuesta;


        }

        [HttpGet]
        [Route("GetProveedor/{Ruc}/{CompanyDB}")]
        public BusinessPartnerViewModel GetProveedor(string Ruc, string CompanyDB)
        {

            BusinessPartnerViewModel obj = _serviceBP.GetProveedor(Ruc, CompanyDB);

            return obj;
        }

        [HttpGet]
        [Route("GetCliente/{Ruc}/{CompanyDB}")]
        public BusinessPartnerViewModel GetCliente(string Ruc, string CompanyDB)
        {

            BusinessPartnerViewModel obj = _serviceBP.GetCliente(Ruc, CompanyDB);

            return obj;
        }

        [HttpGet]
        [Route("GetClienteResp/{Ruc}/{CompanyDB}")]
        public RespuestaGenerica GetClienteResp(string Ruc, string CompanyDB)
        {

            RespuestaGenerica obj = _serviceBP.GetClienteResp(Ruc, CompanyDB);

            return obj;
        }

        [HttpGet]
        [Route("GetDocumentosAsociados/{DocumentoAsociado}/{CompanyDB}")]
        public List<DocumentosAsociadosSapViewModel> GetDocumentosAsociados(string DocumentoAsociado, string CompanyDB)
        {

            List<DocumentosAsociadosSapViewModel> obj = _service.GetDocumentosAsociados(DocumentoAsociado, CompanyDB);

            return obj;
        }

        [HttpGet]
        [Route("GetOrdenesCompraDocumentosAsociados/{DocumentoAsociado}/{CompanyDB}")]
        public List<DocumentosAsociadosSapViewModel> GetOrdenesCompraDocumentosAsociados(string DocumentoAsociado, string CompanyDB)
        {

            List<DocumentosAsociadosSapViewModel> obj = _service.GetOrdenesCompraPorDocAsociados(DocumentoAsociado, CompanyDB);

            return obj;
        }
        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
