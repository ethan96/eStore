using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation;

namespace eStore.UI.Product
{
    public partial class AllProduct : Presentation.eStoreBaseControls.eStoreBasePage
    {
        public string ProductTitle { get; set; }
        public override bool isMobileFriendly
        {
            get
            {
                return true;
            }
            set
            {
                base.isMobileFriendly = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            List<POCOS.ProductCategory> ls=new List<POCOS.ProductCategory>();
            string type = Request["type"];
            string id = Request["id"];
           
            if (!string.IsNullOrEmpty(id))//shop group
            {
                var rc= eStoreContext.Current.Store.getProductCategory(id);
                if (rc != null)
                {
                    ls = rc.childCategoriesX.ToList();
                    hlViewAll.Visible = true;
                    this.ProductTitle = rc.localCategoryNameX;
                }
            }
            else if (string.IsNullOrEmpty(type))
            {
                ls = eStoreContext.Current.Store.getTopLevelStandardProductCategories(eStoreContext.Current.MiniSite);
                ls.AddRange(eStoreContext.Current.Store.getTopLevelCTOSProductCategories(eStoreContext.Current.MiniSite));
                ls = ls.OrderBy(x => x.Sequence).ThenBy(x => x.localCategoryNameX).ToList();
                this.ProductTitle = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Show_All_Products_And_Systems);
            }
            else
            {
                if (type.ToLower() == "all1")
                {
                    var topcs = eStoreContext.Current.Store.getTopLevelStandardProductCategories(eStoreContext.Current.MiniSite);
                    ls = topcs.SelectMany(x => x.childCategoriesX).ToList();
                    this.ProductTitle = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Show_All_Products);
                }
                else if (type.ToLower() == "standard")
                {
                    ls = eStoreContext.Current.Store.getTopLevelStandardProductCategories(eStoreContext.Current.MiniSite);
                    this.ProductTitle = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Show_All_Products);
                }
                else
                {
                    this.ProductTitle = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Show_All_Systems);
                    ls = eStoreContext.Current.Store.getTopLevelCTOSProductCategories(eStoreContext.Current.MiniSite);
                    this.AdRotatorSelect1.BannerList = eStore.Presentation.eStoreContext.Current.Store.sliderBanner("AllProductForSystem");
                    //this.AdRotatorSelect1.BannerWidth = 780;
                }
            }
            if (ls == null || ls.Any() == false)
            {
                ls = eStoreContext.Current.Store.getTopLevelStandardProductCategories(eStoreContext.Current.MiniSite);
                ls.AddRange(eStoreContext.Current.Store.getTopLevelCTOSProductCategories(eStoreContext.Current.MiniSite));
                ls = ls.OrderBy(x => x.Sequence).ThenBy(x => x.localCategoryNameX).ToList();
                this.ProductTitle = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Show_All_Products_And_Systems);
            }

            this.isExistsPageMeta = setPageMeta($"{this.ProductTitle} - {Presentation.eStoreContext.Current.Store.profile.StoreName}"
                , "", "");

           
            ProductCategoryAllList1.allProductCategory = ls;

        }
    }
}