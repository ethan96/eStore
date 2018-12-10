using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation;
using esUtilities;
using System.Text;

namespace eStore.UI.Modules
{
    public partial class Product3DModel : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public POCOS.Part part { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (part == null)
                return;
            StringBuilder ProductBuilder = new StringBuilder();

            foreach (POCOS.ProductResource pr in part.ProductResources)
            {
                if (pr.ResourceType == "3DModel")
                {
                    ProductBuilder.AppendFormat("<li><a href=\"{0}\" target=\"_blank\"  title=\"{1}\">Download <b>{1}</b> Format</a></li>", pr.ResourceURL, pr.ResourceName);
                }
                //Download IGS format
            }
            lProductName.Text = part.SProductID;
            if (ProductBuilder.Length != 0)
                lProductFeature.Text = ProductBuilder.ToString();
            else
                lProductFeature.Text = "<li>&nbsp;</li><li>Have No 3D Model!</li><li>&nbsp;</li>";

        }
    }
}