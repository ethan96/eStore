using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.POCOS;

namespace eStore.UI.Modules.CertifiedPeripherals
{
    public partial class BundleOnly : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public List<StoreProductBundleList> Bundles;
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected override void OnPreRender(EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Bundles != null && Bundles.Any())
                {
                    rpMB.DataSource = Bundles.Where(x => x.IsPrimary);
                    rpMB.DataBind();
                    rpProducts.DataSource = Bundles.Where(x => !x.IsPrimary);
                    rpProducts.DataBind();
                    StoreProductBundleList sbpl = Bundles.Where(x => !x.IsPrimary).FirstOrDefault();
                    if (sbpl != null)
                        lbundlegourpname.Text = sbpl.GroupName;
                    this.Visible = true;
                }
                else
                {
                    this.Visible = false;
                }
            }
            base.OnPreRender(e);
        }

        protected void rpMB_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                StoreProductBundleList bundleitem = (StoreProductBundleList)e.Item.DataItem;
                HyperLink hlproductimg = e.Item.FindControl("hlproductimg") as HyperLink;
                hlproductimg.ImageUrl = bundleitem.partX.thumbnailImageX;
                string producturl = eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(bundleitem.partX); 
                hlproductimg.NavigateUrl = producturl;

                HyperLink hlproductname = e.Item.FindControl("hlproductname") as HyperLink;
                hlproductname.Text = bundleitem.partX.name;
                hlproductname.NavigateUrl = producturl;

                Literal lproductDescX = e.Item.FindControl("lproductDescX") as Literal;
                lproductDescX.Text = bundleitem.partX.productDescX;


                Literal lproductFeatures = e.Item.FindControl("lproductFeatures") as Literal;
                lproductFeatures.Text = bundleitem.partX.productFeatures;

                Literal lcomparecheckbox = e.Item.FindControl("lcomparecheckbox") as Literal;
                lcomparecheckbox.Text = string.Format("<div class=\"epaps-compareBlock\"><input type=\"checkbox\" name=\"ckbcompare\" id=\"ckbcompare\"  value=\"{0}\"><label for=\"ckbcompare\">Compare Product</label></div>", bundleitem.partX.SProductID);
                 
                if(bundleitem.partX is PStoreProduct ){
                PStoreProduct ppart=(PStoreProduct)bundleitem.partX;
                        Literal lpstorefeatures = e.Item.FindControl("lpstorefeatures") as Literal;
                        lpstorefeatures.Text = string.Format("<span class=\"epaps-{0}\"></span><div class=\"epaps-productlogo\"><img src=\"https://wfcache.advantech.com/www/certified-peripherals/documents/LOGO/{1}.png\" /></div>"
                            , ppart.Manufacturer, ppart.SProductID);
              
                }
                
              
                 
                                
            }
        }
    }
}