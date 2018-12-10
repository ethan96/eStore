using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.POCOS;

namespace eStore.UI.Modules.CertifiedPeripherals
{   
    public partial class BundleWith : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public List<StoreProductBundleList> Bundles;
        public PStoreProduct product { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
        
        }
        protected override void OnPreRender(EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Bundles != null && Bundles.Any())
                {
                    var Primaryitem = Bundles.FirstOrDefault(x => x.IsPrimary);

                    if (Primaryitem != null)
                    {
                        Bundles.ForEach(x => x.basePrice = Primaryitem.BundlePricePerSet);
                    }
                    else
                    {
                        this.Visible = true;
                        return;
                    }
                    var ds = (from b in Bundles
                              where !b.IsPrimary
                              group b by b.GroupName into g
                              select new
                              {
                                  GroupName = g.Key,
                                  Products = g.ToList()
                              }).ToList();
                    rpbwm.DataSource = ds;
                    rpbwm.DataBind();
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

                Literal lproductDescX = e.Item.FindControl("lproductDescX") as Literal;
                lproductDescX.Text = bundleitem.Note;

               
                LinkButton lAdd2Cart = e.Item.FindControl("lAdd2Cart") as LinkButton;
                lAdd2Cart.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoCart);
            
                decimal itemlistprice = bundleitem.partX.getListingPrice().value;
                decimal bundelListprice = 0m;
                decimal bundeMarkupprice = ((bundleitem.QuantityPerSet ?? 1) * itemlistprice) + product.getListingPrice().value;
                if (bundleitem.BundlePricePerSet > 0)
                {
                    bundelListprice=((bundleitem.QuantityPerSet ?? 1) * bundleitem.BundlePricePerSet) + bundleitem.basePrice;
                }
                else
                {
                    bundelListprice = bundeMarkupprice;
                }
              
                if (bundelListprice < bundeMarkupprice) {
                    Literal lBundleMarkupPrice = e.Item.FindControl("lBundleMarkupPrice") as Literal;
                    lBundleMarkupPrice.Text = Presentation.Product.ProductPrice.FormartPstorePrice(bundeMarkupprice);

                }
                Literal lBundleListPrice = e.Item.FindControl("lBundleListPrice") as Literal;
                lBundleListPrice.Text = Presentation.Product.ProductPrice.FormartPstorePrice(bundelListprice);
                //if (product.FreeShipping.GetValueOrDefault(false) || bundleitem.StoreProductBundle.FreeShipping.GetValueOrDefault(false))
                if (bundleitem.StoreProductBundle.FreeShipping.GetValueOrDefault(false))
                {
                    Literal lFreeShipping = e.Item.FindControl("lFreeShipping") as Literal;
                    lFreeShipping.Text = string.Format("<div class=\"epaps-productFreeShipping\">  <img src='{0}' /></div>"
                        , ResolveUrl("~/images/AUS/eStore_icon_shipping.png"));
                }
                if (bundleitem.partX is PStoreProduct)
                {
                    PStoreProduct ppart = (PStoreProduct)bundleitem.partX;
                    if (!string.IsNullOrEmpty(ppart.Manufacturer))
                    {
                        Literal lpstorefeatures = e.Item.FindControl("lpstorefeatures") as Literal;
                        lpstorefeatures.Text = string.Format("<div class=\"epaps-productlogo\"><img src=\"https://wfcache.advantech.com/www/certified-peripherals/documents/LOGO/{0}.png\" /></div>"
                           , ppart.Manufacturer);
                    }
                }

            }
        }

        protected void lAdd2Cart_Click(object sender, EventArgs e)
        {

            if (product != null)
            {
                try
                {
                    LinkButton s = (LinkButton)sender;
                    string[] paras = s.CommandArgument.Split('|');

                    POCOS.Product_Bundle bundleproduct = product.getComboProducts(int.Parse(paras[0]), int.Parse(paras[1]));
                    if (bundleproduct != null)
                    {
                        POCOS.Cart cart = Presentation.eStoreContext.Current.UserShoppingCart;
                        CartItem cartitem = cart.addItem(bundleproduct, 1, null, bundleproduct.bundle.originalPrice);
                        cart.save();
                        this.Response.Redirect("~/Cart/Cart.aspx");
                    }
                }
                catch (Exception)
                {
                    
                 
                }
                
            }
        }
    }
}