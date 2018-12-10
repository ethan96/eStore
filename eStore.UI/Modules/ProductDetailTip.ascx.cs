using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class ProductDetailTip : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public POCOS.Part part { get; set; }
        public bool ShowImage { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (part == null)
                return;
            if (part is POCOS.Product)
            {
                var _product = part as POCOS.Product;
                this.lproductdesc.Text = _product.productDescX;
                this.lproductnmae.Text = _product.name;
                productstatus.ImageUrl = string.Format("~/images/{0}.gif", _product.status.ToString());
                this.lproductfeature.Text = ((POCOS.Product)part).productFeatures;
                if (ShowImage)
                    this.imgProductimg.ImageUrl = string.IsNullOrEmpty(_product.thumbnailImageX) ? "/images/photounavailable.gif" : _product.thumbnailImageX;
                else
                    this.imgProductimg.Visible = false;
            }

            else
            {
                this.lproductdesc.Text = part.productDescX;
                this.lproductnmae.Text = part.name;
                this.lproductfeature.Text = part.productFeatures;
                this.productstatus.Visible = false;
                if (ShowImage)
                    this.imgProductimg.ImageUrl = string.IsNullOrEmpty(part.thumbnailImageX) ? "/images/photounavailable.gif" : part.thumbnailImageX;
                else
                    this.imgProductimg.Visible = false;
            }

        }
    }
}