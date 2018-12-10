using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using System.Globalization;

namespace eStore.Presentation.Product
{
    public enum PriceStyle { productprice, productpriceLarge, MinPrice }
    public class ProductPrice
    {
        public static string getPrice(POCOS.Product product, PriceStyle priceStyle=PriceStyle.productprice, Currency currency = null)
        {
            if (product == null)
            {

                return FormartPrice(0);
            }
            string SPECIALpriceformat = "<div class=\"{2}\"><p class=\"specialprice\"><label>" + eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Price) + ":</label><span>{0}</span></p><p class=\"regularprice\"><label>"+
                eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Product_Special)
                +": </label><span>{1}</span></p></div>";
            string REGULARpriceformat = "<div class=\"{1}\"><p class=\"regularprice\"><label>" + eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Price) + ": </label><span>{0}</span></p></div>";

            POCOS.Product.PRICINGMODE pricemode;
            POCOS.Price listprice = new Price(0, product.storeX.defaultCurrency), markupprice = new Price(0, product.storeX.defaultCurrency);
            pricemode = product.getListingPrice(listprice, markupprice,eStoreContext.Current.User==null?null: eStoreContext.Current.User.actingUser.userGradeX);
            if (currency != null && currency.ID != product.storeX.defaultCurrency.ID)
            {
                listprice.exchangeValue(currency);
                markupprice.exchangeValue(currency);
            }
            string rlt = "";
            if (product.ShowPrice == false)
            {
                rlt = FormartPrice(0);
            }
            if (product.PriceType == "RecurringM")
            {
                if (listprice.value > 0)
                    rlt = string.Format("<div class=\"{1}\"><p class=\"regularprice\"><span>{0}/Month</span></p></div>", FormartPrice(listprice.value, listprice.currency.CurrencySign), priceStyle.ToString());
                else
                    rlt = FormartPrice(0);
            }
            else
            {
                switch (pricemode)
                {
                    case POCOS.Product.PRICINGMODE.SPECIAL:
                        {

                            rlt = string.Format(SPECIALpriceformat, FormartPrice(markupprice.value), FormartPrice(listprice.value, listprice.currency.CurrencySign), priceStyle.ToString());

                            break;
                        }
                    case POCOS.Product.PRICINGMODE.REGULAR:
                        {
                            rlt = string.Format(REGULARpriceformat, FormartPrice(listprice.value, listprice.currency.CurrencySign), priceStyle.ToString());

                            break;
                        }
                    case POCOS.Product.PRICINGMODE.NOTAVAILABLE:
                        {
                            product.ShowPrice = false;
                            rlt = FormartPrice(0);
                            break;
                        }
                    default:
                        {
                            product.ShowPrice = false;
                            rlt = FormartPrice(0);
                            break;
                        }

                }
            }

            return rlt;
        }

        public static string getPrice(POCOS.PStoreProduct product, PriceStyle priceStyle = PriceStyle.productprice)
        {
            if (product == null)
            {

                return FormartPrice(0);
            }
            //if (product.IsSoldInBundleOnly)
            //{
            //    return string.Format("<div class=\"epaps-productprice-link\"><a href=\"{0}\">{1}</a></div>",esUtilities.CommonHelper. ResolveUrl( UrlRewriting.MappingUrl.getMappingUrl(product)), eStore.Presentation.eStoreContext.Current.Store.Tanslation("See Bundle Price"));
            //}
            //string SPECIALpriceformat = "<div class=\"epaps-productprice-now\">{1}</div><div class=\"epaps-productprice-ago\">{0}</div>";
            string SPECIALpriceformat = "<div class=\"epaps-productprice-now\">{0}</div>";
            //string REGULARpriceformat = "<div class=\"epaps-productprice-now\">{0}</div>";
            string REGULARpriceformat = "<div class=\"epaps-productprice-regular\">{0}</div>";

            POCOS.Product.PRICINGMODE pricemode;
            POCOS.Price listprice = new Price(), markupprice = new Price();
            pricemode = product.getListingPrice(listprice, markupprice);
            string rlt = "";

            switch (pricemode)
            {
                case POCOS.Product.PRICINGMODE.SPECIAL:
                    {

                        //rlt = string.Format(SPECIALpriceformat, FormartPstorePrice(markupprice.value), FormartPstorePrice(listprice.value), priceStyle.ToString());
                        rlt = string.Format(SPECIALpriceformat, FormartPstorePrice(listprice.value), priceStyle.ToString());
                        //rlt = string.Format(REGULARpriceformat, FormartPstorePrice(listprice.value), priceStyle.ToString());
                        break;
                    }
                case POCOS.Product.PRICINGMODE.REGULAR:
                    {
                        rlt = string.Format(REGULARpriceformat, FormartPstorePrice(listprice.value), priceStyle.ToString());

                        break;
                    }
                case POCOS.Product.PRICINGMODE.NOTAVAILABLE:
                    {

                        rlt = FormartPrice(0);
                        break;
                    }
                default:
                    {

                        rlt = FormartPrice(0);
                        break;
                    }

            }
            return rlt;
        }
        public static string FormartPstorePrice( decimal price)
        {
            if (price == 0)
                return  eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Call_for_Price) ;
            string s = FormartPriceWithDecimal(price);
            if (s.LastIndexOf('.') == s.Length - 3)
            {
                s = s.Insert(s.IndexOf('.') + 1, "<span>") + "</span>";
            }
            return s;
        }


        public static string getAJAXProductPrice(POCOS.Product product, PriceStyle priceStyle = PriceStyle.productprice)
        {
            if (product == null)
                return FormartPrice(0);
            else if (priceStyle == PriceStyle.MinPrice)
            {
                string format = Presentation.eStoreContext.Current.getStringSetting("MinPriceFormat");
                if (string.IsNullOrEmpty(format))
                    format = "{0} {1}";
                return string.Format(format
                    , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_From)
                    , String.Format("<span id=\"{0}\" class=\"ajaxproductprice miniprice\"><img alt=\"loading..\" src=\"{1}images/priceprocessing.gif\" /></span>", product.SProductID,esUtilities.CommonHelper.GetStoreLocation()));
            }
            else
                return String.Format("<span id=\"{0}\" class=\"ajaxproductprice {2}\"><img alt=\"loading..\" src=\"{1}images/priceprocessing.gif\" /></span>", product.SProductID, esUtilities.CommonHelper.GetStoreLocation(), priceStyle.ToString());
        }
        public static string getSimplePrice(POCOS.Product product, PriceStyle priceStyle = PriceStyle.productprice)
        {
            if (product == null)
            {

                return FormartPrice(0);
            }
            string SPECIALpriceformat = "<div class=\"{2}\"><p class=\"specialprice\"><span>{0}</span></p><p class=\"regularprice\"><span>{1}</span></p></div>";
            string REGULARpriceformat = "<div class=\"{1}\"><p class=\"regularprice\"><span>{0}</span></p></div>";

            POCOS.Product.PRICINGMODE pricemode;
            POCOS.Price listprice = new Price(), markupprice = new Price();
            pricemode = product.getListingPrice(listprice, markupprice);
            string rlt = "";
            if (product.ShowPrice == false)
            {
                rlt = FormartPrice(0);
            }
            else if (priceStyle == PriceStyle.MinPrice)
            {
                rlt = FormartPrice(listprice.value);
            }
            else
            {
                switch (pricemode)
                {
                    case POCOS.Product.PRICINGMODE.SPECIAL:
                        {

                            rlt = string.Format(SPECIALpriceformat, FormartPrice(markupprice.value), FormartPrice(listprice.value), priceStyle.ToString());

                            break;
                        }
                    case POCOS.Product.PRICINGMODE.REGULAR:
                        {
                            rlt = string.Format(REGULARpriceformat, FormartPrice(listprice.value), priceStyle.ToString());

                            break;
                        }
                    case POCOS.Product.PRICINGMODE.NOTAVAILABLE:
                        {
                            product.ShowPrice = false;
                            rlt = FormartPrice(0);
                            break;
                        }
                    default:
                        {
                            product.ShowPrice = false;
                            rlt = FormartPrice(0);
                            break;
                        }

                }
            }

            return rlt;
        }
        public static string getPrice(POCOS.Product_Ctos product, POCOS.BTOSystem btos, PriceStyle priceStyle = PriceStyle.productprice)
        {
            string SPECIALpriceformat = "<div class=\"{2}\"><p class=\"specialprice\"><label>" + eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Price) + ": </label><span>{0}</span></p><p class=\"regularprice\"><label>"
            + eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Product_Special) + ": </label><span>{1}</span></p></div>";
            string REGULARpriceformat = "<div class=\"{1}\"><p class=\"regularprice\"><label>" + eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Price) + ": </label><span>{0}</span></p></div>";

            POCOS.Product.PRICINGMODE pricemode;
            POCOS.Price listprice = new Price(), markupprice = new Price();
            pricemode = product.getListingPrice(listprice, markupprice, btos);
            string rlt = "";
            if (product.ShowPrice == false)
            {
                rlt = FormartPrice(0);
            }
            else
            {
                switch (pricemode)
                {
                    case POCOS.Product.PRICINGMODE.SPECIAL:
                        {

                            rlt = string.Format(SPECIALpriceformat, FormartPrice(markupprice.value), FormartPrice(listprice.value), priceStyle.ToString());

                            break;
                        }
                    case POCOS.Product.PRICINGMODE.REGULAR:
                        {
                            rlt = string.Format(REGULARpriceformat, FormartPrice(listprice.value), priceStyle.ToString());

                            break;
                        }
                    case POCOS.Product.PRICINGMODE.NOTAVAILABLE:
                        {
                            product.ShowPrice = false;
                            rlt = FormartPrice(0);
                            break;
                        }
                    default:
                        {
                            product.ShowPrice = false;
                            rlt = FormartPrice(0);
                            break;
                        }

                }
            }

            return rlt;
        }

        public static string getPriceWithoutCurrency(POCOS.Product product)
        {
            if (product == null)
            {

                return FormartPriceWithoutCurrency(0);
            }

            POCOS.Product.PRICINGMODE pricemode;
            POCOS.Price listprice = new Price(), markupprice = new Price();
            pricemode = product.getListingPrice(listprice, markupprice);
            string rlt = "";
            if (product.ShowPrice == false)
            {
                rlt = FormartPriceWithoutCurrency(0);
            }
            else
            {
                switch (pricemode)
                {
                    case POCOS.Product.PRICINGMODE.SPECIAL:
                    case POCOS.Product.PRICINGMODE.REGULAR:
                        {
                            rlt = FormartPriceWithoutCurrency(listprice.value);

                            break;
                        }
                    case POCOS.Product.PRICINGMODE.NOTAVAILABLE:
                    default:
                        {
                            product.ShowPrice = false;
                            rlt = FormartPrice(0);
                            break;
                        }

                }
            }

            return rlt;
        }
        public static string getPrice(string ProductID)
        {
            return (getPriceWithoutCurrency( Presentation.eStoreContext.Current.Store.getProduct(ProductID)));
        }
        public static string getPrice(int ProductID)
        {
            return (getPriceWithoutCurrency(Presentation.eStoreContext.Current.Store.getProduct(ProductID.ToString())));
        }

        public static string getPriceWithSign(object productID)
        {
            POCOS.Product product = Presentation.eStoreContext.Current.Store.getProduct(productID.ToString());
            if (product == null)
                return FormartPriceWithoutCurrency(0);
            else
            {
                POCOS.Product.PRICINGMODE pricemode;
                POCOS.Price listprice = new Price(), markupprice = new Price();
                pricemode = product.getListingPrice(listprice, markupprice);
                string rlt = "";
                if (product.ShowPrice == false)
                    rlt = FormartPriceWithoutCurrency(0);
                else
                {
                    switch (pricemode)
                    {
                        case POCOS.Product.PRICINGMODE.SPECIAL:
                        case POCOS.Product.PRICINGMODE.REGULAR:
                            {
                                rlt = eStoreContext.Current.Store.profile.defaultCurrency.CurrencySign + FormartPriceWithoutCurrency(listprice.value);
                                break;
                            }
                        case POCOS.Product.PRICINGMODE.NOTAVAILABLE:
                        default:
                            {
                                product.ShowPrice = false;
                                rlt = FormartPriceWithoutCurrency(0);
                                break;
                            }

                    }
                }
                return rlt;
            }

        }

        public static string GetCTOSFromPriceString(string CategoryPath, bool isWhitCurrency = false)
        {
            POCOS.ProductCategory category = Presentation.eStoreContext.Current.Store.getProductCategory(CategoryPath);
            if (category != null)
            {
                if (!isWhitCurrency)
                    return FormartPriceWithoutCurrency(category.getLowestPrice());
                else
                    return FormartPriceWithCurrency(category.getLowestPrice());
            }
            else
                return FormartPriceWithoutCurrency(0);
        }
        public static string FormartPrice(decimal? price, string currencySign = "")
        {
            if (price == 0)
                return "<label class=\"callforprice\">" + eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Call_for_Price) + "</label>";
            return FormartPriceWithDecimal(fixExchangeCurrencyPrice(price,currencySign));
        }
        public static string FormartPriceWithoutCurrency(decimal? price)
        {
            if (price == 0)
                return "<p class=\"regularprice\"><span>" + eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Call_for_Price) + "</span></p>";

            string rlt = getPriceFormat(Presentation.eStoreContext.Current.Store.storeID);
            CultureInfo culture = new CultureInfo(Presentation.eStoreContext.Current.Store.profile.CultureCode);
            culture.NumberFormat.CurrencySymbol =string.Empty;
            return price.GetValueOrDefault().ToString(rlt, culture);
        }

        public static string FormartPriceWithParameterCurrency(decimal? price, string currency)
        {
            if (price == 0)
                return "<p class=\"regularprice\"><span>" + eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Call_for_Price) + "</span></p>";

            string rlt = getPriceFormat(Presentation.eStoreContext.Current.Store.storeID);
            CultureInfo culture = new CultureInfo(Presentation.eStoreContext.Current.Store.profile.CultureCode);
            culture.NumberFormat.CurrencySymbol = currency;
            return price.GetValueOrDefault().ToString(rlt, culture);
        }

        public static string FormartPriceWithCurrency(decimal? price)
        {
            if (price == 0)
                return "<p class=\"regularprice\"><span>" + eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Call_for_Price) + "</span></p>";
            return FormartPriceWithDecimal(price);
        }

        public static string FormartPriceWithDecimal(decimal? price)
        {
            return FormartPriceWithDecimal(price, eStoreContext.Current.Store.profile.defaultCurrency.CurrencySign);
        }

        public static string FormartPriceWithDecimal(decimal? price,string currencySign)
        {
            if (string.IsNullOrEmpty(currencySign))
                currencySign = eStoreContext.Current.Store.profile.defaultCurrency.CurrencySign;
            var pc = fixExchangeCurrencyPrice(price,currencySign);
            string rlt = getPriceFormat(Presentation.eStoreContext.Current.Store.storeID);
            CultureInfo culture = new CultureInfo(Presentation.eStoreContext.Current.Store.profile.CultureCode);
            culture.NumberFormat.CurrencySymbol = pc.currency.CurrencySign;
            return pc.value.ToString(rlt, culture);
        }
        public static string FormartPriceWithDecimal(POCOS.Price price)
        {
            return FormartPriceWithDecimal(price.value, price.currency.CurrencySign);
        }

        public static string FormartFreight(decimal? price, string currencySign = "")
        {
            if (price == null || price == 0)
                return eStoreContext.Current.Store.getZeroFreightDispayString();
            else
            {
                if(string.IsNullOrEmpty(currencySign))
                    return  FormartPriceWithDecimal(price);
                else
                    return FormartPriceWithDecimal(price, currencySign);
            }
        }

        public static string FormartTax(decimal? price, string currencySign = "")
        {
         
            if (price == null || price == 0)
                return eStoreContext.Current.Store.getZeroTaxDispayString();
            else
            {
                if(string.IsNullOrEmpty(currencySign))
                    return FormartPriceWithDecimal(price);
                else
                    return FormartPriceWithDecimal(price, currencySign);
            }
               
        }
        public static string FormartPriceWithoutDecimal(decimal? price, POCOS.Currency currency=null)
        {
            string format = getPriceFormat(Presentation.eStoreContext.Current.Store.storeID);
            var pc = fixExchangeCurrencyPrice(price, currency);
            CultureInfo culture = new CultureInfo(Presentation.eStoreContext.Current.Store.profile.CultureCode);
            culture.NumberFormat.CurrencySymbol = pc.currency.CurrencySign;
            return pc.value.ToString(format, culture);
        }
        public static POCOS.Price getSitePrice(decimal? price, POCOS.Currency currency = null)
        {
            string format = getPriceFormat(Presentation.eStoreContext.Current.Store.storeID);
            var pc = fixExchangeCurrencyPrice(price, currency);
            CultureInfo culture = new CultureInfo(Presentation.eStoreContext.Current.Store.profile.CultureCode);
            culture.NumberFormat.CurrencySymbol = pc.currency.CurrencySign;
            pc.valueWithoutCurrency = pc.value.ToString("N", culture);
            pc.valueWithCurrency = pc.value.ToString(format, culture);
            return pc;
        }

        public static string getPriceFormat(string storeid)
        {
            string rlt = "C2";
            if (Utilities.Converter.getCartPriceRoundingUnit(eStoreContext.Current.Store.storeID) < 1)
                rlt = "C2";
            else
                rlt = "C0";
            return rlt;
        
        }

        private static POCOS.Price fixExchangeCurrencyPrice(decimal? price, string currencySign = "")
        {
            if (Presentation.eStoreContext.Current.CurrentCurrency.Equals(Presentation.eStoreContext.Current.Store.profile.defaultCurrency))
                return new Price(price.Value, Presentation.eStoreContext.Current.Store.profile.defaultCurrency);

            if (string.IsNullOrEmpty(currencySign))
                currencySign = Presentation.eStoreContext.Current.Store.profile.defaultCurrency.CurrencySign;

            var ls = Presentation.eStoreContext.Current.Store.profile.getCurrencyBySign(currencySign);
            if (ls != null && ls.Count == 1)
            {
                POCOS.Price pc = new Price(price.GetValueOrDefault(), ls.FirstOrDefault());
                pc.exchangeValue(Presentation.eStoreContext.Current.CurrentCurrency);
                return pc;
            }
            else
                return new Price(price.Value, new Currency() { CurrencySign = currencySign });
        }

        public static POCOS.Price fixExchangeCurrencyPrice(decimal? price, POCOS.Currency currency = null)
        {
            if (currency == null)
            {
                if (Presentation.eStoreContext.Current.CurrentCurrency.Equals(Presentation.eStoreContext.Current.Store.profile.defaultCurrency))
                    return new Price(price.Value, Presentation.eStoreContext.Current.Store.profile.defaultCurrency);
                else
                {
                    POCOS.Price pc = new Price(price.GetValueOrDefault(), Presentation.eStoreContext.Current.Store.profile.defaultCurrency);
                    pc.exchangeValue(Presentation.eStoreContext.Current.CurrentCurrency);
                    return pc;
                }
            }
            else
            {
                if (currency.Equals(Presentation.eStoreContext.Current.CurrentCurrency))
                    return new Price(price.GetValueOrDefault(), currency);
                else
                {
                    POCOS.Price pc = new Price(price.GetValueOrDefault(), currency);
                    pc.exchangeValue(Presentation.eStoreContext.Current.CurrentCurrency);
                    return pc;
                }
            }
        }

        public static decimal changeToStorePrice(decimal? price, POCOS.Currency currency)
        {
            if (price == null)
                return 0;
            if (currency == null)
                return price.GetValueOrDefault();
            POCOS.Price pc = new Price(price.GetValueOrDefault(), currency);
            pc.exchangeValue(Presentation.eStoreContext.Current.Store.profile.defaultCurrency);
            return pc.value;
        }

    }
}
