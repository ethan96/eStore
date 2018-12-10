using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation;
using eStore.Presentation.UrlRewriting;

namespace eStore.UI.Sitemap
{
    public partial class Default : eStore.Presentation.eStoreBaseControls.eStoreBasePage
    {
        #region Property
        public override bool OverwriteMasterPageFile
        {
            get
            {
                return false;
            }
        }
        public ICollection<POCOS.Menu> HeaderMenu { get; set; }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                HeaderMenu = Presentation.eStoreContext.Current.Store.getMenuItems(Presentation.eStoreContext.Current.MiniSite);
                BindStandardCategory();
                BindSystemsCategory();
                BindSolutionService();
                BindFooter();
                //BindCertifiedPeripherals();
            }
        }

        #region Standard Category
        private void BindStandardCategory()
        {
            if (HeaderMenu != null)
            {
                POCOS.Menu productCategoryName = HeaderMenu.FirstOrDefault(p => p.URL.Contains("/Product/AllProduct.aspx?type=standard"));
                if (productCategoryName != null)
                {
                    hlStandardCategory.NavigateUrl = ResolveUrl(MappingUrl.getMappingUrl(productCategoryName));
                    hlStandardCategory.Text = HttpUtility.HtmlEncode(productCategoryName.MenuName);
                }
            }
            List<POCOS.ProductCategory> ls = eStoreContext.Current.Store.getTopLevelStandardProductCategories(eStoreContext.Current.MiniSite);
            if (ls != null && ls.Count > 0)
                rptProductCategory.DataSource = ls;
            rptProductCategory.DataBind();
        }
        //二级category
        protected void rptProductCategory_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                POCOS.ProductCategory pc = e.Item.DataItem as POCOS.ProductCategory;
                Literal ltAnyCategory = e.Item.FindControl("ltAnyCategory") as Literal;
                string categoryHtml = string.Empty;
                generateCategoryHtml(pc, ref categoryHtml);
                ltAnyCategory.Text = categoryHtml;
            }
        }
        //遍历category
        private void generateCategoryHtml(POCOS.ProductCategory pc, ref string categoryHtml)
        {
            List<POCOS.ProductCategory> chileCategory = pc.childCategoriesX.Where(p => !string.IsNullOrEmpty(p.LocalCategoryName)).ToList();
            if (chileCategory != null && chileCategory.Count>0)
            {
                categoryHtml += "<ul class=\"eStoreList\">";
                foreach (var item in chileCategory)
	            {
                    categoryHtml += string.Format("<li><a href='{0}' target='_blank'>{1}</a>",ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(item)),item.LocalCategoryName);
                    //if (item.childCategoriesX != null && item.childCategoriesX.Count > 0)
                    //    generateCategoryHtml(item, ref categoryHtml);
                    categoryHtml += "</li>";
	            }
                categoryHtml += "</ul>";
            }
        }
        #endregion

        #region Systems Category
        private void BindSystemsCategory()
        {
            if (HeaderMenu != null)
            {
                POCOS.Menu systemCategoryName = HeaderMenu.FirstOrDefault(p => p.URL.Contains("/Product/AllProduct.aspx?type=system"));
                if (systemCategoryName != null)
                {
                    hlSystemsCategory.NavigateUrl = ResolveUrl(MappingUrl.getMappingUrl(systemCategoryName));
                    hlSystemsCategory.Text = HttpUtility.HtmlEncode(systemCategoryName.MenuName);
                }
            }
            List<POCOS.ProductCategory> ls = eStoreContext.Current.Store.getTopLevelCTOSProductCategories(eStoreContext.Current.MiniSite);
            if (ls != null && ls.Count > 0)
                rptSystemCategory.DataSource = ls;
            rptSystemCategory.DataBind();
        }
        //二级category
        protected void rptSystemCategory_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                POCOS.ProductCategory pc = e.Item.DataItem as POCOS.ProductCategory;

                Literal ltAnyCategory = e.Item.FindControl("ltAnyCategory") as Literal;
                string categoryHtml = string.Empty;
                generateCategoryHtml(pc, ref categoryHtml);
                ltAnyCategory.Text = categoryHtml;
            }
        }
        #endregion

        #region Solutions Services
        private void BindSolutionService()
        {
            if (HeaderMenu != null)
            {
                List<POCOS.Menu> ls = HeaderMenu.Where(p => !p.URL.Contains("/Product/AllProduct.aspx?type=standard") && !p.URL.Contains("/Product/AllProduct.aspx?type=system")).ToList();
                if (ls != null && ls.Count > 0)
                    rptSolutionServiceMenu.DataSource = ls;
                rptSolutionServiceMenu.DataBind();
            }
        }

        protected void rptSolutionServiceMenu_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                POCOS.Menu pm = e.Item.DataItem as POCOS.Menu;

                Repeater rptSubmenu = e.Item.FindControl("rptSubmenu") as Repeater;
                rptSubmenu.DataSource = pm.subMenusX;
                rptSubmenu.DataBind();
            }
        }
        #endregion

        #region Footer
        private void BindFooter()
        {
            List<POCOS.Menu> ls = Presentation.eStoreContext.Current.Store.getFooterLinks(Presentation.eStoreContext.Current.MiniSite).Take(3).ToList();
            if (ls != null && ls.Count > 0)
                rptFooterMenu.DataSource = ls;
            rptFooterMenu.DataBind();
        }

        protected void rptFooterMenu_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                POCOS.Menu pm = e.Item.DataItem as POCOS.Menu;

                Repeater rptLeftRightmenu = e.Item.FindControl("rptLeftRightmenu") as Repeater;
                rptLeftRightmenu.DataSource = pm.subMenusX;
                rptLeftRightmenu.DataBind();
            }
        }

        protected void rptLeftRightmenu_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                POCOS.Menu pm = e.Item.DataItem as POCOS.Menu;

                Repeater rptSubmenu = e.Item.FindControl("rptSubmenu") as Repeater;
                rptSubmenu.DataSource = pm.subMenusX;
                rptSubmenu.DataBind();
            }
        }
        #endregion

        #region Certified Peripherals[PStore Category]
        //bind pstore category
        private void BindCertifiedPeripherals()
        {
            eStore.BusinessModules.PStore pstore = new eStore.BusinessModules.PStore(Presentation.eStoreContext.Current.Store.profile);
            if (pstore.isActive() == false)
                liCertifiedPeripheral.Visible = false;
            else
            {
                List<POCOS.PStoreProductCategory> pstoreCategoryList = pstore.getTopLevelPStoreCategory();
                if (pstoreCategoryList != null && pstoreCategoryList.Count > 0)
                {
                    hlCertifiedPeripheral.NavigateUrl = ResolveUrl("~/CertifiedPeripherals/Default.aspx");
                    hlCertifiedPeripheral.Text = "Certified Peripherals";

                    rptCertifiedPeripheral.DataSource = pstoreCategoryList;
                    rptCertifiedPeripheral.DataBind();
                }
                else
                    liCertifiedPeripheral.Visible = false;
            }
        }
        #endregion
    }
}