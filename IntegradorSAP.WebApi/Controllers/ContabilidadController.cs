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
    [RoutePrefix("api/Contabilidad")]

    public class ContabilidadController : ApiController
    {
        protected ContabilidadManager _service => new ContabilidadManager();       


        [Route("GuardarAsientoContable")]
        public async Task<RespuestaGenerica> GuardarAsientoContable([FromBody] JournalEntriesViewModel orden)
        {
            RespuestaGenerica respuesta = new RespuestaGenerica();          

            var CompanyDB = orden.CompanyDB;
            respuesta = await _service.GuardarAsientoContableNomina(orden);
            return respuesta;
        }      
    }
}
