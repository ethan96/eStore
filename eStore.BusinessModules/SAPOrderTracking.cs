using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.BusinessModules.USAdvOrderTrkWebService;
using System.Data;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using eStore.POCOS;
using eStore.POCOS.DAL;
using eStore.Utilities;
using System.IO;
using System.Configuration;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
namespace eStore.BusinessModules
{
    /// <summary>
    /// This class is used to get order tracking info. via web service from SAP.
    /// It returns XML string, then it will be saved as XML file in C:\eStoreResources3C
    /// Presentation layer can get the SAP order in this class. The SAP order structure is the same as POCOS.order.
    /// </summary>
    public class SAPOrderTracking
    {
        #region properties
        private Store _store = null;
        public Store Store
        {
            get { return _store; }
            set { _store = value; }
        }

        // PriceOrg is the same in SAP. Ex: AUS store's price org is US01
        private string _priceOrg;
        public string PriceOrd
        {
            get { return _priceOrg; }
        }

        private string _email;
        public string Email
        {
            get { return _email.Trim(); }
        }

        private string _partNo;
        public string PartNo
        {
            get { return _partNo; }
            set { _partNo = value; }
        }

        // Purchased order number, it's provided by customer.
        private string _poNo;
        public string PONO
        {
            get { return _poNo; }
            set { _poNo = value; }
        }

        // Order No.
        private string _soNO;
        public string SONO
        {
            get { return _soNO; }
        }

        // Search particular order period, the default period is 3 month.
        private DateTime _beginDate;
        public DateTime BeginDate
        {
            get { return _beginDate; }
            set { _beginDate = value; }
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get { return _endDate; }
            set { _endDate = value; }
        }

        // It is used to get the returned result from SAP order tracking web service.
        private string _responseXMLstring = "";
        public string ResponseXMLString
        {
            get { return _responseXMLstring; }
            set { _responseXMLstring = value; }
        }

        //internal SAPTrackingOrder _sapOrder;
        protected SAPTrackingOrder _sapOrder;
        public SAPTrackingOrder SAPOrder
        { get { return _sapOrder; } }

        protected SAPTrackingOrderV2 _sapOrderV2;
        public SAPTrackingOrderV2 SAPOrderV2
        { get { return _sapOrderV2; } }
        private bool _fromMyOrder;
        private Order _storeOrder;
        public Order StoreOrder
        {
            get { return _storeOrder; }
        }

        //SAP search period
        //private int  _searchDuringDay = -365;    //previous 90 days
        //public int SearchDuringDay
        //{
        //    get { return _searchDuringDay; }
        //}

        #endregion

        #region methods
        //Constructor
        //public SAPOrderTracking(Store store, string email, string orderNo)
        public SAPOrderTracking(POCOS.Order order, int searchDuringDay = -365, bool v2 = false)
        {
            try
            {
                _storeOrder = null;

                if(order == null)
                    throw new Exception("POCOS.Order can't be null");
                _storeOrder = order;

                _fromMyOrder = v2;

                // To get the price org
                StoreSolution storesln = StoreSolution.getInstance();
                _store = storesln.getStore(order.StoreID);
                _priceOrg = _store.getStoreGlossary("PriceSAPOrg");

                _email = order.UserID;
                _soNO = order.OrderNo;

                // Initial search period.
                _endDate = DateTime.Today;
                _beginDate = _endDate.AddDays(searchDuringDay);

                //Get SAP tracking order via web service, it returns XML in string format
                getSAPOrderTrackingInXMLstr();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This function is used to get SAP order tracking via web service, then save order in C:\eStoreResources3C\SAPOrderTracking
        /// </summary>
        private void getSAPOrderTrackingInXMLstr()
        {
            try
            {
                MyAdvantechOrderTrkWebService.eBizAEU_WS orderTracking = new MyAdvantechOrderTrkWebService.eBizAEU_WS();
                decimal result;
                string beginDate = _beginDate.ToString("yyyyMMdd");
                string endDate = _endDate.ToString("yyyyMMdd");
                result = orderTracking.GetOrdTrkSpeedByMail(_priceOrg, beginDate, endDate, _email, _partNo, _poNo, _soNO, ref _responseXMLstring);

                if (result == 0 && _responseXMLstring.Equals("<NewDataSet />")==false)
                {
                    // Save result to XML file
                    saveSAPOrderTrackingXML();
                }
            }
            catch (SoapException soapex)
            {
                eStoreLoger.Error("Can't get the SAP order tracking via SAP web service.", "", "", _store.storeID, soapex);
                throw soapex;
            }

            catch (Exception ex)
            {
                eStoreLoger.Error("Fail to get the SAP order tracking.", "", "", _store.storeID, ex);
                throw ex;
            }
        }

        /// <summary>
        /// This function is used to save SAP web service result to XML file.
        /// </summary>
        private void saveSAPOrderTrackingXML()
        {
            StringBuilder filePath = new StringBuilder();
            string folderName = (string.IsNullOrEmpty(_store.storeID)) ? "Unknown" : _store.storeID;
            filePath.Append(System.Configuration.ConfigurationManager.AppSettings.Get("SAPOrderTracking_Path")).Append("/").Append(folderName).Append("/");
            // filename is order number
            string filename = "";
            filename = _soNO + ".xml";

            //Check saving directory existent
            if (!Directory.Exists(@filePath.ToString()))
            { 
                //Create default saving folder
                Directory.CreateDirectory(@filePath.ToString());
            }

            using (StreamWriter sw = new StreamWriter(filePath + filename))
            {
                // Replace unknown SAP XML element tag with eStore term
                //reviseSAPXMLelement function will revise sap unknow tag instead of eStore term
                if(!string.IsNullOrEmpty(_responseXMLstring)){
                sw.Write(reviseSAPXMLelement());
                sw.Close();
                }
            }

            //Deserialize process to set SAPTrackingOrder property in this object(XML deserialize as object.)
            try
            {
                deserializeXML(filePath + filename);
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Fail to get the SAP order tracking.", "", "", _store.storeID, ex);
                throw ex;
            }


        }

        /// <summary>
        /// This function is used to parsing SAP xml element tag as we know in eStore 3.0
        /// </summary>
        /// <param name="xml"></param>
        private String reviseSAPXMLelement()
        {
            _responseXMLstring.Trim();
            StringBuilder xml = new StringBuilder(_responseXMLstring);

            xml.Insert(0, "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n");
            xml.Replace("NewDataSet>", "SAPTrackingOrder>");
            xml.Replace("Order_x0020_Lines", "SAPOrderItemDetail");
            xml.Replace("Order_x0020_Header", "SAPOrderHeader");
            xml.Replace("Order_x0020_Schedules", "SAPOrderScheduleLine");
            xml.Replace("Ship_x002F_Sold_x002F_Bill_x0020_to", "CartContacts");
            xml.Replace("Bezei>", "Status>");                                         //Name of the controlling area
            xml.Replace("Vbeln>", "SO_NO>");                                     //Sales and Distribution Document Number
            xml.Replace("Posnr>", "LineNO>");                                     //Item number of the SD document
            xml.Replace("Matnr>", "PartNO>");                                    //Material Number
            xml.Replace("Arktx>", "PartDesc>");                                   //Short Text for Sales Order Item
            xml.Replace("Uepos>", "HigherLevel>");                          //Higher-level item in bill of material structures
            xml.Replace("Vstel>", "Plant>");                                           //Shipping Podecimal/Receiving Podecimal
            xml.Replace("Netpr>", "ListPrice>");                                   //Net price
            xml.Replace("Meins>", "MeasureUnit>");                        //Base Unit of Measure
            xml.Replace("Netwr_Vbap>", "Subtotal>");                    //Net value in document currency
            xml.Replace("Werks>", "Plant>");                                        //Plant (Own or External)
            xml.Replace("Gbsta>", "ProcessStatus>");                      //Overall processing status of the SD document item
            xml.Replace("Vbeln_Vl>", "ShippingNo>");                     //Delivery
            xml.Replace("Posnr_Vl>", "ShippingItem>");                  //Delivery item
            xml.Replace("Lfdat>", "ShippingDate>");                         //Delivery date
            xml.Replace("Lfimg>", "ShippingQty>");                          //Actual quantity delivered (in sales units)
            xml.Replace("Wadat_Ist>", "GoodsMovementDate>"); //Actual goods movement date
            xml.Replace("Bolnr>", "TrackingNo>");                             //Bill of lading
            xml.Replace("Vbeln_Vf>", "BillingDocument>");           //Billing Document
            xml.Replace("Posnr_Vf>", "BillingItem>");                       //Billing item
            xml.Replace("Fkdat>", "BillingDate>");                                  //Billing date for billing index and prdecimalout
            xml.Replace("Fkimg>", "InvoicedQty>");                              //Actual Invoiced Quantity
            xml.Replace("Rfbsk>", "TransferToAccountStatus>");     //Status for transfer to accounting
            xml.Replace("Bezei_Vbrk", "ControllingAreaStatus");                          //Name of the controlling area
            xml.Replace("Netwr_Inv>", "OrderTotalAmount>"); //Net value in document currency
            xml.Replace("Kwmeng>", "OrderQty>");                       //Cumulative order quantity in sales units
            xml.Replace("Inco2>", "ShippingMethod>");               //Incoterms (part 2)
            xml.Replace("Lfimg_Total>", "ActualShippingQty>");    //Actual quantity delivered (in sales units)
            xml.Replace("Listprice>", "SAPListPrice>");
            xml.Replace("Kzwi1>", "TotalAmount>");                       //Subtotal 1 from pricing procedure for condition
            xml.Replace("Wavwr>", "CostInDocCurrency>");       //Cost in document currency
            xml.Replace("Posar", "ItemType");                                     //Item type
            xml.Replace("Kursk_Vbap", "ExchangeRate");               //Exchange Rate for Price Determination
            xml.Replace("Lgort", "StorageLocation");                    //Storage Location
            xml.Replace("Bolnr", "BillOfLanding");                           //Bill of lading
            xml.Replace("Vbeln_Vf", "BillDoc");                    //Billing Document
            xml.Replace("Netwr_Inv_Ln", "NetValueInDocCurrency");        //Net value in document currency
            xml.Replace("Rfbsk", "TransferToAccountingStatus");       //Status for transfer to accounting
            xml.Replace("Trspg", "ShipmentBlockingReason");               //Shipment Blocking Reason

            xml.Replace("Auart>", "OrderType>");                            //Sales Document Type
            xml.Replace("Kunnr>", "CustomerNo>");                       //Customer Number 1
            xml.Replace("City1>", "ShipToCity>");                                     //Ship to city
            xml.Replace("Post_Code1>", "ShipToZipcode>");             //Ship to zipcode
            xml.Replace("Country>", "ShipToCountry>");
            xml.Replace("Street>", "ShipToStreet1>");
            xml.Replace("Str_Suppl1", "ShipToStreet2");
            xml.Replace("Str_Suppl2", "ShipToStreet3");
            xml.Replace("Name1>", "CustomerName>");
            xml.Replace("Vkorg>", "SalesOrg>");                        //Sales Organization
            xml.Replace("Vtweg>", "DistributionChannel>");  //Distribution Channel
            xml.Replace("Spart>", "Division>");                            //Division
            xml.Replace("Vkgrp>", "Salesgroup>");                    //Sales group
            xml.Replace("Vkbur>", "ProfitCenter>");                 //Sales office
            xml.Replace("Audat>", "DocDate>");                           //Document date (date received/sent)
            xml.Replace("Bstnk>", "PO_NO>");                           //Customer purchase order number
            xml.Replace("Waerk>", "Currency>");                      //SD document currency
            xml.Replace("Netwr>", "Subtotal>");                        //Net value in document currency
            xml.Replace("Erdat>", "RecordCreatedDate>");    //Date on which the record was created
            xml.Replace("Erzet>", "EntryTime>");                                //Entry time
            xml.Replace("Ernam>", "CreatedBy>");                            //Name of Person who Created the Object
            xml.Replace("Kursk>", "ExchangeRate>");                         //Exchange Rate for Price Determination
            xml.Replace("Faksk", "BillingBlockInSdDoc");               //Billing block in SD document
            xml.Replace("Lifsk", "DeliveryBlock");                           //Delivery block (document header)
            xml.Replace("Augru", "OrderReason");                       //Order reason (reason for the business transaction)

            xml.Replace("Wmeng>", "OrderQtyInSalesUnits>");     //Order quantity in sales units
            xml.Replace("Etenr>", "ScheduleLine>");                         //Schedule line
            xml.Replace("Bmeng>", "ConfirmedQty>");                 //Confirmed quantity
            xml.Replace("Edatu>", "ScheduleLineDate>");             //Schedule line date
            xml.Replace("xmlns=\"ZSD_SODN_INQ_WSF7\"", "");
            string result = xml.ToString();
            result = result.Insert(result.IndexOf("SAPTrackingOrder") + 17, "\n<SAPOrderItems>");
            result = result.Insert(result.IndexOf("SAPOrderHeader") - 1, "\n</SAPOrderItems>\n");

            if (result.IndexOf("SAPOrderScheduleLine") >0)
            {
                result = result.Insert(result.IndexOf("SAPOrderScheduleLine") - 1, "<SAPOrderScheduleLines>\n");
                result = result.Insert(result.LastIndexOf("SAPOrderScheduleLine") + 21, "</SAPOrderScheduleLines>\n");
            }
            if (result.IndexOf("CartContacts") > 0)
            {
                result = result.Insert(result.IndexOf("CartContacts") - 1, "<SAPOrderCartContacts>\n");
                result = result.Insert(result.LastIndexOf("CartContacts") + 13, "</SAPOrderCartContacts>\n");
            }
            if (result.IndexOf("Conditions") > 0)
            {
                result = result.Insert(result.IndexOf("Conditions") - 1, "<SAPOrderConditions>\n");
                result = result.Insert(result.LastIndexOf("Conditions") + 11, "</SAPOrderConditions>\n");
            }
            return result;
        }

        /// <summary>
        /// This function is used to deserialize XML files as a SAPTrackingOrder instance.
        /// </summary>
        /// <param name="filename"></param>
        private void deserializeXML(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SAPTrackingOrder));

            // Reading the XML document requires a filestream
            Stream fileStream = new FileStream(filename, FileMode.Open);
            //_sapOrder = (SAPTrackingOrder)serializer.Deserialize(fileStream);

            if (_fromMyOrder == true)
            {
                XmlSerializer serializerV2 = new XmlSerializer(typeof(SAPTrackingOrderV2));
                _sapOrderV2 = (SAPTrackingOrderV2)serializerV2.Deserialize(fileStream);
            }
            else
                _sapOrder = (SAPTrackingOrder)serializer.Deserialize(fileStream);
            ////Set LineGroup
            //foreach (SAPOrderItemDetail item in _sapOrder.SAPOrderItems)
            //{
            //    if (item.LineNO >= 100)
            //    {
            //        decimal no = item.LineNO / 100;
            //        int btosNo = System.Convert.ToInt16(Math.Floor(no));
            //        item.LineGrp = btosNo * 100;
            //    }
            //    else
            //        item.LineGrp = item.LineNO;
            //}

            ////Set Last one flag on each item, the flag is only for BTOS configs
            //foreach (SAPOrderItemDetail item in _sapOrder.SAPOrderItems)
            //{
            //    if (item.LineNO >= 100)
            //    {
            //        int tempNo = item.LineGrp;
            //        int i = 0;
            //        i = (from x in _sapOrder.SAPOrderItems
            //             where x.LineGrp.Equals(tempNo)
            //             orderby x.LineNO descending
            //             select x.LineNO).FirstOrDefault() ;

            //        if (item.LineNO == i)
            //            item.LastLineNO = true;

            //        if (item.LineNO == item.LineGrp)
            //            item.FirstLineNO = true;
            //    }
            //}

            fileStream.Close();
        }

        private BTOSystem btosSys;
        /// <summary>
        /// This function is used to compose a POCOS.Order then return to presentation layer. Reading the XML document requires a FileStream.
        /// </summary>
        /// <returns>
        /// situation 1. If orderHelper can't find the order in SAP, then return original order.
        /// situation 2. If orderHelper find the order in SAP , then return updated order.
        /// </returns>
        public POCOS.Order getStoreOrder()
        {
            //Get Order from eStore
          
          
            if (_sapOrder == null)
            {
                _storeOrder.UpdateBySAP = false;
                return _storeOrder;   //return original order to presentation layer
            }
            else
            {
                OrderHelper orderHelper = new OrderHelper();
                Order order = orderHelper.getOrderbyOrderno(_soNO);
                //non estore order
                if (order == null)
                {
                    order = this.StoreOrder;
                    DateTime orderCreatedDate;
                    Match matchResults = null;
                    try
                    {
                        Regex regexObj = new Regex(@"^(\d{4})(\d{2})(\d{2})$");
                        matchResults = regexObj.Match(SAPOrder.SAPOrderHeader.RecordCreatedDate);
                        if (matchResults.Success)
                        {
                            orderCreatedDate = new DateTime(int.Parse(matchResults.Groups[1].Value), int.Parse(matchResults.Groups[2].Value), int.Parse(matchResults.Groups[3].Value));
                            order.OrderDate = orderCreatedDate;
                        }
                        else
                        {
                            // Match attempt failed
                        }
                    }
                    catch (ArgumentException)
                    {
                        // Syntax error in the regular expression
                    }
                    order.cartX.ShipToContact = new CartContact();
                    order.cartX.SoldToContact = new CartContact();
                    order.cartX.BillToContact = new CartContact();
                }

                if (string.IsNullOrEmpty(order.OrderNo))
                    order.OrderNo = _sapOrder.SAPOrderHeader.SO_NO;

                order.TotalDiscount = 0;
                order.TaxDiscount = 0;
                order.FreightDiscount = 0;
                // Update Order's field "UpdateBySAP"
                order.UpdateBySAP = true;

                //Remove all cart items in cart
                order.cartX.removeAllItem();

                //Add user to order.
               // UserHelper userHelper = new UserHelper();
                //User user = userHelper.getUserbyID(_email);
               // order.User = user;

                // Replace ship to address

                CartContacts shipto = _sapOrder.CartContacts.FirstOrDefault(x => x.PARVW == "Ship-to party");
                if (shipto != null)
                {
                    order.cartX.ShipToContact.Address1 = shipto.ADDRESS;

                    order.cartX.ShipToContact.City = shipto.CITY;
                    order.cartX.ShipToContact.State = shipto.STATE;
                    order.cartX.ShipToContact.ZipCode = shipto.ZIP_CODE;
                    order.cartX.ShipToContact.Country = shipto.COUNTRY_NAME;
                    order.cartX.ShipToContact.AttCompanyName = shipto.NAME1;
                    order.cartX.ShipToContact.TelNo = shipto.TEL_NO;
                }


                CartContacts soldto = _sapOrder.CartContacts.FirstOrDefault(x => x.PARVW == "Sold-to party");
                if (soldto != null)
                {
                    order.cartX.SoldToContact.Address1 = soldto.ADDRESS;
                    order.cartX.SoldToContact.City = soldto.CITY;
                    order.cartX.SoldToContact.State = soldto.STATE;
                    order.cartX.SoldToContact.ZipCode = soldto.ZIP_CODE;
                    order.cartX.SoldToContact.Country = soldto.COUNTRY_NAME;
                    order.cartX.SoldToContact.AttCompanyName = soldto.NAME1;
                    order.cartX.SoldToContact.TelNo = soldto.TEL_NO;
                }

                CartContacts billto = _sapOrder.CartContacts.FirstOrDefault(x => x.PARVW == "Bill-to party");
                if (billto != null)
                {
                    order.cartX.BillToContact.Address1 = billto.ADDRESS;

                    order.cartX.BillToContact.City = billto.CITY;
                    order.cartX.BillToContact.State = billto.STATE;
                    order.cartX.BillToContact.ZipCode = billto.ZIP_CODE;
                    order.cartX.BillToContact.Country = billto.COUNTRY_NAME;
                    order.cartX.BillToContact.AttCompanyName = billto.NAME1;
                    order.cartX.BillToContact.TelNo = billto.TEL_NO;
                }

                //freight and tax

                if (_sapOrder.Conditions.Count > 0)
                {
                    try
                    {
                        decimal sapTax = _sapOrder.Conditions.Where(x => x.Cond_Type == "JR2").Sum(x => decimal.Parse(x.Condvalue));
                        if (sapTax > order.Tax)
                            order.Tax = sapTax;
                    }
                    catch (Exception)
                    {
                        
                       
                    }

                    try
                    {
                        decimal sapFright = _sapOrder.Conditions.Where(x => x.Cond_Type == "ZHD0").Sum(x => decimal.Parse(x.Condvalue));
                        if (sapFright > order.Freight)
                            order.Freight = sapFright;
                    }
                    catch (Exception)
                    {
                    }
                }

                // Replace cart items, and add shipping carrier's tracking number
                PartHelper partHelper = new PartHelper();

                //BTOSystem btosSys;
                BTOSConfig btosConfig;
                Part btosMainPart = new Part();
                int btosQty = 0;

                foreach (SAPOrderItemDetail item in _sapOrder.getTopLevelItems())
                {
                    List<SAPOrderItemDetail> subitems = _sapOrder.getSubLevelItems(item);
                    if (subitems == null || subitems.Count == 0)//standard product
                    {
                        try
                        {
                            Part standardProduct = partHelper.getPart(item.PartNO, _store.storeID);
                            if (standardProduct != null)
                            {
                                CartItem cartitem = order.cartX.addItem(standardProduct, System.Convert.ToInt16(item.OrderQty));
                                cartitem.updateUnitPrice(item.ListPrice);
                                cartitem.PackageTrackingNo = item.TrackingNo;
                                cartitem.PackageTrackingStatus = item.Status;
                                //order.cartX.CartItems.Add(cartitem);
                            }
                            else
                            {
                                //Create a dummy part to add to cartitem
                                Part dummyPart = new Part();
                                dummyPart.SProductID = item.PartNO;
                                dummyPart.VendorProductName = item.PartNO;
                                dummyPart.VendorProductDesc = item.PartDesc;
                                dummyPart.LocalPrice = item.ListPrice;
                                dummyPart.StoreID = _store.storeID;
                                CartItem cartitem = order.cartX.addItem(dummyPart, System.Convert.ToInt16(item.OrderQty));
                                cartitem.updateUnitPrice(item.ListPrice);
                                cartitem.PackageTrackingNo = item.TrackingNo;
                                cartitem.PackageTrackingStatus = item.Status;
                                //order.cartX.CartItems.Add(cartitem);
                                order.cartX.updateItem(cartitem);
                                throw new Exception();
                            }
                        }
                        catch (Exception ex)
                        {
                            eStoreLoger.Warn("SAP order has some parts that eStore doesn't have.", "", "", _store.storeID, ex);
                        }
                    }
                    else//ctos
                    {
                        // First BTOS config item
                        btosSys = new BTOSystem();
                        btosSys.BTONo = item.PartNO;
                        btosSys.storeID = _store.storeID;
                        btosMainPart = new Part();
                        btosMainPart.SProductID = item.PartNO;
                        btosMainPart.name = item.PartNO;
                        btosMainPart.VendorProductDesc = item.PartDesc;
                        btosMainPart.LocalPrice = item.ListPrice;
                        btosQty = System.Convert.ToInt16(item.OrderQty);
                        btosSys.parts.Add(btosMainPart, btosQty);

                        btosConfig = new BTOSConfig();
                        foreach (SAPOrderItemDetail child in subitems)
                        {
                            Part btosPart = partHelper.getPart(child.PartNO, _store.storeID);
                            //If part can't be find in DB, then create a dummy part
                            Dictionary<Part, int> parts = new Dictionary<Part, int>();
                            if (btosPart == null)
                            {
                                Part dummyBtosPart = new Part();
                                dummyBtosPart.SProductID = child.PartNO;
                                dummyBtosPart.VendorProductDesc = child.PartDesc;
                                dummyBtosPart.LocalPrice = child.ListPrice;
                                dummyBtosPart.StoreID = _store.storeID;
                                btosConfig = btosSys.addNoneCTOSItem(dummyBtosPart, (int)child.OrderQty / btosQty, child.ListPrice);
                                parts.Add(dummyBtosPart, System.Convert.ToInt16(child.OrderQty));
                            }
                            else
                            {
                                btosConfig = btosSys.addNoneCTOSItem(btosPart, (int)child.OrderQty / btosQty, child.ListPrice);
                                parts.Add(btosPart, System.Convert.ToInt16(child.OrderQty));
                            }
                            btosConfig.parts = parts;
                        }

                        CartItem cartitem;
                        cartitem = order.cartX.addItem(btosMainPart, btosQty, btosSys);
                        cartitem.type = Product.PRODUCTTYPE.CTOS;
                        cartitem.AdjustedPrice = btosSys.Price;
                        cartitem.PackageTrackingNo = item.TrackingNo;
                        cartitem.PackageTrackingStatus = item.Status;
                        order.cartX.updateItem(cartitem);
                        btosMainPart = null;
                        btosSys = null;
                        btosQty = 0;
                    }
                }

                order.cartX.reconcile();
                return order;
            }
        }

        public List<Order> getSAPOrders()
        {
            if (_sapOrderV2 == null)
                return new List<Order>();
            else
            {
                List<Order> orders = new List<Order>();
                foreach (SAPOrderHeader header in _sapOrderV2.SAPOrderHeader)
                {
                    Order o = new Order();
                    o.StoreID = _storeOrder.StoreID;
                    o.OrderNo = header.SO_NO;
                    o.User = _storeOrder.User;
                    o.UserID = _storeOrder.User.UserID;

                    DateTime orderCreatedDate;
                    Match matchResults = null;
                    try
                    {
                        Regex regexObj = new Regex(@"^(\d{4})(\d{2})(\d{2})$");
                        matchResults = regexObj.Match(header.RecordCreatedDate);
                        if (matchResults.Success)
                        {
                            orderCreatedDate = new DateTime(int.Parse(matchResults.Groups[1].Value), int.Parse(matchResults.Groups[2].Value), int.Parse(matchResults.Groups[3].Value));
                            o.OrderDate = orderCreatedDate;
                        }
                        else
                        {
                            // Match attempt failed
                        }
                    }
                    catch (ArgumentException)
                    {
                        // Syntax error in the regular expression
                    }
                    o.cartX.ShipToContact = new CartContact();
                    o.cartX.SoldToContact = new CartContact();
                    o.cartX.BillToContact = new CartContact();

                    o.TotalDiscount = 0;
                    o.TaxDiscount = 0;
                    o.FreightDiscount = 0;
                    // Update Order's field "UpdateBySAP"
                    o.UpdateBySAP = true;

                    //Remove all cart items in cart
                    o.cartX.removeAllItem();

                    CartContacts shipto = _sapOrderV2.CartContacts.FirstOrDefault(x => x.PARVW == "Ship-to party");
                    if (shipto != null)
                    {
                        o.cartX.ShipToContact.Address1 = shipto.ADDRESS;

                        o.cartX.ShipToContact.City = shipto.CITY;
                        o.cartX.ShipToContact.State = shipto.STATE;
                        o.cartX.ShipToContact.ZipCode = shipto.ZIP_CODE;
                        o.cartX.ShipToContact.Country = shipto.COUNTRY_NAME;
                        o.cartX.ShipToContact.AttCompanyName = shipto.NAME1;
                        o.cartX.ShipToContact.TelNo = shipto.TEL_NO;
                    }


                    CartContacts soldto = _sapOrderV2.CartContacts.FirstOrDefault(x => x.PARVW == "Sold-to party");
                    if (soldto != null)
                    {
                        o.cartX.SoldToContact.Address1 = soldto.ADDRESS;
                        o.cartX.SoldToContact.City = soldto.CITY;
                        o.cartX.SoldToContact.State = soldto.STATE;
                        o.cartX.SoldToContact.ZipCode = soldto.ZIP_CODE;
                        o.cartX.SoldToContact.Country = soldto.COUNTRY_NAME;
                        o.cartX.SoldToContact.AttCompanyName = soldto.NAME1;
                        o.cartX.SoldToContact.TelNo = soldto.TEL_NO;
                    }

                    CartContacts billto = _sapOrderV2.CartContacts.FirstOrDefault(x => x.PARVW == "Bill-to party");
                    if (billto != null)
                    {
                        o.cartX.BillToContact.Address1 = billto.ADDRESS;

                        o.cartX.BillToContact.City = billto.CITY;
                        o.cartX.BillToContact.State = billto.STATE;
                        o.cartX.BillToContact.ZipCode = billto.ZIP_CODE;
                        o.cartX.BillToContact.Country = billto.COUNTRY_NAME;
                        o.cartX.BillToContact.AttCompanyName = billto.NAME1;
                        o.cartX.BillToContact.TelNo = billto.TEL_NO;
                    }

                    //freight and tax

                    if (_sapOrderV2.Conditions.Count > 0)
                    {
                        try
                        {
                            decimal sapTax = _sapOrderV2.Conditions.Where(x => x.Cond_Type == "JR2").Sum(x => decimal.Parse(x.Condvalue));
                            if (sapTax > o.Tax)
                                o.Tax = sapTax;
                        }
                        catch (Exception)
                        {


                        }

                        try
                        {
                            decimal sapFright = _sapOrderV2.Conditions.Where(x => x.Cond_Type == "ZHD0").Sum(x => decimal.Parse(x.Condvalue));
                            if (sapFright > o.Freight)
                                o.Freight = sapFright;
                        }
                        catch (Exception)
                        {
                        }
                    }
                    o.cartX.TotalAmount = header.Subtotal;
                    o.currencySign = header.Currency;
                    orders.Add(o);
                }
                return orders.OrderByDescending(o => o.OrderDate).ToList();
            }
        }
        #endregion

        #region SAP Tracking Order Class(inner class)
        /// <summary>
        ///  SAPTrackingOrder is middle interface between SAP XML format and eStore order format
        /// </summary>
        [XmlRoot(ElementName = "SAPTrackingOrder")]
        public class SAPTrackingOrder
        {
            [XmlArray(ElementName = "SAPOrderItems"), XmlArrayItem("SAPOrderItemDetail")]
            public List<SAPOrderItemDetail> SAPOrderItems
            { get; set; }

            [XmlElement(ElementName = "SAPOrderHeader")]
            public SAPOrderHeader SAPOrderHeader
            { get; set; }

            [XmlArray(ElementName = "SAPOrderScheduleLines"), XmlArrayItem("SAPOrderScheduleLine")]
            public List<SAPOrderScheduleLine> SAPOrderSchedules
            { get; set; }
            [XmlArray(ElementName = "SAPOrderCartContacts"), XmlArrayItem("CartContacts")]
            public List<CartContacts> CartContacts
            { get; set; }
            [XmlArray(ElementName = "SAPOrderConditions"), XmlArrayItem("Conditions")]
            public List<Conditions> Conditions
            { get; set; }

            public List<SAPOrderItemDetail> getTopLevelItems()
            {
                if (SAPOrderItems != null)
                {
                    return (from top in SAPOrderItems
                            where top.HigherLevel==0
                            select top
                                ).ToList();
                }
                else
                    return new List<SAPOrderItemDetail>();
            
            }
            public List<SAPOrderItemDetail> getSubLevelItems(SAPOrderItemDetail toplevelitem)
            {
                if (SAPOrderItems != null)
                {

                    return (from sub in SAPOrderItems
                            where sub.HigherLevel == toplevelitem.LineNO && sub.SO_NO == toplevelitem.SO_NO
                            select sub
                                ).ToList();
                }
                else
                    return new List<SAPOrderItemDetail>();

            }
        }

        [XmlRoot(ElementName = "SAPTrackingOrder")]
        public class SAPTrackingOrderV2
        {
            [XmlArray(ElementName = "SAPOrderItems"), XmlArrayItem("SAPOrderItemDetail")]
            public List<SAPOrderItemDetail> SAPOrderItems
            { get; set; }

            [XmlElement(ElementName = "SAPOrderHeader")]
            public List<SAPOrderHeader> SAPOrderHeader
            { get; set; }

            [XmlArray(ElementName = "SAPOrderScheduleLines"), XmlArrayItem("SAPOrderScheduleLine")]
            public List<SAPOrderScheduleLine> SAPOrderSchedules
            { get; set; }
            [XmlArray(ElementName = "SAPOrderCartContacts"), XmlArrayItem("CartContacts")]
            public List<CartContacts> CartContacts
            { get; set; }
            [XmlArray(ElementName = "SAPOrderConditions"), XmlArrayItem("Conditions")]
            public List<Conditions> Conditions
            { get; set; }

            public List<SAPOrderItemDetail> getTopLevelItems()
            {
                if (SAPOrderItems != null)
                {
                    return (from top in SAPOrderItems
                            where top.HigherLevel == 0
                            select top
                                ).ToList();
                }
                else
                    return new List<SAPOrderItemDetail>();

            }
            public List<SAPOrderItemDetail> getSubLevelItems(SAPOrderItemDetail toplevelitem)
            {
                if (SAPOrderItems != null)
                {

                    return (from sub in SAPOrderItems
                            where sub.HigherLevel == toplevelitem.LineNO && sub.SO_NO == toplevelitem.SO_NO
                            select sub
                                ).ToList();
                }
                else
                    return new List<SAPOrderItemDetail>();

            }
        }

        public class SAPOrderItemDetail
        {
            public string Status
            { get; set; }

            public string SO_NO
            { get; set; }

            public int LineNO
            { get; set; }

            //no used for new methods
            ////This property is not returned from SAP. It is used to identify BTOS components group.
            //public int LineGrp
            //{ get; set; }

            //public bool FirstLineNO
            //{ get; set; }

            //public bool LastLineNO
            //{ get; set; }

            public string PartNO
            { get; set; }

            public string PartDesc
            { get; set; }

            //This property is used to identified eA or eP
            public string PartNoType
            { get; set; }

            public int HigherLevel
            { get; set; }

            public string ItemType
            { get; set; }

            public string Plant
            { get; set; }

            public string StoreId
            { get; set; }

            public decimal ListPrice
            { get; set; }

            public string MeasureUnit
            { get; set; }

            public string ExchangeRate
            { get; set; }

            public decimal Subtotal
            { get; set; }

            public string StorageLocation
            { get; set; }

            public string ProcessStatus
            { get; set; }

            public string ShippingNo
            { get; set; }

            public string ShippingItem
            { get; set; }

            public string ShippingDate
            { get; set; }

            public decimal ShippingQty
            { get; set; }

            public string GoodsMovementDate
            { get; set; }

            public string TrackingNo
            { get; set; }

            public string BillingDocument
            { get; set; }

            public string BillingItem
            { get; set; }

            public string BillingDate
            { get; set; }

            public decimal InvoicedQty
            { get; set; }

            public string NetValueInDocCurrency
            { get; set; }

            public string TransferToAccountStatus
            { get; set; }

            public string ControllingAreaStatus
            { get; set; }

            public decimal OrderTotalAmount
            { get; set; }

            public decimal OrderQty
            { get; set; }

            public string ShipmentBlockingReason
            { get; set; }

            public string ShippingMethod
            { get; set; }

            public decimal ActualShippingQty
            { get; set; }

            public decimal SAPListPrice
            { get; set; }

            public decimal TotalAmount
            { get; set; }

            public string CostInDocCurrency
            { get; set; }

            //Constructor
            public SAPOrderItemDetail()
            { }
        }

        public class SAPOrderHeader
        {
            public string SO_NO
            { get; set; }

            public string OrderType
            { get; set; }

            public string CustomerNo
            { get; set; }

            //Ship To, not sold to
            public string ShipToCity
            { get; set; }

            public string ShipToZipcode
            { get; set; }

            public string ShipToStreet1
            { get; set; }

            public string ShipToStreet2
            { get; set; }

            public string ShipToCountry
            { get; set; }

            public string CustomerName
            { get; set; }

            public string SalesOrg
            { get; set; }

            public string StoreId
            { get; set; }

            public string DistributionChannel
            { get; set; }

            public string Division
            { get; set; }

            public string Salesgroup
            { get; set; }

            public string ProfitCenter
            { get; set; }

            // Billing block in SD document
            public string BillingBlockInSdDoc
            { get; set; }

            //Delivery block (document header)
            public string DeliveryBlock
            { get; set; }

            public string DocDate
            { get; set; }

            public string PO_NO
            { get; set; }

            public string Currency
            { get; set; }

            public decimal Subtotal
            { get; set; }

            //Order reason (reason for the business transaction)
            public string OrderReason
            { get; set; }

            public string RecordCreatedDate
            { get; set; }

            public string EntryTime
            { get; set; }

            public string CreatedBy
            { get; set; }

            public decimal ExchangeRate
            { get; set; }

            //Shipment Blocking Reason
            public string ShipmentBlockingReason
            { get; set; }


            //Constructor
            //public SAPCustomerSalesInfo()
            //{ }
        }

        public class SAPOrderScheduleLine
        {
            public string SO_NO
            { get; set; }

            public int LineNO
            { get; set; }

            public decimal ScheduleLine
            { get; set; }

            public string OrderQtyInSalesUnits
            { get; set; }

            public decimal ConfirmedQty
            { get; set; }

            public string ScheduleLineDate
            { get; set; }

            //Constructor
            //public SAPOrderSchedule()
            //{ }
        }

        public class CartContacts
        {
            public string VBELN
            { get; set; }

            public string KUNNR
            { get; set; }

            public string PARVW
            { get; set; }

            public string NAME1
            { get; set; }

            public string ADDRESS
            { get; set; }

            public string CITY
            { get; set; }

            public string STATE
            { get; set; }

            public string ZIP_CODE
            { get; set; }

            public string COUNTRY_NAME
            { get; set; }

            public string TEL_NO
            { get; set; }
        }

        public class Conditions
        {
            public string Sd_Doc
            { get; set; }

            public string Itm_Number
            { get; set; }

            public string Cond_Type
            { get; set; }

            public string Currency
            { get; set; }

            public string Condvalue
            { get; set; }
        }
        //public class SAPOrderBtos
        //{
        //    private SAPOrderItemDetail _btosBTO;
        //    public SAPOrderItemDetail BTOSBTO
        //    {
        //        get { return _btosBTO; }
        //        set { _btosBTO = value; }
        //    }

        //    private List<SAPOrderItemDetail> _btosConfigItems;
        //    public List<SAPOrderItemDetail> BTOSConfigItems
        //    {
        //        get { return _btosConfigItems; }
        //        set { _btosConfigItems = value; }
        //    }

        //    public SAPOrderBtos()
        //    { }
        //}

        #endregion

    }
}
