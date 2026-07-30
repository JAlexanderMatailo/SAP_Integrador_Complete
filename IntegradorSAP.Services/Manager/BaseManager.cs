using Newtonsoft.Json;
using IntegradorSAP.Data.DataAccess;
using IntegradorSAP.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IntegradorSAP.Data.Models;
using IntegradorSAP.Data.Helper;
using IntegradorSAP.Data.Entidades;
using IntegradorSAP.Data.Manager;
namespace IntegradorSAP.Data.Manager
{
    public class BaseManager:DAO
    {
        ServiceLayer_Web servicio = new ServiceLayer_Web();
        public bool Login(string CompanyDB)
        {

            if (!servicio.IsConected)
                servicio.ConectarSL(CompanyDB);

            return servicio.IsConected;

        }

    }
}