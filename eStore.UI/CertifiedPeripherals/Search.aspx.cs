using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation.SearchConfiguration;
using eStore.POCOS;
using eStore.Presentation;
using esUtilities;

namespace eStore.UI.CertifiedPeripherals
{
    public partial class Search : Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Presentation.eStoreContext.Current.SearchConfiguration = new PStoreSearchConfiguration();
        }
        public override bool OverwriteMasterPageFile
        {
            get
            {
                return false;
            }
            set
            {
                base.OverwriteMasterPageFile = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            string style = esUtilities.CommonHelper.GetStoreLocation() + "Styles/CertifiedPeripherals.css";
            AddStyleSheet(style);
            BindScript("url", "jquery.simplePagination.js", "jquery.simplePagination.js");
            string simplePaginationstyle = CommonHelper.GetStoreLocation() + "Styles/simplePagination.css";
            AddStyleSheet(simplePaginationstyle);
 
            BindScript("url", "CertifiedPeripherals", "CertifiedPeripherals.js");
            BindSearchResult();
        }
        private void BindSearchResult()
        {
            BusinessModules.PStore pstore = BusinessModules.PStore.getInstance(eStoreContext.Current.Store.profile);
            if (!pstore.isActive())
            {
                Response.Redirect("~/");
            }
            //string storekeyworddispay = Request["storekeyworddispay"];
            string storekeyworddispay = Request.QueryString["skey"];
            if (string.IsNullOrEmpty(storekeyworddispay))
            {
                storekeyworddispay = Request["storekeyworddispay"];
                if (string.IsNullOrEmpty(storekeyworddispay))
                    storekeyworddispay = Request.QueryString["SearchKeyWordsFromWebService"];
            }

            nofoundmessage.Visible = true ;
            if (!string.IsNullOrEmpty(storekeyworddispay))  //Search keyword is not empty
            {
      
                this.UserActivitLog.ProductID = storekeyworddispay;
                this.UserActivitLog.CategoryType = "Pstore SEARCH";
                POCOS.DAL.PStoreProductCategoryHelper categoryhelper = new POCOS.DAL.PStoreProductCategoryHelper();
                
                List<PStoreProduct> products = pstore.getMatchProducts(storekeyworddispay, false);

                if (products != null && products.Any())
                {
                    var cids = products.Select(x => x.ProductCategoryId).Distinct();
                    if (cids.Any() && cids.Count() > 1)
                    {
                        var category = (from c in cids
                                        let cat = categoryhelper.get(eStoreContext.Current.Store.profile, c)
                                        where cat != null
                                        select new
                                        {
                                            cat.Id,
                                            cat.DisplayName
                                        }).Distinct().ToList();
                        rpSearchResultCategory.DataSource = category;
                        rpSearchResultCategory.DataBind();
                    }
                    rpProducts.DataSource = products;
                    rpProducts.DataBind();
                    nofoundmessage.Visible = false;
                }

            }
        }
    }
}