using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation;

namespace eStore.UI.Modules
{
    public partial class HomeMedialContent : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BindData();

            bindImage();
            //轮播广告
            this.BindScript("url", "carouFredSel", "jquery.carouFredSel-6.2.1-packed.js");
             
        }

        void BindData()
        {
            int itemsCount = 9;
            List<POCOS.ProductCategory> spc = eStoreContext.Current.Store.getTopLevelStandardProductCategories(eStoreContext.Current.MiniSite);
            this.rpProductCategory.DataSource = spc.OrderBy(c => c.Sequence).ThenBy(p=>p.LocalCategoryName).Take(itemsCount);
            this.rpProductCategory.DataBind();
            this.hlMoreProducts.Visible = (spc != null && spc.Count > itemsCount);


            List<POCOS.ProductCategory> cc = eStoreContext.Current.Store.getTopLevelCTOSProductCategories(eStoreContext.Current.MiniSite);
            this.rpSystems.DataSource = cc.OrderBy(c => c.Sequence).ThenBy(p=>p.LocalCategoryName).Take(itemsCount);
            this.rpSystems.DataBind();
            this.hlMoreSystems.Visible = (cc != null && cc.Count > itemsCount);
            IList<POCOS.Menu> sulotions = eStoreContext.Current.Store.getTopLevelSolutionMenus(eStoreContext.Current.MiniSite);
            if (sulotions != null)
             {
                 this.rpSolutions.DataSource = sulotions.OrderBy(s => s.MenuName).Take(itemsCount);
                 this.rpSolutions.DataBind();
             }
             else
                 sulotions = null;
         
            this.hlMoreSolutions.Visible = (sulotions != null && sulotions.Count > itemsCount);

            if (eStoreContext.Current.Store.adamForum == null)
                this.hlAdamForum.Visible = false;
            else
            {
                this.hlAdamForum.Visible = true;
                this.hlAdamForum.NavigateUrl = ResolveUrl(eStoreContext.Current.Store.adamForum.Hyperlink);
               Image imgAdamForum=new Image();
                imgAdamForum.ImageUrl=ResolveUrl( eStoreContext.Current.Store.adamForum.Imagefile);
                if (eStoreContext.Current.Store.adamForum.Target != null)
                    this.hlAdamForum.Target = eStoreContext.Current.Store.adamForum.Target;
                hlAdamForum.Controls.Add(imgAdamForum);
            }

            if (eStoreContext.Current.Store.educationColumns != null)
            {
                this.rpEducation.DataSource = eStoreContext.Current.Store.educationColumns;
                this.rpEducation.DataBind();            
            }


            if (Presentation.eStoreContext.Current.getBooleanSetting("EnableTodaysDeals"))
            {
                if (eStoreContext.Current.Store.todaysDealsColumns != null)
                {
                    this.rptTodaysDeals.DataSource = eStoreContext.Current.Store.todaysDealsColumns.Where(x=>x.MiniSite==Presentation.eStoreContext.Current.MiniSite);
                    this.rptTodaysDeals.DataBind();
                }
            }
            
        }

        protected void bindImage()
        {
            imgProduct.ImageUrl = string.Format("~/images/{0}/image_product.jpg",Presentation.eStoreContext.Current.Store.storeID);
            imgProduct.PostBackUrl = ResolveUrl("~/Product/AllProduct.aspx?type=standard");
            imgSystem.ImageUrl = string.Format("~/images/{0}/image_systems.jpg", Presentation.eStoreContext.Current.Store.storeID);
            imgSystem.PostBackUrl = ResolveUrl("~/Product/AllProduct.aspx?type=system");
            imgSolution.ImageUrl = string.Format("~/images/{0}/images_solutions.jpg", Presentation.eStoreContext.Current.Store.storeID);
        }

        protected string getShotDescription(object pc)
        {
            eStore.POCOS.ProductCategory procate = null;
            string des = "";
            if (pc != null && pc is eStore.POCOS.ProductCategory)
                procate = pc as eStore.POCOS.ProductCategory;
            if (procate != null)
            {
                des = eStore.Presentation.eStoreGlobalResource.getLocalCategoryDescription(procate);
                des = des.Length > 110 ? des.Substring(0, 110) + "..." : des;
            }
            return des;
        }
    }
}