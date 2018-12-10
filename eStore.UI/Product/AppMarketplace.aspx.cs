using eStore.POCOS.DAL;
using eStore.Presentation;
using eStore.Presentation.eStoreBaseControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Product
{
    public partial class AppMarketplace : BaseProduct
    {
        public override bool OverwriteMasterPageFile
        {
            get
            {
                return false;
            }
        }
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

        private int _productWidgetId = 0;
        public int productWidgetId
        {
            get { return _productWidgetId; }
        }
        protected int componentid = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            this.AddStyleSheet(ResolveUrl("~/App_Themes/V4/byAppmarketplace.css"));
            POCOS.Product CurrentProduct = getProduct();


            imgLargePicture.ImageUrl = CurrentProduct.thumbnailImageX;

            SelectyourSystem.NavigateUrl = $"{Request.RawUrl}#SelectyourSystemspan";
            imgLargePicture.AlternateText = CurrentProduct.productDescX;
            imgLargePicture.ToolTip = CurrentProduct.SProductID;


            this.lProductName.Text = CurrentProduct.name;
            this.lShortDescription.Text = CurrentProduct.productDescX;

            this.YouAreHereMutli1.ProductName = CurrentProduct.name;
            this.YouAreHereMutli1.productCategories = CurrentProduct.productCategories.Where(x => x.MiniSite == Presentation.eStoreContext.Current.MiniSite).ToList();
            //ChangeCurrency1.defaultListPrice = product.getListingPrice().value;
            Presentation.eStoreContext.Current.keywords.Add("ProductID", CurrentProduct.SProductID);
            Presentation.eStoreContext.Current.keywords.Add("KeyWords", CurrentProduct.Keywords);
            Presentation.eStoreContext.Current.BusinessGroup = CurrentProduct.businessGroup;
            this.lPrice.Text = Presentation.Product.ProductPrice.getPrice(CurrentProduct);
            var resource = CurrentProduct.productResourcesX;
            var youtube = resource.FirstOrDefault(x => x.ResourceType == "YoutubeVideo");
            var Manual = resource.FirstOrDefault(x => x.ResourceType == "Manual");
            if (youtube != null)
            {
                hlDemo.NavigateUrl = youtube.ResourceURL;
                hlDemo.Visible = true;
                this.BindScript("showVideo", "showVideo", "showVideo();");
            }
            if (Manual != null)
            {
                hlmanual.NavigateUrl = Manual.ResourceURL;
                hlmanual.Visible = true;
            }

           
            if (int.TryParse(CurrentProduct.VProductID, out componentid))
            {
                PartHelper producthelper = new PartHelper();
                List<POCOS.Product> products = null;
                products = producthelper.getCTOSProductsbyComponentid(componentid, Presentation.eStoreContext.Current.Store.storeID);

                ProductsComparison1.parts = products.ToList();

            }



            setCanonicalPage(Presentation.UrlRewriting.MappingUrl.getMappingUrl(CurrentProduct));
            this.setPageMeta(CurrentProduct.pageTitle, CurrentProduct.metaData, CurrentProduct.keywords);

            OpenGraphProtocolAdapter OpenGraphProtocolAdapter = new OpenGraphProtocolAdapter(CurrentProduct);
            OpenGraphProtocolAdapter.addOpenGraphProtocolMetedata(this.Page);

            //20160809 Alex:add Product page structured date 
            StructuredDataMarkup structuredDataMarkup = new StructuredDataMarkup();
            structuredDataMarkup.GenerateProductStruturedData(CurrentProduct, this.Page);
            structuredDataMarkup.GenerateBreadcrumbStruturedData(CurrentProduct, this.Page);
            structuredDataMarkup.GenerateLPSections(CurrentProduct, this.Page);
            if (eStoreContext.Current.getBooleanSetting("hasProductWidget") && CurrentProduct.widgetPagesX != null && CurrentProduct.widgetPagesX.Any())
            {
                var widget = CurrentProduct.widgetPagesX.OrderByDescending(c => c.Id).FirstOrDefault();
                if (widget != null)
                    _productWidgetId = widget.WidgetPageID.GetValueOrDefault();
            }

        }



        private POCOS.Product getProduct()
        {
            string ProductId = esUtilities.CommonHelper.QueryString("ProductID");
            base.ProductNo = ProductId;
            //设置Popular Product id
            //if (eStore.Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("EnablePopularProduct"))
            //    AdPopularProduct1.SproductId = ProductId;

            POCOS.Product product = Presentation.eStoreContext.Current.Store.getProduct(ProductId, true);
            if (product != null && !IsPostBack)
            {
                this.UserActivitLog.ProductID = product.SProductID;
                this.UserActivitLog.CategoryType = product.productType.ToString();
                this.UserActivitLog.CategoryName = product.pageTitle;
            }
            if (product == null)
            {
                this.UserActivitLog.ProductID = ProductId;
                this.UserActivitLog.CategoryType = "ErrorPart";
                //this.UserActivitLog.CategoryName = product.pageTitle;
                base.ToSearch();
            }
            return product;
        }
    }
}