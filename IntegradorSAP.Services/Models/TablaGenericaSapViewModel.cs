using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegradorSAP.Data.Models
{

    public class TablaGenericaSapViewModel
    {

        public string Code { get; set; }
        public string Name { get; set; }

        public Error error { get; set; }
    }
    public class TablaGenericaGuardar
    {

        public string Code { get; set; }
        public string Name { get; set; }

    }

    public class ContenedorGuardar
    {

        public string Code { get; set; }
        public string Name { get; set; }
        public string U_MODELO { get; set; }
        public string U_SERIE { get; set; }
        public string U_EXX_TIPO { get; set; } //
        public string U_EXX_SIZE { get; set; }

    }

    public class Error
    {
        public int code { get; set; }
        public Message message { get; set; }
    }

    public class Message
    {
        public string lang { get; set; }
        public string value { get; set; }
    }

}