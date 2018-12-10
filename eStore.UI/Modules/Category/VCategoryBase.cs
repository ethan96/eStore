using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eStore.UI.Modules.Category
{
    public class VCategoryBase : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        enum ShowType { None, WithNoSub, WithSubOnly, WithSubPiece, WithSubList, WithSubPic }

        private POCOS.ProductCategory _pc;
        public POCOS.ProductCategory productCategory
        {
            get { return _pc; }
            set { _pc = value; }
        }

        public VCategoryBase LoadCategory()
        {
            ShowType type = ShowType.None;
            Enum.TryParse(CategoryTabBll.getShowType(productCategory), out type);
            VCategoryBase sub = null;
            switch (type)
            {
                case ShowType.WithSubOnly:
                    sub = this.LoadControl("Modules/Category/CategoryWithSubOnly.ascx") as VCategoryBase;
                    break;
                case ShowType.WithSubPiece:
                    sub = this.LoadControl("Modules/Category/CategoryWithSubPiece.ascx") as VCategoryBase;
                    break;
                case ShowType.WithSubList:
                    sub = this.LoadControl("Modules/Category/CategoryWithSubList.ascx") as VCategoryBase;
                    break;
                case ShowType.WithSubPic:
                    sub = this.LoadControl("Modules/Category/CategoryWithSubPic.ascx") as VCategoryBase;
                    break;
                case ShowType.WithNoSub:
                default:
                    sub = this.LoadControl("Modules/Category/CategoryWithNoSub.ascx") as VCategoryBase;
                    break;
            }
            if (sub != null)
                sub.productCategory = this.productCategory;
            return sub;
        }


        public virtual string getFormatedAJAXMinPrice()
        {
            string format = Presentation.eStoreContext.Current.getStringSetting("MinPriceFormat");
            if (string.IsNullOrEmpty(format))
                format = "{0} {1}";
            return string.Format(format
                , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_From)
                , string.Format("<span id=\"{0}\" class='CategoryMinPrice'><img alt=\"loading...\" src=\"{1}images/priceprocessing.gif\" /></span>", productCategory.CategoryPath, esUtilities.CommonHelper.GetStoreLocation()));
        }

        public virtual string getDescription()
        {
            return eStore.Presentation.eStoreGlobalResource.getLocalCategoryExtDescription(productCategory);
        }

        public string getImageUrl(object url)
        {
            if (url == null)
                return esUtilities.CommonHelper.GetStoreLocation(false) + "images/photounavailable.gif";
            string c = url.ToString();
            c = String.IsNullOrEmpty(c) ?
                    string.Empty : (eStore.Presentation.eStoreContext.Current.Store.profile.Settings["ProductCategoryPicurePath"].ToString() + c);
            return c;
        }
    }
}