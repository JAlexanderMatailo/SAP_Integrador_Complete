using IntegradorSAP.Data.Entidades;
using IntegradorSAP.Data.Manager;
using IntegradorSAP.Data.Models;
using IntegradorSAP.Services.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace IntegradorSAP.WebApi.Controllers
{
    [RoutePrefix("api/SolicitudTraslado")]

    public class SolicitudesTrasladoController : ApiController
    {

        protected CatalogosManager _serviceCatalog => new CatalogosManager();
        protected SalidasInventarioManager _service => new SalidasInventarioManager();
       

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
        [Route("GetCentrosCostos/{CompanyDB}")]
        public async Task<RespuestaGenerica> GetCentrosCostos( string CompanyDB)
        {
            return await _serviceCatalog.GetListaDistributionRulesDB(CompanyDB);            
        }


        [HttpGet]
        [Route("GetProyectos/{CompanyDB}")]
        public RespuestaGenerica GetProyectos(string CompanyDB)
        {
            return _serviceCatalog.GetListaProyectos(CompanyDB);
        }


        [HttpGet]
        [Route("GetBodegas/{CompanyDB}")]
        public RespuestaGenerica GetBodegas(string CompanyDB)
        {

            return _serviceCatalog.GetListaBodegas(CompanyDB);

        }

        [HttpPost]
        [Route("Post")]
        public RespuestaGenerica Post([FromBody]SolicitudTrasladoViewModel value)
        {
            SolicitudTrasladoViewModel oc = new SolicitudTrasladoViewModel();
            var CompanyDB = value.CompanyDB;
            var respuesta = _service.GuardarSolicituTraslado(value, CompanyDB);
            return respuesta;
        }

        [HttpPost]
        [Route("Delete")]
        public RespuestaGenerica Delete([FromBody]CancelaRegistroSap value)
        {
            SolicitudTrasladoViewModel oc = new SolicitudTrasladoViewModel();        
            var respuesta = _service.CancelarSolicituTraslado(value.DocEntry, value.CompanyDB);
            return respuesta;
        }
    }
}
