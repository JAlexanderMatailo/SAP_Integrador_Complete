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
    [RoutePrefix("api/OrdenesVentaBasica")]

    public class OrdenesVentaBasicaController : ApiController
    {
        
        protected OrdenesVentaManager _service => new OrdenesVentaManager();
        protected CatalogosManager _serviceBP => new CatalogosManager();

        

     
        [HttpGet]
        [Route("Get/{DocEntry}/{CompanyDB}")]
        public  OrdenVentaViewModel Get(int DocEntry, string CompanyDB)
        {


            OrdenVentaViewModel obj = _service.GetOrden(DocEntry, CompanyDB);

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
        public RespuestaGenerica Post([FromBody]OrdenVentaBasicaGuardar value)
        {
            OrdenVentaViewModel oc = new OrdenVentaViewModel() ;
            var CompanyDB = value.CompanyDB;
            var respuesta = _service.GuardarOrdenVentaBasica(value, CompanyDB);

           
            return respuesta;
        }

        [Route("PostGuardarOrdenes")]
        public List<RespuestaGenerica> PostGuardarOrdenes([FromBody]List<OrdenVentaGuardar> ordenes)
        {
           
            List<RespuestaGenerica> respuestas = new List<RespuestaGenerica>();
            RespuestaGenerica respuesta = new RespuestaGenerica();
            foreach (var orden in ordenes)
            {

                var CompanyDB = orden.CompanyDB;
                 respuesta = _service.GuardarOrdenVenta(orden, CompanyDB);
                respuestas.Add(respuesta);
            }
            return respuestas;
        }


        [Route("PostGuardarDocumentoAsociado")]
        public RespuestaGenerica PostGuardarDocumentoAsociado([FromBody]DocumentoAsociadoGuardar documento)
        {

            
            RespuestaGenerica respuesta = new RespuestaGenerica();
          
            var CompanyDB = documento.CompanyDB;
            respuesta = _service.GuardarDocumentoAsociado(documento, CompanyDB);
            
            return respuesta;
        }



        [Route("CancelarOrdenPorDocAsociado")]
        public RespuestaGenerica CancelarOrdenPorDocAsociado(DocumentoAsociadoAnulacion DocumentoAsociado)
        {
          
            string mensaje = _service.CancelarOrdenPorDocAsociado(DocumentoAsociado);

            RespuestaGenerica respuesta = new RespuestaGenerica();
            respuesta.Success = mensaje.Contains("Error") ? false : true;
            respuesta.RespuestaJson=$"{ mensaje }";
            respuesta.ErrMensaje = mensaje;


            return respuesta;


        }

        [HttpGet]
        [Route("GetCliente/{Ruc}/{CompanyDB}")]
        public BusinessPartnerViewModel GetCliente(string Ruc, string CompanyDB)
        {

            BusinessPartnerViewModel obj = _serviceBP.GetCliente(Ruc, CompanyDB);

            return obj;
        }


        [HttpGet]
        [Route("GetDocumentosAsociados/{DocumentoAsociado}/{CompanyDB}")]
        public List<DocumentosAsociadosSapVtaViewModel> GetDocumentosAsociados(string DocumentoAsociado, string CompanyDB)
        {

            List<DocumentosAsociadosSapVtaViewModel> obj = _service.GetDocumentosAsociados(DocumentoAsociado, CompanyDB);

            return obj;
        }

        [HttpGet]
        [Route("GetOrdenesCompraDocumentosAsociados/{DocumentoAsociado}/{CompanyDB}")]
        public List<DocumentosAsociadosSapVtaViewModel> GetOrdenesCompraDocumentosAsociados(string DocumentoAsociado, string CompanyDB)
        {

            List<DocumentosAsociadosSapVtaViewModel> obj = _service.GetOrdenesPorDocAsociados(DocumentoAsociado, CompanyDB,"");

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
