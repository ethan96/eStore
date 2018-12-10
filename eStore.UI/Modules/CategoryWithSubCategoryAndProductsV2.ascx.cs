using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace eStore.UI.Modules
{
    public partial class CategoryWithSubCategoryAndProductsV2 : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public POCOS.ProductCategory category;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack&& category!=null)
            {
                this.rpTabNav.DataSource = category.childCategoriesX;
                this.rpTabNav.DataBind();
                this.rpCategories.DataSource = category.childCategoriesX;
                this.rpCategories.DataBind();
            
            }
            this.AddStyleSheet("/Styles/subcategories.css");
        }



        public string mappCategroyTable(object obj)
        {
            if (obj == null)
                return "";
            ICollection<POCOS.ProductCategory> categoryLs = obj as ICollection<POCOS.ProductCategory>;

            string format = Presentation.eStoreContext.Current.getStringSetting("MinPriceFormat");
            if (string.IsNullOrEmpty(format))
                format = "{0} {1}";

            string tableStr = "<table width='100%' border='0' cellspacing='0' cellpadding='5' class='iPlanet-item'>";

            List<List<POCOS.ProductCategory>> ls = new List<List<POCOS.ProductCategory>>();
            int cur = 0;
            foreach (var pc in categoryLs)
            {
                if (cur % 4 == 0)
                    ls.Add(new List<POCOS.ProductCategory>());
                var lastPc = ls.LastOrDefault();
                lastPc.Add(pc);

                cur++;
            }

            foreach (var lis in ls)
            {
                if (lis.Count < 4)
                {
                    for (int c = lis.Count; c < 4; c++)
                        lis.Add(null);
                }
                string rowTitle = "<tr>";
                string rowImg = "<tr>";
                string rowDescription = "<tr>";
                string rowMore = "<tr>";
                string rowPrice = "<tr>";

                foreach (var li in lis)
                {
                    string url = li == null ? "" : ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(li));
                    string imageurl = "";
                    if (li != null)
                    {
                        if (!string.IsNullOrEmpty(li.ImageURL))
                            imageurl = string.Format("/resource/ProductCategory/{0}", li.ImageURL);
                        else
                        {
                            POCOS.Product product = li == null ? null : li.productList.FirstOrDefault();
                            imageurl = product == null ? "" : product.thumbnailImageX;
                        }
                    }
                    rowTitle += string.Format("<td><h3><a href='{1}'>{0}</a></h3></td>", li == null ? "" : eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(li), url);
                    rowImg += string.Format("<td><div class='iPlanet-img'><a href='{1}'>{0}</a></div></td>",
                        string.IsNullOrEmpty(imageurl) ? "" : "<img src='" + imageurl + "' />", url);
                    string description = "";
                    string localDes = (li == null) ? "" : eStore.Presentation.eStoreGlobalResource.getLocalCategoryDescription(li);
                    if (li != null)
                    {
                        if (localDes.Length > 128)
                        {
                            if (localDes.Substring(128).IndexOf(" ") > 0)
                                description = localDes.Substring(0, 128) + localDes.Substring(128, localDes.Substring(128).IndexOf(" ")) + "...";
                            else
                                description = localDes.Substring(128).Length > 12 ? localDes.Substring(0, 128) + "..." : localDes;
                        }
                        else
                            description = localDes;
                    }
                    rowDescription += string.Format("<td style='vertical-align:top'><p class='iPlanet-itemContent'>{0}</p></td>", description);
                    rowMore += string.Format("<td><a class='{2}' href='{1}'>{0}</a></td>", li == null ? "" : "More Systems", url, li == null ? "" : "iPlanet-more");
                    rowPrice += string.Format("<td><p class='iPlanet-price'>{0}</p></td>", li == null ? "" :
                        string.Format(format , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_From)
                        , string.Format("<span id=\"{0}\"><img alt=\"loading...\" src=\"{1}images/priceprocessing.gif\" /></span>", li.CategoryPath,esUtilities.CommonHelper.GetStoreLocation())));
                }

                tableStr += (rowTitle + "</tr>" + rowImg + "</tr>" + rowDescription + "</tr>" + rowMore + "</tr>" + rowPrice + "</tr> <tr><td height='30' colspan='4'>&nbsp;</td></tr>");
            }

            tableStr += "</table>";

            return tableStr;
        }




    }
}