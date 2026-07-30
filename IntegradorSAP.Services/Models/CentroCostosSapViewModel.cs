using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegradorSAP.Data.Models
{
    public class CentroCostosSapViewModel 
    {
        public string FactorCode { get; set; }
        public string FactorDescription { get; set; }
        public float? TotalFactor { get; set; }
        public string Direct { get; set; }
        public int? InWhichDimension { get; set; }
        public string Active { get; set; }

    }

    public class ConsultaOVRequestModel
    {
        public long DocEntry { get; set; }
        public long DocNum { get; set; }

    }

}
