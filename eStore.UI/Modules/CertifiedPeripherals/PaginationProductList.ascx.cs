using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using esUtilities;

namespace eStore.UI.Modules.CertifiedPeripherals
{
    public partial class PaginationProductList : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public bool ShowCompareCheckbox
        {
            get { return this.ProductList1.ShowCompareCheckbox; }
            set { this.ProductList1.ShowCompareCheckbox = value; }
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

            BindScript("url", "jquery.simplePagination.js", "jquery.simplePagination.js");
           string style = CommonHelper.GetStoreLocation() + "Styles/simplePagination.css";
           AddStyleSheet(style);
        }
    }
}