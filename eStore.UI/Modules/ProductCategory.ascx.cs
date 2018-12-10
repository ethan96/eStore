using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class ProductCategory : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public POCOS.ProductCategory productCategory { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (productCategory != null)
            {
                if (productCategory.childCategoriesX == null || productCategory.childCategoriesX.Count == 0)
                    Response.Redirect(ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(productCategory)));

                generateCategoriesTree(productCategory, null);
            }
        }

        private void generateCategoriesTree(POCOS.ProductCategory category, TreeNode treeNode)
        {
            foreach (POCOS.ProductCategory subcategory in category.childCategoriesX.OrderBy(c => c.LocalCategoryName))
            {
                TreeNode childnode = new TreeNode(eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(subcategory)
                    , subcategory.CategoryID.ToString(), "", ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(subcategory)), "_self");
                generateCategoriesTree(subcategory, childnode);
                if (treeNode == null)
                    this.tvProductCategories.Nodes.Add(childnode);
                else
                    treeNode.ChildNodes.Add(childnode);
            }
        }
    }
}