using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS
{
    /// <summary>
    /// This is a BTOSConfig business extension file
    /// </summary>
    public partial class BTOSConfig
    {
#region Buisness Logic Extension
        static private int noneCTOSComponentId = 999999;
        static private String noneCTOSComponentDesc = "SBC BTO Item";
        private int _warrantyType = -1; //-1 mean not initilized yet, 0 mean not a warranty config, 1 mean a warranty config

        /// <summary>
        /// Default empty constructor -- this is a must needed constructor for entity framework
        /// </summary>
        public BTOSConfig()
        {
        }

        /// <summary>
        /// When no parameter is provided, it is the default constructor for Entitywork to create instance from loading DB data 
        /// or others to create regular BTO system instance.
        /// When parameter is provided and when it's true, the instance created here is to add none CTOS related parts
        /// </summary>
        public BTOSConfig(Boolean noneCTOS=false, String categoryDesc = "", String optionDesc = "")
        {
            if (noneCTOS)
            {
                CategoryComponentID = noneCTOSComponentId;
                CategoryComponentDesc = String.IsNullOrEmpty(categoryDesc) ? noneCTOSComponentDesc : categoryDesc;
                OptionComponentID = noneCTOSComponentId;
                OptionComponentDesc = String.IsNullOrEmpty(optionDesc) ? noneCTOSComponentDesc : optionDesc;
            }
        }

        public Boolean isNoneCTOS()
        {
            if (CategoryComponentID == noneCTOSComponentId)
                return true;
            else
                return false;
        }

        /// <summary>
        /// This is a readonly property.  It return the netPrice of config item
        /// </summary>
        public Decimal netPrice
        {
            get
            {
                Decimal price = 0m;

                foreach (BTOSConfigDetail item in BTOSConfigDetails)
                    price += (Decimal)item.NetPrice * item.Qty;

                price = Math.Round(price, 2);
                return price;
            }
        }

        /// <summary>
        /// This is a readonly property.  It return the adjustedPrice of config item
        /// </summary>
        public Decimal adjustedPrice
        {
            get
            {
                Decimal price = 0m;
                foreach (BTOSConfigDetail item in BTOSConfigDetails)
                    price += (Decimal)item.AdjustedPrice * item.Qty;

                if (price == 0)
                    price = AdjustedPrice.GetValueOrDefault();  //keep existing value without update

                price = Math.Round(price, 2);
                return price;
            }
        }

        /// <summary>
        /// This method is to duplicate itself to another instance
        /// </summary>
        /// <param name="config"></param>
        public void copyTo(BTOSConfig config)
        {
            config.AdjustedPrice = AdjustedPrice;
            //config.BTOConfigID = BTOConfigID; //different BTOS has different configID, the config ID shall not be cloned
            
            //copy BTOSConfigDetails here
            foreach (BTOSConfigDetail detail in BTOSConfigDetails)
            {
                BTOSConfigDetail newItem = new BTOSConfigDetail();
                newItem.AdjustedPrice = detail.AdjustedPrice;
                newItem.Description = detail.Description;
                newItem.MainProduct = detail.MainProduct;
                newItem.NetPrice = detail.NetPrice;
                newItem.Qty = detail.Qty;
                newItem.Warrantable = detail.Warrantable;
                newItem.SProductID = detail.SProductID;
                config.BTOSConfigDetails.Add(newItem);
            }

            config.CategoryComponentDesc = CategoryComponentDesc;
            config.CategoryComponentID = CategoryComponentID;
            config.OptionComponentDesc = OptionComponentDesc;
            config.OptionComponentID = OptionComponentID;
            config.Price = Price;
            config.Qty = Qty;
        }

        private Dictionary<Part, int> _parts = null;
        /// <summary>
        /// parts is a runtime property.  It's inital value is from outside
        /// </summary>
        public Dictionary<Part, int> parts
        {
            get { return _parts; }
            set { _parts = value; }
        }
        public ATP atp {
            get {
                ATP rlt;
                if (parts == null || parts.Count == 0)
                {
                    rlt = new ATP(DateTime.Now, 1);
                    _isBuildin = true;
                }
                else if (parts.Count == 1)
                {
                    rlt = parts.First().Key.atp;
                }
                else
                {
                    rlt = parts.OrderByDescending(x => x.Key.atp.availableDate).First().Key.atp;
                }
                return rlt;
            
            }
        }
        private bool _isBuildin = false;
        public Boolean isBuildin
        {
            get {

                return _isBuildin;
            }
        }
        


        /// <summary>
        /// This method determine if itself is a warranty config
        /// </summary>
        /// <returns></returns>
        public Boolean isWarrantyConfig()
        {
            if (_warrantyType == -1)    //attribute is not initialized yet
            {
                //initialize attribute
                if (CategoryComponentDesc.ToUpper().Contains("EXTENDED WARRANTY"))
                    _warrantyType = 1;
                else
                    _warrantyType = 0;
            }

            if (_warrantyType == 1)
                return true;
            else
                return false;
        }

        /// <summary>
        /// This method indicates whether this is a "none" selection item or not 
        /// </summary>
        /// <returns></returns>
        public Boolean isNoneItem()
        {
            if (OptionComponentDesc.ToUpper().Equals("NONE"))
                return true;
            else
                return false;
        }

        public CTOSBOM matchedOption
        {
            get;
            set;
        }

#endregion //Business logic extension

        #region OM_Only
        #endregion
    }
}
