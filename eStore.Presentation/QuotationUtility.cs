using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using System.Data;


namespace eStore.Presentation
{
    /// <summary>
    /// 
    /// </summary>
    public class QuotationUtility
    {

        protected DataTable getQuotationTableType(string _name = "dtQuotation")
        {
            var _dt4WebService = new DataTable(_name);
            _dt4WebService.Columns.Add(new DataColumn("CartID", typeof(string)));
            _dt4WebService.Columns.Add(new DataColumn("StoreID", typeof(string)));
            _dt4WebService.Columns.Add(new DataColumn("QuotationNumber", typeof(string)));
            _dt4WebService.Columns.Add(new DataColumn("QuoteDate", typeof(DateTime)));
            _dt4WebService.Columns.Add(new DataColumn("QuoteExpiredDate", typeof(DateTime)));
            _dt4WebService.Columns.Add(new DataColumn("Version", typeof(string)));
            _dt4WebService.Columns.Add(new DataColumn("ShipmentTerm", typeof(string)));
            _dt4WebService.Columns.Add(new DataColumn("EarlyShipFlag", typeof(Boolean)));
            _dt4WebService.Columns.Add(new DataColumn("ShippingMethod", typeof(string)));
            _dt4WebService.Columns.Add(new DataColumn("Freight", typeof(string)));
            _dt4WebService.Columns.Add(new DataColumn("Insurance", typeof(string)));
            _dt4WebService.Columns.Add(new DataColumn("Tax", typeof(string)));
            _dt4WebService.Columns.Add(new DataColumn("DueDate", typeof(DateTime)));
            _dt4WebService.Columns.Add(new DataColumn("RequiredDate", typeof(DateTime)));
            _dt4WebService.Columns.Add(new DataColumn("ConfirmedDate", typeof(DateTime)));
            _dt4WebService.Columns.Add(new DataColumn("ConfirmedBy", typeof(string)));
            _dt4WebService.Columns.Add(new DataColumn("TotalAmount", typeof(string)));
            _dt4WebService.Columns.Add(new DataColumn("UserID", typeof(string)));
            _dt4WebService.Columns.Add(new DataColumn("Status", typeof(string)));
            _dt4WebService.Columns.Add(new DataColumn("TaxRate", typeof(string)));
            _dt4WebService.Columns.Add(new DataColumn("PromoteCode", typeof(string)));
            _dt4WebService.Columns.Add(new DataColumn("TotalDiscount", typeof(string)));
            _dt4WebService.Columns.Add(new DataColumn("LocalCurrency", typeof(string)));
            _dt4WebService.Columns.Add(new DataColumn("LocalCurExchangeRate", typeof(string)));
            _dt4WebService.Columns.Add(new DataColumn("Comments", typeof(string)));
            _dt4WebService.Columns.Add(new DataColumn("LastUpdateBy", typeof(string)));
            _dt4WebService.Columns.Add(new DataColumn("ResellerID", typeof(string)));
            _dt4WebService.Columns.Add(new DataColumn("VATNumber", typeof(string)));
            _dt4WebService.Columns.Add(new DataColumn("FreightDiscount", typeof(string)));
            _dt4WebService.Columns.Add(new DataColumn("TaxDiscount", typeof(string)));
            _dt4WebService.Columns.Add(new DataColumn("ResellerCertificate", typeof(string)));
            _dt4WebService.Columns.Add(new DataColumn("DutyAndTax", typeof(string)));
            _dt4WebService.Columns.Add(new DataColumn("TaxAndFees", typeof(string)));
            _dt4WebService.Columns.Add(new DataColumn("VATTax", typeof(string)));
            _dt4WebService.Columns.Add(new DataColumn("OtherTaxAndFees", typeof(string)));
            return _dt4WebService;
        }

        protected DataTable getQuotationCartInforType(string _name = "dtQuotationCartItem")
        {
            var _dtCartItem4WebService = new DataTable(_name);
            _dtCartItem4WebService.Columns.Add(new DataColumn("ProductName", typeof(string)));
            _dtCartItem4WebService.Columns.Add(new DataColumn("VendorProductDesc", typeof(string)));
            _dtCartItem4WebService.Columns.Add(new DataColumn("ABCInd", typeof(string)));
            _dtCartItem4WebService.Columns.Add(new DataColumn("StockStatus", typeof(string)));
            _dtCartItem4WebService.Columns.Add(new DataColumn("ItemType", typeof(string)));
            _dtCartItem4WebService.Columns.Add(new DataColumn("Qty", typeof(string)));
            _dtCartItem4WebService.Columns.Add(new DataColumn("VendorSuggestedPrice", typeof(string)));
            _dtCartItem4WebService.Columns.Add(new DataColumn("UnitPrice", typeof(string)));
            return _dtCartItem4WebService;
        }

        public DataTable changeQuotationListToDataTable(List<Quotation> list)
        {
            DataTable quotationDT = getQuotationTableType();
            if (list != null && list.Any())
            {
                foreach (var item in list)
                    addQuotationToDataTable(item, ref quotationDT);
            }
            return quotationDT;
        }


        public DataSet changeQuotationInforToDataSet(Quotation quotation)
        {
            if (quotation == null)
                return null;
            DataSet ds = new DataSet("quotationDS");
            DataTable quotationInfor = getQuotationTableType();
            addQuotationToDataTable(quotation, ref quotationInfor);
            ds.Tables.Add(quotationInfor);

            DataTable quotationCartTable = getQuotationCartInforType();
            foreach (var item in quotation.cartX.CartItems)
                addQuotationCartInforToDataTable(item, ref quotationCartTable);
            ds.Tables.Add(quotationCartTable);

            return ds;
        }

        /// <summary>
        /// Quotation 添加至 DataTable
        /// </summary>
        /// <param name="quotation"></param>
        /// <param name="quotationDT"></param>
        private void addQuotationToDataTable(Quotation quotation, ref DataTable quotationDT)
        {
            if (quotation != null)
            {
                var item = quotation;

                DataRow dr = quotationDT.NewRow();
                dr["CartID"] = item.CartID;
                dr["StoreID"] = item.StoreID;
                dr["QuotationNumber"] = item.QuotationNumber;
                dr["QuoteDate"] = item.QuoteDate;
                dr["QuoteExpiredDate"] = item.QuoteExpiredDate;
                dr["Version"] = item.Version;
                dr["ShipmentTerm"] = item.ShipmentTerm;
                dr["EarlyShipFlag"] = item.EarlyShipFlag;
                dr["ShippingMethod"] = item.ShippingMethod;
                dr["Freight"] = item.Freight;
                dr["Insurance"] = item.Insurance;
                dr["Tax"] = item.Tax;
                dr["DueDate"] = item.DueDate;
                dr["RequiredDate"] = item.RequiredDate;
                dr["ConfirmedDate"] = item.ConfirmedDate;
                dr["ConfirmedBy"] = item.ConfirmedBy;
                dr["TotalAmount"] = item.TotalAmount;
                dr["UserID"] = item.UserID;
                dr["Status"] = item.Status;
                dr["TaxRate"] = item.TaxRate;
                dr["PromoteCode"] = item.PromoteCode;
                dr["TotalDiscount"] = item.TotalDiscount;
                dr["LocalCurrency"] = item.LocalCurrency;
                dr["LocalCurExchangeRate"] = item.LocalCurExchangeRate;
                dr["Comments"] = item.Comments;
                dr["LastUpdateBy"] = item.LastUpdateBy;
                dr["ResellerID"] = item.ResellerID;
                dr["VATNumber"] = item.VATNumber;
                dr["FreightDiscount"] = item.FreightDiscount;
                dr["TaxDiscount"] = item.TaxDiscount;
                dr["ResellerCertificate"] = item.ResellerCertificate;
                dr["DutyAndTax"] = item.DutyAndTax;
                dr["TaxAndFees"] = item.TaxAndFees;
                dr["VATTax"] = item.VATTax;
                dr["OtherTaxAndFees"] = item.OtherTaxAndFees;
                quotationDT.Rows.Add(dr); 
            }
        }


        private void addQuotationCartInforToDataTable(CartItem item, ref DataTable quotationCartInforDT)
        {
            DataRow dr = quotationCartInforDT.NewRow();

            dr["ProductName"] = item.ProductName;
            dr["VendorProductDesc"] = item.partX.VendorProductDesc;
            dr["ABCInd"] = item.partX.ABCInd;
            dr["StockStatus"] = item.partX.StockStatus;
            dr["ItemType"] = item.ItemType;
            dr["Qty"] = item.Qty;
            dr["VendorSuggestedPrice"] = item.partX.VendorSuggestedPrice;
            dr["UnitPrice"] = item.UnitPrice;
            quotationCartInforDT.Rows.Add(dr);
            if (item.BTOSystem != null)
            {
                foreach (POCOS.BTOSConfig btoc in item.BTOSystem.BTOSConfigs)
                {
                    foreach (POCOS.BTOSConfigDetail btoDetail in btoc.BTOSConfigDetails)
                    {
                        foreach (KeyValuePair<Part, int> kvp in item.btosX.parts)
                        {
                            if (btoDetail.SProductID == kvp.Key.SProductID)
                            {
                                DataRow ff = quotationCartInforDT.NewRow();
                                ff["ProductName"] = btoDetail.SProductID;
                                ff["VendorProductDesc"] = kvp.Key.VendorProductDesc == null ? "" : kvp.Key.VendorProductDesc;
                                ff["ABCInd"] = kvp.Key.ABCInd == null ? "" : kvp.Key.ABCInd;
                                ff["StockStatus"] = kvp.Key.StockStatus == null ? "" : kvp.Key.StockStatus;
                                ff["ItemType"] = kvp.Key.ProductType == null ? "" : kvp.Key.ProductType;
                                ff["Qty"] = btoDetail.Qty;
                                ff["VendorSuggestedPrice"] = btoDetail.NetPrice;
                                ff["UnitPrice"] = btoDetail.AdjustedPrice;
                                quotationCartInforDT.Rows.Add(ff);
                            }
                        }                        
                    }
                }
            }
            if (item.bundleX != null)
            {
                foreach (var b in item.bundleX.BundleItems)
                {
                    DataRow dd = quotationCartInforDT.NewRow();
                    dd["ProductName"] = b.part.SProductID;
                    dd["VendorProductDesc"] = b.part.VendorProductDesc;
                    dd["ABCInd"] = b.part.ABCInd;
                    dd["StockStatus"] = b.part.StockStatus;
                    dd["ItemType"] = b.part.ProductType;
                    dd["Qty"] = b.Qty;
                    dd["VendorSuggestedPrice"] = b.part.VendorSuggestedPrice;
                    dd["UnitPrice"] = b.Price;
                    quotationCartInforDT.Rows.Add(dd);
                }
            }
        }


    }


}
