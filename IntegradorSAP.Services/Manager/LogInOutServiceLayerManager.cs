using Newtonsoft.Json;
using Sap.Data.Hana;
using IntegradorSAP.Data.DataAccess;
using IntegradorSAP.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegradorSAP.Data.Manager
{
    public class LogInOutServiceLayer : DAO
    {
        Helper.ServiceLayer_Web servicio = new Helper.ServiceLayer_Web();
        string Usuario;
        string Ip;
        string errorCode;
        string errMsg;

        public string ErrMsg { get => ErrMsg; set => errMsg = value; }
        public string ErrorCode { get => ErrorCode; set => errorCode = value; }

        public LogInOutServiceLayer()
        { }
        public LogInOutServiceLayer(string user , string ip)

        {
            this.Usuario = user;
            this.Ip = ip;
        }
        /// <summary>
        /// Empresas SAP disponibles, leídas de la configuración (claves Sap.Login.*).
        /// Antes eran ocho empresas con sus contraseñas escritas en el código.
        /// </summary>
        public List<LoginSap> GetCompanies(string CompanyDB)
        {
            List<LoginSap> companies = new List<LoginSap>();

            foreach (string nombre in Helper.SapCompanyCredentials.CompanyDbConfiguradas())
            {
                Helper.SapCredencial credencial = Helper.SapCompanyCredentials.Obtener(nombre);

                LoginSap company = new LoginSap();
                company.CompanyDB = nombre;
                company.UserName = credencial.UserName;
                company.Password = credencial.Password;
                companies.Add(company);
            }

            if (!string.IsNullOrEmpty(CompanyDB))
            {
               companies = companies.Where(p => p.CompanyDB.Contains(CompanyDB)).ToList();
            }

            return companies;
        }

        public string RemoveSpecialCharacters(string str)
        {

            str = str.Replace("%", "").Replace("/", " ").Replace("\\", "_").Replace("*", " ").Replace("+", " ").Replace("\"", " ").Replace("-", " ").Replace("[", " ").Replace("]", "").Replace("{", "").Replace("}", "").Replace("#", " ").Replace(".", " ").Replace("}", "").Replace("\r\n", "").Replace("\n", "").Replace("\r", "");
            //return Regex.Replace(str, @"[^a-zA-Z0-9_.- ]+", "", RegexOptions.Compiled);
            return str;
        }

        public bool LoginOnServiceLayer(string CompanyDB, string UserSap, string PassUserSap)
        {
            LoginSap login = new LoginSap();
            login.CompanyDB = CompanyDB;
            login.UserName = UserSap;
            login.Password = PassUserSap;

            var _json = JsonConvert.SerializeObject(login);

            var respuesta = servicio.SLSendRequestReturnResponse("Login", "POST", _json, System.Net.HttpStatusCode.OK, true);

            if (respuesta.ErrCodigo == 0)
                return true;
            else
                return false;
          

        }

        public bool LogOutServiceLayer()
        {
            var respuesta = servicio.SLSendRequestReturnResponse("Logout", "POST", "", System.Net.HttpStatusCode.NoContent, false);
            //PresentarRespuesta(respuesta);
            return true;
        }


       
    }
}