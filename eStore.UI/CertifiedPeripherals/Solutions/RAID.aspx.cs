using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using esUtilities;
using eStore.POCOS;
using eStore.POCOS.DAL;
using eStore.Presentation.SearchConfiguration;

namespace eStore.UI.CertifiedPeripherals.Solutions
{
    public partial class RAID : Presentation.eStoreBaseControls.eStoreBasePage
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
            BusinessModules.PStore pstore = BusinessModules.PStore.getInstance(Presentation.eStoreContext.Current.Store.profile);
            if (!pstore.isActive())
            {
                Response.Redirect("~/");
            }
            string style = CommonHelper.GetStoreLocation() + "Styles/CertifiedPeripherals.css";
            AddStyleSheet(style);

            BindScript("url", "jquery.easing.1.3", "jquery.easing.1.3.js");
            BindScript("url", "jquery.carouFredSel-6.2.1-packed", "jquery.carouFredSel-6.2.1-packed.js");
            BindScript("url", "jquery.colorbox", "jquery.colorbox.js");

            BindScript("url", "CertifiedPeripherals", "CertifiedPeripherals.js");

            Presentation.eStoreContext.Current.keywords.Add("KeyWords", "CertifiedPeripheralsRAIDSolutions");
            if (!IsPostBack)
            {
                List<POCOS.StoreProductBundleList> bundlelist = pstore.getSolutionList();
                if (bundlelist != null && bundlelist.Any())
                {
                    var bundles = (from b in bundlelist
                                   group b by b.GroupName into g
                                   select new
                                   {
                                       GroupName = g.Key
                                       ,
                                       hasInvalidItems=g.Any(x=>x.partX==null||!x.partX.isOrderable())
                                       ,
                                       GroupItems = (from gi in g
                                                    where gi.partX!=null && gi.partX.isOrderable()
                                                     group gi by gi.StoreProductBundleId into g2
                                                     select new
                                                     {
                                                         TotalPrice = g2.Sum(x => x.BundlePricePerSet * x.QuantityPerSet),
                                                         BundleId=g2.Key,
                                                         GroupName=g.Key,
                                                         Items = (from g2i in g2
                                                                  select new
                                                                  {
                                                                      g2i.QuantityPerSet,
                                                                      g2i.partX.productDescX,
                                                                      g2i.partX.name,
                                                                      g2i.partX.thumbnailImageX,
                                                                      Url = ResolveUrl(Presentation.UrlRewriting.MappingUrl.getMappingUrl(g2i.partX))

                                                                  })

                                                     })
                                   }).Where(x=>x.hasInvalidItems==false);

                    rpBundles.DataSource = bundles;
                    rpBundles.DataBind();
                }
            
            }
        }

        protected void lbAdd2Cart_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton s = (LinkButton)sender;
                string[] paras = s.CommandArgument.Split('|');

                POCOS.Product_Bundle bundleproduct = getComboProducts(int.Parse(paras[0]), paras[1]);
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
 

        /// <summary>
        /// this is for solution
        /// </summary>
        /// <param name="bundleid"></param>
        /// <param name="itemgroup"></param>
        /// <returns></returns>
        POCOS.Product_Bundle getComboProducts(int bundleid, string itemgroup)
        {
            BusinessModules.PStore pstore = BusinessModules.PStore.getInstance(Presentation.eStoreContext.Current.Store.profile);

            List<POCOS.StoreProductBundleList> BundleList = pstore.getSolutionList();
            if (BundleList == null && !BundleList.Any())
                return null;
            List<StoreProductBundleList> items = BundleList.Where(x => x.StoreProductBundleId == bundleid && x.GroupName.Equals(itemgroup, StringComparison.OrdinalIgnoreCase)).ToList();
            if (items == null || !items.Any())
                return null;
            POCOS.Part virtualBundleProduct = (new PartHelper()).getPart("ePAPS-Bundle", pstore.storeID);
            if (virtualBundleProduct == null || !(virtualBundleProduct is POCOS.Product_Bundle))
                return null;
            Bundle initBundle = new Bundle();

            for (int i = 0; i < items.Count; i++)
            {
                StoreProductBundleList item = items[i];
                BundleItem bundleItem = new BundleItem(item.partX, item.QuantityPerSet ?? 1, i + 1, item.BundlePricePerSet);
                initBundle.BundleItems.Add(bundleItem);
            }

            Product_Bundle productBundle = ((Product_Bundle)virtualBundleProduct).clone(initBundle, bundleid);
            productBundle.DisplayPartno = items[0].StoreProductBundle.BundlePartNo;
            productBundle.ProductDesc = items[0].StoreProductBundle.Note + " - " + itemgroup;
            productBundle.CostX = -1;
            productBundle.initialize();

            return productBundle;
        }

    }
}