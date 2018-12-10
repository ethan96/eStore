using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI
{
    public partial class IotSearch : Presentation.eStoreBaseControls.eStoreBasePage
    {/// <summary>
        /// can not over write the master page because used the special holder eStoreConfigurableRightContent,
        /// if this place hodler has controls, it will not apply the setting 
        /// </summary>
        public override bool OverwriteMasterPageFile
        {
            get
            {
                return false;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            BindSearchResult();
        }
        private void BindSearchResult()
        {
            //string storekeyworddispay = Request["storekeyworddispay"];
            string storekeyworddispay = Request.QueryString["skey"];
            if (string.IsNullOrEmpty(storekeyworddispay))
            {
                storekeyworddispay = Request["storekeyworddispay"];
                if (string.IsNullOrEmpty(storekeyworddispay))
                    storekeyworddispay = Request.QueryString["SearchKeyWordsFromWebService"];
            }

            POCOS.ProductSpecRules psr = new POCOS.ProductSpecRules();
            psr = null;

            if (!string.IsNullOrEmpty(storekeyworddispay))  //Search keyword is not empty
            {
                //update search log activity info to relfect searching criteria, but exclude web service search from WWW.
                //The search inquiry from WWW is to check model or product available. It's not really a user key-in keyword
                //只有不为空 才保存成search模式,其他默认为普通log
                this.UserActivitLog.ProductID = storekeyworddispay;
                this.UserActivitLog.CategoryType = "SEARCH";

                //jsut according the search fields
                psr = Presentation.eStoreContext.Current.Store.getMatchProducts(storekeyworddispay, false);
            }
            if (psr != null && psr._products != null)
            {
                if (psr._products.Count > 0)
                {
                    if (psr._products.Count == 1)
                    {
                        //具体产品会转到对应的产品, 这里要write log
                        if (eStore.Presentation.eStoreContext.Current.isRequestFromSearchEngine() == false)
                        {
                            if (!IsPostBack || this.GetType().Name.ToUpper() == "SEARCH_ASPX") // if page is search will write log all ways
                                this.UserActivitLog.save();
                        }
                        Response.Redirect(ResolveUrl(Presentation.UrlRewriting.MappingUrl.getMappingUrl(psr._products.First())));
                    }
                    else
                    {
                        if (eStore.Presentation.eStoreContext.Current.MiniSite.MiniSiteType == POCOS.MiniSite.SiteType.UShop)
                            pnCustom.Visible = false;
                        else
                        {
                            ProductList2.pageSize = 10;
                            ProductList2.productList = psr._products;
                        }
                        string keyword = string.Join(";", psr._products.Select(p => p.SProductID).ToArray());
                        this.isExistsPageMeta = this.setPageMeta(string.Format("Search - {0}", storekeyworddispay), "", keyword);
                        Presentation.eStoreContext.Current.keywords.Add("Keywords", storekeyworddispay);
                        this.lNoMatchedMessage.Visible = false;

                        ProductList1.pageSize = 5;
                        ProductList1.productList = getMinisiteProducts(storekeyworddispay);
                    }
                }
                else
                {
                    this.lNoMatchedMessage.Visible = true;

                    List<string> ls = new List<string>();
                    if (!string.IsNullOrEmpty(storekeyworddispay))
                        ls.Add(storekeyworddispay);
                    this.UserActivitLog.ProductID = string.Join(" | ", ls);
                    this.UserActivitLog.CategoryType = "ErrorPart";
                }
            }
        }

        private List<POCOS.Product> getMinisiteProducts(string storekeyworddispay)
        {
            if (eStore.Presentation.eStoreContext.Current.MiniSite != null)
                return Presentation.eStoreContext.Current.Store.getMatchProducts(storekeyworddispay, eStore.Presentation.eStoreContext.Current.MiniSite, false)._products
                    .Distinct().ToList();
            else
                return new List<POCOS.Product>();
        }
    }
}