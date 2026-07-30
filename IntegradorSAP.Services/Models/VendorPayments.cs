using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegradorSAP.Data.Models
{
    public class VendorPaymentsViewModel
    {
        public string odatametadata { get; set; }
        public int DocNum { get; set; }
        public string DocType { get; set; }
        public string HandWritten { get; set; }
        public string Printed { get; set; }
        public string DocDate { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string Address { get; set; }
        public object CashAccount { get; set; }
        public string DocCurrency { get; set; }
        public float CashSum { get; set; }
        public object CheckAccount { get; set; }
        public string TransferAccount { get; set; }
        public float TransferSum { get; set; }
        public string TransferDate { get; set; }
        public string TransferReference { get; set; }
        public string LocalCurrency { get; set; }
        public float DocRate { get; set; }
        public string Reference1 { get; set; }
        public object Reference2 { get; set; }
        public object CounterReference { get; set; }
        public string Remarks { get; set; }
        public string JournalRemarks { get; set; }
        public string SplitTransaction { get; set; }
        public object ContactPersonCode { get; set; }
        public string ApplyVAT { get; set; }
        public string TaxDate { get; set; }
        public int Series { get; set; }
        public object BankCode { get; set; }
        public object BankAccount { get; set; }
        public float DiscountPercent { get; set; }
        public object ProjectCode { get; set; }
        public string CurrencyIsLocal { get; set; }
        public float DeductionPercent { get; set; }
        public float DeductionSum { get; set; }
        public float CashSumFC { get; set; }
        public float CashSumSys { get; set; }
        public object BoeAccount { get; set; }
        public float BillOfExchangeAmount { get; set; }
        public object BillofExchangeStatus { get; set; }
        public float BillOfExchangeAmountFC { get; set; }
        public float BillOfExchangeAmountSC { get; set; }
        public object BillOfExchangeAgent { get; set; }
        public object WTCode { get; set; }
        public float WTAmount { get; set; }
        public float WTAmountFC { get; set; }
        public float WTAmountSC { get; set; }
        public object WTAccount { get; set; }
        public float WTTaxableAmount { get; set; }
        public string Proforma { get; set; }
        public object PayToBankCode { get; set; }
        public object PayToBankBranch { get; set; }
        public object PayToBankAccountNo { get; set; }
        public string PayToCode { get; set; }
        public object PayToBankCountry { get; set; }
        public string IsPayToBank { get; set; }
        public int DocEntry { get; set; }
        public string PaymentPriority { get; set; }
        public object TaxGroup { get; set; }
        public float BankChargeAmount { get; set; }
        public float BankChargeAmountInFC { get; set; }
        public float BankChargeAmountInSC { get; set; }
        public float UnderOverpaymentdifference { get; set; }
        public float UnderOverpaymentdiffSC { get; set; }
        public float WtBaseSum { get; set; }
        public float WtBaseSumFC { get; set; }
        public float WtBaseSumSC { get; set; }
        public string VatDate { get; set; }
        public string TransactionCode { get; set; }
        public string PaymentType { get; set; }
        public float TransferRealAmount { get; set; }
        public string DocObjectCode { get; set; }
        public string DocTypte { get; set; }
        public string DueDate { get; set; }
        public object LocationCode { get; set; }
        public string Cancelled { get; set; }
        public string ControlAccount { get; set; }
        public float UnderOverpaymentdiffFC { get; set; }
        public string AuthorizationStatus { get; set; }
        public object BPLID { get; set; }
        public object BPLName { get; set; }
        public object VATRegNum { get; set; }
        public object U_EXX_SN { get; set; }
        public string U_SSCREADAR { get; set; }
        public object U_SSIDDOCUMENTO { get; set; }
        public string U_BP_Confd { get; set; }
        public object U_BP_DocNr { get; set; }
        public object U_BP_Seque { get; set; }
        public object U_CTK_BANCO_DEST_NUM { get; set; }
        public object U_CTK_FECHA_ENVIO_BCO { get; set; }
        public string U_CTK_ENVIADO_BCO { get; set; }
        public object U_CTK_ENVIADO_BCO_OBS { get; set; }
        public string U_CTK_REF_BCO { get; set; }
        public string U_CTK_REF_BCO_FECHA { get; set; }
        public object[] PaymentChecks { get; set; }
        public Paymentinvoice[] PaymentInvoices { get; set; }
        public object[] PaymentCreditCards { get; set; }
        public object[] PaymentAccounts { get; set; }
        public Billofexchange BillOfExchange { get; set; }
        public object[] WithholdingTaxCertificatesCollection { get; set; }
        public object[] CashFlowAssignments { get; set; }
        public object[] Payments_ApprovalRequests { get; set; }
        public object[] WithholdingTaxDataWTXCollection { get; set; }
    }

    public class Billofexchange
    {
    }

    public class Paymentinvoice
    {
        public int LineNum { get; set; }
        public int DocEntry { get; set; }
        public float SumApplied { get; set; }
        public float AppliedFC { get; set; }
        public float AppliedSys { get; set; }
        public float DocRate { get; set; }
        public int DocLine { get; set; }
        public string InvoiceType { get; set; }
        public float DiscountPercent { get; set; }
        public float PaidSum { get; set; }
        public int InstallmentId { get; set; }
        public float WitholdingTaxApplied { get; set; }
        public float WitholdingTaxAppliedFC { get; set; }
        public float WitholdingTaxAppliedSC { get; set; }
        public object LinkDate { get; set; }
        public object DistributionRule { get; set; }
        public object DistributionRule2 { get; set; }
        public object DistributionRule3 { get; set; }
        public object DistributionRule4 { get; set; }
        public object DistributionRule5 { get; set; }
        public float TotalDiscount { get; set; }
        public float TotalDiscountFC { get; set; }
        public float TotalDiscountSC { get; set; }
    }

}