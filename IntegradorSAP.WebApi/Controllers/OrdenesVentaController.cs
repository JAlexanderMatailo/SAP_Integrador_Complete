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
    [RoutePrefix("api/OrdenesVenta")]

    public class OrdenesVentaController : ApiController
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
        public RespuestaGenerica Post([FromBody]OrdenVentaGuardar value)
        {
            OrdenVentaViewModel oc = new OrdenVentaViewModel() ;
            var CompanyDB = value.CompanyDB;
            var respuesta = _service.GuardarOrdenVenta(value, CompanyDB);

           
            return respuesta;
        }

        [Route("PostOVCostosLocales")]
        public RespuestaGenerica PostOVCostosLocales([FromBody]OrdenVentaCLGuardar value)
        {
           
            var CompanyDB = value.CompanyDB;
            var respuesta = _service.GuardarOrdenCostosLocalesVenta(value, CompanyDB);


            return respuesta;
        }
        [Route("PostOVGuardarBasica")]
        public RespuestaGenerica PostOVGuardarBasica([FromBody]OrdenVentaBasicaGuardar value)
        {
            
            var CompanyDB = value.CompanyDB;
            var respuesta = _service.GuardarOrdenVentaBasica(value, CompanyDB);


            return respuesta;
        }
        [Route("PostOVGuardarAtcotrans")]
        public RespuestaGenerica PostOVGuardarAtcotrans([FromBody]OrdenVentaAtcontransGuardar value)
        {

            var CompanyDB = value.CompanyDB;
            var respuesta = _service.GuardarOrdenVentaAtcontrans(value, CompanyDB);


            return respuesta;
        }

        [Route("PostOVGuardarRFS")]
        public RespuestaGenerica PostOVGuardarRFS([FromBody] OrdenVentaGuardarRFSViewModel value)
        {

            var CompanyDB = value.CompanyDB;
            var respuesta = _service.GuardarOrdenVentaRFS(value, CompanyDB);


            return respuesta;
        }

        [HttpGet]
        [Route("GetPedidosFacturados")]
        public List<OrdenesFacturadas> GetPedidosFacturados(string CompanyDB, string Ids)
        {
            //Se debe enviar los DocEntry de los pedidos a los que se desea buscar si tienen facturas
            List<OrdenesFacturadas> obj = _service.GetOrdenesVentaFacturadas(CompanyDB, Ids);

            return obj;
        }

        [HttpGet]
        [Route("GetPedidosFacturadosV2")]
        public async Task<RespuestaGenerica> GetPedidosFacturadosV2(string CompanyDB, string Ids)
        {
            //Se debe enviar los DocEntry de los pedidos a los que se desea buscar si tienen facturas
            List<OrdenesFacturadas> obj = await _service.GetOrdenesVentaFacturadasAsync(CompanyDB, Ids);

            var _json = JsonConvert.SerializeObject(obj);

            RespuestaGenerica respuesta = new RespuestaGenerica();
            respuesta.Success = true;
            respuesta.ErrMensaje = $"Se encontraron {obj.Count()} registros." ;
            respuesta.RespuestaJson = _json;
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
            //try
            //{
                RespuestaGenerica respuesta = new RespuestaGenerica();
                var CompanyDB = documento.CompanyDB;
                respuesta = _service.GuardarDocumentoAsociado(documento, CompanyDB);
                return respuesta;
            
            //}catch (Exception ex)
            //{
            //    return new RespuestaGenerica
            //    {
            //        Success = false,
            //        RespuestaJson = ex.Message
            //    };
            //}
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

        [Route("CancelarOrdenByDocEntry")]
        public RespuestaGenerica CancelarOrdenByDocEntry(OrderAnulacion order)
        {

            RespuestaGenerica respuesta = _service.CanceledOrderByDocEntry(order.CompanyDB, order.DocEntry);          

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
        [Route("GetDescuentosEspecialesClientes/{Ruc}/{CompanyDB}")]        
        public RespuestaGenerica GetDescuentosEspecialesClientes(string Ruc,  string CompanyDB)
        {
            
            var obj = _service.GetDescuentosEspecialesClientes(Ruc, CompanyDB);

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
