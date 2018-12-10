using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.PocoX;

namespace eStore.POCOS
{
    abstract public class PromotionCodeEnabled : FollowUpable
    {
        /// <summary>
        /// This method will calculate promotion total discount and return which coupontype is applied
        /// </summary>
        /// <returns></returns>
        protected DiscountDetail calculatePromotionTotalDiscount(User user, Cart cart, Decimal freight, 
                                                                                Decimal tax, String promotionCode,bool TaxAndFreightOnly=false)
        {
            DiscountDetail result = new DiscountDetail();
            promotionAppliedLogs.Clear();
            CampaignManager manager = CampaignManager.getInstance();

            IList<CampaignStrategy> strategies = manager.getApplicableCampaignStrategies(user, cart, promotionCode);
            //clear cart promote
            if (!TaxAndFreightOnly)
            {
                foreach (CartItem item in cart.CartItems)
                {
                    item.PromotionStrategy = null;
                    item.PromotionMessage = null;
                    item.DiscountAmount = null;
                }
            }
            if (strategies == null || strategies.Count == 0)
                result.codeStatus = CampaignManager.PromotionCodeStatus.NotApplicable;
            else
            {
                PromotionPolicy promotion = null;                
                foreach (CampaignStrategy strategy in strategies)
                {
                    switch (strategy.promotionType)
                    {

                        case CampaignStrategy.CouponType.Freight:
                            promotion = manager.getApplicableFreightPromotionPolicy(strategy, cart.TotalAmount);
                            if (promotion != null  )
                            {
                                if (result.freightDiscountAmount == 0)
                                {
                                    Decimal freightDiscountAmount = calculateDiscount(promotion, freight);
                                    //result.discountAmount += freightDiscountAmount;
                                    result.freightDiscountAmount += freightDiscountAmount;
                                    PromotionAppliedLog log = getPromotionAppliedLog(promotion, cart, null);
                                    if (log != null)
                                    {
                                        log.Discounts = freightDiscountAmount;
                                        this.promotionAppliedLogs.Add(log);
                                    }
                                }
                                else
                                {
                                    //don't add to result.AddAppliedStrategy
                                    promotion = null;
                                }
                            }
                            break;
                        case CampaignStrategy.CouponType.Tax:
                            promotion = manager.getApplicableFreightPromotionPolicy(strategy, cart.TotalAmount);
                            if (promotion != null)
                            {
                                if (result.taxDiscountAmount == 0)
                                {
                                    Decimal taxDiscountAmount = calculateDiscount(promotion, tax);
                                    //result.discountAmount += taxDiscountAmount;
                                    result.taxDiscountAmount += taxDiscountAmount;
                                    PromotionAppliedLog log = getPromotionAppliedLog(promotion, cart, null);
                                    if (log != null)
                                    {
                                        log.Discounts = taxDiscountAmount;
                                        this.promotionAppliedLogs.Add(log);
                                    }
                                }
                                else
                                {
                                    //don't add to result.AddAppliedStrategy
                                    promotion = null;
                                }
                            }
                            break;
                        case CampaignStrategy.CouponType.Cart:

                            if (!TaxAndFreightOnly)
                            {
                                promotion = manager.getApplicableCartPromotionPolicy(strategy, cart);
                                if (promotion != null)
                                {
                                    if (strategy.xrule is PocoX.XRule4Gift && strategy.xrule.isApplicable(cart))
                                    {
                                        ((PocoX.XRule4Gift)strategy.xrule).addGift2Cart(strategy, ref cart);

                                        PromotionAppliedLog log = getPromotionAppliedLog(promotion, cart, null);
                                        if (log != null)
                                        {
                                            log.Discounts = cart.cartItemsX.Sum(x=>x.DiscountAmount);
                                            this.promotionAppliedLogs.Add(log);
                                        }
                                    }
                                    else if (isApplyforEntireCart(strategy))
                                    {
                                        if (result.discountAmount == 0)
                                        {
                                            bool hasMatchedProducts = false;
                                            foreach (CartItem item in cart.CartItems)
                                            {
                                                if (item.partX is Product || item.partX is PStoreProduct)
                                                {
                                                    promotion = manager.getApplicableProductPromotionPolicy(strategy, item.partX, CampaignStrategy.CouponType.Cart);
                                                    if (promotion != null && item.PromotionStrategy == null)
                                                    {
                                                        hasMatchedProducts = true;
                                                        break;
                                                    }
                                                }
                                            }
                                            if (hasMatchedProducts)
                                            {
                                                Decimal cartDiscountAmount = calculateDiscount(promotion, cart.TotalAmount);

                                                if(cart.getCostPrice() > (cart.TotalAmount - cartDiscountAmount))
                                                    result.codeStatus = CampaignManager.PromotionCodeStatus.IsBelowCost;
                                                else
                                                    result.discountAmount += cartDiscountAmount;

                                                PromotionAppliedLog log = getPromotionAppliedLog(promotion, cart, null);
                                                if (log != null)
                                                {
                                                    log.Discounts = cartDiscountAmount;
                                                    this.promotionAppliedLogs.Add(log);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //don't add to result.AddAppliedStrategy
                                            promotion = null;
                                        }
                                    }
                                    else
                                    {
                                        List<CartItem> Qualified = new List<CartItem>();
                                        if (strategy.xrule != null && strategy.xrule is PocoX.XRuleChecker) // 如果xrule 不匹配继续
                                        {
                                            POCOS.Cart carttemp = new Cart();
                                            cart.copyTo(carttemp);
                                            if (!strategy.xrule.isApplicable(carttemp))
                                                continue;
                                            Qualified = ((PocoX.XRuleChecker)strategy.xrule).Qualified;
                                        }
                                        int maxQtyLimited = strategy.MaxQtyPerUser.GetValueOrDefault();
                                        int appliedQty = 0;
                                        foreach (CartItem item in cart.CartItems)
                                        {
                                            if (item.partX is Product || item.partX is PStoreProduct)
                                            {
                                                promotion = manager.getApplicableProductPromotionPolicy(strategy, item.partX, CampaignStrategy.CouponType.Cart);
                                                if (promotion != null && item.PromotionStrategy == null)
                                                {
                                                    //check maxQtyLimited, if has value, then check how much can be applied
                                                    int applyingQty = 0;
                                                    if (Qualified.Any())
                                                    {
                                                        applyingQty = getApplyingQty(Qualified.Where(x => x.OldItemNo > 0 ? x.OldItemNo == item.ItemNo : x.ItemNo == item.ItemNo).Select(x => x.Qty).FirstOrDefault(), maxQtyLimited, ref appliedQty);
                                                    }
                                                    else
                                                        applyingQty = getApplyingQty(item.Qty, maxQtyLimited, ref appliedQty);
                                                    if (applyingQty > 0)
                                                    {
                                                        decimal discountAmount = calculateDiscount(promotion, item.UnitPrice * applyingQty , applyingQty);
                                                        if (item.partX.CostX * applyingQty > (item.UnitPrice * applyingQty - discountAmount))
                                                            result.codeStatus = CampaignManager.PromotionCodeStatus.IsBelowCost;
                                                        else
                                                        {
                                                            //result.discountAmount += calculateDiscount(promotion, item.AdjustedPrice);
                                                            result.applicableProductIds.Add(item.partX.SProductID);

                                                            item.PromotionStrategy = strategy.ID;
                                                            item.PromotionMessage = strategy.PromoteDesc;
                                                            item.DiscountAmount = discountAmount;
                                                        }
                                                        PromotionAppliedLog log = getPromotionAppliedLog(promotion, cart, item);
                                                        if (log != null)
                                                        {
                                                            log.Discounts = item.DiscountAmount;
                                                            this.promotionAppliedLogs.Add(log);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    
                    if (promotion != null)
                    {
                        result.AddAppliedStrategy(strategy);
                    }

                }
                if (result.discountAmount > 0 || result.freightDiscountAmount > 0 || result.taxDiscountAmount > 0 || cart.CartItems.Sum(x => x.DiscountAmount) > 0)
                {
                    if (result.discountAmount > 0)
                        result.discountAmount = Utilities.Converter.CartPriceRound(result.discountAmount, cart.StoreID);
                    if (result.freightDiscountAmount > 0)
                        result.freightDiscountAmount = Utilities.Converter.CartPriceRound(result.freightDiscountAmount, cart.StoreID);
                    if (result.taxDiscountAmount > 0)
                        result.taxDiscountAmount = Utilities.Converter.CartPriceRound(result.taxDiscountAmount, cart.StoreID);

                    result.codeStatus = CampaignManager.PromotionCodeStatus.Valid;
                }
            }

            return result;
        }
        private int getApplyingQty(int qty, int maxQtyLimited, ref int appliedQty)
        {
            int applyingQty = 0;
            if (maxQtyLimited==0)
            {
                applyingQty = qty;
            }
            else
            {
                if (maxQtyLimited  - appliedQty >= qty)
                {
                    applyingQty = qty;
                }
                else
                {
                    applyingQty = maxQtyLimited  - appliedQty;
                }
            }
            appliedQty += applyingQty;
            return applyingQty;
        }
        protected POCOS.Cart getFreeGroupShipmentEligibleCartItems(User user, Cart cart)
        {
            CampaignManager manager = CampaignManager.getInstance();

            IList<CampaignStrategy> strategies = manager.getApplicableCampaignStrategies(user, cart, string.Empty);

            var pStoreProductFreeGroupItems =  cart.cartItemsX.Where(x => x.partX.isEPAPS()&&!string.IsNullOrEmpty(x.partX.InventoryProvider ));
            bool matchedFreeGroupStrategies=   strategies != null && strategies.Count > 0 && strategies.Where(x => x.promotionType == CampaignStrategy.CouponType.FreeGroundShipping).Count()>0;

            if (pStoreProductFreeGroupItems.Any() || matchedFreeGroupStrategies)
            {
                PromotionPolicy promotion = null;
                Cart campaignCart = new Cart();
                cart.copyTo(campaignCart);
                List<CartItem> matchedItems = new List<CartItem>();

                if (pStoreProductFreeGroupItems.Any())
                {
                    foreach (CartItem item in pStoreProductFreeGroupItems)
                    {
                        if ( matchedItems.FirstOrDefault(x => x.ItemNo == item.ItemNo) == null)
                        {
                            item.PromotionMessage = "Free Group Shipment Eligible for domestic";
                            matchedItems.Add(item);
                        }
                    }
                }

                if (matchedFreeGroupStrategies)
                {

                    IList<CampaignStrategy> mutlistrategies = strategies.Where(x => x.promotionType == CampaignStrategy.CouponType.FreeGroundShipping && x.CategoryIDList.Contains("&") == true).ToList();
                    if (mutlistrategies != null && mutlistrategies.Count > 0)
                    {

                        foreach (CampaignStrategy ms in mutlistrategies)
                        {
                            string[] conditions = ms.CategoryIDList.Split('&');
                            int matchcount = 0;
                            List<CartItem> matchedItemstmp = new List<CartItem>();
                            foreach (string c in conditions)
                            {
                                bool match = false;
                                CampaignStrategy tempstrategy = ms.clone();
                                tempstrategy.CategoryIDList = c;
                                foreach (CartItem item in campaignCart.CartItems)
                                {
                                    promotion = manager.getApplicableProductPromotionPolicy(tempstrategy, item, CampaignStrategy.CouponType.FreeGroundShipping);
                                    if (promotion != null && (item.PromotionStrategy.HasValue == false || strategies.Any(x => x.ID == item.PromotionStrategy && x.promotionType == CampaignStrategy.CouponType.FreeGroundShipping)))
                                    {
                                        match = true;
                                        if (matchedItemstmp.Contains(item) == false)
                                        {
                                            item.PromotionMessage = ms.PromoteDesc;
                                            item.PromotionStrategy = ms.ID;
                                            matchedItemstmp.Add(item);
                                        }
                                    }

                                }
                                if (match)
                                {
                                    matchcount++;
                                }
                            }
                            if (matchcount == conditions.Length)
                            {
                                foreach (CartItem mi in matchedItemstmp)
                                    if (matchedItems.FirstOrDefault(x => x.ItemNo == mi.ItemNo) == null)
                                        matchedItems.Add(mi);
                            }

                        }

                    }


                    //normally checking
                    IList<CampaignStrategy> freightstrategies = strategies.Where(x => x.promotionType == CampaignStrategy.CouponType.FreeGroundShipping && x.CategoryIDList.Contains("&") == false).ToList();
                    if (freightstrategies != null && freightstrategies.Count > 0)
                    {

                        foreach (CartItem item in campaignCart.CartItems)
                        {
                            bool match = false;
                            foreach (CampaignStrategy strategy in freightstrategies)
                            {

                                promotion = manager.getApplicableProductPromotionPolicy(strategy, item, CampaignStrategy.CouponType.FreeGroundShipping);
                                if (promotion != null && (item.PromotionStrategy.HasValue == false || strategies.Any(x => x.ID == item.PromotionStrategy && x.promotionType == CampaignStrategy.CouponType.FreeGroundShipping)))
                                {
                                    item.PromotionMessage = strategy.PromoteDesc;
                                    item.PromotionStrategy = strategy.ID;
                                    match = true;
                                    break;
                                }

                            }
                            if (match && matchedItems.FirstOrDefault(x => x.ItemNo == item.ItemNo) == null)
                            {
                                matchedItems.Add(item);
                            }
                        }
                    }
                }
               
                if (matchedItems.Count == 0)
                {
                    return null;
                }
                else
                {
                    campaignCart.CartItems.Clear();
                    campaignCart.CartItems = matchedItems;
                    campaignCart.reconcile();
                    return campaignCart;
                }
            }

            return null;
        }

        private bool isApplyforEntireCart(CampaignStrategy strategy)
        {
            if ((strategy.BusinessGroup.ToUpper().Equals("BOTH") || strategy.BusinessGroup.ToUpper().Equals("ALL"))
                //&& strategy.ItemType.ToUpper() == "ALL"
                && (strategy.ProductType.ToUpper() == "ALL" || strategy.ProductType.ToUpper() == "BOTH")
                && strategy.DiscountValueType == "FixValue"
                )
            {
                return true;
            }
            else

                return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        private Decimal calculateDiscount(PromotionPolicy policy, Decimal amount , int? applyingQty = null)
        {
            Decimal discount = 0m;

            if (policy.discountValueType == PromotionPolicy.DiscountValueType.FixValue)
            {
                if (applyingQty != null)
                    discount = (policy.discountPrice * Convert.ToInt32(applyingQty) > amount) ? amount : policy.discountPrice * Convert.ToInt32(applyingQty);
                else
                    discount = (policy.discountPrice > amount) ? amount : policy.discountPrice;
            }
            else
            {
                discount = amount * (1.0m - policy.discountPrice);
                discount = (discount > amount) ? amount : discount;
            }

            return discount;
        }

        private List<PromotionAppliedLog> _promotionAppliedLogs;
        public List<PromotionAppliedLog> promotionAppliedLogs
        {
            get {
                if (_promotionAppliedLogs == null)
                    _promotionAppliedLogs = new List<PromotionAppliedLog>();
                return _promotionAppliedLogs;
            }
        }

        /// <summary>
        /// this function will save PromotionAppliedLog and pandding to  CampaignStrategy's logs
        /// only used when order confirmed
        /// </summary>
        /// <returns></returns>
        public bool savePromotionAppliedLogs(string couponCode)
        {
            try
            {
                foreach (PromotionAppliedLog log in this.promotionAppliedLogs)
                {
                    if (log.save() == 0)
                    {
                        CampaignManager.getInstance().addPromotionAppliedLogs(log, couponCode);
                    }
                }
            }
            catch (Exception ex)
            {

                eStore.Utilities.eStoreLoger.Error("exception at save PromotionAppliedLogs", "", "", "", ex);
            }
            return true;
        }

        private PromotionAppliedLog getPromotionAppliedLog(PromotionPolicy policy, POCOS.Cart cart, POCOS.CartItem cartItem)
        {
            if (policy == null)
                return null;
            
            PromotionAppliedLog log = new PromotionAppliedLog();
            //log.PromotionCode = policy.strategyID.ToString();
            log.CampaignID = policy.campaignID;
            log.PromotionCode = policy.PromotionCode;
            log.CampaignStrategyID = policy.strategyID;
            log.Status = "Confirmed";
            if (this is POCOS.Order)
            {
                log.Type = "Order";
                log.QuoteOrderNo = ((POCOS.Order)this).OrderNo;
            }
            else if (this is POCOS.Quotation)
            {
                log.Type = "Quotation";
                log.QuoteOrderNo = ((POCOS.Quotation)this).QuotationNumber;
            }
            else if (this is POCOS.Cart)
            {
                log.Type = "Cart";
            }
            else
            {
                log.Type = "Other";
            }


            log.UserID = cart.UserID;
            log.CartID = cart.CartID;
            log.StoreID = cart.StoreID;
            //log.CartID = this.OrderNo;
           
            //compose product id list
            if (cartItem != null)
            {
                log.ModelNO = cartItem.SProductID;
                log.Qty = cartItem.Qty;
            }
            else
            {

                log.ModelNO = string.Empty;
                log.Qty = 1;
            }
            log.UpdatedDate = DateTime.Now;
            log.CreatedDate = DateTime.Now;

            return log;
        }
    }

    public class DiscountDetail
    {
        private List<CampaignStrategy> strategies = null;
        public CampaignManager.PromotionCodeStatus codeStatus = CampaignManager.PromotionCodeStatus.Invalid;
        public Decimal discountAmount = 0m;
        public Decimal freightDiscountAmount = 0m;
        public Decimal taxDiscountAmount = 0m;
        public List<String> applicableProductIds = new List<String>();
        public void AddAppliedStrategy(CampaignStrategy strategy)
        {
            if (strategies == null)
                strategies = new List<CampaignStrategy>();
            if (!strategies.Contains(strategy))
                strategies.Add(strategy);
        }
        public List<CampaignStrategy>  AppliedStrategies {
            get {
                return strategies;
            }
        }
    }
}
