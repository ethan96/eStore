using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.CertifiedPeripherals
{
    public partial class AJAXProductList : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public List<POCOS.PStoreProduct> Products;
        protected string productsdata = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
       
            var productmodel = (from p in Products
                                select new
                                {
                                    p.SProductID,
                                    p.name,
                                    p.productDescX,
                                    p.thumbnailImageX,
                                    PromotionType = p.pStorePromotionType == POCOS.StoreDeal.PromotionType.Other ? "" : p.pStorePromotionType.ToString(),
                                    p.Manufacturer,
                                    Price = Presentation.Product.ProductPrice.getPrice(p),
                                    Url = ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(p)),
                                    p.ProductCategoryId,
                                    CheckedComparision=false,
                                    specs = p.simpleSpec
                                });
            string ps=Newtonsoft.Json.JsonConvert.SerializeObject(productmodel);
            productsdata = string.IsNullOrEmpty(ps) ? "[]" : ps;
        }
    }
}