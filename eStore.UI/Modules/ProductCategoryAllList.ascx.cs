using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.BusinessModules;

namespace eStore.UI.Modules
{
    public partial class ProductCategoryAllList : System.Web.UI.UserControl
    {
        public List<POCOS.ProductCategory> allProductCategory { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (allProductCategory != null)
            {
                allProductCategory = allProductCategory.OrderBy(c => c.LocalCategoryName).ToList();


                rp_AllCategoryList.DataSource = allProductCategory;
                rp_AllCategoryList.DataBind();
            }
        }

        protected void rp_AllCategoryList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                POCOS.ProductCategory pc = e.Item.DataItem as POCOS.ProductCategory;
                Literal lt_title = e.Item.FindControl("lt_title") as Literal;

                if (pc != null)
                    lt_title.Text = string.Format("<li><a href='{0}'>{1}</a></li>", ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(pc)), pc.LocalCategoryName);
                
                if (pc.LocalCategoryName == "Certified Peripherals")
                {
                    
                }
                Repeater repert_subCategory = e.Item.FindControl("repert_subCategory") as Repeater;
                if (pc.childCategoriesX.Count > 6)
                {
                    repert_subCategory.DataSource = pc.childCategoriesX.Take(6).ToList();
                    repert_subCategory.DataBind();
                    //HyperLink hl_more = e.Item.FindControl("hl_more") as HyperLink;
                    //hl_more.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_More);
                    //hl_more.NavigateUrl = ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(e.Item.DataItem));
                    //hl_more.Visible = true;

                    Literal lt_more = e.Item.FindControl("lt_more") as Literal;
                    lt_more.Text = string.Format("<li class='more'><a href='{0}'>{1}</a></li>", ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(e.Item.DataItem)), Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_More));
                }
                else
                {
                    repert_subCategory.DataSource = pc.childCategoriesX;
                    repert_subCategory.DataBind();
                }
            }
        }


    }
}