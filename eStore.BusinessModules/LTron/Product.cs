using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.BusinessModules.LTron
{
    [Serializable]
    public class Product
    {
        public string productcode { get; set; }
        public string productdisplayname { get; set; }
        //public string vendor_partno { get; set; }
        public string productmanufacturer { get; set; }
       
        public decimal productprice { get; set; }
        //public string taxableproduct { get; set; }
        //public string productcondition { get; set; }
        public string productdescription { get; set; }
        public string productdescriptionshort { get; set; }
 
        public string extinfo { get; set; }
        //public string productdescription_abovepricing { get; set; }
        public int stockstatus { get; set; }
        public string availability { get; set; }
        public string google_availability { get { return availability; } }
        public decimal productweight { get; set; }
     
        public decimal length { get; set; }
        public decimal width { get; set; }
        public decimal height { get; set; }
     
        public List<string> accessories { get; set; }
        public List<string> replacement { get; set; }
        public ProductCategory ProductCategory { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
        public List<Image> Images { get; set; }
        public string DataSheet { get; set; }
        public List<Filter> Filters { get; set; }
        [Serializable]
        public class Image
        {
            public string URL { get; set; }
            public bool IsMainImage { get; set; }
        }
           [Serializable]
        public class Filter
        {
            public string CategoryName { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
        }

    }

    [Serializable]
    public class ProductInfo
    {
        public string ProductNo { get; set; }
        public string URL { get; set; }
        public string Currency { get; set; }
        public decimal Price { get; set; }
    }
}
