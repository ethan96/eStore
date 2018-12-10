using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.POCOS
{
    public partial class Aperza
    {
        #region Product identifiers
        //Required
        public string manufacturer_brand_id { get; set; } //Defined by Aperza
        //Required
        public string model_number { get; set; } //SproductID
        public string product_sku { get; set; } //Unique identifier
        public string model_gtin { get; set; } //Global trade item number (GTIN) Example. 3234567890126
        #endregion

        #region Price and availability
        public decimal price { get; set; } //Price
        public string lot_price { get; set; } //1:100|10:99.8|100:98.9
        public string currency_code { get; set; } //Currency
        public string stock_status { get; set; } //Stock status
        public string stock_num { get; set; } //Stock qty
        public string delivery_status { get; set; } //Ex. 1: Same day, 2: Within 3 business days, 3: Within 5 business days....exc
        #endregion
        
        //Conditional required
        public string lang_code { get; set; } //Language code EX. ja
        
        #region promotion description
        public string note { get; set; } //EX. Payment fee for cancelation on your customer
        public string pr_comment { get; set; } //EX. Buy 10 get 1 free to get discount
        #endregion

        #region For your conversions
        public string offer_page_url { get; set; } //Product landing page
        public string rfq_flag { get; set; } //RFQ EX. 1: get RFQ, 0: Not get RFQ
        public string agency_type { get; set; } //EX. 1:Special agent, 2: Distributor
        #endregion

        #region Product category
        public string product_category_id1 { get; set; } //L1 category
        public string product_category_id2 { get; set; } //L2 category
        public string product_category_id3 { get; set; } //L3 category
        #endregion

        #region Basic product data
        public string model_name { get; set; } //Display part No.
        public string model_description { get; set; } //Product description
        public string model_page_url { get; set; } //EX. https://buy.advantech.co.jp/%E3%83%AA%E3%83%A2%E3%83%BC%E3%83%88-I-O-%E3%83%A2%E3%82%B8%E3%83%A5%E3%83%BC%E3%83%AB/%E3%82%A4%E3%83%BC%E3%82%B5%E3%83%8D%E3%83%83%E3%83%88-I-O%E3%83%A2%E3%82%B8%E3%83%A5%E3%83%BC%E3%83%AB-%E3%82%A2%E3%83%8A%E3%83%AD%E3%82%B0-I-O%E3%83%A2%E3%82%B8%E3%83%A5%E3%83%BC%E3%83%AB/model-ADAM-6017-CE.htm
        public string model_image_url { get; set; } //EX. https://advdownload.blob.core.windows.net/productfile/Downloadfile/1-313KTT/ADAM-6017_S.jpg
        #endregion
    }
}
