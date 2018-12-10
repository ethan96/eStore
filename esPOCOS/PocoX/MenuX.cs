using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

 namespace eStore.POCOS{ 

public partial class Menu : TaggingEnabled<Menu>
    {
        #region Tagging
        public override object EntityKey
        {
            get
            {
                return this.MenuID;
            }

        }
        public override string EntityStoreId
        {
            get
            {
                return this.StoreID;
            }
        }

        #endregion

        #region Extension Methods 

        public enum DataSource { StandardCategory, CTOSCategory, ApplicationCategory, MinisiteCategory, WidgetPage, CustomURL, PolicyCategoty, None, BottomRight };
    public enum RenderStyle {Category, CategoryList, ProductList, Matrix,Solution, None };
    public enum MenuPosition { Header ,Footer,Landing,None,TodayHighLight}
    /// <summary>
    /// This method returns what's the data source specified in the menu item
    /// </summary>
    public DataSource menuTypeX
    {
        get
        {
            DataSource source = DataSource.None;
            switch (this.MenuType)
            {
                case "StandardCategory":
                    source = DataSource.StandardCategory;
                    break;
                case "CTOSCategory":
                    source = DataSource.CTOSCategory;
                    break;
                case "ApplicationCategory":
                    source = DataSource.ApplicationCategory;
                    break;
                case "uStoreCategory":
                case "MinisiteCategory":
                    source = DataSource.MinisiteCategory;
                    break;
                case "WidgetPage":
                    source = DataSource.WidgetPage;
                    break;
                case "PolicyCategoty":
                    source = DataSource.PolicyCategoty;
                    break;
                case "CustomURL":
                    source = DataSource.CustomURL;
                    break;
                case "BottomRight":
                    source = DataSource.BottomRight;
                    break;
                default:
                    source = DataSource.None;
                    break;
                    
            }

            return source;
        }
    }

    /// <summary>
    /// This method return what's the rendering style of showing category menu item
    /// </summary>
    public RenderStyle DisplayTypeX
    {
        get
        {
            RenderStyle style = RenderStyle.None;
            switch (this.DisplayType)
            {
                case "CategoryList":
                    style = RenderStyle.CategoryList;
                    break;
                case "Category":
                    style = RenderStyle.Category;
                    break;
                case "ProductList":
                    style = RenderStyle.ProductList;
                    break;
                case "Matrix":
                    style = RenderStyle.Matrix;
                    break;
                case "Solution":
                    style = RenderStyle.Solution;
                    break;
                case "None":
                default:
                    style = RenderStyle.None;
                    break;

            }

            return style;
        }
    }
    public MenuPosition PositionX
    {
        get
        {
            MenuPosition position = MenuPosition.None;
            switch (this.Position)
            {
                case "Header":
                    position = MenuPosition.Header;
                    break;
                case "Footer":
                    position = MenuPosition.Footer;
                    break;
                case "Landing":
                    position = MenuPosition.Landing;
                    break;
                case "TodayHighLight":
                    position = MenuPosition.TodayHighLight;
                    break;
                case "None":
                default:
                    position = MenuPosition.None;
                    break;

            }

            return position;
        }
    }
    private ProductCategory _category = null;
    /// <summary>
    /// This property will return product category only when the data source of menu item is product category
    /// </summary>
    public ProductCategory productCategory
    {
        get
        {
            DataSource source = menuTypeX;
            if (source == DataSource.CTOSCategory || source == DataSource.StandardCategory
                ||source== DataSource.MinisiteCategory || source == DataSource.ApplicationCategory)
            {
                if (_category == null)
                {
                    ProductCategoryHelper helper = new ProductCategoryHelper();
                    _category = helper.getProductCategory(CategoryPath, StoreID);
                }

                return _category;
            }
            else
                return null;
        }
    }

    private PolicyCategory _policyCategoty = null;
    public PolicyCategory policyCategoty {
        get {
            if (menuTypeX == DataSource.PolicyCategoty)
            {
                if (_policyCategoty == null)
                {
                    PolicyCategoryHelper helper = new PolicyCategoryHelper();
                    int id = 0;
                    if (int.TryParse(CategoryPath, out id))
                        _policyCategoty = helper.getPolicyCategoryById(id, StoreID);
                }
                return _policyCategoty;
            }
            else
                return null;
        }
    }

    /// <summary>
    /// This extended property is to provide published menus only
    /// </summary>
    public IList<Menu> subMenusX
    {
        get
        {
            if (_subMenusX == null)
            {
                var helper = new MenuHelper();
                var menus = from menu in helper.getCachedMenus(this.StoreID)
                            where menu.Publish == true && menu.ParentMenuID ==this.MenuID
                            orderby menu.Sequence,menu.MenuName
                            select menu;
                var _menus = new List<Menu>();
                foreach (var item in menus)
                {
                    switch (item.menuTypeX)
                    {
                        case DataSource.CTOSCategory:
                        case DataSource.MinisiteCategory:
                        case DataSource.StandardCategory:
                            if (item.productCategory != null && item.productCategory.Publish != false)
                                _menus.Add(item);
                            break;
                        case DataSource.PolicyCategoty:
                                if(item.policyCategoty != null &&  item.policyCategoty.PublishStatus != false)
                                    _menus.Add(item);
                                break;
                        default:
                            _menus.Add(item);
                            break;
                    }
                }

                _subMenusX = _menus;
            }

            return _subMenusX;
        }
    }
    private List<Menu> _subMenusX = null;


    public Menu getSubMenu(String menuId)
    {
        Menu menu = null;
        try
        {
            int id = Convert.ToInt32(menuId);
            return getSubMenu(id);
        }
        catch (Exception)
        {
            ; //the input string is not a number
        }

        return menu;
    }

    /// <summary>
    /// This method is to find menu from its offsprings include children, grandchildren and on.
    /// </summary>
    /// <param name="menuId"></param>
    /// <returns></returns>
    public Menu getSubMenu(int menuId)
    {
        return new MenuHelper().getMenusByid(this.StoreID, menuId);
    }

    /// <summary>
    /// res set Global Resource
    /// </summary>
    /// <param name="resLs"></param>
    public void reSetMenuGlobalResource(List<MenuGlobalResource> resLs)
    {
        if (resLs.Any())
        {
            foreach (var c in resLs)
            {
                var obj = this.MenuGlobalResources.FirstOrDefault(t => t.LanguageId == c.LanguageId);
                if (obj != null)
                    obj.copyFrom(c);
                else
                    this.MenuGlobalResources.Add(c);
            }
        }
        foreach (var c in this.MenuGlobalResources.ToList())
        {
            var obj = resLs.FirstOrDefault(t => c.LanguageId == t.LanguageId);
            if (obj == null)
                this.MenuGlobalResources.Remove(c);
        }
    }


    public string getLocalName(POCOS.Language language)
    {
        string localname = this.MenuName;
        if (language != null && this.MenuGlobalResources.Any(x => x.LanguageId == language.Id))
        {
            localname = this.MenuGlobalResources.First(x => x.LanguageId == language.Id).LocalName;
            if (!string.IsNullOrEmpty(localname))
                return localname;
        }
        return this.MenuName;
    }
 #endregion 
	} 
 }