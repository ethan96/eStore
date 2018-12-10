using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace eStore.POCOS
{
    public partial class CampaignStrategy
    {
        //runtime property
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }

        public enum CouponType { Product, Cart, Freight, FreeGroundShipping, Tax, NotApplicable };

        public CouponType promotionType
        {
            get
            {
                CouponType type = CouponType.Freight;   //default
                switch (DiscountType.ToUpper())
                {
                    case "PRODUCTS":
                        type = CouponType.Product;
                        break;
                    case "CART":
                        type = CouponType.Cart;
                        break;
                    case "FREIGHT":
                        type = CouponType.Freight;
                        break;
                    case "FREEGROUNDSHIPPING":
                        type = CouponType.FreeGroundShipping;
                        break;
                    case "TAX":
                        type = CouponType.Tax;
                        break;
                    default:
                        type = CouponType.NotApplicable;
                        break;
                }

                return type;
            }
        }

        public Boolean isDeleted
        {
            get
            {
                if (!String.IsNullOrEmpty(Status) && Status.ToUpper().Equals("DELETED"))
                    return true;
                else
                    return false;
            }
        }
        private String _mutex = "MUTEX";

        private HashSet<string> _applicableProductIDS;
        public HashSet<string> applicableProductIDS
        {
            get
            {
                lock (_mutex)
                {
                    if (_applicableProductIDS == null)
                    {
                        _applicableProductIDS = new HashSet<string>();
                        if (this.ItemType.ToUpper() == "CATEGORY")
                        {
                            List<String> products = new List<string>();
                            if (this.ProductType.ToUpper() == "EPAPS")
                            {
                                try
                                {
                                    POCOS.Store store = (new DAL.StoreHelper().getStorebyStoreid(this.StoreID));
                                    foreach (int id in (this.CategoryIDList.Split('|').Select(x => int.Parse(x))))
                                    {
                                        PStoreProductCategory pc = (new DAL.PStoreProductCategoryHelper()).get(store,id);
                                        if (pc != null)
                                        {
                                            products.AddRange(pc.productList.Select(x => x.SProductID));
                                        }
                                    }
                                    products = products.Distinct().ToList();
                                }
                                catch (Exception ex)
                                { 
                                
                                }
                            }
                            else
                            {
                                products = (new DAL.ProductCategoryHelper()).getAllProductsInHierarchicalCategories(this.StoreID, this.CategoryIDList);
                            }
                                    foreach (string p in products)
                                _applicableProductIDS.Add(p);
                        }
                    }
                }
                return _applicableProductIDS;
            }
        }

        private PocoX.XRule _xrule;
        public PocoX.XRule xrule
        {
            get
            {
                if (_xrule == null)
                {
                    if (!string.IsNullOrEmpty(this.XRule))
                    {
                        try
                        {
                             System.Xml.XmlDocument xdoc = new System.Xml.XmlDocument();
                            xdoc.LoadXml(this.XRule);
                            string xruletype = "eStore.POCOS.PocoX." + ((xdoc.FirstChild).NextSibling).Name;
                             Assembly assembly = Assembly.GetAssembly(typeof(PocoX.XRule));
                             _xrule = (PocoX.XRule)assembly.CreateInstance(xruletype);
                             reloadXRule();
                        }
                        catch (Exception)
                        {

                            _xrule = null;
                        }
                        
                    }
                }
                return _xrule;
            }
            set {
                _xrule = value;
                uploadXRule();
            }
        }

        public void reloadXRule()
        {
            if (_xrule != null && string.IsNullOrEmpty(XRule) == false)
            {
                _xrule = _xrule.deserialize(XRule);

            }
        }
        public void uploadXRule()
        {
            if (_xrule != null)
            {
                XRule = _xrule.serialize();
            }
        }


        public CampaignStrategy clone()
        {
            CampaignStrategy newStrategy = new CampaignStrategy();

            newStrategy.StoreID = this.StoreID;
            newStrategy.PromotionCode = this.PromotionCode;
            newStrategy.CampaignID = this.CampaignID;
            newStrategy.PromoteDesc = this.PromoteDesc;
            newStrategy.ProductType = this.ProductType;
            newStrategy.BusinessGroup = this.BusinessGroup;
            newStrategy.CategoryIDList = this.CategoryIDList;
            newStrategy.ExceptionPNList = this.ExceptionPNList;
            newStrategy.UserIDList = this.UserIDList;
            newStrategy.CustomerType = this.CustomerType;
            newStrategy.DiscountValue = this.DiscountValue;
            newStrategy.ItemType = this.ItemType;
            newStrategy.MinQTY = this.MinQTY;
            newStrategy.MinAmount = this.MinAmount;
            newStrategy.ExclusiveFlag = this.ExclusiveFlag;
            newStrategy.Priority = this.Priority;
            newStrategy.DiscountType = this.DiscountType;
            newStrategy.BenefitText = this.BenefitText;
            newStrategy.MaxQtyPerUser = this.MaxQtyPerUser;
            newStrategy.MaxTotalAppliedTimes = this.MaxTotalAppliedTimes;
            newStrategy.TotalAppliedTimes = this.TotalAppliedTimes;
            newStrategy.CreatedDATE = this.CreatedDATE;
            newStrategy.CreatedBy = this.CreatedBy;
            newStrategy.BenefitText2 = this.BenefitText2;
            newStrategy.MininumVolume = this.MininumVolume;
            newStrategy.MaxApplicableTimesPerUser = this.MaxApplicableTimesPerUser;
            newStrategy.ExceptionType = this.ExceptionType;
            newStrategy.Status = this.Status;
            newStrategy.MatchCriteria = this.MatchCriteria;
            newStrategy.DiscountValueType = this.DiscountValueType;
            newStrategy.XRule = this.XRule;
            return newStrategy;

        }

        /// <summary>
        /// 
        /// </summary>
        private List<PromotionAppliedLog> _promotionAppliedLogs;
        public List<PromotionAppliedLog> promotionAppliedLogs
        {
            get
            {
                POCOS.DAL.PromotionAppliedLogHelper helper = new DAL.PromotionAppliedLogHelper();
                if (this.Campaign.PromotionType == "System")
                    _promotionAppliedLogs = helper.getPromotionAppliedLogsbyStrategyID(this.StoreID, this.ID, this.PromotionCode);
                else
                    _promotionAppliedLogs = helper.getPromotionAppliedLogsbyStrategyID(this.StoreID, this.ID);
                if (_promotionAppliedLogs == null)
                    _promotionAppliedLogs = new List<PromotionAppliedLog>();
                return _promotionAppliedLogs;
            }
        }

        public int getTotalAppliedTimes()
        {
            return promotionAppliedLogs.Count;
        }

        public int getTotalAppliedTimes(string userid)
        {
            return promotionAppliedLogs.Count(x => x.UserID.Equals(userid, StringComparison.OrdinalIgnoreCase));
        }
        public int getTotalAppliedQty()
        {
            return promotionAppliedLogs.Sum(x => x.Qty).GetValueOrDefault();
        }
        public int getTotalAppliedQty(string userid)
        {
            return promotionAppliedLogs.Where(x => x.UserID.Equals(userid, StringComparison.OrdinalIgnoreCase)).Sum(x => x.Qty).GetValueOrDefault();
        }
    }
}
