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
using static IntegradorSAP.Data.Models.OrdenesCompraATCOViewModel;

namespace IntegradorSAP.WebApi.Controllers
{
    [RoutePrefix("api/OrdenesCompraATCO")]

    public class OrdenesCompraATCOController : ApiController
    {
        protected OrdenesCompraManager _service => new OrdenesCompraManager();
        protected CatalogosManager _serviceBP => new CatalogosManager();
        [Route("Login/{CompanyDB}")]
        public RespuestaGenerica Login(string CompanyDB)
        {

            bool r = _service.Login(CompanyDB);
            RespuestaGenerica respuesta = new RespuestaGenerica();
            respuesta.Success = r;
            respuesta.RespuestaJson = r ? "{true}" : "{false}";
            respuesta.ErrMensaje = r ? "Conexión Exitosa" : "Conexion Fallida ";
            return respuesta;
        }


        [HttpGet]
        [Route("Get/{DocEntry}/{CompanyDB}")]
        public OrdenCompraATCOViewModel Get(int DocEntry, string CompanyDB)
        {


            OrdenCompraATCOViewModel obj = _service.GetOrdenCompraATCO(DocEntry, CompanyDB);

            return obj;
        }

        
        [Route("Post")]
        public RespuestaGenerica Post([FromBody]OrdenCompraATCOGuardar value)
        {
            OrdenCompraATCOViewModel oc = new OrdenCompraATCOViewModel();

            var CompanyDB = value.CompanyDB;
            var respuesta = _service.GuardarOrdenCompraATCO(value, CompanyDB);


            return respuesta;
        }


        [Route("PostGuardarOrdenes")]
        public List<RespuestaGenerica> PostGuardarOrdenes([FromBody]List<OrdenCompraATCOGuardar> ordenes)
        {

            List<RespuestaGenerica> respuestas = new List<RespuestaGenerica>();
            RespuestaGenerica respuesta = new RespuestaGenerica();
            foreach (var orden in ordenes)
            {

                var CompanyDB = orden.CompanyDB;
                respuesta = _service.GuardarOrdenCompraATCO(orden, CompanyDB);
                respuestas.Add(respuesta);
            }
            return respuestas;
        }

    }
}