using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.Presentation.eStoreBaseControls
{
    public class BaseProduct : eStoreBasePage
    {
        private POCOS.Product _CurrProduct;

        public POCOS.Product CurrProduct
        {
            get { return _CurrProduct; }
            set
            {
                _CurrProduct = value;
                if (_CurrProduct != null)
                    ProductNo = _CurrProduct.SProductID;
            }
        }

        public string ProductNo { get; set; }
        public void ToSearch()
        {
            if (Session["fromStore"] != null)
            {
                string formStoreId = Session["fromStore"].ToString();
                var protemp = Presentation.eStoreContext.Current.Store.getPart(ProductNo, formStoreId);
                if (protemp != null)
                {
                    if (protemp is POCOS.Product_Ctos)
                    {
                        var prols = eStore.Presentation.eStoreContext.Current.Store.SearchCtosWithDefaultPartNo(formStoreId, ProductNo);
                        if (prols.Any())
                            Response.Redirect(esUtilities.CommonHelper.GetStoreLocation(UseSSL) + $"Compare.aspx?parts={string.Join(",", prols.Select(c => c.TarSProductID))}&fp={ProductNo}");
                    }
                    else if (protemp is POCOS.Product_Bundle)
                    {

                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(protemp.ModelNo))
                            Response.Redirect(esUtilities.CommonHelper.GetStoreLocation(UseSSL) + $"Search.aspx?skey={protemp.ModelNo}");
                    }
                }
            }
            Response.Redirect(esUtilities.CommonHelper.GetStoreLocation(UseSSL) + $"Search.aspx?skey={ProductNo}");

        }
    }
}
