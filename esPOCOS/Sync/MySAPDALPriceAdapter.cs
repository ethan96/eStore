using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.MySAPDAL;

namespace eStore.POCOS.Sync
{
    public class MySAPDALPriceAdapter
    {
        public class MySAPDALResult
        {
            public MySAPDALResult() { }
            public MySAPDALResult(string productID, decimal listPrice, decimal unitPrice, decimal recyleFee, decimal tax, bool _includeTax)
            {
                this.ProductID = productID;
                this.ListPrice = listPrice;
                this.UnitPrice=unitPrice;
                this.RecyleFee=recyleFee;
                this.Tax=tax;
                this.includeTax = _includeTax;
            }
            public string ProductID { get; set; }
            public decimal ListPrice { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal RecyleFee { get; set; }
            public decimal Tax { get; set; }
            public bool includeTax { get; set; }
            public decimal TaxRate
            {
                get
                {
                    return ListPrice == 0 || Tax == 0 ? 0 : Tax / ListPrice;
                }
            }
        }

        private Dictionary<Part, int> ErrorBatch = new Dictionary<Part, int>();
        private Dictionary<Part, int> SyncBatch ;

        private string SAPOrg { get; set; }
        public string PriceSAPLvl { get; set; }
        public bool includeTax { get; set; }

        System.Text.StringBuilder sbErrors = new System.Text.StringBuilder();
        public string ErrorMessage { get { return sbErrors.ToString(); } }
       

        public List<MySAPDALResult> getSAPPrice(Store store, List<Part> syncParts)
        {
            if (store == null || syncParts == null)
                return null;
            Dictionary<Part, int> DicsyncParts = syncParts.ToDictionary(x => x, x => 1);
            return getSAPPrice(store, DicsyncParts);
        }

        private List<MySAPDALResult> result;

        public List<MySAPDALResult> getSAPPrice(Store store, Dictionary<Part, int> syncParts)
        {
            if (store == null || syncParts == null)
                return null;
            result = new List<MySAPDALResult>();
            SyncBatch =  syncParts ;
            SAPOrg = store.getStringSetting("PriceSAPOrg");
            if (string.IsNullOrEmpty(PriceSAPLvl))
                PriceSAPLvl = store.getStringSetting("PriceSAPLvlL1");
            sbErrors = new StringBuilder();
          
            while (SyncBatch.Count > 0)
            {
                getSAPPriceFromWebService(store, SyncBatch);
            }

            Dictionary<Part, int> ckeckingErrorBatch = new Dictionary<Part, int>();
            foreach (var e in ErrorBatch)
            {
                ckeckingErrorBatch.Add(e.Key, e.Value);
            }
            ErrorBatch.Clear();
            //get from sap one by one for erroritems
            foreach (var item in ckeckingErrorBatch)
            {
                getSAPPriceFromWebService(store, ckeckingErrorBatch.TakeWhile(x => x.Key == item.Key).ToDictionary(x => x.Key, x => x.Value));
            }
            if (ErrorBatch.Count > 0)
            {
                string cannotgetpricelist = string.Join(",", (from error in ErrorBatch
                                                              select error.Key.SProductID).ToArray());
                sbErrors.AppendFormat("can't get price items list: {0}", cannotgetpricelist);
            }
            return result;
        }

        private void getSAPPriceFromWebService(Store store, Dictionary<Part, int> syncParts)
        {
            try
            {
                //abr call different ws method, tax=BX13+BX23+BX72+BX82
                if (store.StoreID == "ABR")
                    getSAPPriceGenerallyforABR(syncParts);
                else
                    getSAPPriceGenerally(syncParts);
            }
            catch (Exception ex)
            {
                //when error insert syncparts into ErrorBatch and clear SyncBatch 
                foreach (var p in syncParts)
                {
                    ErrorBatch.Add(p.Key, p.Value);
                }
                SyncBatch.Clear();
                sbErrors.Append(ex.Message);
                
            }
      
        }

        private void getSAPPriceGenerally(Dictionary<Part, int> syncParts)
        {
            MySAPDAL.SAPDALDS.ProductOutDataTable POutDt;

        
            MySAPDAL.SAPDALDS.ProductInDataTable PInDt = new MySAPDAL.SAPDALDS.ProductInDataTable();
           
            foreach (var part in syncParts)
            {

                MySAPDAL.SAPDALDS.ProductInRow PIn = PInDt.NewProductInRow();
                PIn.QTY = part.Value;
                PIn.PART_NO = part.Key.SProductID;
                PInDt.AddProductInRow(PIn);
            }
            MySAPDAL.MYSAPDAL ws = new MySAPDAL.MYSAPDAL();
            bool returnresult = false;
            string strErr=string.Empty;
            POutDt = new SAPDALDS.ProductOutDataTable();
            try
            {
                returnresult = ws.GetPrice(this.PriceSAPLvl, PriceSAPLvl, this.SAPOrg, PInDt, ref POutDt, ref strErr);
            }
            catch (Exception ex)
            {
                PInDt = null;
                returnresult = false;
                strErr = ex.Message;
            } 
            
            if (returnresult == true && string.IsNullOrEmpty(strErr) && POutDt!=null)
            {
             
                foreach (MySAPDAL.SAPDALDS.ProductOutRow row in POutDt.Rows)
                {

                    decimal listprice = 0; 
                    decimal.TryParse(row.LIST_PRICE,out listprice);
                    decimal unitprice = 0;
                    decimal.TryParse(row.UNIT_PRICE, out unitprice);
          
                    decimal recylefee = 0;
                    decimal.TryParse(row.RECYCLE_FEE, out recylefee);

                    //原本service返回时如果list price为0会使用unit price来代替，更改后不会替换(仅针对cn10的设置)
                    if (listprice == 0)
                        listprice = unitprice;
                    decimal tax = 0;
                    decimal.TryParse(row.TAX, out tax);
                    decimal rate = (tax == 0 || listprice == 0) ? 0m : tax / listprice;
                    decimal[] resutlrow = { listprice, recylefee, tax, rate };
                    result.Add(new MySAPDALResult(row.PART_NO, listprice, unitprice, recylefee, tax,includeTax));


                }
                SyncBatch.Clear();
            }
            else if (returnresult == true && string.IsNullOrEmpty(strErr) == false)
            {
                POCOS.Part invalidpart = (from p in syncParts
                                          where strErr.ToUpper().Contains(p.Key.SProductID)
                                          select p.Key).FirstOrDefault();

                if (invalidpart != null&& !ErrorBatch.ContainsKey(invalidpart))
                {
                    //ErrorBatch.Add(invalidpart,syncParts[invalidpart]);

                    SyncBatch.Remove(invalidpart);
                     
                }
                else
                {
                    foreach (var p in syncParts)
                    {
                        ErrorBatch.Add(p.Key, p.Value);
                    }
                    SyncBatch.Clear();
                }
                sbErrors.Append(strErr);
            }
            else
            {
                foreach (var p in syncParts)
                {
                    ErrorBatch.Add(p.Key, p.Value);
                }
                SyncBatch.Clear();
                sbErrors.Append(strErr);
            }
          
        }

        private void getSAPPriceGenerallyforABR(Dictionary<Part, int> syncParts)
        {
            MySAPDAL.SAPDALDS.ProductOutDataTable POutDt;


            MySAPDAL.SAPDALDS.ProductInDataTable PInDt = new MySAPDAL.SAPDALDS.ProductInDataTable();

            foreach (var part in syncParts)
            {

                MySAPDAL.SAPDALDS.ProductInRow PIn = PInDt.NewProductInRow();
                PIn.QTY = part.Value;
                PIn.PART_NO = part.Key.SProductID;
                PInDt.AddProductInRow(PIn);
            }
            MySAPDAL.MYSAPDAL ws = new MySAPDAL.MYSAPDAL();
            bool returnresult = false;
            string strErr = string.Empty;
            POutDt = new MySAPDAL.SAPDALDS.ProductOutDataTable();
            try
            {
                MySAPDAL.SAPOrderType type = MySAPDAL.SAPOrderType.ZQTR;
                returnresult = ws.GetPriceV2(this.PriceSAPLvl, PriceSAPLvl, this.SAPOrg, type,PInDt, ref POutDt, ref strErr);
            }
            catch (Exception ex)
            {
                PInDt = null;
                returnresult = false;
                strErr = ex.Message;
            }

            if (returnresult == true && string.IsNullOrEmpty(strErr) && POutDt != null)
            {

                foreach (MySAPDAL.SAPDALDS.ProductOutRow row in POutDt.Rows)
                {

                    decimal listprice = 0;
                    decimal.TryParse(row.LIST_PRICE, out listprice);
                    decimal unitprice = 0;
                    decimal.TryParse(row.UNIT_PRICE, out unitprice);

                    decimal recylefee = 0;
                    decimal.TryParse(row.RECYCLE_FEE, out recylefee);

                    decimal tax = 0;
                    decimal.TryParse(row.TAX, out tax);
                    decimal rate = (tax == 0 || listprice == 0) ? 0m : tax / listprice;
                    decimal[] resutlrow = { listprice, recylefee, tax, rate };
                    result.Add(new MySAPDALResult(row.PART_NO, listprice, unitprice, recylefee, tax, includeTax));


                }
                SyncBatch.Clear();
            }
            else if (returnresult == true && string.IsNullOrEmpty(strErr) == false)
            {
                POCOS.Part invalidpart = (from p in syncParts
                                          where strErr.ToUpper().Contains(p.Key.SProductID)
                                          select p.Key).FirstOrDefault();

                if (invalidpart != null && !ErrorBatch.ContainsKey(invalidpart))
                {
                    //ErrorBatch.Add(invalidpart,syncParts[invalidpart]);

                    SyncBatch.Remove(invalidpart);

                }
                else
                {
                    foreach (var p in syncParts)
                    {
                        ErrorBatch.Add(p.Key, p.Value);
                    }
                    SyncBatch.Clear();
                }
                sbErrors.Append(strErr);
            }
            else
            {
                foreach (var p in syncParts)
                {
                    ErrorBatch.Add(p.Key, p.Value);
                }
                SyncBatch.Clear();
                sbErrors.Append(strErr);
            }

        }
        private void getSAPPriceForABR(Dictionary<Part, int> syncParts)
        {
            MySAPDAL.SAPDALDS.ProductOut_ABRDataTable POutDt;


            MySAPDAL.SAPDALDS.ProductInDataTable PInDt = new MySAPDAL.SAPDALDS.ProductInDataTable();

            foreach (var part in syncParts)
            {

                MySAPDAL.SAPDALDS.ProductInRow PIn = PInDt.NewProductInRow();
                PIn.QTY = part.Value;
                PIn.PART_NO = part.Key.SProductID;
                PInDt.AddProductInRow(PIn);
            }
            MySAPDAL.MYSAPDAL ws = new MySAPDAL.MYSAPDAL();
            bool returnresult = false;
            string strErr = string.Empty;
            POutDt = new SAPDALDS.ProductOut_ABRDataTable();
            SAPOrderType type = SAPOrderType.ZORC;
            try
            {
                returnresult = ws.GetMultiPrice_ABR_TAX_2(this.SAPOrg, type, this.PriceSAPLvl, PriceSAPLvl, PInDt, ref POutDt, ref strErr);
            }
            catch (Exception ex)
            {
                PInDt = null;
                returnresult = false;
                strErr = ex.Message;
            }
            //1 return true no errors
            if (returnresult == true && string.IsNullOrEmpty(strErr) && POutDt != null)
            {

                foreach (MySAPDAL.SAPDALDS.ProductOut_ABRRow row in POutDt.Rows)
                {
                    try
                    {
                        decimal listprice = 0;
                        if (!string.IsNullOrEmpty(row.LIST_PRICE))
                            decimal.TryParse(row.LIST_PRICE, out listprice);

                        decimal unitprice = 0;
                        if (!string.IsNullOrEmpty(row.NET_PRICE))
                            decimal.TryParse(row.NET_PRICE, out unitprice);

                        decimal recylefee = 0;

                        decimal tax = 0, bx13 = 0, bx23 = 0, bx72 = 0, bx82 = 0;
                        if (!string.IsNullOrEmpty(row.BX13))
                            decimal.TryParse(row.BX13, out bx13);
                        if (!string.IsNullOrEmpty(row.BX23))
                            decimal.TryParse(row.BX23, out bx23);
                        if (!string.IsNullOrEmpty(row.BX72))
                            decimal.TryParse(row.BX72, out bx72);
                        if (!string.IsNullOrEmpty(row.BX82))
                            decimal.TryParse(row.BX82, out bx82);
                        tax = bx13 + bx23 + bx72 + bx82;
                        decimal rate = (tax == 0 || listprice == 0) ? 0m : tax / listprice;
                        listprice = listprice + tax;
                        decimal[] resutlrow = { listprice, recylefee, tax, rate };
                        result.Add(new MySAPDALResult(row.PART_NO, listprice, unitprice, recylefee, tax, includeTax));
                    }
                    catch (Exception)//exception becauls nulldb files
                    {

                        //POCOS.Part invalidpart = (from p in syncParts
                        //                          where row.PART_NO==p.Key.SProductID
                        //                          select p.Key).FirstOrDefault();

                        //if (invalidpart != null && !ErrorBatch.ContainsKey(invalidpart))
                        //{
                        //    //ErrorBatch.Add(invalidpart, syncParts[invalidpart]);

                        //    SyncBatch.Remove(invalidpart);

                        //}
                    }
               
                }
                SyncBatch.Clear();
            }
            else if (string.IsNullOrEmpty(strErr) == false)
            {
                POCOS.Part invalidpart = (from p in syncParts
                                          where strErr.ToUpper().Contains(p.Key.SProductID)
                                          select p.Key).FirstOrDefault();

                //if matched parts in error message, then remove it, else, remove all.
                if (invalidpart != null && !ErrorBatch.ContainsKey(invalidpart))
                {
                    //ErrorBatch.Add(invalidpart, syncParts[invalidpart]);

                    SyncBatch.Remove(invalidpart);

                }
                else
                {
                    foreach (var p in syncParts)
                    {
                        ErrorBatch.Add(p.Key, p.Value);
                    }
                    SyncBatch.Clear();
                }
                sbErrors.Append(strErr);
            }
            else
            {
                foreach (var p in syncParts)
                {
                    ErrorBatch.Add(p.Key, p.Value);
                }
                SyncBatch.Clear();
                sbErrors.Append(strErr);
            }
          
           
        }
    }
}
