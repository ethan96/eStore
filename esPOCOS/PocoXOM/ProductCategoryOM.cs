using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{

    public partial class ProductCategory
    {

        private List<Widget> _widgetlist = null;
        private List<Menu> _menulist = null;

        #region Extension Methods
        /// <summary>
        /// Get Widget reference
        /// </summary>
        public  List<Widget> widgetList
        {
            get
            {
                if (_widgetlist == null)
                {
                    ProductCategoryHelper helper = new ProductCategoryHelper();
                    _widgetlist = helper.getWidgetsByProductCategory(this);
                }
                return _widgetlist;
            }

        }
        /// <summary>
        /// Get Menu reference
        /// </summary>
        public List<Menu> menuList
        {
            get
            {
                if (_menulist == null)
                {
                    ProductCategoryHelper helper = new ProductCategoryHelper();
                    _menulist = helper.getMenuByProductCategory(this);
                }
                return _menulist;
            }

        }

        #region OM Extension Methods

        /// <summary>
        /// This property indicates if current product has all mandatory data for publish to eStore. 
        /// It's mainly used in OM to check product information readiness.
        /// The validation criteria is as following
        /// 1. DisplayPartno, Status, ProductDesc, ProductFeatures
        /// 2. ImageURL, CreatedBy, DisplayPartNo, (DimensionHeight/Width/Length commented out)
        /// 3. Price and Product status
        /// 4. Product features and data sheets
        /// </summary>
        public Boolean validForPublish
        {
            get
            {
                validate();

                //additional validation
                if (String.IsNullOrEmpty(this.ImageURL))
                    error_message.Add(new PocoX.ErrorMessage("ImageURL", "Missing ProductCategory image information"));
                if (String.IsNullOrEmpty(this.Description ))
                    error_message.Add(new PocoX.ErrorMessage("Description", "Missing ProductCategory Deacription Information"));

                if (String.IsNullOrEmpty(this.Keywords))
                    error_message.Add(new PocoX.ErrorMessage("Description", "Missing ProductCategory Keywords Information"));

                if (String.IsNullOrEmpty(this.PageTitle))
                    error_message.Add(new PocoX.ErrorMessage("PageTitle", "Missing ProductCategory PageTitle Information"));
                if (String.IsNullOrEmpty(this.ProductDivision))
                    error_message.Add(new PocoX.ErrorMessage("ProductDivision", "Missing ProductCategory ProductDivision Information"));
                if (String.IsNullOrEmpty(this.LocalCategoryName))
                  error_message.Add(new PocoX.ErrorMessage("LocalCategoryName", "Missing ProductCategory LocalCategoryName Information"));

                if (this.RuleSet == null || this.RuleSet.RuleSetDetails.Count<0)
                    error_message.Add(new PocoX.ErrorMessage("RuleSet", "Please Set ProductCategory RuleSet first before modify ProductCateogry Information"));

                if (error_message.Count > 0)
                    return false;
                else
                    return true;

            }
        }


        public Boolean validForDelete
        {
            get
            {
                //additional validation
                if (this.menuList == null)
                    error_message.Add(new PocoX.ErrorMessage("menuList", "Refer to Menu List"));
                if (this.widgetList  == null)
                    error_message.Add(new PocoX.ErrorMessage("widgetList", "Refer to widget List"));
                if (this.ChildCategories.Count >0)
                    error_message.Add(new PocoX.ErrorMessage("ChildCategories", "Refer to Child Categories"));
                if (error_message.Count > 0)
                    return false;
                else
                    return true;
            
            }
        }

        private List<ProductCategory> _ancestorOM;
        public List<ProductCategory> ancestorXOM
        {
            get
            {
                if (_ancestorOM == null)
                {
                    _ancestorOM = new List<ProductCategory>();
                    if (this.parentCategoryXOM != null)
                    {
                        _ancestorOM.Add(this.parentCategoryXOM);
                        _ancestorOM.AddRange(this.parentCategoryXOM.ancestorXOM);
                    }
                }
                return _ancestorOM;
            }
        }

        private ProductCategory _parentCategoryXOM;
        public ProductCategory parentCategoryXOM
        {
            get
            {
                if (_parentCategoryXOM == null)
                {
                    ProductCategoryHelper helper = new ProductCategoryHelper();
                    _parentCategoryXOM = helper.getParentCategory(this, false);
                }
                return _parentCategoryXOM;
            }
        }
        #endregion





        #endregion

    }


}
