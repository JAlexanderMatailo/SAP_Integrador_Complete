using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegradorSAP.Data.Models
{

    public class ProfitCenterViewModel
    {
        public string CenterCode { get; set; }
        public string CenterName { get; set; }
        public string GroupCode { get; set; }
        public int InWhichDimension { get; set; }
        public object CostCenterType { get; set; }
        public string EffectiveFrom { get; set; }
        public object EffectiveTo { get; set; }
        public string Active { get; set; }
    }



    public class ProfitCenterGuardarViewModel
    {
        public string CenterCode { get; set; }
        public string CenterName { get; set; }
        public DateTime EffectiveFrom { get; set; }

        public string GroupCode { get; set; }

        public int InWhichDimension { get; set; }
        
        //public DateTime EffectiveTo { get; set; }
    }

    public class ResponseItemSap
    {
        public string CompanyDB { get; set; }
        public string CodeItemSap { get; set; }
    }


    public class ProjectsGuardarViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public String ValidFrom { get; set; }
        public String ValidTo { get; set; }
    }
}