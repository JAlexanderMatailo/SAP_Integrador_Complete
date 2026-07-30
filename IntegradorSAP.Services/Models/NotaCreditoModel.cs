using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegradorSAP.Data.Models
{

    public class NotaCreditoModel
    {
        public string CompanyDB { get; set; }
        //public string DocType { get; set; }
        public int DocEntry { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public long FolioNumber { get; set; }
        public string FolioPrefixString { get; set; }
        public string DocumentSubType { get; set; }

        public string U_DOC_DECLARABLE { get; set; }
        public string U_EXX_TIPO_TRANSACC { get; set; }
        public string NumAtCard { get; set; }
        public string U_EXX_DOC_GEN { get; set; }
        public string U_SER_EST { get; set; }
        public string U_SER_PE { get; set; }
        //public float DocRate { get; set; }
        public string U_NUM_AUTOR { get; set; }
        public string U_COD_ST { get; set; }
        public string U_tipo_comprob { get; set; }
        public string U_TIP_DOC_APLIC { get; set; }
        public string Comments { get; set; }
        public string JournalMemo { get; set; }
        public List<DocumentlineNotaCreditoModel> DocumentLines { get; set; }
    }

    public class DocumentlineNotaCreditoModel
    {
        public string ItemCode { get; set; }
        //public string Dscription { get; set; }
        public decimal Quantity { get; set; }
        public string OcrCode { get; set; }
        public decimal UnitPrice { get; set; }
        public string CostingCode { get; set; }
        public string CostingCode2 { get; set; }
        public string CostingCode3 { get; set; }
        public string CostingCode4 { get; set; }
        public string CostingCode5 { get; set; }
        public string U_DetGlosaMrk { get; set; }

    }
   


}