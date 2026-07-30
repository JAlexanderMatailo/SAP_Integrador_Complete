using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace IntegradorSAP.Data
{
    public class WebApiApplication : System.Web.HttpApplication
    {
       
        protected void Application_Start()
        {
            var logger = NLog.LogManager.Setup();
           
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}