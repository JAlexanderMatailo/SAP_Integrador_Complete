using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegradorSAP.Data.Models
{

  
    public class ClienteDescuento
    {
        public string Ruc { get; set; }
        public string ItemCode { get; set; } //tYes tNo
        public string CardCode { get; set; } //tYes tNo
        public decimal Price { get; set; } //tYes tNo
        public string Currency { get; set; } //tYes tNo
        public decimal DiscountPercent { get; set; } //tYes tNo


    }

    public class ClienteDescuentoInto
    {
        public string RUC { get; set; }
         public string CompanyDB { get; set; } //tYes tNo

  
    }

}