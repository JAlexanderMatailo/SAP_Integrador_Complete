using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegradorSAP.Data.Models
{
    public class TransferenciaPago
    {


        string _CodigoOrientacion;
        public string CodigoOrientacion
        {
            get { return _CodigoOrientacion; }
            set { _CodigoOrientacion = value; }
        }

        string _NumCuentaEmpresa;
        public string NumCuentaEmpresa
        {
            get { return _NumCuentaEmpresa; }
            set { _NumCuentaEmpresa = value; }
        }
        string _Secuencial;


        public string Secuencial
        {
            get { return _Secuencial; }
            set { _Secuencial = value; }
        }
        string _NumComprobante;
        public string NumComprobante
        {
            get { return _NumComprobante; }
            set { _NumComprobante = value; }
        }
        string _Contrapartida;

        public string Contrapartida
        {
            get { return _Contrapartida; }
            set { _Contrapartida = value; }
        }
        string _Moneda;

        public string Moneda
        {
            get { return _Moneda; }
            set { _Moneda = value; }
        }
        decimal _Valor;

        public decimal Valor
        {
            get { return _Valor; }
            set { _Valor = value; }
        }
        string _FormaPago;

        public string FormaPago
        {
            get { return _FormaPago; }
            set { _FormaPago = value; }
        }
        string _CodigoBancoDestino;

        public string CodigoBancoDestino
        {
            get { return _CodigoBancoDestino; }
            set { _CodigoBancoDestino = value; }
        }
        string _TipoCuentaDestino;

        public string TipoCuentaDestino
        {
            get { return _TipoCuentaDestino; }
            set { _TipoCuentaDestino = value; }
        }
        string _NumCuentaDestino;

        public string NumCuentaDestino
        {
            get { return _NumCuentaDestino; }
            set { _NumCuentaDestino = value; }
        }
        string _TipoIDCliente;

        public string TipoIDCliente
        {
            get { return _TipoIDCliente; }
            set { _TipoIDCliente = value; }
        }
        string _IdCliente;

        public string IdCliente
        {
            get { return _IdCliente; }
            set { _IdCliente = value; }
        }

        string _Beneficiario;

        public string Beneficiario
        {
            get { return _Beneficiario; }
            set { _Beneficiario = value; }
        }
        string _Direccion;

        public string Direccion
        {
            get { return _Direccion; }
            set { _Direccion = value; }
        }
        string _Ciudad;

        public string Ciudad
        {
            get { return _Ciudad; }
            set { _Ciudad = value; }
        }


        string _Telefono;

        public string Telefono
        {
            get { return _Telefono; }
            set { _Telefono = value; }
        }

        string _Localidad;

        public string Localidad
        {
            get { return _Localidad; }
            set { _Localidad = value; }
        }
        string _Referencia;

        public string Referencia
        {
            get { return _Referencia; }
            set { _Referencia = value; }
        }
        string _ReferenciaAdicional;

        public string ReferenciaAdicional
        {
            get { return _ReferenciaAdicional; }
            set { _ReferenciaAdicional = value; }
        }


        string _estado;

        public string Estado
        {
            get { return _estado; }
            set { _estado = value; }
        }
        string _ErrMsg;

        public string ErrMsg
        {
            get { return _ErrMsg; }
            set { _ErrMsg = value; }
        }

        string _CodigoBancoOrigen;

        long _DocEntry;

        long _DocNum;

        public long DocEntry { get => _DocEntry; set => _DocEntry = value; }
        public long DocNum { get => _DocNum; set => _DocNum = value; }
        public string CodigoBancoOrigen { get => _CodigoBancoOrigen; set => _CodigoBancoOrigen = value; }

        public DateTime FechaEnvioBanco { get; set; }
        public bool EnviadoBanco { get; set; }
        public string EnviadoBancoObservacion { get; set; }
    }


    public class TransferenciaProcesada
    {

        public DateTime U_CTK_FECHA_ENVIO_BCO { get; set; }
        public string U_CTK_ENVIADO_BCO { get; set; }
        public string U_CTK_ENVIADO_BCO_OBS { get; set; }

    }

    public class TransferenciaReferenciaViewModel
    {
        public long DocEntry { get; set; }
        public long DocNum { get; set; }
        public string NumeroCuenta { get; set; }

        public string CodigoBanco { get; set; }
        public string NumeroReferencia { get; set; }

    }

    public class TransferenciaReferenciaResponse
    {

        public string Remarks { get; set; }
        public string JournalRemarks { get; set; }

        //public string U_CTK_REF_BCO { get; set; }
        //public DateTime U_CTK_REF_BCO_FECHA
        //{
        //    get; set;

        //}

    }
    
}