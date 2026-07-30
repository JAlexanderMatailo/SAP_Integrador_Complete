using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegradorSAP.Data.Models
{
    public class Inventario
    {
        public string odatametadata { get; set; }
        public int DocEntry { get; set; }
        public int Series { get; set; }
        public string Printed { get; set; }
        public string DocDate { get; set; }
        public string DueDate { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string Address { get; set; }
        public string Reference1 { get; set; }
        public string Reference2 { get; set; }
        public string Comments { get; set; }
        public string JournalMemo { get; set; }
        public int PriceList { get; set; }
        public int SalesPersonCode { get; set; }
        public string FromWarehouse { get; set; }
        public string ToWarehouse { get; set; }
        public string CreationDate { get; set; }
        public string UpdateDate { get; set; }
        public int FinancialPeriod { get; set; }
        public int TransNum { get; set; }
        public int DocNum { get; set; }
        public string TaxDate { get; set; }
        public int ContactPerson { get; set; }
        public string FolioPrefixString { get; set; }
        public int FolioNumber { get; set; }
        public string DocstringCode { get; set; }
        public string AuthorizationStatus { get; set; }
        public string BPLID { get; set; }
        public string BPLName { get; set; }
        public string VATRegNum { get; set; }
        public string AuthorizationCode { get; set; }
        public string StartDeliveryDate { get; set; }
        public string StartDeliveryTime { get; set; }
        public string EndDeliveryDate { get; set; }
        public string EndDeliveryTime { get; set; }
        public string VehiclePlate { get; set; }
        public string ATDocumentType { get; set; }
        public string EDocExportFormat { get; set; }
        public string ElecCommStatus { get; set; }
        public string ElecCommMessage { get; set; }
        public string PointOfIssueCode { get; set; }
        public string Letter { get; set; }
        public string FolioNumberFrom { get; set; }
        public string FolioNumberTo { get; set; }
        public string AttachmentEntry { get; set; }
        public string DocumentStatus { get; set; }
        public string U_SER_EST { get; set; }
        public string U_SER_PE { get; set; }
        public string U_NUM_AUTOR { get; set; }
        public string U_COD_ST { get; set; }
        public string U_SER_EST_FR { get; set; }
        public string U_SER_PEFR { get; set; }
        public string U_NUM_AUT_FR { get; set; }
        public string U_NUM_FAC_REL { get; set; }
        public string U_COMP_RET { get; set; }
        public string U_SERIE_RET { get; set; }
        public string U_NUM_AUT_RET { get; set; }
        public string U_FEC_INI_TRAS { get; set; }
        public string U_FEC_FIN_TRAS { get; set; }
        public string U_fecha_emi_doc_rel { get; set; }
        public string U_NUM_DECLAR_ADU { get; set; }
        public string U_MOT_TRASLADO { get; set; }
        public string U_PUNTO_PART { get; set; }
        public string U_NUM_GUIA { get; set; }
        public string U_CORRELATIVO { get; set; }
        public string U_VERIFICADOR { get; set; }
        public string U_TRANSPORTE { get; set; }
        public string U_TRANSPORTISTA { get; set; }
        public string U_FECHA_EMBARQUE { get; set; }
        public string U_Exx_IP_Pago { get; set; }
        public string U_Exx_IP_Pais { get; set; }
        public string U_Exx_IP_DobleTrib { get; set; }
        public string U_Exx_IP_SujetRet_NL { get; set; }
        public string U_Exx_FechaRet { get; set; }
        public string U_Exx_Rembolso { get; set; }
        public string U_Exx_FPagxDoc { get; set; }
        public string U_TIP_DOC_APLIC { get; set; }
        public string U_tipo_export { get; set; }
        public string U_tipo_comprob { get; set; }
        public string U_DOC_DECLARABLE { get; set; }
        public string U_DISTRITO_ADU { get; set; }
        public string U_REFRENDO_ANIO { get; set; }
        public string U_REFRENDO_REG { get; set; }
        public float U_VALOR_FOB { get; set; }
        public string U_NUM_DOC_TRANSP { get; set; }
        public string U_NUM_FUE { get; set; }
        public string U_MOT_NC { get; set; }
        public string U_MOT_ND { get; set; }
        public string U_EXX_FE_TIPCOM { get; set; }
        public string U_EXX_FE_TIPAMB { get; set; }
        public string U_EXX_FE_CODNUM { get; set; }
        public string U_EXX_FE_TIPEMI { get; set; }
        public string U_EXX_FE_DIFVER { get; set; }
        public string U_EXX_FE_FECAUT { get; set; }
        public string U_Exx_Din_Cons { get; set; }
        public string U_Exx_FE_Paisdestin { get; set; }
        public string U_Exx_pagoRegFis { get; set; }
        public string U_Exx_fechaPagoDiv { get; set; }
        public float U_Exx_imRentaSoc { get; set; }
        public string U_Exx_anioUtDiv { get; set; }
        public string U_Exx_numCajBan { get; set; }
        public float U_Exx_precCajBan { get; set; }
        public string U_EXX_CAIM { get; set; }
        public string U_EXX_Hora_Lleg { get; set; }
        public string U_EXX_Hora_Sal { get; set; }
        public string U_Exx_doc_compensac { get; set; }
        public string U_EXX_FPAGO_VENTAS { get; set; }
        public string U_EXX_FACT_NEG { get; set; }
        public string U_EXX_FPAGO_COMPRAS { get; set; }
        public string U_EXX_COMPENSADO { get; set; }
        public float U_EXX_VAL_COMP { get; set; }
        public string U_Exx_DenoFiscal { get; set; }
        public string U_Exx_ingFueGra_IR { get; set; }
        public string U_Exx_Paraiso_Fis { get; set; }
        public string U_Exx_TipIngExt { get; set; }
        public string U_Exx_TipRegFis { get; set; }
        public float U_Exx_ValoImpExt { get; set; }
        public string U_EXX_MAN_AG { get; set; }
        public string U_EXX_TIPO_TRANSACC { get; set; }
        public string U_EXX_DOC_GEN { get; set; }
        public string U_EXX_FE_DESERR { get; set; }
        public string U_EXX_FE_CODERR { get; set; }
        public string U_EXX_FE_Estado { get; set; }
        public string U_EXX_FE_ClaAcc { get; set; }
        public string U_EXX_Serie_Guia { get; set; }
        public string U_EXX_FE_PdfCreado { get; set; }
        public string U_EXX_FE_PdfError { get; set; }
        public string U_EXX_FE_MailEnviado { get; set; }
        public string U_EXX_FE_MailError { get; set; }
        public string U_EXX_FE_Reemb { get; set; }
        public string U_Exx_FE_ComExt { get; set; }
        public string U_Exx_FE_IncoTermFac { get; set; }
        public string U_Exx_FE_LugIncoTerm { get; set; }
        public string U_Exx_FE_PaisOrigen { get; set; }
        public string U_Exx_FE_PuertoEmb { get; set; }
        public string U_Exx_FE_PuertoDest { get; set; }
        public string U_Exx_FE_Paisadquis { get; set; }
        public string U_Exx_FE_Incotermto { get; set; }
        public string U_Exx_FE_Fleteinter { get; set; }
        public string U_Exx_FE_Segurointe { get; set; }
        public string U_EXX_CONTENEDOR { get; set; }
        public string U_LocalCtaContab { get; set; }
        public string U_CLAVE_ACCESO { get; set; }
        public string U_ESTADO_AUTORIZACIO { get; set; }
        public string U_NUM_AUTO_FAC { get; set; }
        public string U_FECHA_AUT_FACT { get; set; }
        public string U_OBSERVACION_FACT { get; set; }
        public string U_SSCREADAR { get; set; }
        public string U_SSIDDOCUMENTO { get; set; }
        public string U_HRH_Serie { get; set; }
        public string U_HRH_Modo_Fact { get; set; }
        public string U_HRH_Lote { get; set; }
        public string U_LQ_CLAVE { get; set; }
        public string U_LQ_ESTADO { get; set; }
        public string U_LQ_NUM_AUTO { get; set; }
        public string U_LQ_FECHA_AUT { get; set; }
        public string U_LQ_OBSERVACION { get; set; }
        public string U_DOC_REF { get; set; }
        public object[] StockTransfer_ApprovalRequests { get; set; }
        public Stocktransferline[] StockTransferLines { get; set; }
        public Stocktransfertaxextension StockTransferTaxExtension { get; set; }
    }

    public class Stocktransfertaxextension
    {
        public string SupportVAT { get; set; }
        public string FormNumber { get; set; }
        public string TransactionCategory { get; set; }
    }

    public class Stocktransferline
    {
        public int LineNum { get; set; }
        public int DocEntry { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public float Quantity { get; set; }
        public float Price { get; set; }
        public string Currency { get; set; }
        public string Rate { get; set; }
        public float DiscountPercent { get; set; }
        public string VendorNum { get; set; }
        public string SerialNumber { get; set; }
        public string WarehouseCode { get; set; }
        public string FromWarehouseCode { get; set; }
        public string ProjectCode { get; set; }
        public float Factor { get; set; }
        public float Factor2 { get; set; }
        public float Factor3 { get; set; }
        public float Factor4 { get; set; }
        public string DistributionRule { get; set; }
        public string DistributionRule2 { get; set; }
        public string DistributionRule3 { get; set; }
        public string DistributionRule4 { get; set; }
        public string DistributionRule5 { get; set; }
        public string UseBaseUnits { get; set; }
        public string MeasureUnit { get; set; }
        public float UnitsOfMeasurment { get; set; }
        public string BaseType { get; set; }
        public int BaseLine { get; set; }
        public int BaseEntry { get; set; }
        public float UnitPrice { get; set; }
        public int UoMEntry { get; set; }
        public string UoMCode { get; set; }
        public float InventoryQuantity { get; set; }
        public float RemainingOpenQuantity { get; set; }
        public float RemainingOpenInventoryQuantity { get; set; }
        public string LineStatus { get; set; }
        public float U_EXX_FE_ValICEVta { get; set; }
        public float U_EXX_FE_PorICEVta { get; set; }
        public string U_EXX_C_PATRIMONIO { get; set; }
        public string U_DetGlosaMrk { get; set; }
        public float U_CantCont { get; set; }
        public string[] SerialNumbers { get; set; }
        public string[] BatchNumbers { get; set; }
        public string[] StockTransferLinesBinAllocations { get; set; }
    }


    public class Articulo
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal Price { get; set; }
    
    }

}