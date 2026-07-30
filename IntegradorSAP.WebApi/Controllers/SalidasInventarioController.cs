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
    [RoutePrefix("api/SalidasInventari")]

    public class SalidasInventarioController : ApiController
    {

        protected SalidasInventarioManager _service => new SalidasInventarioManager();
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
        public SalidasInventarioViewModel Get(int DocEntry, string CompanyDB)
        {


            SalidasInventarioViewModel obj = _service.Get(DocEntry, CompanyDB);

            return obj;
        }

        [Route("Post")]
        public RespuestaGenerica Post([FromBody]SalidasInventarioGuardarViewModel value)
        {
            SalidasInventarioViewModel oc = new SalidasInventarioViewModel();
            var CompanyDB = value.CompanyDB;
            var respuesta = _service.Guardar(value, CompanyDB);


            return respuesta;
        }
    }
}
