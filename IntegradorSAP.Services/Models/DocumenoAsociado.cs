using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegradorSAP.Data.Models
{

    public class DocumenoAsociadoItem
    {
        public string odatametadata { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int DocEntry { get; set; }
        public string Canceled { get; set; }
        public string Object { get; set; }
        public object LogInst { get; set; }
        public int UserSign { get; set; }
        public string Transfered { get; set; }
        public string CreateDate { get; set; }
        public int CreateTime { get; set; }
        public string UpdateDate { get; set; }
        public int UpdateTime { get; set; }
        public string DataSource { get; set; }
        public string U_EXX_VIAJE { get; set; }
        public string U_EXX_TDOC { get; set; }
        public string U_EXX_MBL { get; set; }
        public string U_EXX_HBL { get; set; }
        public string U_EXX_TIP_OP { get; set; }
        public string U_EXX_TIP_EMB { get; set; }
        public object U_EXX_PORI { get; set; }
        public object U_EXX_PDEST { get; set; }
        public string U_EXX_CVEND { get; set; }
        public string U_EXX_DVEND { get; set; }
        public string U_EXX_BOOK { get; set; }
        public string U_EXX_CON_CAR { get; set; }
        public string U_EXX_CAMCLAVE { get; set; }
        public string U_EXX_SEM { get; set; }
        public string U_EXX_FE_INI { get; set; }
        public string U_EXX_FE_FIN { get; set; }
        public string U_EXX_TIP_CONT { get; set; }
        public string U_EXX_TI_ORD { get; set; }
        public string U_EXX_COD_CLI { get; set; }
        public string U_EXX_NOMB_CLI { get; set; }
        public string U_EXX_FEC_EMI { get; set; }
        public string U_EXX_EMB { get; set; }
        public string U_EXX_CONS { get; set; }
        public string U_EXX_LINEA { get; set; }
        public float U_EXX_TO_FLE { get; set; }
        public float U_EXX_SER_LOG { get; set; }
        public float U_EXX_COS_TOT { get; set; }
        public float U_EXX_O_LIQ { get; set; }
        public float U_EXX_TP_TER { get; set; }
        public float U_EXX_TO_OPRO { get; set; }
        public float U_EXX_TO_AG { get; set; }
        public float U_EXX_TO_PRO { get; set; }
        public float U_EXX_POR_SERLOG { get; set; }
        public object U_EXX_CONT { get; set; }
        public string U_EXX_DES_LIN { get; set; }
        public string U_EXX_NUM_GUIA { get; set; }
        public string U_EXX_MAQ { get; set; }
        public string U_EXX_OBSERV { get; set; }
        public string U_EXX_COD_AG { get; set; }
        public string U_EXX_DES_AG { get; set; }
        public string U_EXX_NAVE { get; set; }
        public string U_EXX_NUM_COT { get; set; }
        public string U_EXX_ROPOPA { get; set; }
    }

    public class ListaDocumentosAsociados
    {
        public string odatametadata { get; set; }
        public List<DocumenoAsociado> DocumenoAsociado { get; set; }
        public string odatanextLink { get; set; }
    }


    public class DocumenoAsociado
    {
        public string error { get; set; }
        public string Code { get; set; }
        public object Name { get; set; }
        public long DocEntry { get; set; }
        public string Canceled { get; set; }
        public string Object { get; set; }
        public object LogInst { get; set; }
        public int UserSign { get; set; }
        public string Transfered { get; set; }
        public string CreateDate { get; set; }
        public int CreateTime { get; set; }
        public string UpdateDate { get; set; }
        public int UpdateTime { get; set; }
        public string DataSource { get; set; }
        public string U_EXX_VIAJE { get; set; }
        public string U_EXX_TDOC { get; set; }
        public string U_EXX_MBL { get; set; }
        public string U_EXX_HBL { get; set; }
        public string U_EXX_TIP_OP { get; set; }
        public string U_EXX_TIP_EMB { get; set; }
        public string U_EXX_PORI { get; set; }
        public string U_EXX_PDEST { get; set; }
        public string U_EXX_CVEND { get; set; }
        public string U_EXX_DVEND { get; set; }
        public string U_EXX_BOOK { get; set; }
        public string U_EXX_CON_CAR { get; set; }
        public string U_EXX_CAMCLAVE { get; set; }
        public int U_EXX_SEM { get; set; }
        public string U_EXX_FE_INI { get; set; }
        public string U_EXX_FE_FIN { get; set; }
        public string U_EXX_TIP_CONT { get; set; }
        public string U_EXX_TI_ORD { get; set; }
        public string U_EXX_COD_CLI { get; set; }
        public string U_EXX_NOMB_CLI { get; set; }
        public string U_EXX_FEC_EMI { get; set; }
        public string U_EXX_EMB { get; set; }
        public string U_EXX_CONS { get; set; }
        public string U_EXX_LINEA { get; set; }
        public float U_EXX_TO_FLE { get; set; }
        public float U_EXX_SER_LOG { get; set; }
        public float U_EXX_COS_TOT { get; set; }
        public float U_EXX_O_LIQ { get; set; }
        public float U_EXX_TP_TER { get; set; }
        public float U_EXX_TO_OPRO { get; set; }
        public float U_EXX_TO_AG { get; set; }
        public float U_EXX_TO_PRO { get; set; }
        public float U_EXX_POR_SERLOG { get; set; }
        public string U_EXX_CONT { get; set; }
        public string U_EXX_DES_LIN { get; set; }
        public string U_EXX_NUM_GUIA { get; set; }
        public string U_EXX_MAQ { get; set; }
        public string U_EXX_OBSERV { get; set; }
        public string U_EXX_COD_AG { get; set; }
        public string U_EXX_DES_AG { get; set; }
        public string U_EXX_NAVE { get; set; }
        public string U_EXX_NUM_COT { get; set; }
        public string U_EXX_ROPOPA { get; set; }
    }

    public class DocumentoAsociadoSave
    {

        public long DocEntry { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }

        public string U_EXX_VIAJE { get; set; }
        public string U_EXX_TDOC { get; set; }
        public string U_EXX_MBL { get; set; }
        public string U_EXX_HBL { get; set; }
        public string U_EXX_TIP_OP { get; set; }
        public string U_EXX_TIP_EMB { get; set; }
        public string U_EXX_PORI { get; set; }
        public string U_EXX_PDEST { get; set; }
        public string U_EXX_CVEND { get; set; }
        public string U_EXX_DVEND { get; set; }
        public string U_EXX_BOOK { get; set; }
        public string U_EXX_CON_CAR { get; set; }
        public string U_EXX_CAMCLAVE { get; set; }
        public string U_EXX_SEM { get; set; }
        public DateTime U_EXX_FE_INI { get; set; }
        public DateTime U_EXX_FE_FIN { get; set; }
        public string U_EXX_TIP_CONT { get; set; }
        public string U_EXX_TI_ORD { get; set; }
        public string U_EXX_COD_CLI { get; set; }
        public string U_EXX_NOMB_CLI { get; set; }
        public DateTime U_EXX_FEC_EMI { get; set; }
        public string U_EXX_EMB { get; set; }
        public string U_EXX_CONS { get; set; }
        public string U_EXX_LINEA { get; set; }
        public float U_EXX_TO_FLE { get; set; }
        public float U_EXX_SER_LOG { get; set; }
        public float U_EXX_COS_TOT { get; set; }
        public float U_EXX_O_LIQ { get; set; }
        public float U_EXX_TP_TER { get; set; }
        public float U_EXX_TO_OPRO { get; set; }
        public float U_EXX_TO_AG { get; set; }
        public float U_EXX_TO_PRO { get; set; }
        public float U_EXX_POR_SERLOG { get; set; }
        public string U_EXX_CONT { get; set; }
        public string U_EXX_DES_LIN { get; set; }
        public string U_EXX_NUM_GUIA { get; set; }
        public string U_EXX_MAQ { get; set; }
        public string U_EXX_OBSERV { get; set; }
        public string U_EXX_COD_AG { get; set; }
        public string U_EXX_DES_AG { get; set; }
        public string U_EXX_NAVE { get; set; }

        public string U_EXX_NUM_COT { get; set; }
        public string U_EXX_ROPOPA { get; set; }


    }


}