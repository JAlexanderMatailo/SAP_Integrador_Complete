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
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace IntegradorSAP.WebApi.Controllers
{
    [RoutePrefix("api/Inventario")]

    public class InventarioController : ApiController
    {

        protected InventarioManager _service => new InventarioManager();
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
        public InventarioViewModel Get(int DocEntry, string CompanyDB)
        {


            InventarioViewModel obj = _service.Get(DocEntry, CompanyDB);

            return obj;
        }


        [HttpGet]
        [Route("GetItemsServiciosSeaboard/{CompanyDB}")]
        public List<ArticuloViewModel> GetItemsServiciosSeaboard(string CompanyDB)
        {


            var obj = _service.GetItemsServiciosSeaboard(CompanyDB);

            return obj;
        }



        [Route("Post")]
        public async Task<RespuestaGenerica> Post([FromBody] InventarioGuardarViewModel value)
        {

            InventarioViewModel oc = new InventarioViewModel();
            var CompanyDB = value.CompanyDB;
            RespuestaGenerica respuesta = new RespuestaGenerica();

            respuesta = await _service.GuardarTransferenciaStock(value, CompanyDB);

            // return respuestaTansSt;

            if (respuesta.Success)
            {
                var transfstock = JsonConvert.DeserializeObject<StockInventarioGuardarViewModel>(respuesta.RespuestaJson);

                if (value.DocumentLines != null)
                {
                    //foreach(var item in value.DocumentLines)
                    //{
                    //    item.WarehouseCode = transfstock.FromWarehouse;
                    //}
                    respuesta = await _service.GuardarSalidaInventario(value, CompanyDB, transfstock.DocEntry, transfstock.DocNum);

                }

                // return respuestaSal;
            }
            return respuesta;

        }

        [Route("PostV1")]
        public async Task<RespuestaGenerica> GuardarSalidaInventario([FromBody] InventarioGuardarViewModel value)
        {

            InventarioViewModel oc = new InventarioViewModel();
            var CompanyDB = value.CompanyDB;
            RespuestaGenerica respuesta = new RespuestaGenerica();

            respuesta = await _service.GuardarSalidaInventario(value, CompanyDB, null, null);


            return respuesta;

        }



        [HttpGet]
        [Route("GetMaterialesSap/{CompanyDB}")]
        public async Task<RespuestaGenerica> GetMaterialesSap(string CompanyDB)
        {
            
            var obj = await _service.GetMaterialesSap(CompanyDB);

            return obj;
        }
    }
}
