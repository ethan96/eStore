using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eStore.UI.Models
{
    public class TodaysHighlight
    {
        public TodaysHighlight() { }
        public TodaysHighlight(POCOS.ProductCategory category)
        {
            Url = esUtilities.CommonHelper.ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(category));
            Name = eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(category);
            Description = eStore.Presentation.eStoreGlobalResource.getLocalCategoryDescription(category);  
            Icon = "";
            Image = string.IsNullOrEmpty(category.ImageURL) ?
                "https://buy.advantech.com/images/photounavailable.gif" :
                (category.ImageURL.ToLower().StartsWith("http") ?
                category.ImageURL : esUtilities.CommonHelper.ResolveUrl(eStore.Presentation.eStoreContext.Current.Store.profile.Settings["ProductCategoryPicurePath"].ToString() + category.ImageURL));
            decimal lowestprice = category.getLowestPrice();
            if (lowestprice == 0m)
            {
                string format = "<span class=\"price callforprice\" >{0}</span>";
                Price =string.Format(format, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Call_for_Price));
            }
            else
            {
                string format = "From<span>{0}</span><span class=\"price\" >{1}</span>";
                Price = string.Format(format, eStore.Presentation.eStoreContext.Current.CurrentCurrency.CurrencySign
                , eStore.Presentation.Product.ProductPrice.FormartPriceWithoutCurrency(lowestprice));
            }
              
            CurrencySign = eStore.Presentation.eStoreContext.Current.CurrentCurrency.CurrencySign;
            ReadText = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_More);
        }
        public TodaysHighlight(POCOS.ProductCategory category,int seq):this(category)
        {
            this.Sequence = seq;
        }
        public TodaysHighlight(POCOS.Product product)
        {
            string surl = eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(product);
            if (!string.IsNullOrEmpty(surl))
                Url = esUtilities.CommonHelper.ResolveUrl(surl);
            else
            {
                Url = "#"; 
            }
            Name = product.name;
            Description = product.productDescX;
            Icon = product.status.ToString();
            Image = product.thumbnailImageX;
            decimal price = product.getListingPrice().value;
            if (price == 0m)
            {
                string format = "<span class=\"price callforprice\" >{0}</span>";
                Price = string.Format(format, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Call_for_Price));
            }
            else
            {
                string format = "From<span>{0}</span><span class=\"price\" >{1}</span>";
                Price = string.Format(format, eStore.Presentation.eStoreContext.Current.CurrentCurrency.CurrencySign
                , eStore.Presentation.Product.ProductPrice.FormartPriceWithoutCurrency(price));
            }
              
            CurrencySign = eStore.Presentation. eStoreContext.Current.CurrentCurrency.CurrencySign;
            ReadText = eStore.Presentation.eStoreLocalization.Tanslation("eStore_Read_More");
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Icon { get; set; }
        public string Price { get; set; }
        public string CurrencySign { get; set; }
        public string Url { get; set; }
        public int Sequence { get; set; }
        public string ReadText { get; set; }
    }
}