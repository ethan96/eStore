using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using esUtilities;

namespace eStore.UI.Modules.IoTMart
{
    public partial class PaginationProductList : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        private int _count = 15;
        public int count
        {
            get { return _count; }
            set { _count = value; }
        }

        private bool? _isShowPagination = null;

        public bool isShowPagination
        {
            get 
            {
                if (_isShowPagination == null)
                    _isShowPagination = products.Count > _count;
                return _isShowPagination.GetValueOrDefault(); 
            }
            set { _isShowPagination = value; }
        }


        private string _newid;
        protected string newid
        {
            get 
            { 
                if(string.IsNullOrEmpty(_newid))
                {
                    if(_category != null)
                        _newid = _category.CategoryPath;
                    else
                        _newid = this.ClientID;
                }
                return _newid; 
            }
        }


        private POCOS.ProductCategory _category;
        public POCOS.ProductCategory category
        {
            get { return _category; }
            set 
            { 
                _category = value;
                _products = value.productList ?? new List<POCOS.Product>();
            }
        }
        

        private List<POCOS.Product> _products = new List<POCOS.Product>();
        public List<POCOS.Product> products
        {
            get { return _products; }
            set { _products = value ?? new List<POCOS.Product>(); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            bindProducts();
        }

        void bindProducts()
        {
            if (_products != null && _products.Any())
            {
                var pros = from p in _products
                           select new
                           {
                               SProductID = p.SProductID,
                               name = p.DisplayPartno,
                               desc = p.productDescX,
                               img = p.thumbnailImageX,
                               InCompareList = Presentation.Product.ProductCompareManagement.GetCompareProductsIds().Exists(x => x == p.SProductID),
                               price = Presentation.Product.ProductPrice.getAJAXProductPrice(p),
                               link = ResolveUrl(Presentation.UrlRewriting.MappingUrl.getMappingUrl(p)),
                               statusseq = ((int)p.status < (int)POCOS.Product.PRODUCTSTATUS.PHASED_OUT) ? 0 : 1,
                               listingOrder = (p.isOrderable(true) && p.getListingPrice().value > 0) ? 0 : 1,
                               status = "<img src='" + esUtilities.CommonHelper.GetStoreLocation(false) + "images/" + p.status + ".gif'/>",
                               phasedOut = p.notAvailable,
                               DisableAddtoCart = (p.notAvailable || p.ShowPrice == false),
                               ReplaceProducts = p.ReplaceProductsX,
                               ProductType = p.ProductType,
                               MininumnOrderQty = p.MininumnOrderQty,
                               CategorySeq = p.CategorySeq,
                               LastUpdated = p.LastUpdated
                           };
                rpProList.DataSource = pros.OrderBy(c => c.CategorySeq).ThenBy(c => c.LastUpdated).ToList();
                rpProList.DataBind();
            }
        }

        protected string binPhaseOutProduct(object pPhaseOut, object pProductType, object plink, object pSProductID, object pName, object pStatus)
        {
            bool phaseOut = (bool)pPhaseOut;
            if (phaseOut)
                return string.Format("<a href='{0}' id='{1}' name='{2}'class=\"jTipProductDetail\">{2}</a>&nbsp;{3}<br /><span class='colorRed'>Phase out</span>", plink, this.ClientID + pSProductID, pName, pStatus);
            else
                return string.Format("<a href='{0}' id='{1}' name='{2}'class=\"jTipProductDetail\">{2}</a>&nbsp;{3}", plink, this.ClientID + pSProductID, pName, pStatus);
        }
    }
}