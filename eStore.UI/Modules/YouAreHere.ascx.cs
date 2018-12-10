using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace eStore.UI.Modules
{
    public partial class YouAreHere : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public POCOS.ProductCategory productCategory { get; set; }
        public object Navigator { get; set; }
        public string ProductName { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected override void OnPreRender(EventArgs e)
        {
            StringBuilder sbYouarehere = new StringBuilder();
            sbYouarehere.AppendFormat(" <div class=\"eStore_breadcrumb eStore_block980\"><a href=\"{0}\">{1}</a>"
             , ResolveUrl("~/")
             , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Home));
            sbYouarehere.Append(getYourAreHereString(Navigator, false));
            sbYouarehere.Append("</div>");
            lYouarehere.Text = sbYouarehere.ToString();

            base.OnPreRender(e);
        }

        internal string getYourAreHereString(object nevigator , bool isLink)
        {
            if (productCategory != null)
            {
                StringBuilder sbYouarehere = new StringBuilder();
                if (!string.IsNullOrEmpty(ProductName))
                {
                    if (productCategory != null)
                    {
                        CreatYouareHeres(productCategory, true, ref sbYouarehere);
                    }
                }
                else
                {
                    if (productCategory != null)
                    {
                        CreatYouareHeres(productCategory, false, ref sbYouarehere);
                    }
                }
                return sbYouarehere.ToString();
            }
            else if (nevigator!=null && nevigator is POCOS.PolicyCategory)
            {
                StringBuilder sbYouarehere = new StringBuilder();
                CreatYouareHeres((POCOS.PolicyCategory)nevigator, false, ref sbYouarehere);
                return sbYouarehere.ToString();
            }
            else
                return string.Empty;
        }
        void CreatYouareHeres(POCOS.ProductCategory category, bool isLink, ref StringBuilder sbYouarehere)
        {
            if(isLink)

                sbYouarehere.Insert(0, string.Format(" <a href=\"{0}\">{1}</a> "
                 , esUtilities.CommonHelper.ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(category, eStore.Presentation.eStoreContext.Current.MiniSite))
                 ,esUtilities.CommonHelper.RemoveHtmlTags(eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(category),true)));
            else
                sbYouarehere.Insert(0, string.Format(" {0}"
                            ,esUtilities.CommonHelper.RemoveHtmlTags(eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(category),true)));
            if (category.parentCategoryX != null)
            {
                CreatYouareHeres(category.parentCategoryX, true, ref sbYouarehere);
            }
        }

        void CreatYouareHeres(POCOS.PolicyCategory category, bool isLink, ref StringBuilder sbYouarehere)
        {
            if (isLink)

                sbYouarehere.Insert(0, string.Format(" <a href=\"{0}\">{1}</a> "
                 , ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(category, eStore.Presentation.eStoreContext.Current.MiniSite))
                 , category.Name));
            else
                sbYouarehere.Insert(0, string.Format(" {0}"
                            , category.Name));
            if (category.ParentCategory != null)
            {
                CreatYouareHeres(category.ParentCategory, true, ref sbYouarehere);
            }
        }
    }

}