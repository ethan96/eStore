using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.Utilities;
using eStore.POCOS;

namespace eStore.BusinessModules.TaxService
{
    public class BRRate : FixRate
    {
        private POCOS.Cart _cart;
        private Store _store;
        protected override void calculateTax(POCOS.Cart cart, Store store)
        {

            decimal totaltax = 0;
            _cart = cart;
            _store = store;
            try
            {

                foreach (POCOS.CartItem cartitem in cart.CartItems)
                {
                    switch (cartitem.type)
                    {
                        case POCOS.Product.PRODUCTTYPE.STANDARD:
                            totaltax += cartitem.AdjustedPrice * getRate(cartitem.SProductID);

                            break;
                        case POCOS.Product.PRODUCTTYPE.CTOS:
                            foreach (var config in cartitem.btosX.BTOSConfigs)
                            {
                                foreach (var detail in config.BTOSConfigDetails)
                                {
                                    totaltax += cartitem.Qty * config.Qty * detail.Qty * detail.AdjustedPrice.Value * getRate(detail.SProductID);
                                }

                            }
                            break;
                        //case POCOS.Product.PRODUCTTYPE.BUNDLE:
                        //    foreach (var bundleItem in cartitem.bundleX.BundleItems)
                        //    {
                        //        totaltax += cartitem.Qty * bundleItem.Qty.Value * bundleItem.AdjustedPrice.Value * getSAPRateByPN(cartitem.SProductID, cart, store);

                        //    }
                        //    break;

                    }

                }

                this.Amount = totaltax;
                this.Rate = totaltax / cart.TotalAmount * 100;
                this.Status = true;
            }

            catch (Exception ex)
            {

                this.Amount = 0;
                this.Rate = 0;
                this.Status = false;
                this.ErrorCode = "Failed to get tax rate";
                eStoreLoger.Error("Failed to get ABR tax rate", cart.UserID, "", cart.StoreID, ex);
            }

        }

        private Dictionary<Part, int> getParts(POCOS.Cart cart)
        {

            Dictionary<Part, int> partItems = new Dictionary<Part, int>();
            int qty = 0;

            foreach (POCOS.CartItem cartitem in cart.CartItems)
            {
                switch (cartitem.type)
                {
                    case POCOS.Product.PRODUCTTYPE.STANDARD:
                        qty = cartitem.Qty;

                        if (partItems.ContainsKey(cartitem.partX))
                            partItems[cartitem.partX] = partItems[cartitem.partX] + qty;
                        else
                            partItems.Add(cartitem.partX, qty);
                        break;
                    case POCOS.Product.PRODUCTTYPE.CTOS:
                        foreach (var partItem in cartitem.btosX.parts)
                        {
                            qty = partItem.Value;

                            if (partItems.ContainsKey(partItem.Key))
                                partItems[partItem.Key] = partItems[partItem.Key] + qty;
                            else
                                partItems.Add(partItem.Key, qty);
                        }
                        break;
                    //case POCOS.Product.PRODUCTTYPE.BUNDLE:
                    //    foreach (var partItem in cartitem.bundleX.parts)
                    //    {
                    //        qty = partItem.Value;

                    //        if (partItems.ContainsKey(partItem.Key))
                    //            partItems[partItem.Key] = partItems[partItem.Key] + qty;
                    //        else
                    //            partItems.Add(partItem.Key, qty);
                    //    }
                    //    break;

                }


            }
            return partItems;
        }
        private decimal? _fixedRate;
        private decimal getFixedRate(POCOS.Cart cart, Store store)
        {
            if (_fixedRate.HasValue == false)
            {
                FixRate provider = new FixRate(); //可以考虑直接继承 FixRate 然后使用base.calculateTax 这样可以直接删除 GetcalculateTax function
                provider.GetcalculateTax(cart, store);
                _fixedRate = provider.Rate / 100;
            }
            return _fixedRate.Value;
        }
        List<POCOS.Sync.MySAPDALPriceAdapter.MySAPDALResult> result;
        private decimal getRate(string productID)
        {
            if (result == null)
            {
                POCOS.Sync.MySAPDALPriceAdapter adapter = new POCOS.Sync.MySAPDALPriceAdapter();
                result = adapter.getSAPPrice(_store.profile, getParts(_cart));

            }
            if (result != null)
            {
                POCOS.Sync.MySAPDALPriceAdapter.MySAPDALResult matched = result.FirstOrDefault(x => x.ProductID.Equals(productID, StringComparison.OrdinalIgnoreCase));
                if (matched != null && matched.TaxRate > 0)
                    return matched.TaxRate;
                else
                    return getFixedRate(_cart, _store);
            }

            else
                return getFixedRate(_cart, _store);

        }
    }
}
