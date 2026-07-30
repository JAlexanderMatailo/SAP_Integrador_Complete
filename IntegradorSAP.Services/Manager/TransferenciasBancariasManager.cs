using Newtonsoft.Json;
//using Sap.Data.Hana;
using IntegradorSAP.Data.DataAccess;
using IntegradorSAP.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sap.Data.Hana;

namespace IntegradorSAP.Data.Manager
{
    public class TransferenciasBancariasManager : DAO
    {
        Helper.ServiceLayer_Web servicio = new Helper.ServiceLayer_Web();
        string Usuario;
        string Ip;
        string errorCode;
        string errMsg;

        public string ErrMsg { get => ErrMsg; set => errMsg = value; }
        public string ErrorCode { get => ErrorCode; set => errorCode = value; }

        public TransferenciasBancariasManager()
        { }
        public TransferenciasBancariasManager(string user , string ip)

        {
            this.Usuario = user;
            this.Ip = ip;
        }
        /// <summary>
        /// Empresas SAP disponibles, leídas de la configuración (claves Sap.Login.*).
        /// Antes eran seis empresas con sus contraseñas escritas en el código.
        /// </summary>
        public List<LoginSap> GetCompanies(string CompanyDB)
        {
            List<LoginSap> companies = new List<LoginSap>();

            foreach (string nombre in Helper.SapCompanyCredentials.CompanyDbConfiguradas())
            {
                Helper.SapCredencial credencial = Helper.SapCompanyCredentials.Obtener(nombre);

                LoginSap company = new LoginSap();
                company.CompanyDB = nombre;
                company.UserName = credencial.UserName;
                company.Password = credencial.Password;
                companies.Add(company);
            }

            if (!string.IsNullOrEmpty(CompanyDB))
            {
               companies = companies.Where(p => p.CompanyDB.Contains(CompanyDB)).ToList();
            }

            return companies;
        }

        public string RemoveSpecialCharacters(string str)
        {

            str = str.Replace("%", "").Replace("/", " ").Replace("\\", "_").Replace("*", " ").Replace("+", " ").Replace("\"", " ").Replace("-", " ").Replace("[", " ").Replace("]", "").Replace("{", "").Replace("}", "").Replace("#", " ").Replace(".", " ").Replace("}", "").Replace("\r\n", "").Replace("\n", "").Replace("\r", "");
            //return Regex.Replace(str, @"[^a-zA-Z0-9_.- ]+", "", RegexOptions.Compiled);
            return str;
        }

        public bool LoginOnServiceLayer(string CompanyDB, string UserSap, string PassUserSap)
        {
            LoginSap login = new LoginSap();
            login.CompanyDB = CompanyDB;
            login.UserName = UserSap;
            login.Password = PassUserSap;

            var _json = JsonConvert.SerializeObject(login);

            var respuesta = servicio.SLSendRequestReturnResponse("Login", "POST", _json, System.Net.HttpStatusCode.OK, true);

            if (respuesta.ErrCodigo == 0)
                return true;
            else
                return false;
          

        }

        public bool LogOutServiceLayer()
        {
            var respuesta = servicio.SLSendRequestReturnResponse("Logout", "POST", "", System.Net.HttpStatusCode.NoContent, false);
            //PresentarRespuesta(respuesta);
            return true;
        }


        public List<CuentaBancaria> GetCuentaBancarias(EnumCodigoBanco codigoBanco , string CompanyDB )
        {
            HanaCommand comm = null;
            HanaDataReader reader = null;
            List<CuentaBancaria> cuentasbancarias = new List<CuentaBancaria>();
            CuentaBancaria iDocAsoDetalle = new CuentaBancaria();
            try
            {
                conectarHana(CompanyDB);
                string StrQty = @"select t1.""BankName"",t1.""CountryCod"",t0.""BankCode"",t0.""Account"", t0.""GLAccount"" from ""DSC1"" T0 JOIN ""ODSC"" T1 ON T1.""BankCode"" = t0.""BankCode""  ";
                if (codigoBanco != EnumCodigoBanco.TodosBancos)
                    StrQty += $@" WHERE t1.""CountryCod"" ='EC' and t0.""BankCode""='{((int)codigoBanco)}' "; 
               
                comm = new HanaCommand(StrQty, Connection);

                reader = comm.ExecuteReader();

                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {


                            iDocAsoDetalle = new CuentaBancaria();
                            iDocAsoDetalle.EmpresaSap = CompanyDB;
                            iDocAsoDetalle.CodigoBanco = ((int)codigoBanco).ToString();
                            iDocAsoDetalle.NombreBanco = reader.GetValue(0).ToString();
                            iDocAsoDetalle.NumeroCuenta = reader.GetValue(3).ToString();
                            iDocAsoDetalle.GLAccount = reader.GetValue(4).ToString();


                            cuentasbancarias.Add(iDocAsoDetalle);

                        }
                    }
                }
               

            }
            catch (Exception ex)
            {
                ErrorMensaje = ex.Message;
            }
            finally
            {


                LiberarVariables(ref Connection, ref comm, ref reader);
            }

            return cuentasbancarias;
        }

        public List<TransferenciaPago> GetTransferenciaPagos(CuentaBancaria cuenta)
        {
            HanaCommand comm = null;
            HanaDataReader reader = null;
            List<TransferenciaPago> transferencias = new List<TransferenciaPago>();
            TransferenciaPago iDocAsoDetalle = new TransferenciaPago();
            try
            {
                conectarHana(cuenta.EmpresaSap);
                string StrQty = "CALL  \"" + cuenta.EmpresaSap + "\".\"CTK_ARCHIVO_BANCO_TRANSF\" (";
                StrQty += " '" + cuenta.NumeroCuenta + "' ,'" + cuenta.CodigoBanco + "' ";
                StrQty += " )";
                comm = new HanaCommand(StrQty, Connection);

                reader = comm.ExecuteReader();

                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {


                            iDocAsoDetalle = new TransferenciaPago();
                            iDocAsoDetalle.DocEntry = Convert.ToInt64(reader.GetValue(20));
                            iDocAsoDetalle.DocNum = Convert.ToInt64(reader.GetValue(21));
                            iDocAsoDetalle.CodigoBancoOrigen = cuenta.CodigoBanco;
                            iDocAsoDetalle.CodigoOrientacion = reader.GetValue(0).ToString();
                            iDocAsoDetalle.NumCuentaEmpresa = reader.GetValue(1).ToString();
                            iDocAsoDetalle.Secuencial = reader.GetValue(2).ToString();
                            iDocAsoDetalle.NumComprobante = reader.GetValue(3).ToString();
                            iDocAsoDetalle.Contrapartida = reader.GetValue(4).ToString();
                            iDocAsoDetalle.Moneda = reader.GetValue(5).ToString();
                            iDocAsoDetalle.Valor = Convert.ToDecimal(reader.GetValue(6));
                            iDocAsoDetalle.FormaPago = reader.GetValue(7).ToString();
                            iDocAsoDetalle.CodigoBancoDestino = reader.GetValue(8).ToString();
                            iDocAsoDetalle.TipoCuentaDestino = reader.GetValue(9).ToString();
                            iDocAsoDetalle.NumCuentaDestino = reader.GetValue(10).ToString();
                            iDocAsoDetalle.TipoIDCliente = reader.GetValue(11).ToString();
                            iDocAsoDetalle.IdCliente = reader.GetValue(12).ToString();
                            iDocAsoDetalle.Beneficiario = reader.GetValue(13).ToString();
                            iDocAsoDetalle.Direccion = reader.GetValue(14).ToString();
                            iDocAsoDetalle.Ciudad = reader.GetValue(15).ToString();
                            iDocAsoDetalle.Telefono = reader.GetValue(16).ToString();
                            iDocAsoDetalle.Referencia = reader.GetValue(18).ToString();
                            iDocAsoDetalle.ReferenciaAdicional = reader.GetValue(19).ToString();

                            transferencias.Add(iDocAsoDetalle);

                        }
                    }
                }



            }
            catch (Exception ex)
            {
                ErrorMensaje = ex.Message;
            }
            finally
            {


                LiberarVariables(ref Connection, ref comm, ref reader);
            }

            return transferencias;
        }

        public bool GrabarTransferenciasProcedadas(CuentaBancaria cuentabanc, List<TransferenciaPago> transferencias, string FileName)
        {
            TransferenciaProcesada tp;

            foreach (var t in transferencias)
            {

                string _recurso = $"VendorPayments({t.DocEntry})";

                tp = new TransferenciaProcesada();
                tp.U_CTK_ENVIADO_BCO = "S";
                tp.U_CTK_FECHA_ENVIO_BCO = DateTime.Now.Date;
                tp.U_CTK_ENVIADO_BCO_OBS = $"Envia a Banco {cuentabanc.NombreBanco}-{cuentabanc.NumeroCuenta} {DateTime.Now }. File:{FileName}";
                var _json = JsonConvert.SerializeObject(tp);


                var respuesta = servicio.SLSendRequestReturnResponse(_recurso, "PATCH", _json, System.Net.HttpStatusCode.NoContent, false);
                //PresentarRespuesta(respuesta);
                if (respuesta.Success)
                {
                    t.FechaEnvioBanco = tp.U_CTK_FECHA_ENVIO_BCO;
                    t.EnviadoBanco = tp.U_CTK_ENVIADO_BCO == "S" ? true : false;
                    t.EnviadoBancoObservacion = tp.U_CTK_ENVIADO_BCO_OBS;
                }

            }
            //TempData["MensajeExito"] = $"Se han transmitdo {transferencias.Count} Transferencias del banco {cuentabanc.NombreBanco} {cuentabanc.CodigoBanco}"; 

            return true;
        }

        public bool ActualizarReferenciasDeTransferencias(CuentaBancaria cuentabanc, List<TransferenciaReferenciaViewModel> transferencias)
        {
            TransferenciaReferenciaResponse tp;

            foreach (var t in transferencias)
            {string referencia = string.Empty;

                //GET PAGO REALIZADO A PROVEEDORES
                Int64 idasiento = this.GetIdAsientoDePagoEfectuado(t.DocEntry,cuentabanc.EmpresaSap);
                JournalEntriesViewModel asiento = this.GetAsientoContable(idasiento);
                
                if (asiento!=null)
                {
                    t.NumeroReferencia = t.NumeroReferencia.Replace("\r", "").Replace("\r", "");

                    var line = asiento.JournalEntryLines.FirstOrDefault(p => p.AccountCode == cuentabanc.GLAccount);
                    line.AdditionalReference = t.NumeroReferencia;

                    JournalEntriesViewModel asientoNew = new JournalEntriesViewModel();
                    asientoNew.JournalEntryLines = new Journalentryline[asiento.JournalEntryLines.Count()];
                    asientoNew.JournalEntryLines[0] = new Journalentryline();
                    asientoNew.JournalEntryLines[0].AdditionalReference = t.NumeroReferencia;
                    asientoNew.JournalEntryLines[0].AccountCode = cuentabanc.GLAccount;

                    //PATCH PAGO REALIZADO A PROVEEDORES
                    string _recurso = $"JournalEntries({t.DocEntry})";
                    var _json = JsonConvert.SerializeObject(asientoNew);
                    var respuesta = servicio.SLSendRequestReturnResponse(_recurso, "PATCH", _json, System.Net.HttpStatusCode.NoContent, false);
                    


                    if (respuesta.Success)
                    {

                        return true;
                    }
                    else
                    {
                       
                        this.ErrorCode = respuesta.ErrCodigo.ToString();
                        this.ErrMsg = respuesta.ErrException.ToString();
                        return false;
                    }
                }

            }
            //TempData["MensajeExito"] = $"Se han transmitdo {transferencias.Count} Transferencias del banco {cuentabanc.NombreBanco} {cuentabanc.CodigoBanco}"; 

            return true;
        }

        public VendorPaymentsViewModel GetPagoEfectuado(Int64 Id)
        {
            string _recurso = $"VendorPayments({Id})";
            var respuesta = servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);
            VendorPaymentsViewModel vpay = JsonConvert.DeserializeObject<VendorPaymentsViewModel>(respuesta.RespuestaJson);

            return vpay;
        }

        public Int64 GetIdAsientoDePagoEfectuado(Int64 DocEntryDePago, string CompanyDB)
        {
            HanaCommand comm = null;
            HanaDataReader reader = null;
         
            try
            {
                conectarHana(CompanyDB);
                string StrQty = $@"select ""TransId""  from OJDT  where ""BaseRef""={DocEntryDePago} AND ""TransType""=46 ";
              
                comm = new HanaCommand(StrQty, Connection);

                reader = comm.ExecuteReader();

                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {

                            long idtrans = Convert.ToInt64(reader.GetValue(0));

                            return idtrans;

                        }
                    }
                }


            }
            catch (Exception ex)
            {
                ErrorMensaje = ex.Message;
            }
            finally
            {


                LiberarVariables(ref Connection, ref comm, ref reader);
            }

            return 0;
        }


        public JournalEntriesViewModel GetAsientoContable(Int64 Id)
        {
            string _recurso = $"JournalEntries({Id})";
            var respuesta = servicio.SLSendRequestReturnResponse(_recurso, "GET", "", System.Net.HttpStatusCode.OK, false);
            JournalEntriesViewModel vpay = JsonConvert.DeserializeObject<JournalEntriesViewModel>(respuesta.RespuestaJson);

            return vpay;
        }
    }
}