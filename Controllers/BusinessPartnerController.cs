using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _10xErp.Models;
using _10xErp.Helpers;
using System.Data;

namespace _10xErp.Controllers
{
    public class BusinessPartnerController : Controller
    {
        private readonly DataHelper objHlpr = new DataHelper();
        // GET: BusinessPartner
        public ActionResult Index()
        {
            List<CustomerModal> customersList = GetCustomerListList();
            return View(customersList);
        }

        public ActionResult ViewCustomerDetails(string cardCode)
        {
            ViewBag.Current = "View";
            CustomerModal customerDetails = new CustomerModal();
            customerDetails = GetCustomerDetails(cardCode);
            return View(customerDetails);
        }

        private CustomerModal GetCustomerDetails(string cardCode)
        {
            CustomerModal objCustomer = new CustomerModal();

            try
            {
                ///string strSQL = "SELECT T0.\"CardCode\", T0.\"CardName\",T0.\"E_Mail\" ,T0.\"Phone1\", T0.\"Phone2\", T0.\"CntctPrsn\" FROM OCRD T0";
                string strSQL = "SELECT T0.\"CardCode\", T0.\"CardName\",(Case when T0.\"CardType\"='C' then 'Customers' when T0.\"CardType\"='S' then 'Suppliers' else '' end)as \"CardType\"," +
                    " T0.\"CardFName\",T0.\"Balance\",T0.\"OrdersBal\", (select \"GroupName\" from OCRG where \"GroupCode\"=T0.\"GroupCode\") as \"Group\", (select \"CurrName\" from OCRN where \"CurrCode\"=T0.\"Currency\") as \"Currency\", " +
                    " (select \"PymntGroup\" from OCTG where \"GroupNum\"=T0.\"GroupNum\") as \"PaymentTerms\", T0.\"LicTradNum\", T0.\"E_Mail\" ,T0.\"Phone1\", T0.\"Phone2\", T0.\"CntctPrsn\",T0.\"Cellular\",T0.\"Fax\",T0.\"E_Mail\", " +
                    " (case when T0.\"validFor\"='Y' then 'Active' else 'Inactive' end) as \"BPStatus\", (select \"descript\" from OTER where \"territryID\"=T0.\"Territory\" and ISNULL(T0.\"Territory\",'')<>'' ) as \"Territory\" , " +
                    " T0.\"CreditLine\",T0.\"DebtLine\",(select \"SlpName\" from OSLP where \"SlpCode\" =T0.\"SlpCode\") as \"SlpName\"," +
                    " (select \"Listname\" from OPLN where OPLN.\"ListNum\"=  T0.\"ListNum\") as \"ListName\" " +
                    " FROM OCRD T0 where T0.\"CardCode\" = '" + cardCode + "' order by T0.\"CardCode\"";
                DataSet dsData = objHlpr.getDataSet(strSQL);

                foreach (DataRow dr in dsData.Tables[0].Rows)
                {
                    CustomerModal obj = new CustomerModal()
                    {
                        CardCode = dr["CardCode"].ToString(),
                        CardType = dr["CardType"].ToString(),
                        CardName = dr["CardName"].ToString(),
                        CardFName = dr["CardFName"].ToString(),
                        Cellular = dr["Cellular"].ToString(),
                        Balance = Convert.ToDouble(dr["Balance"].ToString()),
                        OrderBal = Convert.ToDouble(dr["OrdersBal"].ToString()),
                        CustGrp = dr["Group"].ToString(),
                        BPStatus = dr["BPStatus"].ToString(),
                        Phone1 = dr["Phone1"].ToString(),
                        Phone2 = dr["Phone2"].ToString(),
                        Email = dr["E_Mail"].ToString(),
                        ContactPerson = dr["CntctPrsn"].ToString(),
                        Territory = dr["Territory"].ToString(),
                        DebtLimit = dr["DebtLine"].ToString(),
                        CreditLimit = dr["CreditLine"].ToString(),
                        Currency = dr["Currency"].ToString(),
                        LicTradNum = dr["LicTradNum"].ToString(),
                        Fax = dr["Fax"].ToString(),
                        SalesEmp = dr["SlpName"].ToString(),
                        Pricelist = dr["ListName"].ToString(),
                        PaymentTerms = dr["PaymentTerms"].ToString(),
                    };
                    obj.lstShipToAddr = GetAddress(obj.CardCode, "S");
                    obj.lstBillToAddr = GetAddress(obj.CardCode, "B");
                    obj.ShipToCount = obj.lstShipToAddr?.Count ?? 0;
                    obj.BillToCount = obj.lstBillToAddr?.Count ?? 0;
                    //customersList.Add(obj);

                    objCustomer = obj;
                }
            }
            catch (Exception ex)
            {

                ex.Message.ToString();
            }

            return objCustomer;
        }

        private List<CustomerModal> GetCustomerListList()
        {
            List<CustomerModal> customersList = new List<CustomerModal>();

            try
            {
                string strSQL = "SELECT T0.\"CardCode\", T0.\"CardName\",T0.\"Phone1\", T0.\"Phone2\", T0.\"CntctPrsn\" " +
                    " FROM OCRD T0 order by T0.\"CardCode\"";
                DataSet dsData = objHlpr.getDataSet(strSQL);

                foreach (DataRow dr in dsData.Tables[0].Rows)
                {
                    CustomerModal obj = new CustomerModal()
                    {
                        CardCode = dr["CardCode"].ToString(),
                        CardName = dr["CardName"].ToString(),
                        Phone1 = dr["Phone1"].ToString(),
                        Phone2 = dr["Phone2"].ToString(),
                        ContactPerson = dr["CntctPrsn"].ToString()
                    };
                    obj.lstShipToAddr = GetAddress(obj.CardCode, "S");
                    obj.lstBillToAddr = GetAddress(obj.CardCode, "B");
                    obj.ShipToCount = obj.lstShipToAddr?.Count ?? 0;
                    obj.BillToCount = obj.lstBillToAddr?.Count ?? 0;
                    customersList.Add(obj);
                }
            }
            catch (Exception ex)
            {

                ex.Message.ToString();
            }

            return customersList;
        }

        public List<BPAddressModel> GetAddress(string bpCardCode, string Addresstype)
        {
            List<BPAddressModel> lstAddressDocs = new List<BPAddressModel>();
            try
            {
                if (bpCardCode != null)
                {

                    var AddrsQry = "SELECT T1.\"Address\", T1.\"Address2\", T1.\"Address3\", T1.\"Street\", T1.\"Block\"," +
                        " (select \"Name\" from OCRY where \"Code\"=T1.\"Country\") as \"Country\",T1.\"GlblLocNum\", T1.\"City\", T1.\"ZipCode\", T1.\"County\"," +
                        " (select \"Name\" from OCST where \"Code\"=T1.\"State\" and Country = T1.\"Country\") as \"State\", T1.\"AdresType\" FROM " +
                                   "\"OCRD\"  T0 INNER JOIN CRD1 T1 ON T0.\"CardCode\" = T1.\"CardCode\" WHERE T1.\"AdresType\" ='" + Addresstype + "' " +
                                   " and (T1.\"CardCode\" ='" + bpCardCode + "' or T0.\"CardName\" like '%" + bpCardCode + "%')";

                    var ds = objHlpr.getDataSet(AddrsQry);
                    var count = 1;
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        BPAddressModel obj = new BPAddressModel
                        {
                            Sno = count++,
                            //billTo = dr["Address2"].ToString(),
                            Address = dr["Address"].ToString(),
                            Address2 = dr["Address2"].ToString(),
                            Address3 = dr["Address3"].ToString(),
                            //addressName2 = dr["Address2"].ToString(),
                            //addressName3 = dr["Address3"].ToString(),
                            Street = dr["Street"].ToString(),
                            Block = dr["Block"].ToString(),
                            City = dr["City"].ToString(),
                            ZIPCode = dr["ZipCode"].ToString(),
                            Country = dr["Country"].ToString(),
                            State = dr["State"].ToString(),
                            FedaralTaxID = dr["GlblLocNum"].ToString()
                        };
                        lstAddressDocs.Add(obj);
                    }


                }
            }
            catch (Exception e)
            {
            }
            finally
            {

            }
            return lstAddressDocs;

        }


    }
}