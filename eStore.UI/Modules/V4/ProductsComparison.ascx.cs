using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.V4
{
    public partial class ProductsComparison : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected string comparisionproducts
        {
            get
            {

                string content = string.Empty;
                string mobilecontent = string.Empty;
                bool contentloaded = false, mobilecontentloaded = false;

                Presentation.Product.ProductCompareManagement mgt = new Presentation.Product.ProductCompareManagement();
                if (Request.Browser.IsMobileDevice)
                {
                    mobilecontent = mgt.generateCompareMobileUI(parts);
                    mobilecontentloaded = true;
                }
                else
                {
                    content = mgt.generateCompareUI(parts);
                    contentloaded = true;
                }

                return $@"<div class=""eStore_compare_content"">
            <h2 class=""row20"">
               {Title}<span><a href=""#"">
                    <img src=""{esUtilities.CommonHelper.ResolveUrl("~/")}images/eStore_print.png"" onclick=""window.print();return false;"" alt=""print this page"" /></a>
                </span><span id=""pager"" class=""pager""></span>
            </h2>
            <div class=""productlist""  data-loaded=""{contentloaded}"" data-init=""False"">
                {content}
            </div>
        </div>
  
        <div class=""eStore_compare_content_Mobile"">
            <h2>
                {Title}</h2>
            <div class=""productlist"" data-loaded=""{mobilecontentloaded}"" data-init=""False"">
                {mobilecontent}
            </div>
        </div>";
            }
        }
        public List<POCOS.Product> parts;
        public string Title { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected override void OnPreRender(EventArgs e)
        {
            if(parts != null)
                ScriptManager.RegisterArrayDeclaration(this, "comparisionproducts", string.Join(",", parts.Select(x => "'" + x.SProductID + "'").ToArray()));
            base.OnPreRender(e);
        }
    }
}