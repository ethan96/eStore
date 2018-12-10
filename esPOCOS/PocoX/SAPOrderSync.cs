using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.OMBusiness.SAPOrderSync

namespace eStore.POCOS.SAP
{
    public class SAPOrderSync {
      public   Order order;
      public   SAP

        public SAPOrderSync() {
            order = new Order();
        }

        
    
    }

    public class Order {

        private string _PartialShipment;

        public readonly string Order_Type = "ZOR";

        //US01
        public string Sales_Organization; 

        //10, 30(KA)
        public string Distribution_Channel;

        //10, 20 customer division
        public string Division;

        //2700, 2100, 2200, 2300
        public string Sales_Office;

        //272, 274, 276, 278
        public string Sales_Group;

        //USH1
        public string Delivery_Plant;

        //09 ??
        public string Ship_Condition;

        //FB1
        public string Inco_Term1;
        public string Inco_Term2;
        public string Credit_Status;
        public string Order_Number;
        public string Customer_ID;
        public string Ship_To_ID;
        public DateTime Order_Date;
        public DateTime Require_Date;
        public string Ship_Term;
        public string Credit_Term;
        public string Remarks;
        public string PCF_Code;
        public string Comments;

        // own Carrier - account, shipping method
        public string Sales_Note;
        public string External_Note;
        public string Op_Note;

        //VE Sales person
        public string Sale_Person_ID1;

        //ZM Sales assistant/coordinator
        public string Sale_Person_ID2;

        //Z2
        public string Sale_Person_ID3;

        public string Order_Currency;
        public string Customer_PO_Number;
        public string TO_Site;

        //Only allow if non-shipme
        public string Partial_Shipment;                            
        public string Early_Ship;
        public string FOB_Point;
        

        public List<Order_Line> orderlines;    
    }

   public  class Order_Line {
        private int _lineno;
        private DateTime _lineRequiredDate;

        public string Sales_District;
        public string Order_Number;
        public string Item_Category;
        public int Higher_Level;
        public int Storage_Location;
        
        public int Line {

        set{
            _lineno = value ;
        }
        
         get{
            return _lineno;
            }
        }


        public int Line_Seq;
        public string Item_Number;
        public int Qty;
        public decimal Unit_Price;

        public DateTime Line_Require_Date {
            set { 
                //Need to add avoid Holidays
                if (DateTime.Now.Hour>=11)
                    _lineRequiredDate = _lineRequiredDate.AddDays(1);
                }

            get {
            
                return _lineRequiredDate ;
            }
        
        }
        public DateTime Line_Due_Date;
        public DateTime Request_Date;
        public string Line_Delivery_Plant;
    

    }
}
