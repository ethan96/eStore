using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using eStore.Utilities;

 

namespace eStore.Proxy
{
 public class SAPProxy
    {
       public static int webserviceTimeout = 10000;
       public static string servicePartPrefix = "AGS";

        public List<ProductAvailability> getMultiATP(string SAPDeliveryPlant, System.DateTime requestDate, Hashtable partnos)
        {

            if (partnos == null || partnos.Count == 0)
                return new List<ProductAvailability>();

            string xmlInput = null;
            string xmlOut = "";
            string xmlLog = "";
            Advantech.SAP.WS_eCommerce oObj = new Advantech.SAP.WS_eCommerce();
            List<ProductAvailability> lProductAv = new List<ProductAvailability>();

            XmlDocument oRequestXmlDoc = new XmlDocument();
            //Dim oRootNode, oNewNode As XmlNode
            XmlElement oElementRoot = default(XmlElement);
            XmlElement oElementNode = default(XmlElement);
            oElementRoot = oRequestXmlDoc.CreateElement("insert");
            oRequestXmlDoc.AppendChild(oElementRoot);

            //<rs:insert> <z:row WERK="EUH1" MATNR="ADAM-3016-AE" REQ_QTY="99999" REQ_DATE="2008-04-22T00:00:00" UNI="PC"/> </rs:insert> </rs:data> </xml> 

            foreach (string partno in partnos.Keys )
            {

                if (partno.ToLower().StartsWith(servicePartPrefix) == false)
                {
                
                    XmlAttribute oAttriWERK = default(XmlAttribute);
                    XmlAttribute oAttriMATNR = default(XmlAttribute);
                    XmlAttribute oAttriREQ_QTY = default(XmlAttribute);
                    XmlAttribute oAttriREQ_DATE = default(XmlAttribute);
                    XmlAttribute oAttriUNI = default(XmlAttribute);
                    oElementNode = oRequestXmlDoc.CreateElement("row");
                    oAttriWERK = oRequestXmlDoc.CreateAttribute("WERK");
                    oAttriMATNR = oRequestXmlDoc.CreateAttribute("MATNR");
                    oAttriREQ_QTY = oRequestXmlDoc.CreateAttribute("REQ_QTY");
                    oAttriREQ_DATE = oRequestXmlDoc.CreateAttribute("REQ_DATE");
                    oAttriUNI = oRequestXmlDoc.CreateAttribute("UNI");

                    oAttriWERK.Value = SAPDeliveryPlant;
                    oElementNode.Attributes.Append(oAttriWERK);

                    int qty = (int) partnos[partno];

                    System.Text.RegularExpressions.Regex isNumeric = new System.Text.RegularExpressions.Regex(@"^\d+$");

                    if (isNumeric.IsMatch(partno))
                    {
                        string newPartNo = partno;
                        if (partno.Length > 18)
                            newPartNo = partno.Substring(0, 18);
                        newPartNo = newPartNo.PadLeft(18, '0');
                        oAttriMATNR.Value = newPartNo.ToUpper();
                    }
                    else
                    {
                        oAttriMATNR.Value = partno.Trim().ToUpper();
                    }

                    oElementNode.Attributes.Append(oAttriMATNR);
                    oAttriREQ_QTY.Value =  qty.ToString();
                    oElementNode.Attributes.Append(oAttriREQ_QTY); 
                   // oAttriREQ_DATE.Value = string.Format("yyyy-MM-ddThh:mm:ss", DateTime.Now);
                    oAttriREQ_DATE.Value = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss");
                    oElementNode.Attributes.Append(oAttriREQ_DATE);
                    oAttriUNI.Value = "PC";
                    oElementNode.Attributes.Append(oAttriUNI);
                    oElementRoot.AppendChild(oElementNode);
                }
            }
            oRequestXmlDoc.AppendChild(oElementRoot);
            xmlInput = oRequestXmlDoc.InnerXml;

            try
            {

            oObj.Timeout = webserviceTimeout;

            oObj.GetMultiATP(xmlInput, out xmlOut,  out xmlLog);

            DataSet ds = new DataSet();
       
            System.IO.StringReader sr = new System.IO.StringReader(xmlOut);
            
                 ds.ReadXml(sr);                
               
                foreach (DataRow orow in ds.Tables["row"].Rows)
                {
                    ProductAvailability pa = new ProductAvailability();  
                    pa.PartNO= orow["part"].ToString();
                    pa.RequestDate = DateTime.Parse(orow["date"].ToString());
                    pa.RequestQty = int.Parse(orow["qty_req"].ToString());
                    pa.Flag = orow["flag"].ToString();                    
                    pa.Type = orow["type"].ToString();
                    pa.QtyATP  = int.Parse(orow["qty_atp"].ToString());
                    pa.QtyFulfill= int.Parse( orow["Qty_Fulfill"].ToString());
                    pa.QtyATB = int.Parse(orow["Qty_ATB"].ToString());
                    lProductAv.Add(pa);
                }
                 
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message + ":-->" + xmlInput + ":-->" + xmlOut, "", "", "", ex);
                lProductAv.ForEach(x => x.Message = "Exception at Part ATP");
            }

            return lProductAv;
        }

        /// <summary>
        /// Return latest availability date as string
        /// </summary>
        /// <param name="PartNO"></param>
        /// <param name="qtyQtyATP"></param>
        /// <param name="productAV"></param>
        /// <returns></returns>

        public string getAvailability(string PartNO, ref int qtyQtyATP , List<ProductAvailability> productAV)
        {
            if (string.IsNullOrEmpty(PartNO) | productAV == null)
            {
                return null;
            }
 
              if (PartNO.ToUpper().StartsWith("AGS") || PartNO.ToUpper().StartsWith("OPTION"))
               {
                   qtyQtyATP = 99;
                   return DateTime.Now.ToString();
              }

            var _part = (from a in productAV.Where(x => x.PartNO == PartNO && x.QtyFulfill > 0 && x.Type != "SUPPLY_LT")
                         orderby a.RequestDate descending
                         select a).FirstOrDefault();                            
                        
           if (_part !=null) {

                if (!string.IsNullOrEmpty(_part.Message))
                {
                    qtyQtyATP = 0;
                    return DateTime.MaxValue.ToString();
                }
                else
                {
                    qtyQtyATP = _part.QtyATB;
                    return _part.RequestDate.ToString();
                }
           }               
            else
            {
                qtyQtyATP = 0;
              return  DateTime.MaxValue.ToString();
            }
        }

        /// <summary>
        /// get part ATP from WS
        /// </summary>
        /// <param name="PartNO"></param>
        /// <param name="productAV"></param>
        /// <returns></returns>
        public ProductAvailability GetAvailability(string PartNO, List<ProductAvailability> productAV)
        {
            if (string.IsNullOrEmpty(PartNO) | productAV == null)
            {
                return new ProductAvailability { QtyATP = 1, RequestDate = DateTime.Now, Message = "Error GetAvailability has no PartNo" };
            }

            if (PartNO.ToUpper().StartsWith("AGS") || PartNO.ToUpper().StartsWith("OPTION"))
            {
                return new ProductAvailability { QtyATP = 1, RequestDate = DateTime.Now };
            }

            var _part = (from a in productAV.Where(x => x.PartNO == PartNO && x.QtyFulfill > 0 && x.Type != "SUPPLY_LT")
                         orderby a.RequestDate descending
                         select a).FirstOrDefault();

            if (_part != null)
            {
                return _part;
            }
            else
            {
                return new ProductAvailability { QtyATP = 0, RequestDate = DateTime.MaxValue };
            }
        }

        public decimal getProductPriceFromSap(string partno, string PriceSAPOrg, string PriceSAPLvlL1)
        {
                                    
            int iRet = 0;
            AdvantechSAPPrice.eBizAEU_WS wsprice = new AdvantechSAPPrice.eBizAEU_WS();


            string xmlin = null;
            string xmlout = null;
            string strPartNOList = null;
            string[] arrPartNO = null;

            strPartNOList = partno;
            arrPartNO = strPartNOList.Split(';');
            xmlin = "<NewDataSet>";

            for (int i = 0; i <= arrPartNO.Length - 1; i++)
            {
                xmlin = xmlin + "<Table1>" + "<Matnr>" + arrPartNO[i] + "</Matnr>" + " <Mglme>1</Mglme>" + "</Table1>";
            }

            xmlin = xmlin + "</NewDataSet>";
            try
            {

                iRet = wsprice.eBizAEU_GetMultiPrice("168", PriceSAPOrg, PriceSAPLvlL1, xmlin, out xmlout);

                DataSet oDsWrap = new DataSet();
                System.IO.StringReader sr = new System.IO.StringReader(xmlout);
                oDsWrap.ReadXml(sr);


                if (oDsWrap.Tables[0].Rows.Count > 0)
                {
                    decimal price = decimal.Parse(oDsWrap.Tables[0].Rows[0]["Netwr"].ToString());

                    return price;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                eStore.Utilities.eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return 0;
            }
        }


        
    }
}
