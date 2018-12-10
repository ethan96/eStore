using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using esUtilities;
using eStore.POCOS;
using eStore.Presentation.SearchConfiguration;

namespace eStore.UI.CertifiedPeripherals
{
    public partial class Product : Presentation.eStoreBaseControls.eStoreBasePage
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
            if (!string.IsNullOrEmpty(Request["ProductID"]))
            {
                POCOS.Product part = Presentation.eStoreContext.Current.Store.getProduct(Request["ProductID"]);
                if (part != null)
                {
                    Response.Redirect(Presentation.UrlRewriting.MappingUrl.getMappingUrl(part));
                }
                else
                {
                    Presentation.eStoreContext.Current.AddStoreErrorCode("Product is not available", null, true, 404);
                    return;
                }
            }


            BusinessModules.PStore pstore = BusinessModules.PStore.getInstance(Presentation.eStoreContext.Current.Store.profile);
            if (!pstore.isActive())
            {
                Response.Redirect("~/");
            }
            string style = CommonHelper.GetStoreLocation() + "Styles/CertifiedPeripherals.css";
            AddStyleSheet(style);
            lAdd2Cart.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoCart);
            
            if (!string.IsNullOrEmpty(Request["ProductID"]))
            {
                POCOS.Part part = Presentation.eStoreContext.Current.Store.getPart(Request["ProductID"]);
                if (part != null && part is POCOS.PStoreProduct)
                {
                    POCOS.PStoreProduct product = (POCOS.PStoreProduct)part;
                    if (!product.isOrderable())
                    {
                        Presentation.eStoreContext.Current.AddStoreErrorCode("Product is not available", null, true, 410);
                        return;
                    }
                    lAdd2Cart.Visible = product.isOrderable() && product.getListingPrice().value > 0;
                    if (product.isLongevity)
                    {
                        iconLongevity.Visible = true;
                    }
                    else
                        iconLongevity.Visible = false;

                    if (product.LimitQty.HasValue && product.LimitQty.GetValueOrDefault() > 0)
                    {
                        lLimitedQuantity.Text = "Limited Quantity";
                    }
                    if (product.SpecialShipment.HasValue && product.SpecialShipment.GetValueOrDefault())
                    {
                        lSpecialShipment.Text = "Call for shipping options";
                        lAdd2Cart.Visible = false;
                    }
                    hlSpecialOrder.Visible = product.SpecialOrder.GetValueOrDefault(false);
                    hlSpecialOrder.NavigateUrl = ResolveUrl("~/information.aspx?type=PStoreReturnPolicy");
                    Presentation.eStoreContext.Current.keywords.Add("ProductID", product.SProductID);
                    YouAreHere1.productCategory = product.category;
                    YouAreHere1.ProductName = product.name;
                    if (product.BundleList != null && product.BundleList.Any())
                    {
                        if (product.IsSoldInBundleOnly)
                        {
                      
                           //this.BundleOnly1.Bundles = product.BundleList;
                       
                        }
                        else
                        {
                            this.BundleWith1.product = product;
                            this.BundleWith1.Bundles = product.BundleList;
                        
                        }
                    }
                    this.isExistsPageMeta = this.setPageMeta(product.pageTitle, product.metaData, product.keywords);

                    string features = product.productFeatures;

                    if (!string.IsNullOrEmpty(features))
                    {
                        lfeatures.Text = string.Format("<ul class='epaps-productCol-features'>{0}{1}</ul>", features, pstore.policylink());
                    }

                    List<  POCOS.Advertisement> ads = pstore.getTodaysDeals(product);
                  if (ads != null)
                  {
                      var ds = (from ad in ads
                                select new
                                {
                                    Hyperlink = esUtilities.CommonHelper.ConvertToAppVirtualPath(ad.Hyperlink)
                                    ,
                                    Title = ad.AlternateText
                                    ,
                                    image = ad.Imagefile.StartsWith("http", true, null) ? ad.Imagefile : String.Format("{0}resource{1}", esUtilities.CommonHelper.GetStoreLocation(), ad.Imagefile)
                                    ,
                                    Target = string.IsNullOrEmpty(ad.Target) ? "_self" : ad.Target
                                }

                                );
                      rpvideo.DataSource = ds;
                      rpvideo.DataBind();
                  }
              

                  List<ProductResource> productResourcesX = pstore.getPAPSProductResource(product.StoreID, product.SProductID);
                  if (productResourcesX != null && productResourcesX.Any(x => x.ResourceType == "LargeImages"))
                    {
                        var largeimages = productResourcesX.Where(x => x.ResourceType == "LargeImages").ToList();
                        imgProductImage.ImageUrl = largeimages.First().ResourceURL;
                        if (largeimages.Count > 0)
                        {

                            rpThumbnails.DataSource = largeimages;
                            rpThumbnails.DataBind();
                            rpThumbnails.Visible = true;
                            this.BindScript("url", "lightbox", "jquery.lightbox-0.5.min.js");
                            this.BindScript("Script", "lightBox", "$(function() {$(\".epaps-listImg a\").lightBox({maxHeight: 600,maxWidth: 800}); });");
                            this.AddStyleSheet(ResolveUrl("~/Styles/jquery.lightbox-0.5.css"));
                            this.BindScript("url", "productaddons", "productaddons.js");
                        }
                        else
                            rpThumbnails.Visible = false;
                    }
                    else
                    { imgProductImage.ImageUrl = product.thumbnailImageX; }
                 
                 //   ImgThumbnail.ImageUrl = product.thumbnailImageX;
                    ldescript.Text = product.productDescX;
                    lspecdesc.Text = product.productDescX;

                    lIsFreeShipping.Visible = product.FreeShipping == null ? false : (bool)product.FreeShipping;
                    lPrice.Text = eStore.Presentation.Product.ProductPrice.getPrice(product);

                    if (product.associate.Any())
                    {
                        AssociateProductslist.Subject = "Associate Products";
                        AssociateProductslist.Products = product.associate;
                    }
             
                    if (product.youMayAlsoBuy.Any())
                    {
                        YouMayAlsoBuylist.Subject = "You May Also Buy";
                        YouMayAlsoBuylist.Products = product.youMayAlsoBuy;
                    }
 
                    if (product.specs!=null && product.specs.Any())
                    {
                        rpSpec.DataSource = product.specs;
                        rpSpec.DataBind();
                        ldescript.Text = product.productDescX;
                        specpanle.Visible = true;

                    }
                    else
                    {
                        specpanle.Visible = false;
                    }

                }
                else
                {
                    Presentation.eStoreContext.Current.AddStoreErrorCode("Product is not available", null, true,404);
                    return;
                }

            }
            else
            {
                Presentation.eStoreContext.Current.AddStoreErrorCode("Product is not available", null, true, 404);
                return;


            }

        }

        protected override void OnPreRender(EventArgs e)
        {
            BindScript("url", "CertifiedPeripherals", "CertifiedPeripherals.js");

            base.OnPreRender(e);
        }

        protected void lAdd2Cart_Click(object sender, EventArgs e)
        {
             POCOS.Part part = Presentation.eStoreContext.Current.Store.getPart(Request["ProductID"]);
                if (part != null && part is POCOS.PStoreProduct)
                {
                    POCOS.Cart cart = Presentation.eStoreContext.Current.UserShoppingCart;
                    CartItem cartitem = cart.addItem(part, 1);
                    cart.save();
                    this.Response.Redirect("~/Cart/Cart.aspx");
                }
     }
    }
}