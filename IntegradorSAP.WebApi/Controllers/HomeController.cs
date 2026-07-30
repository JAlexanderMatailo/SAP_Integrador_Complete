using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntegradorSAP.WebApi.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// La raiz del sitio lleva a Swagger.
        ///
        /// Este integrador no tiene interfaz de usuario: es solo una API. La
        /// vista Views/Home/Index.cshtml era la plantilla por defecto de MVC y no
        /// aportaba nada, asi que / redirige a la documentacion, que es lo unico
        /// que alguien quiere ver al abrir el sitio.
        ///
        /// Se hace aqui, y no solo con la pagina de inicio del proyecto, para que
        /// funcione con cualquier host: IIS Express al pulsar F5, IIS en el
        /// servidor, o el sitio publicado.
        ///
        /// Es un 302 (temporal) a proposito y no un 301: si algun dia se quiere
        /// una pagina de inicio de verdad, un 301 quedaria cacheado en los
        /// navegadores y costaria revertirlo.
        /// </summary>
        public ActionResult Index()
        {
            return Redirect(RutaInterfazSwagger);
        }

        /// <summary>
        /// /swagger tambien lleva a la interfaz, porque es la URL que la gente
        /// escribe por costumbre. La sirve un archivo estatico y no
        /// EnableSwaggerUi: ver la explicacion en SwaggerConfig.
        /// </summary>
        public ActionResult Swagger()
        {
            return Redirect(RutaInterfazSwagger);
        }

        private const string RutaInterfazSwagger = "~/swagger-ui/index.html";
    }
}
