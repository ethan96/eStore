using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace eStore.UI.Product
{
    public partial class PrintComparisonTable : Presentation.eStoreBaseControls.eStoreBasePagePrint
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //<meta name="robots" content="noodp">
            HtmlMeta ogImage = new HtmlMeta();
            ogImage.Attributes.Add("name", "robots");
            ogImage.Attributes.Add("content", "noodp");
            this.Header.Controls.Add(ogImage);
            this.isExistsPageMeta = setPageMeta(
     $"Print Compare List - {Presentation.eStoreContext.Current.Store.profile.StoreName}", $"{Presentation.eStoreContext.Current.Store.profile.StoreName} Comparison Results", "");

        }
    }
}