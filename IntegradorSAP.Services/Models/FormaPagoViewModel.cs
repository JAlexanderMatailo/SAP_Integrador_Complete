using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegradorSAP.Data.Models
{

    public class EXX_FPAGO_VENT
    {
        //    public string odatametadata { get; set; }
        public string Code { get; set; }
        //public object Name { get; set; }
        //public long DocEntry { get; set; }
        //public string Canceled { get; set; }
        //public string Object { get; set; }
        //public object LogInst { get; set; }
        //public int UserSign { get; set; }
        //public string Transfered { get; set; }
        //public string CreateDate { get; set; }
        //public int CreateTime { get; set; }
        //public object UpdateDate { get; set; }
        //public object UpdateTime { get; set; }
        //public string DataSource { get; set; }
        public string U_Exx_referencia { get; set; }
        public List<EXX_FPAGO_VENT_DETCollection> EXX_FPAGO_VENT_DETCollection { get; set; }
    }

    public class EXX_FPAGO_VENT_DETCollection
    {
        public string Code { get; set; }
        public int LineId { get; set; }
        //public string Object { get; set; }
        //public object LogInst { get; set; }
        public string U_Exx_Forma_Pago { get; set; }
        //public object U_Exx_Plazo { get; set; }
        public float U_Exx_Total { get; set; }
        //public object U_Exx_Unidad_Tiempo { get; set; }
    }

}