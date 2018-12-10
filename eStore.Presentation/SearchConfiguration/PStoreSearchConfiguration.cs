using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.Presentation.SearchConfiguration
{
    public class PStoreSearchConfiguration : ISearchConfiguration
    {

        public AJAX.AJAXFunctionType HeaderAJAXFunctionType
        {
            get { return AJAX.AJAXFunctionType.getPstoreSearchKey; }
        }

        public string ResultPageUrl
        {
            get { return "~/CertifiedPeripherals/Search.aspx"; }
        }

        public AJAX.AJAXFunctionType AdvertisementAJAXFunctionType
        {
            get { return AJAX.AJAXFunctionType.getPstoreAdBanner; }
        }
    }
}
