using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.Presentation.SearchConfiguration
{
    public class eStoreSearchConfiguration : ISearchConfiguration
    {

        public POCOS.ProductSpecRules getMatchProducts(string keyword, bool includeinvalid = false, int maxCount = 9999)
        {
            return Presentation.eStoreContext.Current.Store.getMatchProducts(keyword, false, maxCount);
        }

        public AJAX.AJAXFunctionType HeaderAJAXFunctionType
        {
            get { return AJAX.AJAXFunctionType.getSearchKeyV2; }
        }

        public string ResultPageUrl
        {
            get { return "~/Search.aspx"; }
        }

        public AJAX.AJAXFunctionType AdvertisementAJAXFunctionType
        {
            get { return AJAX.AJAXFunctionType.getAdBanner; }
        }
    }
}
