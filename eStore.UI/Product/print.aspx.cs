using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation.eStoreBaseControls;
using System.Web.UI.HtmlControls;

namespace eStore.UI.Product
{
    public partial class print : eStoreBasePagePrint
    {
        protected void Page_Load(object sender, EventArgs e)
        {//<meta name="robots" content="noodp">
            HtmlMeta ogImage = new HtmlMeta();
            ogImage.Attributes.Add("name", "robots");
            ogImage.Attributes.Add("content", "noodp");
            this.Header.Controls.Add(ogImage);
            POCOS.Product product = getProduct();
            if (product != null)
            {
                this.isExistsPageMeta = setPageMeta(
$"Print - {product.name} - {Presentation.eStoreContext.Current.Store.profile.StoreName}", $"Print - {product.name} - {Presentation.eStoreContext.Current.Store.profile.StoreName}", "");

            }
            else
            {
                this.isExistsPageMeta = setPageMeta(
$"Print Product - {Presentation.eStoreContext.Current.Store.profile.StoreName}",
$"Print Product - {Presentation.eStoreContext.Current.Store.profile.StoreName}", "");

            }
        }
        protected override void OnInit(EventArgs e)
        {
            this.EnsureChildControls();
            base.OnInit(e);
        }

        protected override void CreateChildControls()
        {
            if (!base.ChildControlsCreated)
            {
                base.CreateChildControls();
                this.LoadPrintControl();
                base.ChildControlsCreated = true;
            }
        }

        private void LoadPrintControl()
        {
            POCOS.Product product = getProduct();

            if (product is POCOS.Product_Ctos)
            {
                eStore.UI.Modules.CTOSPrint printer = (eStore.UI.Modules.CTOSPrint)LoadControl("~/Modules/CTOSPrint.ascx");
                printer.product = product;
                this.phPrinter.Controls.Add(printer);
            }

            else if (product is POCOS.Product)
            {
                eStore.UI.Modules.ProductPrint printer = (Modules.ProductPrint)LoadControl("~/Modules/ProductPrint.ascx");
                printer.product = product;
                this.phPrinter.Controls.Add(printer);
            }

        }
        //Get Product Property
        private POCOS.Product getProduct()
        {
            string ProductId = esUtilities.CommonHelper.QueryString("ProductID");
            POCOS.Product _product = Presentation.eStoreContext.Current.Store.getProduct(ProductId);
            return _product;
        }
    }
}