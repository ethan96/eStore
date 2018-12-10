using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation;
using eStore.POCOS.DAL;
using eStore.POCOS;
using System.Web.DynamicData;
using eStore.Presentation.Product;

namespace eStore.UI.Modules
{
    public partial class ProductPrint : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public POCOS.Product product { get; set; }
        bool? _ShowATP = null;
        public Boolean ShowATP
        {
            get
            {
                if (_ShowATP == null)
                {
                    if (Presentation.eStoreContext.Current.User == null)
                        _ShowATP = false;
                    else
                        _ShowATP = Presentation.eStoreContext.Current.User.actingUser.hasRight(POCOS.User.ACToken.ATP);
                }
                return _ShowATP ?? false;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (product != null)
                {
                    RenderProduct(product);
                }             
            }
        }


        private void RenderProduct(POCOS.Product product)
        {
            this.lProductName.Text = product.name;
            this.lShortDescription.Text = product.productDescX;
            this.lProductFeature.Text = product.productFeatures;
            this.lProductprice.Text = Presentation.Product.ProductPrice.getPrice(product);

            //this.ProductLiterature1.AddPart((POCOS.Part)product);
            imgProduct.ImageUrl = product.thumbnailImageX;

            IEnumerable<RelatedProduct> RelatedProducts = product.getAccessories(1);

            IEnumerable<PeripheralCompatible> PeripheralCompatibles = product.PeripheralCompatibles;
            if (product is Product_Bundle)
            {
                Product_Bundle currentBundle = product as Product_Bundle;
                PeripheralCompatibles = currentBundle.peripheralCompatiblesX;
                if (currentBundle.bundle != null && currentBundle.bundle.BundleItems.Any())
                {
                    this.rpBundletems.DataSource = currentBundle.bundle.BundleItems;
                    this.rpBundletems.DataBind();
                }
            }
            if (PeripheralCompatibles != null)
            {
                this.rpPeripheralCompatibles.DataSource = (from pc in PeripheralCompatibles
                                                           where pc.PeripheralProduct.Publish == true
                                                           group pc.PeripheralProduct by pc.PeripheralProduct.PeripheralsubCatagory.PeripheralCatagory
                                                               into _peripheralProduct
                                                               select
                                                               new PerpherailProductXX
                                                               {
                                                                   ID = _peripheralProduct.Key.catagoryID,
                                                                   CategoryName = _peripheralProduct.Key.name,
                                                                   peripheralProduct = _peripheralProduct.Where(x => x.partIsOrderable == true).ToList(),
                                                                   seq = _peripheralProduct.Key.seq
                                                               }).OrderBy(c => c.seq).ToList();
            }

            if (ShowATP)
            {
                //acquire ATP value of relatedProducts
                PartHelper helper = new PartHelper();
                Dictionary<Part, int> updatingList = new Dictionary<Part, int>();
                if (PeripheralCompatibles != null)
                {
                    foreach (PeripheralCompatible pc in PeripheralCompatibles)
                    {
                        foreach (var itemPart in pc.PeripheralProduct.partsX)
                        {
                            if (itemPart != null && !updatingList.ContainsKey(itemPart))
                                updatingList.Add(itemPart, 1);
                        }
                    }
                }
                foreach (RelatedProduct rp in product.RelatedProductsX)
                {
                    if (!updatingList.ContainsKey(rp.RelatedPart))
                        updatingList.Add(rp.RelatedPart, 1);
                }

                //invoke PartHelper to update ATP information of the parts listed in updatingList
                helper.setATPs(Presentation.eStoreContext.Current.Store.profile, updatingList);
            }

            this.rpPeripheralCompatibles.DataBind();
            this.rpPeripheralCompatibles.Visible = true;

            this.rpRelatedProducts.DataSource = (from rp in product.RelatedProductsX
                                                 select rp.RelatedPart);
            this.rpRelatedProducts.DataBind();


            IList<VProductMatrix> vProductMatrixs = product.specs;
            vProductMatrixs = vProductMatrixs.Where(p => p.LocalCatName != "Base").ToList();
            if (vProductMatrixs.Count() > 0)
            {
                this.gvSpec.DataSource = vProductMatrixs.ToList(); 
                this.gvSpec.DataBind();
            }else{
                h3CatAttribute.Visible = false;
            }
        }

        protected void rpPeripheralCompatibles_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Repeater rpPeripheralProducts = e.Item.FindControl("rpPeripheralProducts") as Repeater;
                PerpherailProductXX pc = e.Item.DataItem as PerpherailProductXX;
                rpPeripheralProducts.DataSource = pc.peripheralProduct.OrderBy(c => c.SProductID).ToList();
                rpPeripheralProducts.DataBind();
            }
        }
    }
}