using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegradorSAP.Data.Models
{


    public class FacturaListObj
    {
        public string odatametadata { get; set; }
        public List<FacturaResultModel> value { get; set; }
    }

    public class FacturaResultModel
    {
        public long DocEntry { get; set; }
        public long DocNum { get; set; }
        public string DocType { get; set; }
        public float DocTotal { get; set; }

        public long FolioNumber { get; set; }
        public string DocumentSubType { get; set; }
        public string U_EXX_FPAGO_VENTAS { get; set; }
        public string U_SER_PE { get; set; }
        public string U_SER_EST { get; set; }

        public string DocumentStatus { get; set; }

        public string U_CTK_DocEntryRel { get; set; }

        public string U_CTK_DocNumRel { get; set; }
        
        public string U_EXX_DOC_GEN { get; set; }

        public string CardCode { get; set; }
        public string CardName { get; set; }

    }

  

    public class FacturaVentaCreateModel
    {
        //   public long DocNum { get; set; }
        public string DocType { get; set; }
        //public string HandWritten { get; set; }
        //public string Printed { get; set; }
        public DateTime DocDate { get; set; }
        //  public long DocEntry { get; set; }
        public DateTime DocDueDate { get; set; }
        public string CardCode { get; set; }
        // public string CardName { get; set; }
        // public string Address { get; set; }
        public string NumAtCard { get; set; }
        public string U_CTK_DocNumRel { get; set; }
        public string U_CTK_DocEntryRel { get; set; }


        public decimal DocTotal { get; set; }
        //public object AttachmentEntry { get; set; }
        //public string DocCurrency { get; set; }
        //public decimal DocRate { get; set; }
        public string Reference1 { get; set; }
        public string U_NUM_AUTOR { get; set; }
        // public object Reference2 { get; set; }
        public string Comments { get; set; }
        public string JournalMemo { get; set; }
        //public int PaymentGroupCode { get; set; }
        public string DocTime { get; set; }
        public int SalesPersonCode { get; set; }
        public int TrnspCode { get; set; }
        //public string U_CTK_DocEntryRel { get; set; }
        //public string U_CTK_DocNumRel { get; set; }

        //public string Confirmed { get; set; }
        //public object ImportFileNum { get; set; }
        //public string SummeryType { get; set; }
        //public int ContactPersonCode { get; set; }
        //    public string ShowSCN { get; set; }
        public long Series { get; set; }
        public DateTime TaxDate { get; set; }
        //public string PartialSupply { get; set; }
        //public string DocObjectCode { get; set; }
        //public string ShipToCode { get; set; }
        //public object Indicator { get; set; }
        //public string FederalTaxID { get; set; }
        public float DiscountPercent { get; set; }
        //public object PaymentReference { get; set; }
        //public string CreationDate { get; set; }
        //public string UpdateDate { get; set; }
        //public int FinancialPeriod { get; set; }
        //public int TransNum { get; set; }
        public decimal VatSum { get; set; }
        public decimal VatSumSys { get; set; }
        public decimal VatSumFc { get; set; }
        //public string NetProcedure { get; set; }
        //public decimal DocTotalFc { get; set; }
        //public decimal DocTotalSys { get; set; }
        //public object Form1099 { get; set; }
        //public object Box1099 { get; set; }
        //public string RevisionPo { get; set; }
        //public object RequriedDate { get; set; }
        //public object CancelDate { get; set; }
        //public string BlockDunning { get; set; }
        //public string Submitted { get; set; }
        //public int Segment { get; set; }
        //public string PickStatus { get; set; }
        //public string Pick { get; set; }
        public string PaymentMethod { get; set; }
        //public string PaymentBlock { get; set; }
        //public object PaymentBlockEntry { get; set; }
        //public object CentralBankIndicator { get; set; }
        //public string MaximumCashDiscount { get; set; }
        //public string Reserve { get; set; }
        //public object Project { get; set; }
        //public object ExemptionValidityDateFrom { get; set; }
        //public object ExemptionValidityDateTo { get; set; }
        //public string WareHouseUpdateType { get; set; }
        //public string Rounding { get; set; }
        //public object ExternalCorrectedDocNum { get; set; }
        //public object InternalCorrectedDocNum { get; set; }
        //public object NextCorrectingDocument { get; set; }
        //public string DeferredTax { get; set; }
        //public object TaxExemptionLetterNum { get; set; }
        //public decimal WTApplied { get; set; }
        //public decimal WTAppliedFC { get; set; }
        //public string BillOfExchangeReserved { get; set; }
        //public object AgentCode { get; set; }
        //public decimal WTAppliedSC { get; set; }
        //public decimal TotalEqualizationTax { get; set; }
        //public decimal TotalEqualizationTaxFC { get; set; }
        //public decimal TotalEqualizationTaxSC { get; set; }
        //public int NumberOfInstallments { get; set; }
        //public string ApplyTaxOnFirstInstallment { get; set; }
        //public decimal WTNonSubjectAmount { get; set; }
        //public decimal WTNonSubjectAmountSC { get; set; }
        //public decimal WTNonSubjectAmountFC { get; set; }
        //public decimal WTExemptedAmount { get; set; }
        //public decimal WTExemptedAmountSC { get; set; }
        //public decimal WTExemptedAmountFC { get; set; }
        //public decimal BaseAmount { get; set; }
        //public decimal BaseAmountSC { get; set; }
        //public decimal BaseAmountFC { get; set; }
        //public decimal WTAmount { get; set; }
        //public decimal WTAmountSC { get; set; }
        //public decimal WTAmountFC { get; set; }
        //public object VatDate { get; set; }
        public String DocumentsOwner { get; set; }
        public string FolioPrefixString { get; set; }
        public long FolioNumber { get; set; }
        public string DocumentSubType { get; set; }
        //public object BPChannelCode { get; set; }
        //public object BPChannelContact { get; set; }
        //public string Address2 { get; set; }
        //public string DocumentStatus { get; set; }
        //public string PeriodIndicator { get; set; }
        //public string PayToCode { get; set; }
        //public object ManualNumber { get; set; }
        //public string UseShpdGoodsAct { get; set; }
        //public string IsPayToBank { get; set; }
        //public object PayToBankCountry { get; set; }
        //public object PayToBankCode { get; set; }
        //public object PayToBankAccountNo { get; set; }
        //public object PayToBankBranch { get; set; }
        //public object BPL_IDAssignedToInvoice { get; set; }
        //public decimal DownPayment { get; set; }
        //public string ReserveInvoice { get; set; }
        //public int LanguageCode { get; set; }
        //public object TrackingNumber { get; set; }
        public string PickRemark { get; set; }
        //public object ClosingDate { get; set; }
        //public object SequenceCode { get; set; }
        //public object SequenceSerial { get; set; }
        //public object SeriesString { get; set; }
        //public object SubSeriesString { get; set; }
        //public string SequenceModel { get; set; }
        //public string UseCorrectionVATGroup { get; set; }
        public float TotalDiscount { get; set; }
        //public decimal DownPaymentAmount { get; set; }
        //public decimal DownPaymentPercentage { get; set; }
        //public string DownPaymentType { get; set; }
        //public decimal DownPaymentAmountSC { get; set; }
        //public decimal DownPaymentAmountFC { get; set; }
        //public decimal VatPercent { get; set; }
        //public decimal ServiceGrossProfitPercent { get; set; }
        public string OpeningRemarks { get; set; }
        public string ClosingRemarks { get; set; }
        //public decimal RoundingDiffAmount { get; set; }
        //public decimal RoundingDiffAmountFC { get; set; }
        //public decimal RoundingDiffAmountSC { get; set; }
        //public string Cancelled { get; set; }
        //public object SignatureInputMessage { get; set; }
        //public object SignatureDigest { get; set; }
        //public object CertificationNumber { get; set; }
        //public object PrivateKeyVersion { get; set; }
        //public string ControlAccount { get; set; }
        //public string InsuranceOperation347 { get; set; }
        //public string ArchiveNonremovableSalesQuotation { get; set; }
        //public object GTSChecker { get; set; }
        //public object GTSPayee { get; set; }
        //public int ExtraMonth { get; set; }
        //public int ExtraDays { get; set; }
        //public int CashDiscountDateOffset { get; set; }
        //public string StartFrom { get; set; }
        //public string NTSApproved { get; set; }
        //public object ETaxWebSite { get; set; }
        //public object ETaxNumber { get; set; }
        //public object NTSApprovedNumber { get; set; }
        //public string EDocGenerationType { get; set; }
        //public object EDocSeries { get; set; }
        //public object EDocNum { get; set; }
        //public object EDocExportFormat { get; set; }
        //public string EDocStatus { get; set; }
        //public object EDocErrorCode { get; set; }
        //public object EDocErrorMessage { get; set; }
        //public string DownPaymentStatus { get; set; }
        //public object GroupSeries { get; set; }
        //public object GroupNumber { get; set; }
        //public string GroupHandWritten { get; set; }
        //public object ReopenOriginalDocument { get; set; }
        //public object ReopenManuallyClosedOrCanceledDocument { get; set; }
        //public string CreateOnlineQuotation { get; set; }
        //public object POSEquipmentNumber { get; set; }
        //public object POSManufacturerSerialNumber { get; set; }
        //public object POSCashierNumber { get; set; }
        //public string ApplyCurrentVATRatesForDownPaymentsToDraw { get; set; }
        //public string ClosingOption { get; set; }
        //public object SpecifiedClosingDate { get; set; }
        //public string OpenForLandedCosts { get; set; }
        //public string AuthorizationStatus { get; set; }
        public float TotalDiscountFC { get; set; }
        public float TotalDiscountSC { get; set; }
        //public string RelevantToGTS { get; set; }
        //public object BPLName { get; set; }
        //public object VATRegNum { get; set; }
        //public object AnnualInvoiceDeclarationReference { get; set; }
        //public object Supplier { get; set; }
        //public object Releaser { get; set; }
        //public object Receiver { get; set; }
        //public object BlanketAgreementNumber { get; set; }
        //public string IsAlteration { get; set; }
        //public string CancelStatus { get; set; }
        //public string AssetValueDate { get; set; }
        //public string DocumentDelivery { get; set; }
        public string AuthorizationCode { get; set; }
        //public object StartDeliveryDate { get; set; }
        //public object StartDeliveryTime { get; set; }
        //public object EndDeliveryDate { get; set; }
        //public object EndDeliveryTime { get; set; }
        //public object VehiclePlate { get; set; }
        //public object ATDocumentType { get; set; }
        //public object ElecCommStatus { get; set; }
        //public object ElecCommMessage { get; set; }
        //public string ReuseDocumentNum { get; set; }
        //public string ReuseNotaFiscalNum { get; set; }
        //public string PrintSEPADirect { get; set; }
        //public object FiscalDocNum { get; set; }
        //public object POSDailySummaryNo { get; set; }
        //public object POSReceiptNo { get; set; }
        //public object PointOfIssueCode { get; set; }
        //public object Letter { get; set; }
        //public object FolioNumberFrom { get; set; }
        //public object FolioNumberTo { get; set; }
        //public string InterimType { get; set; }
        //public int RelatedType { get; set; }
        //public object RelatedEntry { get; set; }
        public string U_SER_EST { get; set; }
        public string U_SER_PE { get; set; }
        public string U_COD_ST { get; set; }
        //public object U_SER_EST_FR { get; set; }
        //public object U_SER_PEFR { get; set; }
        //public object U_NUM_AUT_FR { get; set; }
        //public object U_NUM_FAC_REL { get; set; }
        //       public object U_COMP_RET { get; set; }
        //public object U_SERIE_RET { get; set; }
        //    public object U_NUM_AUT_RET { get; set; }
        //public object U_FEC_INI_TRAS { get; set; }
        //public object U_FEC_FIN_TRAS { get; set; }
        //        public object U_fecha_emi_doc_rel { get; set; }
        //      public object U_NUM_DECLAR_ADU { get; set; }
        //     public object U_MOT_TRASLADO { get; set; }
        //   public object U_PUNTO_PART { get; set; }
        //       public object U_CORRELATIVO { get; set; }
        //       public object U_VERIFICADOR { get; set; }
        //public object U_TRANSPORTE { get; set; }
        //public object U_TRANSPORTISTA { get; set; }
        public DateTime U_FECHA_EMBARQUE { get; set; }
        //public string U_Exx_IP_Pago { get; set; }
        //   public string U_Exx_IP_Pais { get; set; }
        //       public string U_Exx_IP_DobleTrib { get; set; }
        //   public string U_Exx_IP_SujetRet_NL { get; set; }
        //    public object U_Exx_FechaRet { get; set; }
        //public object U_Exx_Rembolso { get; set; }
        //    public object U_Exx_FPagxDoc { get; set; }
        //public object U_TIP_DOC_APLIC { get; set; }
        public string U_tipo_export { get; set; }
        public string U_tipo_comprob { get; set; }
        public string U_DOC_DECLARABLE { get; set; }
        // public object U_DISTRITO_ADU { get; set; }
        public int U_REFRENDO_ANIO { get; set; }
        //public object U_REFRENDO_REG { get; set; }
        public float U_VALOR_FOB { get; set; }
        //     public object U_NUM_DOC_TRANSP { get; set; }
        //public object U_NUM_FUE { get; set; }
        //public object U_MOT_NC { get; set; }
        //public object U_MOT_ND { get; set; }
        //public object U_EXX_FE_TIPCOM { get; set; }
        //public object U_EXX_FE_TIPAMB { get; set; }
        //      public object U_EXX_FE_CODNUM { get; set; }
        //public object U_EXX_FE_TIPEMI { get; set; }
        //    public object U_EXX_FE_DIFVER { get; set; }
        //     public object U_EXX_FE_FECAUT { get; set; }
        //     public string U_Exx_Din_Cons { get; set; }
        public string U_Exx_FE_Paisdestin { get; set; }
        //public string U_Exx_pagoRegFis { get; set; }
        //  public object U_Exx_fechaPagoDiv { get; set; }
        //    public decimal U_Exx_imRentaSoc { get; set; }
        //public object U_Exx_anioUtDiv { get; set; }
        //    public object U_Exx_numCajBan { get; set; }
        //public decimal U_Exx_precCajBan { get; set; }
        //   public object U_EXX_CAIM { get; set; }
        //    public object U_EXX_Hora_Lleg { get; set; }
        //     public object U_EXX_Hora_Sal { get; set; }
        //   public string U_Exx_doc_compensac { get; set; }
        public string U_EXX_FPAGO_VENTAS { get; set; }
        //public string U_EXX_FACT_NEG { get; set; }
        //   public object U_EXX_FPAGO_COMPRAS { get; set; }
        //   public object U_EXX_COMPENSADO { get; set; }
        //    public decimal U_EXX_VAL_COMP { get; set; }
        //    public object U_Exx_DenoFiscal { get; set; }
        public string U_Exx_ingFueGra_IR { get; set; }
        //    public object U_Exx_Paraiso_Fis { get; set; }
        public string U_Exx_TipIngExt { get; set; }
        public string U_Exx_TipRegFis { get; set; }
        //    public decimal U_Exx_ValoImpExt { get; set; }
        public string U_EXX_MAN_AG { get; set; }
        public string U_EXX_TIPO_TRANSACC { get; set; }
        public string U_EXX_DOC_GEN { get; set; }
        //   public object U_EXX_FE_DESERR { get; set; }
        //   public object U_EXX_FE_CODERR { get; set; }
        //public string U_EXX_FE_Estado { get; set; }
        //public object U_EXX_FE_ClaAcc { get; set; }
        //    public object U_EXX_Serie_Guia { get; set; }
        // public string U_EXX_FE_PdfCreado { get; set; }
        //  public object U_EXX_FE_PdfError { get; set; }
        public string U_EXX_FE_MailEnviado { get; set; }
        //    public object U_EXX_FE_MailError { get; set; }
        //   public object U_EXX_FE_Reemb { get; set; }

        //CAMPOS PARA FACTURAS DE EXPORTACION
        public object U_Exx_FE_ComExt { get; set; } //EXPORTADOR
        public object U_Exx_FE_IncoTermFac { get; set; } // GUAYAQUIL
        public object U_Exx_FE_LugIncoTerm { get; set; } //CIF
        public object U_Exx_FE_PaisOrigen { get; set; } //593
        public object U_Exx_FE_PuertoEmb { get; set; } //GUAYAQUIL
        public object U_Exx_FE_PuertoDest { get; set; } //PAIS DEL CLIENTE
        public object U_Exx_FE_Paisadquis { get; set; } //593
        public object U_Exx_FE_Incotermto { get; set; } //FOB
        //   public object U_Exx_FE_Fleteinter { get; set; }
        //public object U_Exx_FE_Segurointe { get; set; }
        //       public object U_EXX_TDOCSI { get; set; }
        //       public object U_EXX_LINEASI { get; set; }
        //    public object U_EXX_NAVESI { get; set; }
        //      public object U_EXX_VIAJESI { get; set; }
        //    public object U_EXX_TIP_OPSI { get; set; }
        //public object U_CLAVE_ACCESO { get; set; }
        //public string U_ESTADO_AUTORIZACIO { get; set; }
        //public object U_NUM_AUTO_FAC { get; set; }
        //    public object U_FECHA_AUT_FACT { get; set; }
        //   public object U_OBSERVACION_FACT { get; set; }
        //public string U_SSCREADAR { get; set; }
        //    public object U_SSIDDOCUMENTO { get; set; }
        public string U_HRH_Serie { get; set; }
        public string U_HRH_Modo_Fact { get; set; }
        public string U_HRH_Lote { get; set; }
        public string U_LocalCtaContab { get; set; }
        ////    public object U_LQ_CLAVE { get; set; }
        //public string U_LQ_ESTADO { get; set; }
        //public object U_LQ_NUM_AUTO { get; set; }
        //public object U_LQ_FECHA_AUT { get; set; }
        //public object U_LQ_OBSERVACION { get; set; }
        // public object U_DOC_REF { get; set; }
        public object U_CTK_Lote { get; set; }
        public object U_CTK_Generado { get; set; }
        //public object U_CTK_Observacion { get; set; }
        public string U_CTK_FechaHoraGeneracion { get; set; }
        //   public object[] Document_ApprovalRequests { get; set; }
        public List<DocumentlineVentaModel> DocumentLines { get; set; }
        //public object[] DocumentAdditionalExpenses { get; set; }
        //public object[] WithholdingTaxDataWTXCollection { get; set; }
        //public object[] WithholdingTaxDataCollection { get; set; }
        //public object[] DocumentPackages { get; set; }
        //public object[] DocumentSpecialLines { get; set; }
        //public Documentinstallment[] DocumentInstallments { get; set; }
        //public object[] DownPaymentsToDraw { get; set; }
        //public Taxextension TaxExtension { get; set; }
        //public Addressextension AddressExtension { get; set; }


    }


    public class FacturaVentaRespCreateModel
    {
        public long DocNum { get; set; }
        public string DocType { get; set; }
        //public string HandWritten { get; set; }
        //public string Printed { get; set; }
        public DateTime DocDate { get; set; }
        public long DocEntry { get; set; }
        public DateTime DocDueDate { get; set; }
        public string CardCode { get; set; }
        // public string CardName { get; set; }
        // public string Address { get; set; }
        //   public string NumAtCard { get; set; }
        public decimal DocTotal { get; set; }
        //public object AttachmentEntry { get; set; }
        //public string DocCurrency { get; set; }
        //public decimal DocRate { get; set; }
        public string Reference1 { get; set; }
        public string U_NUM_AUTOR { get; set; }
        // public object Reference2 { get; set; }
        public string Comments { get; set; }
        public string JournalMemo { get; set; }
        //public int PaymentGroupCode { get; set; }
        public string DocTime { get; set; }
        public int SalesPersonCode { get; set; }
        public int TrnspCode { get; set; }
        //public string Confirmed { get; set; }
        //public object ImportFileNum { get; set; }
        //public string SummeryType { get; set; }
        //public int ContactPersonCode { get; set; }
        //    public string ShowSCN { get; set; }
        public long Series { get; set; }
        public DateTime TaxDate { get; set; }
        //public string PartialSupply { get; set; }
        //public string DocObjectCode { get; set; }
        //public string ShipToCode { get; set; }
        //public object Indicator { get; set; }
        //public string FederalTaxID { get; set; }
        public decimal DiscountPercent { get; set; }
        //public object PaymentReference { get; set; }
        //public string CreationDate { get; set; }
        //public string UpdateDate { get; set; }
        //public int FinancialPeriod { get; set; }
        //public int TransNum { get; set; }
        //public decimal VatSum { get; set; }
        //public decimal VatSumSys { get; set; }
        //public decimal VatSumFc { get; set; }
        //public string NetProcedure { get; set; }
        //public decimal DocTotalFc { get; set; }
        //public decimal DocTotalSys { get; set; }
        //public object Form1099 { get; set; }
        //public object Box1099 { get; set; }
        //public string RevisionPo { get; set; }
        //public object RequriedDate { get; set; }
        //public object CancelDate { get; set; }
        //public string BlockDunning { get; set; }
        //public string Submitted { get; set; }
        //public int Segment { get; set; }
        //public string PickStatus { get; set; }
        //public string Pick { get; set; }
        //public string PaymentMethod { get; set; }
        //public string PaymentBlock { get; set; }
        //public object PaymentBlockEntry { get; set; }
        //public object CentralBankIndicator { get; set; }
        //public string MaximumCashDiscount { get; set; }
        //public string Reserve { get; set; }
        //public object Project { get; set; }
        //public object ExemptionValidityDateFrom { get; set; }
        //public object ExemptionValidityDateTo { get; set; }
        //public string WareHouseUpdateType { get; set; }
        //public string Rounding { get; set; }
        //public object ExternalCorrectedDocNum { get; set; }
        //public object InternalCorrectedDocNum { get; set; }
        //public object NextCorrectingDocument { get; set; }
        //public string DeferredTax { get; set; }
        //public object TaxExemptionLetterNum { get; set; }
        //public decimal WTApplied { get; set; }
        //public decimal WTAppliedFC { get; set; }
        //public string BillOfExchangeReserved { get; set; }
        //public object AgentCode { get; set; }
        //public decimal WTAppliedSC { get; set; }
        //public decimal TotalEqualizationTax { get; set; }
        //public decimal TotalEqualizationTaxFC { get; set; }
        //public decimal TotalEqualizationTaxSC { get; set; }
        //public int NumberOfInstallments { get; set; }
        //public string ApplyTaxOnFirstInstallment { get; set; }
        //public decimal WTNonSubjectAmount { get; set; }
        //public decimal WTNonSubjectAmountSC { get; set; }
        //public decimal WTNonSubjectAmountFC { get; set; }
        //public decimal WTExemptedAmount { get; set; }
        //public decimal WTExemptedAmountSC { get; set; }
        //public decimal WTExemptedAmountFC { get; set; }
        //public decimal BaseAmount { get; set; }
        //public decimal BaseAmountSC { get; set; }
        //public decimal BaseAmountFC { get; set; }
        //public decimal WTAmount { get; set; }
        //public decimal WTAmountSC { get; set; }
        //public decimal WTAmountFC { get; set; }
        //public object VatDate { get; set; }
        //public int DocumentsOwner { get; set; }
        public string FolioPrefixString { get; set; }
        public long FolioNumber { get; set; }
        public string DocumentSubType { get; set; }
        //public object BPChannelCode { get; set; }
        //public object BPChannelContact { get; set; }
        //public string Address2 { get; set; }
        //public string DocumentStatus { get; set; }
        //public string PeriodIndicator { get; set; }
        //public string PayToCode { get; set; }
        //public object ManualNumber { get; set; }
        //public string UseShpdGoodsAct { get; set; }
        //public string IsPayToBank { get; set; }
        //public object PayToBankCountry { get; set; }
        //public object PayToBankCode { get; set; }
        //public object PayToBankAccountNo { get; set; }
        //public object PayToBankBranch { get; set; }
        //public object BPL_IDAssignedToInvoice { get; set; }
        //public decimal DownPayment { get; set; }
        //public string ReserveInvoice { get; set; }
        //public int LanguageCode { get; set; }
        //public object TrackingNumber { get; set; }
        //public string PickRemark { get; set; }
        //public object ClosingDate { get; set; }
        //public object SequenceCode { get; set; }
        //public object SequenceSerial { get; set; }
        //public object SeriesString { get; set; }
        //public object SubSeriesString { get; set; }
        //public string SequenceModel { get; set; }
        //public string UseCorrectionVATGroup { get; set; }
        //public decimal TotalDiscount { get; set; }
        //public decimal DownPaymentAmount { get; set; }
        //public decimal DownPaymentPercentage { get; set; }
        //public string DownPaymentType { get; set; }
        //public decimal DownPaymentAmountSC { get; set; }
        //public decimal DownPaymentAmountFC { get; set; }
        //public decimal VatPercent { get; set; }
        //public decimal ServiceGrossProfitPercent { get; set; }
        //public string OpeningRemarks { get; set; }
        //public string ClosingRemarks { get; set; }
        //public decimal RoundingDiffAmount { get; set; }
        //public decimal RoundingDiffAmountFC { get; set; }
        //public decimal RoundingDiffAmountSC { get; set; }
        //public string Cancelled { get; set; }
        //public object SignatureInputMessage { get; set; }
        //public object SignatureDigest { get; set; }
        //public object CertificationNumber { get; set; }
        //public object PrivateKeyVersion { get; set; }
        //public string ControlAccount { get; set; }
        //public string InsuranceOperation347 { get; set; }
        //public string ArchiveNonremovableSalesQuotation { get; set; }
        //public object GTSChecker { get; set; }
        //public object GTSPayee { get; set; }
        //public int ExtraMonth { get; set; }
        //public int ExtraDays { get; set; }
        //public int CashDiscountDateOffset { get; set; }
        //public string StartFrom { get; set; }
        //public string NTSApproved { get; set; }
        //public object ETaxWebSite { get; set; }
        //public object ETaxNumber { get; set; }
        //public object NTSApprovedNumber { get; set; }
        //public string EDocGenerationType { get; set; }
        //public object EDocSeries { get; set; }
        //public object EDocNum { get; set; }
        //public object EDocExportFormat { get; set; }
        //public string EDocStatus { get; set; }
        //public object EDocErrorCode { get; set; }
        //public object EDocErrorMessage { get; set; }
        //public string DownPaymentStatus { get; set; }
        //public object GroupSeries { get; set; }
        //public object GroupNumber { get; set; }
        //public string GroupHandWritten { get; set; }
        //public object ReopenOriginalDocument { get; set; }
        //public object ReopenManuallyClosedOrCanceledDocument { get; set; }
        //public string CreateOnlineQuotation { get; set; }
        //public object POSEquipmentNumber { get; set; }
        //public object POSManufacturerSerialNumber { get; set; }
        //public object POSCashierNumber { get; set; }
        //public string ApplyCurrentVATRatesForDownPaymentsToDraw { get; set; }
        //public string ClosingOption { get; set; }
        //public object SpecifiedClosingDate { get; set; }
        //public string OpenForLandedCosts { get; set; }
        //public string AuthorizationStatus { get; set; }
        //public decimal TotalDiscountFC { get; set; }
        //public decimal TotalDiscountSC { get; set; }
        //public string RelevantToGTS { get; set; }
        //public object BPLName { get; set; }
        //public object VATRegNum { get; set; }
        //public object AnnualInvoiceDeclarationReference { get; set; }
        //public object Supplier { get; set; }
        //public object Releaser { get; set; }
        //public object Receiver { get; set; }
        //public object BlanketAgreementNumber { get; set; }
        //public string IsAlteration { get; set; }
        //public string CancelStatus { get; set; }
        //public string AssetValueDate { get; set; }
        //public string DocumentDelivery { get; set; }
        public string AuthorizationCode { get; set; }
        //public object StartDeliveryDate { get; set; }
        //public object StartDeliveryTime { get; set; }
        //public object EndDeliveryDate { get; set; }
        //public object EndDeliveryTime { get; set; }
        //public object VehiclePlate { get; set; }
        //public object ATDocumentType { get; set; }
        //public object ElecCommStatus { get; set; }
        //public object ElecCommMessage { get; set; }
        //public string ReuseDocumentNum { get; set; }
        //public string ReuseNotaFiscalNum { get; set; }
        //public string PrintSEPADirect { get; set; }
        //public object FiscalDocNum { get; set; }
        //public object POSDailySummaryNo { get; set; }
        //public object POSReceiptNo { get; set; }
        //public object PointOfIssueCode { get; set; }
        //public object Letter { get; set; }
        //public object FolioNumberFrom { get; set; }
        //public object FolioNumberTo { get; set; }
        //public string InterimType { get; set; }
        //public int RelatedType { get; set; }
        //public object RelatedEntry { get; set; }
        public string U_SER_EST { get; set; }
        public string U_SER_PE { get; set; }
        //   public string U_COD_ST { get; set; }
        //public object U_SER_EST_FR { get; set; }
        //public object U_SER_PEFR { get; set; }
        //public object U_NUM_AUT_FR { get; set; }
        //public object U_NUM_FAC_REL { get; set; }
        //       public object U_COMP_RET { get; set; }
        //public object U_SERIE_RET { get; set; }
        //    public object U_NUM_AUT_RET { get; set; }
        //public object U_FEC_INI_TRAS { get; set; }
        //public object U_FEC_FIN_TRAS { get; set; }
        //        public object U_fecha_emi_doc_rel { get; set; }
        //      public object U_NUM_DECLAR_ADU { get; set; }
        //     public object U_MOT_TRASLADO { get; set; }
        //   public object U_PUNTO_PART { get; set; }
        //       public object U_CORRELATIVO { get; set; }
        //       public object U_VERIFICADOR { get; set; }
        //public object U_TRANSPORTE { get; set; }
        //public object U_TRANSPORTISTA { get; set; }
        //     public object U_FECHA_EMBARQUE { get; set; }
        //public string U_Exx_IP_Pago { get; set; }
        //   public string U_Exx_IP_Pais { get; set; }
        //       public string U_Exx_IP_DobleTrib { get; set; }
        //   public string U_Exx_IP_SujetRet_NL { get; set; }
        //    public object U_Exx_FechaRet { get; set; }
        //public object U_Exx_Rembolso { get; set; }
        //    public object U_Exx_FPagxDoc { get; set; }
        //public object U_TIP_DOC_APLIC { get; set; }
        public string U_tipo_export { get; set; }
        public string U_tipo_comprob { get; set; }
        //    public string U_DOC_DECLARABLE { get; set; }
        // public object U_DISTRITO_ADU { get; set; }
        //public int U_REFRENDO_ANIO { get; set; }
        //public object U_REFRENDO_REG { get; set; }
        //       public decimal U_VALOR_FOB { get; set; }
        //     public object U_NUM_DOC_TRANSP { get; set; }
        //public object U_NUM_FUE { get; set; }
        //public object U_MOT_NC { get; set; }
        //public object U_MOT_ND { get; set; }
        //public object U_EXX_FE_TIPCOM { get; set; }
        //public object U_EXX_FE_TIPAMB { get; set; }
        //      public object U_EXX_FE_CODNUM { get; set; }
        //public object U_EXX_FE_TIPEMI { get; set; }
        //    public object U_EXX_FE_DIFVER { get; set; }
        //     public object U_EXX_FE_FECAUT { get; set; }
        //     public string U_Exx_Din_Cons { get; set; }
        //     public string U_Exx_FE_Paisdestin { get; set; }
        //public string U_Exx_pagoRegFis { get; set; }
        //  public object U_Exx_fechaPagoDiv { get; set; }
        //    public decimal U_Exx_imRentaSoc { get; set; }
        //public object U_Exx_anioUtDiv { get; set; }
        //    public object U_Exx_numCajBan { get; set; }
        //public decimal U_Exx_precCajBan { get; set; }
        //   public object U_EXX_CAIM { get; set; }
        //    public object U_EXX_Hora_Lleg { get; set; }
        //     public object U_EXX_Hora_Sal { get; set; }
        //   public string U_Exx_doc_compensac { get; set; }
        public string U_EXX_FPAGO_VENTAS { get; set; }
        //public string U_EXX_FACT_NEG { get; set; }
        //   public object U_EXX_FPAGO_COMPRAS { get; set; }
        //   public object U_EXX_COMPENSADO { get; set; }
        //    public decimal U_EXX_VAL_COMP { get; set; }
        //    public object U_Exx_DenoFiscal { get; set; }
        //     public object U_Exx_ingFueGra_IR { get; set; }
        //    public object U_Exx_Paraiso_Fis { get; set; }
        //  public object U_Exx_TipIngExt { get; set; }
        //public object U_Exx_TipRegFis { get; set; }
        //    public decimal U_Exx_ValoImpExt { get; set; }
        //     public string U_EXX_MAN_AG { get; set; }
        //   public string U_EXX_TIPO_TRANSACC { get; set; }
        //public string U_EXX_DOC_GEN { get; set; }
        //   public object U_EXX_FE_DESERR { get; set; }
        //   public object U_EXX_FE_CODERR { get; set; }
        //public string U_EXX_FE_Estado { get; set; }
        //public object U_EXX_FE_ClaAcc { get; set; }
        //    public object U_EXX_Serie_Guia { get; set; }
        // public string U_EXX_FE_PdfCreado { get; set; }
        //  public object U_EXX_FE_PdfError { get; set; }
        public string U_EXX_FE_MailEnviado { get; set; }
        //    public object U_EXX_FE_MailError { get; set; }
        //   public object U_EXX_FE_Reemb { get; set; }
        //    public object U_Exx_FE_ComExt { get; set; }
        //    public object U_Exx_FE_IncoTermFac { get; set; }
        //    public object U_Exx_FE_LugIncoTerm { get; set; }
        //    public object U_Exx_FE_PaisOrigen { get; set; }
        //public object U_Exx_FE_PuertoEmb { get; set; }
        //public object U_Exx_FE_PuertoDest { get; set; }
        //    public object U_Exx_FE_Paisadquis { get; set; }
        //     public object U_Exx_FE_Incotermto { get; set; }
        //   public object U_Exx_FE_Fleteinter { get; set; }
        //public object U_Exx_FE_Segurointe { get; set; }
        //       public object U_EXX_TDOCSI { get; set; }
        //       public object U_EXX_LINEASI { get; set; }
        //    public object U_EXX_NAVESI { get; set; }
        //      public object U_EXX_VIAJESI { get; set; }
        //    public object U_EXX_TIP_OPSI { get; set; }
        //public object U_CLAVE_ACCESO { get; set; }
        //public string U_ESTADO_AUTORIZACIO { get; set; }
        //public object U_NUM_AUTO_FAC { get; set; }
        //    public object U_FECHA_AUT_FACT { get; set; }
        //   public object U_OBSERVACION_FACT { get; set; }
        //public string U_SSCREADAR { get; set; }
        //    public object U_SSIDDOCUMENTO { get; set; }
        public string U_HRH_Serie { get; set; }
        //    public string U_HRH_Modo_Fact { get; set; }
        //    public string U_HRH_Lote { get; set; }
        //    public string U_LocalCtaContab { get; set; }
        ////    public object U_LQ_CLAVE { get; set; }
        //public string U_LQ_ESTADO { get; set; }
        //public object U_LQ_NUM_AUTO { get; set; }
        //public object U_LQ_FECHA_AUT { get; set; }
        //public object U_LQ_OBSERVACION { get; set; }
        // public object U_DOC_REF { get; set; }
        //public object U_CTK_Lote { get; set; }
        //public object U_CTK_Generado { get; set; }
        //public object U_CTK_Observacion { get; set; }
        //public object U_CtkFechaHoraGeneracion { get; set; }
        //   public object[] Document_ApprovalRequests { get; set; }
        public List<DocumentlineRespVentaModel> DocumentLines { get; set; }
        //public object[] DocumentAdditionalExpenses { get; set; }
        //public object[] WithholdingTaxDataWTXCollection { get; set; }
        //public object[] WithholdingTaxDataCollection { get; set; }
        //public object[] DocumentPackages { get; set; }
        //public object[] DocumentSpecialLines { get; set; }
        //public Documentinstallment[] DocumentInstallments { get; set; }
        //public object[] DownPaymentsToDraw { get; set; }
        //public Taxextension TaxExtension { get; set; }
        //public Addressextension AddressExtension { get; set; }
    }

    public class FacturaVentaModel
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string DocType { get; set; }
        //public string HandWritten { get; set; }
        //public string Printed { get; set; }
        public string DocDate { get; set; }
        public string DocDueDate { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string Address { get; set; }
        public string NumAtCard { get; set; }
        public decimal DocTotal { get; set; }
        //public object AttachmentEntry { get; set; }
        //public string DocCurrency { get; set; }
        //public decimal DocRate { get; set; }
        //public string Reference1 { get; set; }
        //public object Reference2 { get; set; }
        //public string Comments { get; set; }
        //public string JournalMemo { get; set; }
        //public int PaymentGroupCode { get; set; }
        //public string DocTime { get; set; }
        //public int SalesPersonCode { get; set; }
        //public int TransportationCode { get; set; }
        //public string Confirmed { get; set; }
        //public object ImportFileNum { get; set; }
        //public string SummeryType { get; set; }
        //public int ContactPersonCode { get; set; }
        //public string ShowSCN { get; set; }
        public int Series { get; set; }
        public string TaxDate { get; set; }
        //public string PartialSupply { get; set; }
        //public string DocObjectCode { get; set; }
        //public string ShipToCode { get; set; }
        //public object Indicator { get; set; }
        //public string FederalTaxID { get; set; }
        //public decimal DiscountPercent { get; set; }
        //public object PaymentReference { get; set; }
        //public string CreationDate { get; set; }
        //public string UpdateDate { get; set; }
        //public int FinancialPeriod { get; set; }
        //public int TransNum { get; set; }
        //public decimal VatSum { get; set; }
        //public decimal VatSumSys { get; set; }
        //public decimal VatSumFc { get; set; }
        //public string NetProcedure { get; set; }
        //public decimal DocTotalFc { get; set; }
        //public decimal DocTotalSys { get; set; }
        //public object Form1099 { get; set; }
        //public object Box1099 { get; set; }
        //public string RevisionPo { get; set; }
        //public object RequriedDate { get; set; }
        //public object CancelDate { get; set; }
        //public string BlockDunning { get; set; }
        //public string Submitted { get; set; }
        //public int Segment { get; set; }
        //public string PickStatus { get; set; }
        //public string Pick { get; set; }
        //public string PaymentMethod { get; set; }
        //public string PaymentBlock { get; set; }
        //public object PaymentBlockEntry { get; set; }
        //public object CentralBankIndicator { get; set; }
        //public string MaximumCashDiscount { get; set; }
        //public string Reserve { get; set; }
        //public object Project { get; set; }
        //public object ExemptionValidityDateFrom { get; set; }
        //public object ExemptionValidityDateTo { get; set; }
        //public string WareHouseUpdateType { get; set; }
        //public string Rounding { get; set; }
        //public object ExternalCorrectedDocNum { get; set; }
        //public object InternalCorrectedDocNum { get; set; }
        //public object NextCorrectingDocument { get; set; }
        //public string DeferredTax { get; set; }
        //public object TaxExemptionLetterNum { get; set; }
        //public decimal WTApplied { get; set; }
        //public decimal WTAppliedFC { get; set; }
        //public string BillOfExchangeReserved { get; set; }
        //public object AgentCode { get; set; }
        //public decimal WTAppliedSC { get; set; }
        //public decimal TotalEqualizationTax { get; set; }
        //public decimal TotalEqualizationTaxFC { get; set; }
        //public decimal TotalEqualizationTaxSC { get; set; }
        //public int NumberOfInstallments { get; set; }
        //public string ApplyTaxOnFirstInstallment { get; set; }
        //public decimal WTNonSubjectAmount { get; set; }
        //public decimal WTNonSubjectAmountSC { get; set; }
        //public decimal WTNonSubjectAmountFC { get; set; }
        //public decimal WTExemptedAmount { get; set; }
        //public decimal WTExemptedAmountSC { get; set; }
        //public decimal WTExemptedAmountFC { get; set; }
        //public decimal BaseAmount { get; set; }
        //public decimal BaseAmountSC { get; set; }
        //public decimal BaseAmountFC { get; set; }
        //public decimal WTAmount { get; set; }
        //public decimal WTAmountSC { get; set; }
        //public decimal WTAmountFC { get; set; }
        //public object VatDate { get; set; }
        //public int DocumentsOwner { get; set; }
        //public string FolioPrefixString { get; set; }
        //public int FolioNumber { get; set; }
        //public string DocumentSubType { get; set; }
        //public object BPChannelCode { get; set; }
        //public object BPChannelContact { get; set; }
        //public string Address2 { get; set; }
        //public string DocumentStatus { get; set; }
        //public string PeriodIndicator { get; set; }
        //public string PayToCode { get; set; }
        //public object ManualNumber { get; set; }
        //public string UseShpdGoodsAct { get; set; }
        //public string IsPayToBank { get; set; }
        //public object PayToBankCountry { get; set; }
        //public object PayToBankCode { get; set; }
        //public object PayToBankAccountNo { get; set; }
        //public object PayToBankBranch { get; set; }
        //public object BPL_IDAssignedToInvoice { get; set; }
        //public decimal DownPayment { get; set; }
        //public string ReserveInvoice { get; set; }
        //public int LanguageCode { get; set; }
        //public object TrackingNumber { get; set; }
        //public string PickRemark { get; set; }
        //public object ClosingDate { get; set; }
        //public object SequenceCode { get; set; }
        //public object SequenceSerial { get; set; }
        //public object SeriesString { get; set; }
        //public object SubSeriesString { get; set; }
        //public string SequenceModel { get; set; }
        //public string UseCorrectionVATGroup { get; set; }
        //public decimal TotalDiscount { get; set; }
        //public decimal DownPaymentAmount { get; set; }
        //public decimal DownPaymentPercentage { get; set; }
        //public string DownPaymentType { get; set; }
        //public decimal DownPaymentAmountSC { get; set; }
        //public decimal DownPaymentAmountFC { get; set; }
        //public decimal VatPercent { get; set; }
        //public decimal ServiceGrossProfitPercent { get; set; }
        //public string OpeningRemarks { get; set; }
        //public string ClosingRemarks { get; set; }
        //public decimal RoundingDiffAmount { get; set; }
        //public decimal RoundingDiffAmountFC { get; set; }
        //public decimal RoundingDiffAmountSC { get; set; }
        //public string Cancelled { get; set; }
        //public object SignatureInputMessage { get; set; }
        //public object SignatureDigest { get; set; }
        //public object CertificationNumber { get; set; }
        //public object PrivateKeyVersion { get; set; }
        //public string ControlAccount { get; set; }
        //public string InsuranceOperation347 { get; set; }
        //public string ArchiveNonremovableSalesQuotation { get; set; }
        //public object GTSChecker { get; set; }
        //public object GTSPayee { get; set; }
        //public int ExtraMonth { get; set; }
        //public int ExtraDays { get; set; }
        //public int CashDiscountDateOffset { get; set; }
        //public string StartFrom { get; set; }
        //public string NTSApproved { get; set; }
        //public object ETaxWebSite { get; set; }
        //public object ETaxNumber { get; set; }
        //public object NTSApprovedNumber { get; set; }
        //public string EDocGenerationType { get; set; }
        //public object EDocSeries { get; set; }
        //public object EDocNum { get; set; }
        //public object EDocExportFormat { get; set; }
        //public string EDocStatus { get; set; }
        //public object EDocErrorCode { get; set; }
        //public object EDocErrorMessage { get; set; }
        //public string DownPaymentStatus { get; set; }
        //public object GroupSeries { get; set; }
        //public object GroupNumber { get; set; }
        //public string GroupHandWritten { get; set; }
        //public object ReopenOriginalDocument { get; set; }
        //public object ReopenManuallyClosedOrCanceledDocument { get; set; }
        //public string CreateOnlineQuotation { get; set; }
        //public object POSEquipmentNumber { get; set; }
        //public object POSManufacturerSerialNumber { get; set; }
        //public object POSCashierNumber { get; set; }
        //public string ApplyCurrentVATRatesForDownPaymentsToDraw { get; set; }
        //public string ClosingOption { get; set; }
        //public object SpecifiedClosingDate { get; set; }
        //public string OpenForLandedCosts { get; set; }
        //public string AuthorizationStatus { get; set; }
        //public decimal TotalDiscountFC { get; set; }
        //public decimal TotalDiscountSC { get; set; }
        //public string RelevantToGTS { get; set; }
        //public object BPLName { get; set; }
        //public object VATRegNum { get; set; }
        //public object AnnualInvoiceDeclarationReference { get; set; }
        //public object Supplier { get; set; }
        //public object Releaser { get; set; }
        //public object Receiver { get; set; }
        //public object BlanketAgreementNumber { get; set; }
        //public string IsAlteration { get; set; }
        //public string CancelStatus { get; set; }
        //public string AssetValueDate { get; set; }
        //public string DocumentDelivery { get; set; }
        public object AuthorizationCode { get; set; }
        //public object StartDeliveryDate { get; set; }
        //public object StartDeliveryTime { get; set; }
        //public object EndDeliveryDate { get; set; }
        //public object EndDeliveryTime { get; set; }
        //public object VehiclePlate { get; set; }
        //public object ATDocumentType { get; set; }
        //public object ElecCommStatus { get; set; }
        //public object ElecCommMessage { get; set; }
        //public string ReuseDocumentNum { get; set; }
        //public string ReuseNotaFiscalNum { get; set; }
        //public string PrintSEPADirect { get; set; }
        //public object FiscalDocNum { get; set; }
        //public object POSDailySummaryNo { get; set; }
        //public object POSReceiptNo { get; set; }
        //public object PointOfIssueCode { get; set; }
        //public object Letter { get; set; }
        //public object FolioNumberFrom { get; set; }
        //public object FolioNumberTo { get; set; }
        //public string InterimType { get; set; }
        //public int RelatedType { get; set; }
        //public object RelatedEntry { get; set; }
        public string U_SER_EST { get; set; }
        public string U_SER_PE { get; set; }
        public string U_NUM_AUTOR { get; set; }
        //  public string U_COD_ST { get; set; }
        //public object U_SER_EST_FR { get; set; }
        //public object U_SER_PEFR { get; set; }
        //        public object U_NUM_AUT_FR { get; set; }
        //       public object U_NUM_FAC_REL { get; set; }
        //   public object U_COMP_RET { get; set; }
        //public object U_SERIE_RET { get; set; }
        //        public object U_NUM_AUT_RET { get; set; }
        public object U_FEC_INI_TRAS { get; set; }
        public object U_FEC_FIN_TRAS { get; set; }
        //          public object U_fecha_emi_doc_rel { get; set; }
        //     public object U_NUM_DECLAR_ADU { get; set; }
        //public object U_MOT_TRASLADO { get; set; }
        //      public object U_PUNTO_PART { get; set; }
        //    public object U_NUM_GUIA { get; set; }
        //   public object U_CORRELATIVO { get; set; }
        //        public object U_VERIFICADOR { get; set; }
        //public object U_TRANSPORTE { get; set; }
        //public object U_TRANSPORTISTA { get; set; }
        public object U_FECHA_EMBARQUE { get; set; }
        //       public string U_Exx_IP_Pago { get; set; }
        //public string U_Exx_IP_Pais { get; set; }
        //        public string U_Exx_IP_DobleTrib { get; set; }
        //       public string U_Exx_IP_SujetRet_NL { get; set; }
        //       public object U_Exx_FechaRet { get; set; }
        //    public object U_Exx_Rembolso { get; set; }
        //     public object U_Exx_FPagxDoc { get; set; }
        //public object U_TIP_DOC_APLIC { get; set; }
        public object U_tipo_export { get; set; }
        public string U_tipo_comprob { get; set; }
        //    public string U_DOC_DECLARABLE { get; set; }
        //     public object U_DISTRITO_ADU { get; set; }
        //public int U_REFRENDO_ANIO { get; set; }
        //public object U_REFRENDO_REG { get; set; }
        //        public decimal U_VALOR_FOB { get; set; }
        //public object U_NUM_DOC_TRANSP { get; set; }
        //      public object U_NUM_FUE { get; set; }
        //public object U_MOT_NC { get; set; }
        //public object U_MOT_ND { get; set; }
        //      public object U_EXX_FE_TIPCOM { get; set; }
        //    public object U_EXX_FE_TIPAMB { get; set; }
        //     public object U_EXX_FE_CODNUM { get; set; }
        public object U_EXX_FE_TIPEMI { get; set; }
        //    public object U_EXX_FE_DIFVER { get; set; }
        //public object U_EXX_FE_FECAUT { get; set; }
        //  public string U_Exx_Din_Cons { get; set; }
        //      public string U_Exx_FE_Paisdestin { get; set; }
        public string U_Exx_pagoRegFis { get; set; }
        //       public object U_Exx_fechaPagoDiv { get; set; }
        //     public decimal U_Exx_imRentaSoc { get; set; }
        //    public object U_Exx_anioUtDiv { get; set; }
        //      public object U_Exx_numCajBan { get; set; }
        public decimal U_Exx_precCajBan { get; set; }
        //      public object U_EXX_CAIM { get; set; }
        //      public object U_EXX_Hora_Lleg { get; set; }
        //       public object U_EXX_Hora_Sal { get; set; }
        //    public string U_Exx_doc_compensac { get; set; }
        //     public string U_EXX_FPAGO_VENTAS { get; set; }
        //public string U_EXX_FACT_NEG { get; set; }
        public object U_EXX_FPAGO_COMPRAS { get; set; }
        //  public object U_EXX_COMPENSADO { get; set; }
        //      public decimal U_EXX_VAL_COMP { get; set; }
        //    public object U_Exx_DenoFiscal { get; set; }
        //    public object U_Exx_ingFueGra_IR { get; set; }
        //       public object U_Exx_Paraiso_Fis { get; set; }
        //      public object U_Exx_TipIngExt { get; set; }
        //      public object U_Exx_TipRegFis { get; set; }
        //       public decimal U_Exx_ValoImpExt { get; set; }
        //       public string U_EXX_MAN_AG { get; set; }
        //  public string U_EXX_TIPO_TRANSACC { get; set; }
        //public string U_EXX_DOC_GEN { get; set; }
        //public object U_EXX_FE_DESERR { get; set; }
        //public object U_EXX_FE_CODERR { get; set; }
        //  public string U_EXX_FE_Estado { get; set; }
        //   public object U_EXX_FE_ClaAcc { get; set; }
        //     public object U_EXX_Serie_Guia { get; set; }
        //  public string U_EXX_FE_PdfCreado { get; set; }
        //      public object U_EXX_FE_PdfError { get; set; }
        //public string U_EXX_FE_MailEnviado { get; set; }
        //public object U_EXX_FE_MailError { get; set; }
        //  public object U_EXX_FE_Reemb { get; set; }
        //   public object U_Exx_FE_ComExt { get; set; }
        //public object U_Exx_FE_IncoTermFac { get; set; }
        //public object U_Exx_FE_LugIncoTerm { get; set; }
        //      public object U_Exx_FE_PaisOrigen { get; set; }
        //       public object U_Exx_FE_PuertoEmb { get; set; }
        //      public object U_Exx_FE_PuertoDest { get; set; }
        //       public object U_Exx_FE_Paisadquis { get; set; }
        //           public object U_Exx_FE_Incotermto { get; set; }
        //    public object U_Exx_FE_Fleteinter { get; set; }
        //      public object U_Exx_FE_Segurointe { get; set; }
        //public object U_EXX_TDOCSI { get; set; }
        //public object U_EXX_LINEASI { get; set; }
        //     public object U_EXX_NAVESI { get; set; }
        //   public object U_EXX_VIAJESI { get; set; }
        //public object U_EXX_TIP_OPSI { get; set; }
        //    public object U_CLAVE_ACCESO { get; set; }
        //public string U_ESTADO_AUTORIZACIO { get; set; }
        //public object U_FECHA_AUT_FACT { get; set; }
        //    public object U_OBSERVACION_FACT { get; set; }
        //public string U_SSCREADAR { get; set; }
        //public object U_SSIDDOCUMENTO { get; set; }
        public string U_HRH_Serie { get; set; }
        //public string U_HRH_Modo_Fact { get; set; }
        //public string U_HRH_Lote { get; set; }
        //public string U_LocalCtaContab { get; set; }
        //public object U_LQ_CLAVE { get; set; }
        //public string U_LQ_ESTADO { get; set; }
        //public object U_LQ_NUM_AUTO { get; set; }
        public object U_LQ_FECHA_AUT { get; set; }
        //   public object U_LQ_OBSERVACION { get; set; }
        //   public object U_DOC_REF { get; set; }
        public object U_CTK_Lote { get; set; }
        public object U_CTK_Generado { get; set; }
        public object U_CTK_Observacion { get; set; }
        public object U_CtkFechaHoraGeneracion { get; set; }
        //   public object[] Document_ApprovalRequests { get; set; }
        public List<DocumentlineVentaModel> DocumentLines { get; set; }
        //public object[] DocumentAdditionalExpenses { get; set; }
        //public object[] WithholdingTaxDataWTXCollection { get; set; }
        //public object[] WithholdingTaxDataCollection { get; set; }
        //public object[] DocumentPackages { get; set; }
        //public object[] DocumentSpecialLines { get; set; }
        //public Documentinstallment[] DocumentInstallments { get; set; }
        //public object[] DownPaymentsToDraw { get; set; }
        //public Taxextension TaxExtension { get; set; }
        //public Addressextension AddressExtension { get; set; }
    }


    public class DocumentlineVentaModel
    {
        public int LineNum { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public decimal Quantity { get; set; }
        public string ShipDate { get; set; }
        public decimal Price { get; set; }
        public decimal PriceAfterVAT { get; set; }
        public string Currency { get; set; }
        public decimal? Rate { get; set; }
        public decimal DiscountPercent { get; set; }
        public string VendorNum { get; set; }
        //public object SerialNum { get; set; }
        public string WarehouseCode { get; set; }
        //public int SalesPersonCode { get; set; }
        public decimal CommisionPercent { get; set; }
        public string TreeType { get; set; }
        public string AccountCode { get; set; }
        //public string UseBaseUnits { get; set; }
        public string SupplierCatNum { get; set; }
        public string CostingCode { get; set; }
        public string ProjectCode { get; set; }
        public string BarCode { get; set; }
        public string VatGroup { get; set; }
        //public decimal Height1 { get; set; }
        //public object Hight1Unit { get; set; }
        //public decimal Height2 { get; set; }
        //public object Height2Unit { get; set; }
        //public decimal Lengh1 { get; set; }
        //public object Lengh1Unit { get; set; }
        //public decimal Lengh2 { get; set; }
        //public object Lengh2Unit { get; set; }
        //public decimal Weight1 { get; set; }
        //public object Weight1Unit { get; set; }
        //public decimal Weight2 { get; set; }
        //public object Weight2Unit { get; set; }
        //public decimal Factor1 { get; set; }
        //public decimal Factor2 { get; set; }
        //public decimal Factor3 { get; set; }
        //public decimal Factor4 { get; set; }
        //public string BaseType { get; set; }
        //public string BaseEntry { get; set; }
        //public string BaseLine { get; set; }
        //public decimal Volume { get; set; }


        //public int VolumeUnit { get; set; }
        //public decimal Width1 { get; set; }
        //public object Width1Unit { get; set; }
        //public decimal Width2 { get; set; }
        //public object Width2Unit { get; set; }
        public string Address { get; set; }
        public string TaxCode { get; set; }
        public string TaxType { get; set; }
        public string TaxLiable { get; set; }
        //public string PickStatus { get; set; }
        //public decimal PickQuantity { get; set; }
        //public object PickListIdNumber { get; set; }
        //   public object OriginalItem { get; set; }
        public string BackOrder { get; set; }
        public string FreeText { get; set; }
        public int ShippingMethod { get; set; }
        //public object POTargetNum { get; set; }
        public string POTargetEntry { get; set; }
        //        public object POTargetRowNum { get; set; }
        //public string CorrectionInvoiceItem { get; set; }
        //public decimal CorrInvAmountToStock { get; set; }
        //public decimal CorrInvAmountToDiffAcct { get; set; }
        //public decimal AppliedTax { get; set; }
        //public decimal AppliedTaxFC { get; set; }
        //public decimal AppliedTaxSC { get; set; }
        //public string WTLiable { get; set; }
        //public string DeferredTax { get; set; }
        //public decimal EqualizationTaxPercent { get; set; }
        //public decimal TotalEqualizationTax { get; set; }
        //public decimal TotalEqualizationTaxFC { get; set; }
        //public decimal TotalEqualizationTaxSC { get; set; }
        public decimal NetTaxAmount { get; set; }
        public decimal NetTaxAmountFC { get; set; }
        public decimal NetTaxAmountSC { get; set; }
        public string MeasureUnit { get; set; }
        //public decimal UnitsOfMeasurment { get; set; }
        public float LineTotal { get; set; }
        public decimal TaxPercentagePerRow { get; set; }
        public decimal TaxTotal { get; set; }
        //public string ConsumerSalesForecast { get; set; }
        //public decimal ExciseAmount { get; set; }
        public decimal TaxPerUnit { get; set; }
        //public decimal TotalInclTax { get; set; }
        //public object CountryOrg { get; set; }
        //public string SWW { get; set; }
        //public object TransactionType { get; set; }
        //public string DistributeExpense { get; set; }
        //public string ShipToCode { get; set; }
        //public decimal RowTotalFC { get; set; }
        public decimal RowTotalSC { get; set; }
        //public decimal LastBuyInmPrice { get; set; }
        //public decimal LastBuyDistributeSumFc { get; set; }
        //public decimal LastBuyDistributeSumSc { get; set; }
        //public decimal LastBuyDistributeSum { get; set; }
        //public decimal StockDistributesumForeign { get; set; }
        //public decimal StockDistributesumSystem { get; set; }
        //public decimal StockDistributesum { get; set; }
        //public decimal StockInmPrice { get; set; }
        //public string PickStatusEx { get; set; }
        //public decimal TaxBeforeDPM { get; set; }
        //public decimal TaxBeforeDPMFC { get; set; }
        //public decimal TaxBeforeDPMSC { get; set; }
        //public object CFOPCode { get; set; }
        //public object CSTCode { get; set; }
        //public object Usage { get; set; }
        //public string TaxOnly { get; set; }
        //public int VisualOrder { get; set; }
        //public decimal BaseOpenQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        //public string LineStatus { get; set; }
        //public decimal PackageQuantity { get; set; }
        public string Text { get; set; }
        //public string LineType { get; set; }
        public string COGSCostingCode { get; set; }
        //public string COGSAccountCode { get; set; }
        //public string ChangeAssemlyBoMWarehouse { get; set; }
        //public decimal GrossBuyPrice { get; set; }
        //public int GrossBase { get; set; }
        //public decimal GrossProfitTotalBasePrice { get; set; }
        public string CostingCode2 { get; set; }
        public string CostingCode3 { get; set; }
        public string CostingCode4 { get; set; }
        public string CostingCode5 { get; set; }

        public string U_DetGlosaMrk { get; set; }
        //public object ItemDetails { get; set; }
        //public object LocationCode { get; set; }
        //public string ActualDeliveryDate { get; set; }
        //public decimal RemainingOpenQuantity { get; set; }
        //public decimal OpenAmount { get; set; }
        //public decimal OpenAmountFC { get; set; }
        //public decimal OpenAmountSC { get; set; }
        //        public object ExLineNo { get; set; }
        //        public object RequiredDate { get; set; }
        //           public decimal RequiredQuantity { get; set; }
        //public object COGSCostingCode2 { get; set; }
        //public object COGSCostingCode3 { get; set; }
        //public object COGSCostingCode4 { get; set; }
        //public object COGSCostingCode5 { get; set; }
        //public object CSTforIPI { get; set; }
        //public object CSTforPIS { get; set; }
        //public object CSTforCOFINS { get; set; }
        //public object CreditOriginCode { get; set; }
        //public string WithoutInventoryMovement { get; set; }
        //public object AgreementNo { get; set; }
        //public object AgreementRowNumber { get; set; }
        //public string ShipToDescription { get; set; }
        //public object ActualBaseEntry { get; set; }
        //public object ActualBaseLine { get; set; }
        //     public int DocEntry { get; set; }
        //public decimal Surpluses { get; set; }
        //public decimal DefectAndBreakup { get; set; }
        //public decimal Shortages { get; set; }
        //public string ConsiderQuantity { get; set; }
        //public string PartialRetirement { get; set; }
        //public decimal RetirementQuantity { get; set; }
        //public decimal RetirementAPC { get; set; }
        //public string ThirdParty { get; set; }
        //public object ExpenseType { get; set; }
        //public object ReceiptNumber { get; set; }
        //public object ExpenseOperationType { get; set; }
        //public object FederalTaxID { get; set; }
        //public int UoMEntry { get; set; }
        //public string UoMCode { get; set; }
        //public decimal InventoryQuantity { get; set; }
        //public decimal RemainingOpenInventoryQuantity { get; set; }
        //public object ParentLLineNum { get; set; }
        //public int? Incoterms { get; set; }
        // public int? TransportMode { get; set; }
        //public string ItemType { get; set; }
        //public string ChangeInventoryQuantityIndependently { get; set; }
        //public object FreeOfChargeBP { get; set; }
        //public decimal U_EXX_FE_ValICEVta { get; set; }
        //public decimal U_EXX_FE_PorICEVta { get; set; }
        //public object U_EXX_C_PATRIMONIO { get; set; }

        //public Linetaxjurisdiction[] LineTaxJurisdictions { get; set; }
        //public object[] ExportProcesses { get; set; }
        //public object[] DocumentLineAdditionalExpenses { get; set; }
        //public object[] WithholdingTaxLines { get; set; }
        //public object[] SerialNumbers { get; set; }
        //public object[] BatchNumbers { get; set; }
        //public object[] DocumentLinesBinAllocations { get; set; }
    }
    public class DocumentlineCreateVentaModel
    {
        //public int LineNum { get; set; }
        //public string ItemCode { get; set; }
        //public string ItemDescription { get; set; }
        //public decimal Quantity { get; set; }
        //public string ShipDate { get; set; }
        //public decimal Price { get; set; }
        ////public decimal PriceAfterVAT { get; set; }
        ////public string Currency { get; set; }
        //public decimal Rate { get; set; }
        //public decimal DiscountPercent { get; set; }
        //public string VendorNum { get; set; }
        ////public object SerialNum { get; set; }
        //public string WarehouseCode { get; set; }
        ////public int SalesPersonCode { get; set; }
        ////public decimal CommisionPercent { get; set; }
        ////public string TreeType { get; set; }
        //public string AccountCode { get; set; }
        ////public string UseBaseUnits { get; set; }
        ////public string SupplierCatNum { get; set; }
        //public string CostingCode { get; set; }
        ////public string ProjectCode { get; set; }
        ////public object BarCode { get; set; }
        ////public string VatGroup { get; set; }
        ////public decimal Height1 { get; set; }
        ////public object Hight1Unit { get; set; }
        ////public decimal Height2 { get; set; }
        ////public object Height2Unit { get; set; }
        ////public decimal Lengh1 { get; set; }
        ////public object Lengh1Unit { get; set; }
        ////public decimal Lengh2 { get; set; }
        ////public object Lengh2Unit { get; set; }
        ////public decimal Weight1 { get; set; }
        ////public object Weight1Unit { get; set; }
        ////public decimal Weight2 { get; set; }
        ////public object Weight2Unit { get; set; }
        ////public decimal Factor1 { get; set; }
        ////public decimal Factor2 { get; set; }
        ////public decimal Factor3 { get; set; }
        ////public decimal Factor4 { get; set; }
        //public int? BaseType { get; set; }
        //public long? BaseEntry { get; set; }
        //public int? BaseLine { get; set; }
        ////public decimal Volume { get; set; }


        ////public int VolumeUnit { get; set; }
        ////public decimal Width1 { get; set; }
        ////public object Width1Unit { get; set; }
        ////public decimal Width2 { get; set; }
        ////public object Width2Unit { get; set; }
        //public string Address { get; set; }
        //public string TaxCode { get; set; }
        ////public string TaxType { get; set; }
        ////public string TaxLiable { get; set; }
        ////public string PickStatus { get; set; }
        ////public decimal PickQuantity { get; set; }
        ////public object PickListIdNumber { get; set; }
        ////   public object OriginalItem { get; set; }
        ////public string BackOrder { get; set; }
        //public string FreeText { get; set; }
        ////public int ShippingMethod { get; set; }
        ////public object POTargetNum { get; set; }
        ////public string POTargetEntry { get; set; }
        ////        public object POTargetRowNum { get; set; }
        ////public string CorrectionInvoiceItem { get; set; }
        ////public decimal CorrInvAmountToStock { get; set; }
        ////public decimal CorrInvAmountToDiffAcct { get; set; }
        ////public decimal AppliedTax { get; set; }
        ////public decimal AppliedTaxFC { get; set; }
        ////public decimal AppliedTaxSC { get; set; }
        ////public string WTLiable { get; set; }
        ////public string DeferredTax { get; set; }
        ////public decimal EqualizationTaxPercent { get; set; }
        ////public decimal TotalEqualizationTax { get; set; }
        ////public decimal TotalEqualizationTaxFC { get; set; }
        ////public decimal TotalEqualizationTaxSC { get; set; }
        ////public decimal NetTaxAmount { get; set; }
        ////public decimal NetTaxAmountFC { get; set; }
        ////public decimal NetTaxAmountSC { get; set; }
        ////public string MeasureUnit { get; set; }
        ////public decimal UnitsOfMeasurment { get; set; }
        //public decimal LineTotal { get; set; }
        //public decimal TaxPercentagePerRow { get; set; }
        //public decimal TaxTotal { get; set; }
        ////public string ConsumerSalesForecast { get; set; }
        ////public decimal ExciseAmount { get; set; }
        //public decimal TaxPerUnit { get; set; }
        ////public decimal TotalInclTax { get; set; }
        ////public object CountryOrg { get; set; }
        ////public string SWW { get; set; }
        ////public object TransactionType { get; set; }
        ////public string DistributeExpense { get; set; }
        ////public string ShipToCode { get; set; }
        ////public decimal RowTotalFC { get; set; }
        ////public decimal RowTotalSC { get; set; }
        ////public decimal LastBuyInmPrice { get; set; }
        ////public decimal LastBuyDistributeSumFc { get; set; }
        ////public decimal LastBuyDistributeSumSc { get; set; }
        ////public decimal LastBuyDistributeSum { get; set; }
        ////public decimal StockDistributesumForeign { get; set; }
        ////public decimal StockDistributesumSystem { get; set; }
        ////public decimal StockDistributesum { get; set; }
        ////public decimal StockInmPrice { get; set; }
        ////public string PickStatusEx { get; set; }
        ////public decimal TaxBeforeDPM { get; set; }
        ////public decimal TaxBeforeDPMFC { get; set; }
        ////public decimal TaxBeforeDPMSC { get; set; }
        ////public object CFOPCode { get; set; }
        ////public object CSTCode { get; set; }
        ////public object Usage { get; set; }
        ////public string TaxOnly { get; set; }
        ////public int VisualOrder { get; set; }
        ////public decimal BaseOpenQuantity { get; set; }
        //public decimal UnitPrice { get; set; }
        ////public string LineStatus { get; set; }
        ////public decimal PackageQuantity { get; set; }
        ////public string Text { get; set; }
        ////public string LineType { get; set; }
        ////public string COGSCostingCode { get; set; }
        ////public string COGSAccountCode { get; set; }
        ////public string ChangeAssemlyBoMWarehouse { get; set; }
        ////public decimal GrossBuyPrice { get; set; }
        ////public int GrossBase { get; set; }
        ////public decimal GrossProfitTotalBasePrice { get; set; }
        //public string CostingCode2 { get; set; }
        //public string CostingCode3 { get; set; }
        //public string CostingCode4 { get; set; }
        //public string CostingCode5 { get; set; }
        ////public object ItemDetails { get; set; }
        ////public object LocationCode { get; set; }
        ////public string ActualDeliveryDate { get; set; }
        ////public decimal RemainingOpenQuantity { get; set; }
        ////public decimal OpenAmount { get; set; }
        ////public decimal OpenAmountFC { get; set; }
        ////public decimal OpenAmountSC { get; set; }
        ////        public object ExLineNo { get; set; }
        ////        public object RequiredDate { get; set; }
        ////           public decimal RequiredQuantity { get; set; }
        ////public object COGSCostingCode2 { get; set; }
        ////public object COGSCostingCode3 { get; set; }
        ////public object COGSCostingCode4 { get; set; }
        ////public object COGSCostingCode5 { get; set; }
        ////public object CSTforIPI { get; set; }
        ////public object CSTforPIS { get; set; }
        ////public object CSTforCOFINS { get; set; }
        ////public object CreditOriginCode { get; set; }
        ////public string WithoutInventoryMovement { get; set; }
        ////public object AgreementNo { get; set; }
        ////public object AgreementRowNumber { get; set; }
        ////public string ShipToDescription { get; set; }
        ////public object ActualBaseEntry { get; set; }
        ////public object ActualBaseLine { get; set; }
        ////     public int DocEntry { get; set; }
        ////public decimal Surpluses { get; set; }
        ////public decimal DefectAndBreakup { get; set; }
        ////public decimal Shortages { get; set; }
        ////public string ConsiderQuantity { get; set; }
        ////public string PartialRetirement { get; set; }
        ////public decimal RetirementQuantity { get; set; }
        ////public decimal RetirementAPC { get; set; }
        ////public string ThirdParty { get; set; }
        ////public object ExpenseType { get; set; }
        ////public object ReceiptNumber { get; set; }
        ////public object ExpenseOperationType { get; set; }
        ////public object FederalTaxID { get; set; }
        ////public int UoMEntry { get; set; }
        ////public string UoMCode { get; set; }
        ////public decimal InventoryQuantity { get; set; }
        ////public decimal RemainingOpenInventoryQuantity { get; set; }
        ////public object ParentLLineNum { get; set; }
        ////    public int? Incoterms { get; set; }
        ////public int? TransportMode { get; set; }
        ////public string ItemType { get; set; }
        ////public string ChangeInventoryQuantityIndependently { get; set; }
        ////public object FreeOfChargeBP { get; set; }
        ////public decimal U_EXX_FE_ValICEVta { get; set; }
        ////public decimal U_EXX_FE_PorICEVta { get; set; }
        ////public object U_EXX_C_PATRIMONIO { get; set; }
        ////public object U_DetGlosaMrk { get; set; }
        ////public Linetaxjurisdiction[] LineTaxJurisdictions { get; set; }
        ////public object[] ExportProcesses { get; set; }
        ////public object[] DocumentLineAdditionalExpenses { get; set; }
        ////public object[] WithholdingTaxLines { get; set; }
        ////public object[] SerialNumbers { get; set; }
        ////public object[] BatchNumbers { get; set; }
        ////public object[] DocumentLinesBinAllocations { get; set; }
    }


    public class DocumentlineRespVentaModel
    {
        public int LineNum { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public decimal Quantity { get; set; }
        public string ShipDate { get; set; }
        public decimal Price { get; set; }
        //public decimal PriceAfterVAT { get; set; }
        //public string Currency { get; set; }
        public decimal Rate { get; set; }
        public decimal DiscountPercent { get; set; }
        public string VendorNum { get; set; }
        //public object SerialNum { get; set; }
        public string WarehouseCode { get; set; }
        //public int SalesPersonCode { get; set; }
        //public decimal CommisionPercent { get; set; }
        //public string TreeType { get; set; }
        public string AccountCode { get; set; }
        //public string UseBaseUnits { get; set; }
        //public string SupplierCatNum { get; set; }
        public string CostingCode { get; set; }
        //public string ProjectCode { get; set; }
        //public object BarCode { get; set; }
        //public string VatGroup { get; set; }
        //public decimal Height1 { get; set; }
        //public object Hight1Unit { get; set; }
        //public decimal Height2 { get; set; }
        //public object Height2Unit { get; set; }
        //public decimal Lengh1 { get; set; }
        //public object Lengh1Unit { get; set; }
        //public decimal Lengh2 { get; set; }
        //public object Lengh2Unit { get; set; }
        //public decimal Weight1 { get; set; }
        //public object Weight1Unit { get; set; }
        //public decimal Weight2 { get; set; }
        //public object Weight2Unit { get; set; }
        //public decimal Factor1 { get; set; }
        //public decimal Factor2 { get; set; }
        //public decimal Factor3 { get; set; }
        //public decimal Factor4 { get; set; }
        //public int? BaseType { get; set; }
        //public long? BaseEntry { get; set; }
        //public int? BaseLine { get; set; }
        //public decimal Volume { get; set; }


        //public int VolumeUnit { get; set; }
        //public decimal Width1 { get; set; }
        //public object Width1Unit { get; set; }
        //public decimal Width2 { get; set; }
        //public object Width2Unit { get; set; }
        public string Address { get; set; }
        public string TaxCode { get; set; }
        //public string TaxType { get; set; }
        //public string TaxLiable { get; set; }
        //public string PickStatus { get; set; }
        //public decimal PickQuantity { get; set; }
        //public object PickListIdNumber { get; set; }
        //   public object OriginalItem { get; set; }
        //public string BackOrder { get; set; }
        //public string FreeText { get; set; }
        //public int ShippingMethod { get; set; }
        //public object POTargetNum { get; set; }
        //public string POTargetEntry { get; set; }
        //        public object POTargetRowNum { get; set; }
        //public string CorrectionInvoiceItem { get; set; }
        //public decimal CorrInvAmountToStock { get; set; }
        //public decimal CorrInvAmountToDiffAcct { get; set; }
        //public decimal AppliedTax { get; set; }
        //public decimal AppliedTaxFC { get; set; }
        //public decimal AppliedTaxSC { get; set; }
        //public string WTLiable { get; set; }
        //public string DeferredTax { get; set; }
        //public decimal EqualizationTaxPercent { get; set; }
        //public decimal TotalEqualizationTax { get; set; }
        //public decimal TotalEqualizationTaxFC { get; set; }
        //public decimal TotalEqualizationTaxSC { get; set; }
        //public decimal NetTaxAmount { get; set; }
        //public decimal NetTaxAmountFC { get; set; }
        //public decimal NetTaxAmountSC { get; set; }
        //public string MeasureUnit { get; set; }
        //public decimal UnitsOfMeasurment { get; set; }
        public decimal LineTotal { get; set; }
        public decimal TaxPercentagePerRow { get; set; }
        public decimal TaxTotal { get; set; }
        //public string ConsumerSalesForecast { get; set; }
        //public decimal ExciseAmount { get; set; }
        public decimal TaxPerUnit { get; set; }
        //public decimal TotalInclTax { get; set; }
        //public object CountryOrg { get; set; }
        //public string SWW { get; set; }
        //public object TransactionType { get; set; }
        //public string DistributeExpense { get; set; }
        //public string ShipToCode { get; set; }
        //public decimal RowTotalFC { get; set; }
        //public decimal RowTotalSC { get; set; }
        //public decimal LastBuyInmPrice { get; set; }
        //public decimal LastBuyDistributeSumFc { get; set; }
        //public decimal LastBuyDistributeSumSc { get; set; }
        //public decimal LastBuyDistributeSum { get; set; }
        //public decimal StockDistributesumForeign { get; set; }
        //public decimal StockDistributesumSystem { get; set; }
        //public decimal StockDistributesum { get; set; }
        //public decimal StockInmPrice { get; set; }
        //public string PickStatusEx { get; set; }
        //public decimal TaxBeforeDPM { get; set; }
        //public decimal TaxBeforeDPMFC { get; set; }
        //public decimal TaxBeforeDPMSC { get; set; }
        //public object CFOPCode { get; set; }
        //public object CSTCode { get; set; }
        //public object Usage { get; set; }
        //public string TaxOnly { get; set; }
        //public int VisualOrder { get; set; }
        //public decimal BaseOpenQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        //public string LineStatus { get; set; }
        //public decimal PackageQuantity { get; set; }
        //public string Text { get; set; }
        //public string LineType { get; set; }
        //public string COGSCostingCode { get; set; }
        //public string COGSAccountCode { get; set; }
        //public string ChangeAssemlyBoMWarehouse { get; set; }
        //public decimal GrossBuyPrice { get; set; }
        //public int GrossBase { get; set; }
        //public decimal GrossProfitTotalBasePrice { get; set; }
        public string CostingCode2 { get; set; }
        public string CostingCode3 { get; set; }
        public string CostingCode4 { get; set; }
        public string CostingCode5 { get; set; }
        //public object ItemDetails { get; set; }
        //public object LocationCode { get; set; }
        //public string ActualDeliveryDate { get; set; }
        //public decimal RemainingOpenQuantity { get; set; }
        //public decimal OpenAmount { get; set; }
        //public decimal OpenAmountFC { get; set; }
        //public decimal OpenAmountSC { get; set; }
        //        public object ExLineNo { get; set; }
        //        public object RequiredDate { get; set; }
        //           public decimal RequiredQuantity { get; set; }
        //public object COGSCostingCode2 { get; set; }
        //public object COGSCostingCode3 { get; set; }
        //public object COGSCostingCode4 { get; set; }
        //public object COGSCostingCode5 { get; set; }
        //public object CSTforIPI { get; set; }
        //public object CSTforPIS { get; set; }
        //public object CSTforCOFINS { get; set; }
        //public object CreditOriginCode { get; set; }
        //public string WithoutInventoryMovement { get; set; }
        //public object AgreementNo { get; set; }
        //public object AgreementRowNumber { get; set; }
        //public string ShipToDescription { get; set; }
        //public object ActualBaseEntry { get; set; }
        //public object ActualBaseLine { get; set; }
        public int DocEntry { get; set; }
        //public decimal Surpluses { get; set; }
        //public decimal DefectAndBreakup { get; set; }
        //public decimal Shortages { get; set; }
        //public string ConsiderQuantity { get; set; }
        //public string PartialRetirement { get; set; }
        //public decimal RetirementQuantity { get; set; }
        //public decimal RetirementAPC { get; set; }
        //public string ThirdParty { get; set; }
        //public object ExpenseType { get; set; }
        //public object ReceiptNumber { get; set; }
        //public object ExpenseOperationType { get; set; }
        //public object FederalTaxID { get; set; }
        //public int UoMEntry { get; set; }
        //public string UoMCode { get; set; }
        //public decimal InventoryQuantity { get; set; }
        //public decimal RemainingOpenInventoryQuantity { get; set; }
        //public object ParentLLineNum { get; set; }
        //    public int? Incoterms { get; set; }
        //public int? TransportMode { get; set; }
        //public string ItemType { get; set; }
        //public string ChangeInventoryQuantityIndependently { get; set; }
        //public object FreeOfChargeBP { get; set; }
        //public decimal U_EXX_FE_ValICEVta { get; set; }
        //public decimal U_EXX_FE_PorICEVta { get; set; }
        //public object U_EXX_C_PATRIMONIO { get; set; }
        //public object U_DetGlosaMrk { get; set; }
        //public Linetaxjurisdiction[] LineTaxJurisdictions { get; set; }
        //public object[] ExportProcesses { get; set; }
        //public object[] DocumentLineAdditionalExpenses { get; set; }
        //public object[] WithholdingTaxLines { get; set; }
        //public object[] SerialNumbers { get; set; }
        //public object[] BatchNumbers { get; set; }
        //public object[] DocumentLinesBinAllocations { get; set; }
    }



}