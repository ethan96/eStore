using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace eStore.Presentation
{
    public class GoogleGAHelper
    {
        #region 2017/02/23 Alex, enhance ecommerce testing - push datalayer

        public static StringBuilder MeasureProductImpressions(List<POCOS.Product> products)
        {

            StringBuilder MeasureProductImpressions = new StringBuilder();
            int position = 1;
            MeasureProductImpressions.AppendLine("window.dataLayer = window.dataLayer || []");

            MeasureProductImpressions.AppendLine("dataLayer.push ({");
            MeasureProductImpressions.AppendLine("'ecommerce': {");
            MeasureProductImpressions.AppendFormat("'currencyCode': '{0}',", getStoreCurrencyCodeForGA());
            MeasureProductImpressions.AppendLine("'impressions': [ ");

            foreach (POCOS.Product product in products)
            {

                MeasureProductImpressions.AppendLine("{");
                MeasureProductImpressions.AppendFormat("'name': '{0}',", product.name);
                MeasureProductImpressions.AppendFormat("'id': '{0}',", product.SProductID);
                MeasureProductImpressions.AppendLine("'list': 'ProductCategoryPage',");
                MeasureProductImpressions.AppendFormat("'category': '{0}',", product.productType.ToString());
                //MeasureProductImpressions.AppendFormat("'price': {0},", Presentation.Product.ProductPrice.getPrice(product));
                //MeasureProductImpressions.AppendFormat("'position': {0}", position);
                MeasureProductImpressions.Append("},");
                position++;
            }

            MeasureProductImpressions.Length--;
            MeasureProductImpressions.Append("]}});");

            return MeasureProductImpressions;
            
        }

        public static StringBuilder MeasureProductDetail(POCOS.Product product)
        {


            StringBuilder MeasureProductDetail = new StringBuilder();
            MeasureProductDetail.AppendLine("window.dataLayer = window.dataLayer || []");
            MeasureProductDetail.AppendLine("dataLayer.push ({");
            MeasureProductDetail.AppendLine("'event': 'viewProductDetail',");
            MeasureProductDetail.AppendLine("'ecommerce': {");
            MeasureProductDetail.AppendFormat("'currencyCode': '{0}',", getStoreCurrencyCodeForGA());
            MeasureProductDetail.AppendLine("'detail': { ");
            //MeasureProductDetail.AppendFormat("'actionField': {'list': '{0}'}, ", list);
            MeasureProductDetail.AppendLine("'products': [ ");

            MeasureProductDetail.AppendLine("{");
            MeasureProductDetail.AppendFormat("'name': '{0}',", product.name);
            MeasureProductDetail.AppendFormat("'id': '{0}',", product.SProductID);
            MeasureProductDetail.AppendFormat("'category': '{0}',", product.productType.ToString());
            //MeasureProductDetail.AppendFormat("'price': {0},", Presentation.Product.ProductPrice.getPrice(product));
            MeasureProductDetail.Append("}");

            MeasureProductDetail.Append("]}}});");

            return MeasureProductDetail;
        }

        public static StringBuilder MeasureProductClicks(POCOS.Product product, string list)
        {

            StringBuilder MeasureProductClicks = new StringBuilder();
            int position = 1;
            MeasureProductClicks.AppendLine("window.dataLayer = window.dataLayer || []");

            MeasureProductClicks.AppendLine("dataLayer.push ({");

            MeasureProductClicks.AppendLine("'event': 'productClick',");
            MeasureProductClicks.AppendLine("'ecommerce': {");
            MeasureProductClicks.AppendFormat("'currencyCode': '{0}',", getStoreCurrencyCodeForGA());
            MeasureProductClicks.AppendLine("'click': { ");
            MeasureProductClicks.AppendFormat("'actionField': {'list': '{0}', 'action':'click'}, ", list);
            MeasureProductClicks.AppendLine("'products': [ ");

            MeasureProductClicks.AppendLine("{");
            MeasureProductClicks.AppendFormat("'name': '{0}',", product.name);
            MeasureProductClicks.AppendFormat("'id': '{0}',", product.SProductID);
            MeasureProductClicks.AppendFormat("'category': '{0}',", product.productType.ToString());
            MeasureProductClicks.AppendFormat("'price': {0},", Presentation.Product.ProductPrice.getPrice(product));
            MeasureProductClicks.AppendFormat("'position': {0}", position);
            MeasureProductClicks.Append("}");

            MeasureProductClicks.Append("]}}});");

            return MeasureProductClicks;

        }

        public static StringBuilder MeasureAddToCart(POCOS.Product product)
        {

            StringBuilder MeasureAddToCart = new StringBuilder();

            MeasureAddToCart.AppendLine("window.dataLayer = window.dataLayer || []");

            MeasureAddToCart.AppendLine("dataLayer.push ({");
            MeasureAddToCart.AppendLine("'event': 'addToCart',");
            MeasureAddToCart.AppendLine("'ecommerce': {");
            MeasureAddToCart.AppendFormat("'currencyCode': '{0}',", getStoreCurrencyCodeForGA());
            MeasureAddToCart.AppendLine("'add': { ");
            MeasureAddToCart.AppendLine("'products': [ ");

            MeasureAddToCart.AppendLine("{");
            MeasureAddToCart.AppendFormat("'name': '{0}',", product.name);
            MeasureAddToCart.AppendFormat("'id': '{0}',", product.SProductID);
            MeasureAddToCart.AppendFormat("'category': '{0}',", product.productType.ToString());
            //MeasureAddToCart.AppendFormat("'price': {0},", Presentation.Product.ProductPrice.getPrice(product));
            MeasureAddToCart.Append("}");

            MeasureAddToCart.Append("]}}});");

            return MeasureAddToCart;
        }

        public static StringBuilder MeasureAddToCart2(POCOS.CartItem ci)
        {

            StringBuilder MeasureAddToCart = new StringBuilder();

            MeasureAddToCart.AppendLine("window.dataLayer = window.dataLayer || []");

            MeasureAddToCart.AppendLine("dataLayer.push ({");
            MeasureAddToCart.AppendLine("'event': 'addToCart',");
            MeasureAddToCart.AppendLine("'ecommerce': {");
            MeasureAddToCart.AppendFormat("'currencyCode': '{0}',", getStoreCurrencyCodeForGA());
            MeasureAddToCart.AppendLine("'add': { ");
            MeasureAddToCart.AppendLine("'products': [ ");

            if (ci.BTOSystem != null)
            {
                string productname = ci.ProductName;
                if (ci.BTOSystem.isSBCBTOS())
                {
                    productname = ci.ProductName + (from bc in ci.BTOSystem.BTOSConfigs
                                                    from bd in bc.BTOSConfigDetails
                                                    select bd.SProductID
                                                        ).FirstOrDefault();
                }

                MeasureAddToCart.AppendLine("{");
                MeasureAddToCart.AppendFormat("'name': '{0}',", ci.ProductName);
                MeasureAddToCart.AppendFormat("'id': '{1}({0})',", ci.SProductID, productname);
                MeasureAddToCart.AppendFormat("'category': '{0}',", ci.type.ToString());
                MeasureAddToCart.AppendFormat("'price': {0},", ci.UnitPrice.ToString());
                MeasureAddToCart.AppendFormat("'quantity': {0},", ci.Qty);
                MeasureAddToCart.Append("}");
                MeasureAddToCart.Append("]}}});");


            }
            else if (ci.bundleX != null)
            {
                foreach (POCOS.BundleItem bi in ci.bundleX.BundleItems)
                {
                    MeasureAddToCart.AppendLine("{");
                    MeasureAddToCart.AppendFormat("'name': '{0}',", bi.ItemSProductID);
                    MeasureAddToCart.AppendFormat("'id': '{0}',", bi.ItemSProductID);
                    MeasureAddToCart.AppendFormat("'category': '{0}',", ci.type.ToString());
                    MeasureAddToCart.AppendFormat("'price': {0},", bi.adjustedPrice.ToString());
                    MeasureAddToCart.AppendFormat("'quantity': {0},", ci.Qty * bi.Qty);
                    MeasureAddToCart.Append("}");
                    MeasureAddToCart.Append("]}}});");

                }
            }
            else
            {
                MeasureAddToCart.AppendLine("{");
                MeasureAddToCart.AppendFormat("'name': '{0}',", ci.ProductName);
                MeasureAddToCart.AppendFormat("'id': '{0}',", ci.SProductID);
                MeasureAddToCart.AppendFormat("'category': '{0}',", ci.type.ToString());
                MeasureAddToCart.AppendFormat("'price': {0},", ci.UnitPrice.ToString());
                MeasureAddToCart.AppendFormat("'quantity': {0},", ci.Qty);
                MeasureAddToCart.Append("}");
                MeasureAddToCart.Append("]}}});");
            }

            return MeasureAddToCart;
        }


        public static StringBuilder MeasureRemoveCart(POCOS.CartItem ci)
        {

            StringBuilder MeasureRemoveCart = new StringBuilder();

            MeasureRemoveCart.AppendLine("window.dataLayer = window.dataLayer || []");
            MeasureRemoveCart.AppendLine("dataLayer.push ({");
            MeasureRemoveCart.AppendLine("'event': 'removeFromCart',");
            MeasureRemoveCart.AppendLine("'ecommerce': {");
            MeasureRemoveCart.AppendFormat("'currencyCode': '{0}',", getStoreCurrencyCodeForGA());
            MeasureRemoveCart.AppendLine("'remove': { ");
            MeasureRemoveCart.AppendLine("'products': [ ");

            if (ci.BTOSystem != null)
            {
                string productname = ci.ProductName;
                if (ci.BTOSystem.isSBCBTOS())
                {
                    productname = ci.ProductName + (from bc in ci.BTOSystem.BTOSConfigs
                                                    from bd in bc.BTOSConfigDetails
                                                    select bd.SProductID
                                                        ).FirstOrDefault();
                }

                MeasureRemoveCart.AppendLine("{");
                MeasureRemoveCart.AppendFormat("'name': '{0}',", ci.ProductName);
                MeasureRemoveCart.AppendFormat("'id': '{1}({0})',", ci.SProductID, productname);
                MeasureRemoveCart.AppendFormat("'category': '{0}',", ci.type.ToString());
                MeasureRemoveCart.AppendFormat("'price': {0},", ci.UnitPrice.ToString());
                MeasureRemoveCart.AppendFormat("'quantity': {0},", ci.Qty);
                MeasureRemoveCart.Append("}");
                MeasureRemoveCart.Append("]}}});");


            }
            else if (ci.bundleX != null)
            {
                foreach (POCOS.BundleItem bi in ci.bundleX.BundleItems)
                {
                    MeasureRemoveCart.AppendLine("{");
                    MeasureRemoveCart.AppendFormat("'name': '{0}',", bi.ItemSProductID);
                    MeasureRemoveCart.AppendFormat("'id': '{0}',", bi.ItemSProductID);
                    MeasureRemoveCart.AppendFormat("'category': '{0}',", ci.type.ToString());
                    MeasureRemoveCart.AppendFormat("'price': {0},", bi.adjustedPrice.ToString());
                    MeasureRemoveCart.AppendFormat("'quantity': {0},", ci.Qty * bi.Qty);
                    MeasureRemoveCart.Append("}");
                    MeasureRemoveCart.Append("]}}});");

                }
            }
            else
            {
                MeasureRemoveCart.AppendLine("{");
                MeasureRemoveCart.AppendFormat("'name': '{0}',", ci.ProductName);
                MeasureRemoveCart.AppendFormat("'id': '{0}',", ci.SProductID);
                MeasureRemoveCart.AppendFormat("'category': '{0}',", ci.type.ToString());
                MeasureRemoveCart.AppendFormat("'price': {0},", ci.UnitPrice.ToString());
                MeasureRemoveCart.AppendFormat("'quantity': {0},", ci.Qty);
                MeasureRemoveCart.Append("}");
                MeasureRemoveCart.Append("]}}});");
            }            

            return MeasureRemoveCart;
        }

        public static StringBuilder MeasureCheckout(POCOS.Order order, int stepNo)
        {

            StringBuilder MeasureCheckout = new StringBuilder();
            MeasureCheckout.AppendLine("window.dataLayer = window.dataLayer || []");

            MeasureCheckout.AppendLine("dataLayer.push ({");
            MeasureCheckout.AppendFormat("'event': 'checkout-step{0}',",stepNo.ToString());
            MeasureCheckout.AppendLine("'ecommerce': {");
            MeasureCheckout.AppendFormat("'currencyCode': '{0}',", getStoreCurrencyCodeForGA());
            MeasureCheckout.AppendLine("'checkout': {");
            MeasureCheckout.AppendLine("'actionField': {");
            MeasureCheckout.AppendFormat("'step': {0}",stepNo);
            MeasureCheckout.AppendLine("},");
            MeasureCheckout.AppendLine("'products': [ ");

            foreach (POCOS.CartItem ci in order.cartX.cartItemsX)
            {
                if (ci.BTOSystem != null)
                {
                    string productname = ci.ProductName;
                    if (ci.BTOSystem.isSBCBTOS())
                    {
                        productname = ci.ProductName + (from bc in ci.BTOSystem.BTOSConfigs
                                                        from bd in bc.BTOSConfigDetails
                                                        select bd.SProductID
                                                            ).FirstOrDefault();
                    }
                    MeasureCheckout.AppendLine("{");
                    MeasureCheckout.AppendFormat("'name': '{0}',", ci.ProductName);
                    MeasureCheckout.AppendFormat("'id': '{1}({0})',", ci.SProductID, productname);
                    MeasureCheckout.AppendFormat("'category': '{0}',", ci.type.ToString());
                    MeasureCheckout.AppendFormat("'price': {0},", ci.UnitPrice.ToString());
                    MeasureCheckout.AppendFormat("'quantity': {0}", ci.Qty);
                    MeasureCheckout.Append("},");
                }
                else if (ci.bundleX != null)
                {
                    foreach (POCOS.BundleItem bi in ci.bundleX.BundleItems)
                    {
                        MeasureCheckout.AppendLine("{");
                        MeasureCheckout.AppendFormat("'name': '{0}',", bi.ItemSProductID);
                        MeasureCheckout.AppendFormat("'id': '{0}',", bi.ItemSProductID);
                        MeasureCheckout.AppendFormat("'category': '{0}',", ci.type.ToString());
                        MeasureCheckout.AppendFormat("'price': {0},", bi.adjustedPrice.ToString());
                        MeasureCheckout.AppendFormat("'quantity': {0}", ci.Qty * bi.Qty);
                        MeasureCheckout.Append("},");
                    }
                }
                else
                {
                    MeasureCheckout.AppendLine("{");
                    MeasureCheckout.AppendFormat("'name': '{0}',", ci.ProductName);
                    MeasureCheckout.AppendFormat("'id': '{0}',", ci.SProductID);
                    MeasureCheckout.AppendFormat("'category': '{0}',", ci.type.ToString());
                    MeasureCheckout.AppendFormat("'price': {0},", ci.UnitPrice.ToString());
                    MeasureCheckout.AppendFormat("'quantity': {0}", ci.Qty);
                    MeasureCheckout.Append("},");
                }
            }

            MeasureCheckout.Length--;
            MeasureCheckout.Append("]}}});");

            return MeasureCheckout;

        }

        public static StringBuilder MeasurePurchases(POCOS.Order order)
        {

            StringBuilder MeasurePurchases = new StringBuilder();
            MeasurePurchases.AppendLine("window.dataLayer = window.dataLayer || []");
            MeasurePurchases.AppendLine("dataLayer.push ({");
            MeasurePurchases.AppendLine("'event': 'transaction',");
            MeasurePurchases.AppendLine("'ecommerce': {");
            MeasurePurchases.AppendFormat("'currencyCode': '{0}',", getStoreCurrencyCodeForGA());          
            MeasurePurchases.AppendLine("'purchase': {");
            MeasurePurchases.AppendLine("'actionField': {");
            MeasurePurchases.AppendFormat("'id': '{0}',", order.OrderNo);
            MeasurePurchases.AppendFormat("'affiliation': '{0}',", Presentation.eStoreContext.Current.Store.profile.StoreName);
            MeasurePurchases.AppendFormat("'revenue': {0},", order.totalAmountX);
            MeasurePurchases.AppendFormat("'shipping': {0},", order.Freight);
            MeasurePurchases.AppendFormat("'shippingMethod': '{0}',", order.ShippingMethod);
            MeasurePurchases.AppendFormat("'tax': {0},", order.Tax);
            MeasurePurchases.AppendFormat("'coupon': '{0}',", order.PromoteCode);
            MeasurePurchases.AppendFormat("'discount': {0},", order.TotalDiscount);
            MeasurePurchases.AppendFormat("'paymentType': '{0}'", order.paymentTypeX.ToString());
            MeasurePurchases.AppendLine("},");
            MeasurePurchases.AppendLine("'products': [ ");

            foreach (POCOS.CartItem ci in order.cartX.cartItemsX)
            {
                if (ci.BTOSystem != null)
                {
                    string productname = ci.ProductName;
                    if (ci.BTOSystem.isSBCBTOS())
                    {
                        productname = ci.ProductName + (from bc in ci.BTOSystem.BTOSConfigs
                                                        from bd in bc.BTOSConfigDetails
                                                        select bd.SProductID
                                                            ).FirstOrDefault();
                    }
                    MeasurePurchases.AppendLine("{");
                    MeasurePurchases.AppendFormat("'name': '{0}',", ci.ProductName);
                    MeasurePurchases.AppendFormat("'id': '{1}({0})',", ci.SProductID, productname);
                    MeasurePurchases.AppendFormat("'category': '{0}',", ci.type.ToString());
                    MeasurePurchases.AppendFormat("'price': {0},", ci.UnitPrice.ToString());
                    MeasurePurchases.AppendFormat("'quantity': {0}", ci.Qty);
                    MeasurePurchases.Append("},");
                }
                else if (ci.bundleX != null)
                {
                    foreach (POCOS.BundleItem bi in ci.bundleX.BundleItems)
                    {
                        MeasurePurchases.AppendLine("{");
                        MeasurePurchases.AppendFormat("'name': '{0}',", bi.ItemSProductID);
                        MeasurePurchases.AppendFormat("'id': '{0}',", bi.ItemSProductID);
                        MeasurePurchases.AppendFormat("'category': '{0}',", ci.type.ToString());
                        MeasurePurchases.AppendFormat("'price': {0},", bi.adjustedPrice.ToString());
                        MeasurePurchases.AppendFormat("'quantity': {0}", ci.Qty * bi.Qty);
                        MeasurePurchases.Append("},");
                    }
                }
                else
                {
                    MeasurePurchases.AppendLine("{");
                    MeasurePurchases.AppendFormat("'name': '{0}',", ci.ProductName);
                    MeasurePurchases.AppendFormat("'id': '{0}',", ci.SProductID);
                    MeasurePurchases.AppendFormat("'category': '{0}',", ci.type.ToString());
                    MeasurePurchases.AppendFormat("'price': {0},", ci.UnitPrice.ToString());
                    MeasurePurchases.AppendFormat("'quantity': {0}", ci.Qty);
                    MeasurePurchases.Append("},");
                }
            }

            MeasurePurchases.Length--;
            MeasurePurchases.Append("]}}});");

            return MeasurePurchases;

        }

        private static string getStoreCurrencyCodeForGA()
        {
            string currencyCode = eStoreContext.Current.CurrentCurrency.CurrencyID;

            switch (currencyCode)
            {
                case "RM":
                    currencyCode = "MYR";
                    break;
                case "RMB":
                    currencyCode = "CNY";
                    break;
                case "NTD":
                    currencyCode = "TWD";
                    break;
                case "YEN":
                    currencyCode = "JPY";
                    break;
            }
            return currencyCode;
        }
        #endregion
    }
}

