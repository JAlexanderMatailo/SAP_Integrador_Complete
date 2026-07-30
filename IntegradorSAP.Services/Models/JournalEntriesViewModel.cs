using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegradorSAP.Data.Models
{
    

     public class JournalEntriesViewModel
    {
            public string CompanyDB { get; set; }

            public string ReferenceDate { get; set; }
            public string Memo { get; set; }
            public string Reference { get; set; }
            public string Reference2 { get; set; }
            public object TransactionCode { get; set; }
            public object ProjectCode { get; set; }
            public string TaxDate { get; set; }
            public int JdtNum { get; set; }
            public object Indicator { get; set; }
            public string UseAutoStorno { get; set; }
            public object StornoDate { get; set; }
            public object VatDate { get; set; }
            public int Series { get; set; }
            public string StampTax { get; set; }
            public string DueDate { get; set; }
            public string AutoVAT { get; set; }
            public int Number { get; set; }
            public object FolioNumber { get; set; }
            public object FolioPrefixString { get; set; }
            public string ReportEU { get; set; }
            public string Report347 { get; set; }
            public string Printed { get; set; }
            public object LocationCode { get; set; }
            public string OriginalJournal { get; set; }
            public int Original { get; set; }
            public string BaseReference { get; set; }
            public string BlockDunningLetter { get; set; }
            public string AutomaticWT { get; set; }
            public float WTSum { get; set; }
            public float WTSumSC { get; set; }
            public float WTSumFC { get; set; }
            public object SignatureInputMessage { get; set; }
            public object SignatureDigest { get; set; }
            public object CertificationNumber { get; set; }
            public object PrivateKeyVersion { get; set; }
            public string Corisptivi { get; set; }
            public object Reference3 { get; set; }
            public object DocumentType { get; set; }
            public string DeferredTax { get; set; }
            public object BlanketAgreementNumber { get; set; }
            public object OperationCode { get; set; }
            public string ResidenceNumberType { get; set; }
            public string ECDPostingType { get; set; }
            public object ExposedTransNumber { get; set; }
            public object PointOfIssueCode { get; set; }
            public object Letter { get; set; }
            public object FolioNumberFrom { get; set; }
            public object FolioNumberTo { get; set; }
            public Journalentryline[] JournalEntryLines { get; set; }
            public object[] WithholdingTaxDataCollection { get; set; }
        }

    public class Journalentryline
        {
            public int Line_ID { get; set; }
            public string AccountCode { get; set; }
            public float Debit { get; set; }
            public float Credit { get; set; }
            public float FCDebit { get; set; }
            public float FCCredit { get; set; }
            public object FCCurrency { get; set; }
            public string DueDate { get; set; }
            public string ShortName { get; set; }
            public string ContraAccount { get; set; }
            public string LineMemo { get; set; }
            public string ReferenceDate1 { get; set; }
            public object ReferenceDate2 { get; set; }
            public string Reference1 { get; set; }
            public string Reference2 { get; set; }
            public string ProjectCode { get; set; }
            public object CostingCode { get; set; }
            public string TaxDate { get; set; }
            public float BaseSum { get; set; }
            public object TaxGroup { get; set; }
            public float DebitSys { get; set; }
            public float CreditSys { get; set; }
            public object VatDate { get; set; }
            public string VatLine { get; set; }
            public float SystemBaseAmount { get; set; }
            public float VatAmount { get; set; }
            public float SystemVatAmount { get; set; }
            public float GrossValue { get; set; }
            public string AdditionalReference { get; set; }
            public object CheckAbs { get; set; }
            public object CostingCode2 { get; set; }
            public object CostingCode3 { get; set; }
            public object CostingCode4 { get; set; }
            public object TaxCode { get; set; }
            public string TaxPostAccount { get; set; }
            public object CostingCode5 { get; set; }
            public object LocationCode { get; set; }
            public string ControlAccount { get; set; }
            public float EqualizationTaxAmount { get; set; }
            public float SystemEqualizationTaxAmount { get; set; }
            public float TotalTax { get; set; }
            public float SystemTotalTax { get; set; }
            public string WTLiable { get; set; }
            public string WTRow { get; set; }
            public string PaymentBlock { get; set; }
            public object BlockReason { get; set; }
            public string FederalTaxID { get; set; }
            public object BPLID { get; set; }
            public object BPLName { get; set; }
            public object VATRegNum { get; set; }
            public string PaymentOrdered { get; set; }
            public object ExposedTransNumber { get; set; }
            public int DocumentArray { get; set; }
            public int DocumentLine { get; set; }
            public object U_BD_Exp { get; set; }
            public object[] CashFlowAssignments { get; set; }
        }


    public class JournalEntriesCreateViewModel
    {

        public string ReferenceDate { get; set; }
        public string Memo { get; set; }
        public string Reference { get; set; }
        public string Reference2 { get; set; }
        public object ProjectCode { get; set; }
        public string TaxDate { get; set; }
        //public object Indicator { get; set; }
        //public string UseAutoStorno { get; set; }
        //public object StornoDate { get; set; }
        //public object VatDate { get; set; }
        public int Series { get; set; }
       // public string StampTax { get; set; }
        public string DueDate { get; set; }
        //public string AutoVAT { get; set; }
        //public int Number { get; set; }
        //public object FolioNumber { get; set; }
        //public object FolioPrefixString { get; set; }
        //public string ReportEU { get; set; }
        //public string Report347 { get; set; }
        //public string Printed { get; set; }
        //public object LocationCode { get; set; }
        //public string OriginalJournal { get; set; }
        //public int Original { get; set; }
        //public string BaseReference { get; set; } //
        //public string BlockDunningLetter { get; set; }
        //public string AutomaticWT { get; set; }
        //public float WTSum { get; set; }
        //public float WTSumSC { get; set; }
        //public float WTSumFC { get; set; }
        //public object SignatureInputMessage { get; set; }
        //public object SignatureDigest { get; set; }
        //public object CertificationNumber { get; set; }
        //public object PrivateKeyVersion { get; set; }
        //public string Corisptivi { get; set; }
        public object Reference3 { get; set; }
       // public object DocumentType { get; set; }
        //public string DeferredTax { get; set; }
        //public object BlanketAgreementNumber { get; set; }
        //public object OperationCode { get; set; }
        //public string ResidenceNumberType { get; set; }
        //public string ECDPostingType { get; set; }
        //public object ExposedTransNumber { get; set; }
        //public object PointOfIssueCode { get; set; }
        //public object Letter { get; set; }
        //public object FolioNumberFrom { get; set; }
        //public object FolioNumberTo { get; set; }
        public List<JournalentryCreateline> JournalEntryLines { get; set; }
        public object[] WithholdingTaxDataCollection { get; set; }
    }

    public class JournalentryCreateline
    {
        public int Line_ID { get; set; }
        public string AccountCode { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal FCDebit { get; set; }
        public decimal FCCredit { get; set; }
        //public decimal FCCurrency { get; set; }
        public string DueDate { get; set; }
        public string ShortName { get; set; } // = AccountCode o Codigo de proveedor/cliente
        public string ContraAccount { get; set; } // = Cuenta de Contrapartida
        public string LineMemo { get; set; } //Glosa del asiento
        public string ReferenceDate1 { get; set; }
        public object ReferenceDate2 { get; set; }
        public string Reference1 { get; set; }
        public string Reference2 { get; set; }
        public string ProjectCode { get; set; }
        public string CostingCode { get; set; }
        public string TaxDate { get; set; }
        public decimal BaseSum { get; set; } = 0;
       // public object TaxGroup { get; set; }
        //public decimal DebitSys { get; set; }
        //public decimal CreditSys { get; set; }
        // public object VatDate { get; set; }
        // public string VatLine { get; set; }
        public decimal SystemBaseAmount { get; set; } = 0;
        public decimal VatAmount { get; set; } = 0;
        public decimal SystemVatAmount { get; set; } = 0;
        public decimal GrossValue { get; set; } = 0;
        public string AdditionalReference { get; set; }
       // public object CheckAbs { get; set; }
        public object CostingCode2 { get; set; }
        public object CostingCode3 { get; set; }
        public object CostingCode4 { get; set; }
      //  public object TaxCode { get; set; }
      //  public string TaxPostAccount { get; set; }
        public object CostingCode5 { get; set; }
        //public object LocationCode { get; set; }
        public string ControlAccount { get; set; }
        //AccountCode
        // public float EqualizationTaxAmount { get; set; }
        //public float SystemEqualizationTaxAmount { get; set; }
        public float TotalTax { get; set; }=0;
        //public float SystemTotalTax { get; set; } = 0;
        //public string WTLiable { get; set; }
       // public string WTRow { get; set; }
       //public string PaymentBlock { get; set; }
       // public object BlockReason { get; set; }
        //public string FederalTaxID { get; set; }
        //public object BPLID { get; set; }
        //public object BPLName { get; set; }
        //public object VATRegNum { get; set; }
        //public string PaymentOrdered { get; set; }
        //public object ExposedTransNumber { get; set; }
        //public int DocumentArray { get; set; }
        //public int DocumentLine { get; set; }
        //public object U_BD_Exp { get; set; }
        //public object U_EXX_C_PATRIMONIO { get; set; }
        //public object[] CashFlowAssignments { get; set; }
    }

}