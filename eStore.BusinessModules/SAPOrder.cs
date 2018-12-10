using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.BusinessModules.SAPOrderWebService;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Data;
using System.Collections;
using eStore.POCOS;
using eStore.Utilities;
using esUtilities;

namespace eStore.BusinessModules
{
    public class SAPOrder
    {
        private string _storeId = "";

        private string _orderNo;
        public string OrderNo
        {
            get { return _orderNo; }
        }

        private User _user;
        public User User
        {
            get { return _user; }
        }

        private DataSet _dsOrderDetail;
        public DataSet DSOrderDetail
        {
            get { return _dsOrderDetail; }
        }
             

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="orderNo"></param>
        public SAPOrder(string storeId,string orderNo, User user)
        {
            _storeId = storeId;
            _orderNo = orderNo;
            _user = user;
            getOrderDatail();
        }

        public SAPOrder(string storeId, string orderNo)
        {
            _storeId = storeId;
            _orderNo = orderNo;
            getOrderDatail();
        }

        /// <summary>
        /// This function is used to get the DataSet of order detail via SAP web service. If success, then set property _dsOrderDetail.
        /// </summary>
        private void getOrderDatail()
        {
            try
            {
                // If SAP order web service is available, then set dataset to property.
                OrderWebService ws = new OrderWebService();
                ws.Timeout = 10000;
                if (ws.getOrderDetail(_storeId, _orderNo, "").Tables.Count == 3 && ws.getOrderDetail(_storeId, _orderNo, "").HasErrors == false)
                    _dsOrderDetail = ws.getOrderDetail(_storeId, _orderNo, "");
            }
            catch (SoapException soapex)
            {
                eStoreLoger.Error("Can't get the SAP order detail via SAP web service.", "", "", _storeId, soapex);
                throw soapex;
            }

            catch (Exception ex)
            {
                eStoreLoger.Error("Can't get the SAP order detail via SAP web service.", "", "", _storeId, ex);
                throw ex;
            }
        }


        /// <summary>
        ///  This function is used to get the order detail via SAP web service
        /// </summary>
        /// <returns>SAPOrderResponse</returns>
        public SAPOrderResponse getSAPOrderResponse()
        {
            SAPOrderResponse response = new SAPOrderResponse();

            // Get OrderInfo
            SAPOrderInfo orderInfo;
            DataRow drOrderInfo = _dsOrderDetail.Tables["OrderInfo"].Rows[0];
            orderInfo = new SAPOrderInfo(drOrderInfo["OrderNo"].ToString(), 
                                                                           drOrderInfo["ERP_ID"].ToString(), 
                                                                           drOrderInfo["PO_NO"].ToString(), 
                                                                           drOrderInfo["Order_Note"].ToString(), 
                                                                           drOrderInfo["Ship_VIA"].ToString(), 
                                                                           drOrderInfo["Shipment_Term"].ToString(), 
                                                                           drOrderInfo["Due_Date"].ToString() );
            response.OrderInfo = orderInfo;

            // Get OrderAddressInfo
            SAPAddressInfo addressInfo;
            DataRow drAddressInfo = _dsOrderDetail.Tables["AddressInfo"].Rows[0];
            addressInfo = new SAPAddressInfo(drAddressInfo["AddressType"].ToString(), drAddressInfo["Value"].ToString());
            response.AddressInfo = addressInfo;

            // Get OrderDetail
            List<SAPOrderDetail> orderDetails = new List<SAPOrderDetail>();
            foreach (DataRow dr in _dsOrderDetail.Tables["OrderDetail"].Rows)
            {
                SAPOrderDetail  orderDetail;
                orderDetail = new SAPOrderDetail(dr["ORDER_NO"].ToString(),
                                                                                      System.Convert.ToInt16(dr["LINE_NO"]),
                                                                                      dr["PRODUCT_LINE"].ToString(),
                                                                                      dr["PART_NO"].ToString(),
                                                                                      dr["ORDER_LINE_TYPE"].ToString(),
                                                                                      System.Convert.ToInt16(dr["QTY"]),
                                                                                      System.Convert.ToDecimal(dr["LIST_PRICE"]),
                                                                                      System.Convert.ToDecimal(dr["UNIT_PRICE"]),
                                                                                      System.Convert.ToDateTime(dr["REQUIRED_DATE"]),
                                                                                      System.Convert.ToDateTime(dr["DUE_DATE"]),
                                                                                      dr["ERP_SITE"].ToString(),
                                                                                      dr["ERP_LOCATION"].ToString(),
                                                                                      dr["AUTO_ORDER_FLAG"].ToString(),
                                                                                      System.Convert.ToInt16(dr["AUTO_ORDER_QTY"]),
                                                                                      System.Convert.ToInt16(dr["PARENT_LINE_NO"]),
                                                                                      System.Convert.ToDateTime(dr["Supplier_due_date"]),
                                                                                      System.Convert.ToDouble(dr["Subtotal"]));
                orderDetails.Add(orderDetail);
            }
            response.OrderDetail = orderDetails;

            return response;
        }
        
    }

    // This class represents reponse from SAP Order web service
    public class SAPOrderResponse
    {
        private List<SAPOrderDetail> _orderDetail;
        public List<SAPOrderDetail> OrderDetail
        {
            get { return _orderDetail; }
            set { _orderDetail = value; }
        }

        private SAPAddressInfo _addressInfo;
        public SAPAddressInfo AddressInfo
        {
            get { return _addressInfo; }
            set { _addressInfo = value; }
        }

        private SAPOrderInfo _orderInfo;
        public SAPOrderInfo OrderInfo
        {
            get { return _orderInfo; }
            set { _orderInfo = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SAPOrderResponse()
        { }
    }

    public class SAPOrderDetail
    {
        #region Properties
        /// <summary>
        /// Properties
        /// </summary>
        private string _orderNo;
        public string OrderNo
        { get { return _orderNo; } }

        private int _lineNo;
        public int LineNo
        { get { return _lineNo; } }

        private string _productLine;
        public string ProductLine
        { get { return _productLine; } }

        private string _partNo;
        public string PartNo
        { get { return _partNo; } }

        private string _orderLineType;
        public string OrderLineType
        { get { return _orderLineType; } }

        private int _qty;
        public int Qty
        { get { return _qty; } }

        private decimal _listPrice;
        public decimal ListPrice
        { get { return _listPrice; } }

        private decimal _unitPrice;
        public decimal UnitPrice
        { get { return _unitPrice; } }

        private DateTime _requiredDate;
        public DateTime RequiredDate
        { get { return _requiredDate; } }

        private DateTime _dueDate;
        public DateTime DueDate
        { get { return _dueDate; } }

        private string _erpSite;
        public string ERPsite
        { get { return _erpSite; } }

        private string _erpLocation;
        public string ERPLocation
        { get { return _erpLocation; } }

        private string _autoOrderFlag;
        public string AutoOrderFlag
        { get { return _autoOrderFlag; } }

        private int _autoOrderQty;
        public int AutoOrderQty
        { get { return _autoOrderQty; } }

        private int _parentLineNo;
        public int ParentLineNo
        { get { return _parentLineNo; } }

        private DateTime _supplierDueDate;
        public DateTime SupplierDueDate
        { get { return _supplierDueDate; } }

        private double _subtotal;
        public double Subtotal
        { get { return _subtotal; } }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public SAPOrderDetail(string orderNo, 
                                                      int lineNo, 
                                                      string productLine, 
                                                      string partNo, 
                                                      string orderLineType, 
                                                      int qty, 
                                                      decimal listPrice, 
                                                      decimal unitPrice, 
                                                      DateTime requiredDate, 
                                                      DateTime dueDate, 
                                                      string erpSite, 
                                                      string erpLocation, 
                                                      string autoOrderFlag, 
                                                      int autoOrderQty, 
                                                      int parentLineNo, 
                                                      DateTime supplierDueDate, 
                                                      double subtotal)
        {
            _orderNo = orderNo;
            _lineNo = lineNo;
            _productLine = productLine;
            _partNo = partNo;
            _orderLineType = orderLineType;
            _qty = qty;
            _listPrice = listPrice;
            _unitPrice = unitPrice;
            _requiredDate = requiredDate;
            _dueDate = dueDate;
            _erpSite = erpSite;
            _erpLocation = erpLocation;
            _autoOrderFlag = autoOrderFlag;
            _autoOrderQty = autoOrderQty;
            _parentLineNo = parentLineNo;
            _supplierDueDate = supplierDueDate;
            _subtotal = subtotal;
        }
    }

    public class SAPAddressInfo
    {
        private string _addressType;
        public string AddressType
        {
            get { return _addressType; }
        }

        private string _value;
        public string Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SAPAddressInfo(string addressType, string value)
        {
            _addressType = addressType;
            _value = value;
        }
    }

    public class SAPOrderInfo
    {
        private string _orderNo;
        public string OrderNo
        {
            get { return _orderNo; }
        }

        private string _erpId;
        public string ERPID
        {
            get { return _erpId; }
        }

        private string _poNo;
        public string PONO
        {
            get { return _poNo; }
        }

        private string _orderNote;
        public string OrderNote
        {
            get { return _orderNo; }
        }

        private string _shipVia;
        public string ShipgVia
        {
            get { return _shipVia; }
        }

        private string _shipmentTerm;
        public string ShipmentTerm
        {
            get { return _shipmentTerm; }
        }

        private string _dueDate;
        public string DueDate
        {
            get { return _dueDate; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SAPOrderInfo(string order_no, string erp_id, string po_no, string order_note, string ship_via, string shipment_term, string due_date)
        {
            _orderNo = order_no;
            _erpId = erp_id;
            _poNo = po_no;
            _orderNote = order_no;
            _shipVia = ship_via;
            _shipmentTerm = shipment_term;
            _dueDate = due_date;
        }
    }
}
