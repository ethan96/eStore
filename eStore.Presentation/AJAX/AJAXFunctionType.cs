using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.Presentation.AJAX
{
    public enum AJAXFunctionType
    {
        getCBOMPrice = 0,
        setCartContent = 1,
        getSearchKey = 2,
        getOderByPNSearchKey = 3,
        getProductPrice = 4,
        getSAPCompany = 5,
        getStoreCountries = 6,
        getAdBanner = 7,
        CurrencyExchange = 8,
        LowestPrice = 9,
        SuggestingProduct=10,
        ScriptTranslation=11,
        ProductPrice=12,
        getPartDependency = 13,
        getCMSByPartNumber = 14,
        CrossSellProduct=15,
        getPopularProduct = 16,
        getPstoreSearchKey = 17,
        getPstoreAdBanner = 18,
        CreateUnicaActivity = 19,
        getSearchKeyV2 = 20,
        ValidateUSAShiptoAddress = 21
    }
}
