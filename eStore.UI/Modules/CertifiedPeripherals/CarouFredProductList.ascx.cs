using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.CertifiedPeripherals
{
    public partial class CarouFredProductList : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {

        public bool ShowBorder
        {
            get { return this.ProductList1.ShowBorder; }
            set { this.ProductList1.ShowBorder = value; }
        }


        public bool ShowCompareCheckbox
        {
            get { return this.ProductList1.ShowCompareCheckbox; }
            set { this.ProductList1.ShowCompareCheckbox = value; }
        }


        public bool ShowPrice
        {
            get { return this.ProductList1.ShowPrice; }
            set { this.ProductList1.ShowPrice = value; }
        }

        public string Subject { get; set; }


        public bool ShowActions
        {
            get { return this.ProductList1.ShowActions; }
            set { this.ProductList1.ShowActions = value; }
        }
        public List<POCOS.PStoreProduct> Products
        {
            set
            {

                this.ProductList1.Products = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            BindScript("url", "jquery.easing.1.3", "jquery.easing.1.3.js");
            BindScript("url", "jquery.carouFredSel-6.2.1-packed", "jquery.carouFredSel-6.2.1-packed.js");
            BindScript("url", "jquery.colorbox", "jquery.colorbox.js");
        }
        protected override void OnPreRender(EventArgs e)
        {

            base.OnPreRender(e);
            if (this.ProductList1.Products == null || !this.ProductList1.Products.Any())
                this.Visible = false;
        }
    }
}