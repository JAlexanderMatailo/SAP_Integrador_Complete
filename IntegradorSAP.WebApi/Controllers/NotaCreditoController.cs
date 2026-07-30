using IntegradorSAP.Data.Entidades;
using IntegradorSAP.Data.Manager;
using IntegradorSAP.Data.Models;
using IntegradorSAP.Services.Manager;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IntegradorSAP.WebApi.Controllers
{
    [RoutePrefix("api/NotaCredito")]

    public class NotaCreditoController : ApiController
    {
        private Logger _logger = LogManager.GetLogger("DataLog");
        protected NotasCreditoManager _service => new NotasCreditoManager();
   

        [Route("Post")]
        public RespuestaGenerica GuardarOrdenVentaTurnos([FromBody] NotaCreditoModel orden)
        {
            RespuestaGenerica respuesta = new RespuestaGenerica();
            respuesta = _service.GuardarNotaCredito(orden);
            return respuesta;
        }


        
    }
}
