using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace eStore.POCOS.PocoX
{
    [Serializable]
    public class XRule
    {
        public XRule() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="para">cart or part</param>
        /// <returns></returns>
        public virtual bool isApplicable(object para)
        { return true; }

        /// <summary>
        /// return fixed value discount
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        public virtual decimal getDiscount(object para)
        {
            return 0m;
        }
        public string serialize()
        {
            StringWriter stringWriter = new StringWriter();
            XmlSerializer xmlSerializer = new XmlSerializer(this.GetType());
            xmlSerializer.Serialize(stringWriter, this);
            string serializedXML = stringWriter.ToString();
            return serializedXML;
        }
        public XRule deserialize(string xml)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(this.GetType());
                StringReader rd = new StringReader(xml);
                object xrule = xmlSerializer.Deserialize(rd);
                if (xrule is XRule)
                    return (XRule)xrule;
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public class Condition {
            public Condition() { }
            public Condition(string productid, int qty)
            {
                ProductID = productid;
                Qty = qty;
            }
            public string ProductID { get; set; }
            public int Qty { get; set; }
        }


    }
    [Serializable]
    public class XRule4Gift : XRule
    {
        public XRule4Gift()
            : base()
        {

        }
        /// <summary>
        /// check ctos/bundle detial  or not
        /// </summary>
        public bool? CheckGiftInDetails { get; set; }
        /// <summary>
        /// if true then such as per 5 get 1 else only get on per cart
        /// </summary>
        /// 
        
        public bool? PerUnit { get; set; }
        public List<Condition> Gift { get; set; }
        private Dictionary<string, int> gift;
        private Dictionary<string, int> GiftDict {
            get {
                if (gift == null)
                {
                    if (Gift == null || Gift.Count == 0)
                        gift= new Dictionary<string, int>();
                    else
                    {
                        gift = Gift.ToDictionary(x => x.ProductID, y => y.Qty);
                    }
                }
                return gift;
            }
        }

        public List<Condition> Criteria { get; set; }
        private Dictionary<string, int> _CriteriaDict;
        private Dictionary<string, int> CriteriaDict {
            get
            {
                if (_CriteriaDict == null)
                {
                    if (Criteria == null || Criteria.Count == 0)
                        _CriteriaDict = new Dictionary<string, int>();
                    else
                    {
                        _CriteriaDict = Criteria.ToDictionary(x => x.ProductID, y => y.Qty);
                    }
                }
                return _CriteriaDict;
            }
        }
        public override bool isApplicable(object para)
        {
            if (para is Cart)
                return isApplicable((Cart)para);
            else
                return false; 
        }
        private bool isApplicable(POCOS.Cart cart)
        {
            //needn't check if doesn't set Criteria
            if (CriteriaDict == null || CriteriaDict.Count == 0)
                return true;

            int matchedcnt=0;
            foreach (var c in CriteriaDict)
            {
                if (cart.cartItemsX.Any(x => x.Qty >= c.Value && x.SProductID == c.Key))
                {
                    matchedcnt++;
                }
            }
            return matchedcnt == CriteriaDict.Count;
        }


 

        public void addGift2Cart(CampaignStrategy strategy,ref POCOS.Cart cart)
        {
            //do nothing  if not applicable
            if (isApplicable(cart) == false)
                return;

            //do nothing  if not Gift setting
            if (GiftDict == null || GiftDict.Count == 0)
                return;
            //duplicate gift setting
            Dictionary<string, int> estoreGift = new Dictionary<string, int>();
            int multiplier = 1;
            if (this.PerUnit.GetValueOrDefault() && CriteriaDict!=null &&   CriteriaDict.Count>0)
            {
                multiplier=(from ci in cart.cartItemsX
                 from c in CriteriaDict
                 where ci.Qty >= c.Value && ci.SProductID == c.Key
                 select (int)ci.Qty / c.Value).Min();
            }

            foreach (var g in GiftDict)
                estoreGift.Add(g.Key, g.Value * multiplier);

            #region check and set discount for gift already in cart
            foreach (CartItem item in cart.cartItemsX)
            {
                if (estoreGift.ContainsKey(item.SProductID) && estoreGift[item.SProductID] > 0 && item.PromotionStrategy==null)
                {
                    item.PromotionStrategy = strategy.ID;
                    item.PromotionMessage = strategy.PromoteDesc;


                    if (estoreGift[item.SProductID] <= item.Qty)
                    {
                        item.DiscountAmount = item.partX.getListingPrice().value * estoreGift[item.SProductID];
                        estoreGift[item.SProductID] = 0;
                    }
                    else//still have gifts left
                    {
                        item.DiscountAmount = item.partX.getListingPrice().value * item.Qty;
                        estoreGift[item.SProductID] -= item.Qty;
                    }
                }
                else if (CheckGiftInDetails.GetValueOrDefault() && item.PromotionStrategy == null && Criteria.Any(x=>x.ProductID==item.SProductID))
                {
                    Dictionary<Part, int> parts = new Dictionary<Part, int>();
                    if (item.partX is Product_Ctos)
                    {
                        Product_Ctos ctos = (Product_Ctos)item.partX;
                        if (item.btosX != null && item.btosX.parts.Any())
                        {
                            parts = item.btosX.parts.ToDictionary(x => x.Key, y => y.Value * item.Qty);
                 
                        }
                    
                    }
                    else if (item.partX is Product_Bundle)
                    { }


                    if (parts.Any())
                    {
                        decimal totaldiscount = 0m;

                        foreach (var p in parts)
                        {
                            if (estoreGift.ContainsKey(p.Key.SProductID) && estoreGift[p.Key.SProductID] > 0)
                            {
                                if (estoreGift[p.Key.SProductID] <= p.Value)
                                {
                                    totaldiscount += p.Key.getListingPrice().value * estoreGift[p.Key.SProductID];
                                    estoreGift[p.Key.SProductID] = 0;
                                }
                                else//still have gifts left
                                {
                                    totaldiscount+= p.Key.getListingPrice().value * p.Value;
                                    estoreGift[p.Key.SProductID] -= p.Value;
                                }
                            }
                        }
                        if (totaldiscount > 0)
                        {
                            item.PromotionStrategy = strategy.ID;
                            item.PromotionMessage = strategy.PromoteDesc;
                            item.DiscountAmount = totaldiscount;
                        }
                    }
                
                }

            }
            #endregion

            #region auto add to cart if still have gift left
            foreach (var g in estoreGift.Where(x => x.Value > 0))
            {
                Part part = (new POCOS.DAL.PartHelper()).getPart(g.Key, cart.storeX);
                if (part == null)
                    continue;
                CartItem newitem = cart.addItem(part, g.Value);
                newitem.PromotionStrategy = strategy.ID;
                newitem.PromotionMessage = strategy.PromoteDesc;
                newitem.DiscountAmount = newitem.AdjustedPrice;
            }
            #endregion
        }
    }


    public class XRuleChecker : XRule
    {
        public int XRuleSetId { get; set; }
         [XmlIgnoreAttribute]
        public List<CartItem> Qualified = new List<CartItem>();
        public override bool isApplicable(object para)
        {
            if (XRuleSetId != null && XRuleSetId > 0)
            {
                XRuleSet rs = (new POCOS.DAL.XRuleSetHelper()).getXRuleSet(XRuleSetId);
                if (rs == null)
                    return false;
                else
                {
                    XRuleMatcher matcher = new XRuleMatcher(rs, ((POCOS.Cart)para).cartItemsX);
                    if (matcher.isMatched)
                        Qualified = matcher.getQualified();
                    return matcher.isMatched;
                }
            }
            else

            return base.isApplicable(para);
        }
    }
    [Serializable]
    public class XRule4UserRestriction : XRule
    {
        public XRule4UserRestriction() : base() { }
        public override bool isApplicable(object para)
        {
            if (para is User)
                return isApplicable((User)para);
            else
                return false;
        }
        private bool isApplicable(User user)
        {
            return false;
        }
    }
}


