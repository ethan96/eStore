using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.POCOS.PocoX;

 namespace eStore.POCOS{ 

public partial class ProductCategory: TaggingEnabled<ProductCategory>
    {
        #region Tagging
        public override object EntityKey
        {
            get
            {
                return this.CategoryID;
            }
 
        }
        public override string EntityStoreId
        {
            get
            {
                return this.Storeid;
            }
        }

        #endregion

        private List<Product> _productList = null;
    public enum Category_Type { CTOS, Standard, Application, None, uStoreCategory, Combo,Promotion,Delivery, ApplicationCategory };
    public enum RenderStyle { CategoryList, ProductList,ProductListWithModel, Matrix, Tabs,CustomURL, None, Promotions, SelectBySpec };
    public enum Sort_Type { Seq, PriceLowest, PriceHighest, LatestedAdd }

        #region Extension Methods 

        public ProductCategory()
        {
            error_message = new List<ErrorMessage>();
        }
        private Store.BusinessGroup gorup = Store.BusinessGroup.eP;
        private bool gorupIsInit = false;
        public Store.BusinessGroup businessGroup
        {
            get
            {
                if (!gorupIsInit)
                {
                    if (!String.IsNullOrEmpty(this.ProductDivision) && Enum.TryParse<Store.BusinessGroup>(this.ProductDivision, out gorup))
                    {

                    }
                    else if (this.parentCategoryX != null)
                    {
                        gorup = this.parentCategoryX.businessGroup;
                    }
                    else
                        gorup = Store.BusinessGroup.eP;
                    gorupIsInit = true;
                }
                return gorup;
            }
        }


        /// <summary>
        /// getLowerstPrice method handles CTOS category and standard product category in different way.  To get CTOS system listing
        /// price, system need to perform CTOS component validation first to make sure this product is orderable.  Due to the complexity of 
        /// system validation, it takes a lot of system resources and can cause great performance hint.  To avoid system performance hit, 
        /// we play a trick here.  For CTOS category system will first retrieve entire product list from it's subcategories.   And sort product
        /// price by its StorePrice property.   This property value shall be updated periodically by sync job.   Therefore we assume it reflects
        /// the latest sync result.   Once the sorting is done, system will perform system component validation from the product that has the
        /// lowest StorePrice.   System will return the latest getListingPrice once the lowest pricing product is valid.   The result of this
        /// trick may not always be able to reflect 100% true picture, but its accuracy shall be close to 100% if StorePrice is updated periodically.
        /// </summary>
        /// <returns></returns>
        public Decimal getLowestPrice()
    {
        if (this.MinimumPrice.HasValue)
            return this.MinimumPrice.GetValueOrDefault();
        else
        {
            Decimal price = 0m;
            try
            {
                price = (from product in productList
                         where product.getListingPrice().value > 0
                         select product.getListingPrice().value).Min();
            }
            catch (Exception)
            {
                price = 0m;
            }
            this.MinimumPrice = price;
            return price;
        }
    }

    public bool  refreshMinimumPrice()
    {
        bool  result = false;
        Decimal price = 0m;
        try
        {
            var eexls = this.childCategoriesX.Where(c => !c.MinimumPriceExempted.GetValueOrDefault()).ToList();
            if (eexls.Any())
            {
                foreach (ProductCategory sub in eexls)
                {
                    if ( sub.refreshMinimumPrice())
                        result = true;
                }
                price = eexls.Where(c => c.MinimumPrice.HasValue && c.MinimumPrice > 0).Min(x => x.MinimumPrice).GetValueOrDefault(0m);
            }
            else
            {
                if (productList.Any())
                {
                    price = (from product in productList
                             where product.getListingPrice().value > 0
                             select product.getListingPrice().value).Min();
                }
            }
          
        }
        catch (Exception)
        {
            price = 0m;
            result = false;
            return result;
        }
        if (this.MinimumPrice.HasValue)
        {
            if (this.MinimumPrice.GetValueOrDefault() != price)
            {
                this.MinimumPrice = price;
                result = true;
            }
        }
        else
        {  
            this.MinimumPrice = price;
            result = true;
        }
        if (result)
            this.save(); // save mini price
        return result;
    }

    public Decimal getHighestPrice()
    {
        Decimal price = -1.0m;

        try
        {
            price = (from product in productList
                     where product.getListingPrice().value > 0
                     select product.getListingPrice().value).Max();
        }
        catch
        {
            price = 0m;
        }

        return price;
    }

    public List<Product> productList
    {
        get
        {
            if (_productList == null)
            {
                ProductCategoryHelper helper = new ProductCategoryHelper();
                _productList = new List<Product>();
                if (this.childCategoriesX.Count > 0)
                {
                    foreach (ProductCategory pc in this.childCategoriesX)
                    {
                        _productList.AddRange(pc.productList);
                    }
                }
                else
                {
                    _productList = helper.getCategoryProductList(this);
                }

                if (!isCTOSCategory())
                {
                    _productList = (from p in _productList
                                    where p.publishStatus == Product.PUBLISHSTATUS.PUBLISHED
                                    select p).ToList();
                }
            }
            return _productList;
        }
        set
        {
            _productList = value;
        }
    }

    /// <summary>
    /// async
    /// </summary>
    /// <returns></returns>
    public async System.Threading.Tasks.Task<bool> PerGetproductListAsycn()
    {
        return await System.Threading.Tasks.Task.Run(() => {
            var c = this.simpleProductList;
            return true;
        });
    }

    public void resetProductList()
    { 
        if (this.childCategoriesX.Count > 0)
        {
            foreach (ProductCategory pc in this.childCategoriesX)
                pc.resetProductList();
        }
        productList = null;
    }


    protected  List<SimpleProduct> _simpleProductList = null;

    public virtual List<SimpleProduct> simpleProductList
    {
        get 
        {
            if (_simpleProductList == null)
            {
                var items = new List<SimpleProduct>();
                if (this.childCategoriesX.Count > 0)
                {
                    foreach (ProductCategory pc in this.childCategoriesX)
                    {
                        foreach (var item in pc.simpleProductList)
                        {
                            if (items.FirstOrDefault(c => c.SProductID.Equals(item.SProductID, StringComparison.OrdinalIgnoreCase)) == null)
                                items.Add(item);
                        }
                    }
                }
                else
                {
                    ProductCategoryHelper helper = new ProductCategoryHelper();
                    items = helper.getSimpleProducts(this);
                }
                _simpleProductList = items;
            }
            return _simpleProductList; 
        }
        set
        {
            _simpleProductList = value;
        }
    }

    public void resetSimpleProductList()
    { 
        _simpleProductList = null;
    }

    public void clearSimpleProductList()
    {
        _simpleProductList = new List<SimpleProduct>();
    }

    public void addSimpleProduct(SimpleProduct sp)
    { 
        if(_simpleProductList == null)
            _simpleProductList = new List<SimpleProduct>();
        _simpleProductList.Add(sp);
    }


    public List<ProductCategory> getLeafCategories()
    {
        if (this.childCategoriesX.Count > 0)
        {
            return this.childCategoriesX.SelectMany(x => x.getLeafCategories()).Where(x=>x!=null).ToList();
        }
        else
        return null;
    }
    public ProductCategory getRootCategory()
    {
        if (this.parentCategoryX == null)
            return this;
        else
        {
            return this.parentCategoryX.getRootCategory();
        }
    }
     public ProductCategory getRootCategory(int assignedrcid)
    {
        if (this.parentCategoryX == null|| assignedrcid==this.CategoryID)
            return this;
        else
        {
            return this.parentCategoryX.getRootCategory(assignedrcid);
        }
    }
    /// <summary>
    /// 获取category的所有父categoryids
    /// </summary>
    /// <returns></returns>
    public List<int> GetParentIds()
    {
        List<int> ls = new List<int>();
        GetParentId(this.parentCategoryX, ref ls);
        return ls;
    }
    private void GetParentId(ProductCategory cate, ref List<int> ls)
    {
        if (cate == null)
            return;
        ls.Add(cate.CategoryID);
        if (cate.parentCategoryX != null)
            GetParentId(cate.parentCategoryX, ref ls);

    }
    

    public string getFullPathName()
    {
        return getPathName(this);
    }

    protected string getPathName(ProductCategory pc)
    {
        if (pc == null)
            return "";
        if (pc.parentCategoryX == null)
            return pc.CategoryName;
        else
            return getPathName(pc.parentCategoryX) + " --> " + pc.CategoryName;

    }


    public Boolean isCTOSCategory()
    {
        if (categoryTypeX == Category_Type.CTOS)
            return true;
        else
            return false;
    }

    private Category_Type _dynamicCategoryType = Category_Type.None;
    public Category_Type dynamicCategoryType
    {
        get {
            if (_dynamicCategoryType == Category_Type.None)
            {
                if (categoryTypeX == Category_Type.uStoreCategory)
                {
                    if (this.productList == null || this.productList.Any() == false)
                        _dynamicCategoryType = categoryTypeX;
                    else if (this.productList.Any(x => x is POCOS.Product_Ctos) == false)//no ctos
                    {
                        _dynamicCategoryType = Category_Type.Standard;
                    }
                    else if (this.productList.Count() == this.productList.Count(x => x is POCOS.Product_Ctos))// all ctos
                    {
                        _dynamicCategoryType = Category_Type.CTOS;
                    }
                    else // other, 
                    {
                        _dynamicCategoryType = Category_Type.Combo;

                    }


                
                }
                else
                {
                    _dynamicCategoryType = categoryTypeX;

                }
            
            }
            return _dynamicCategoryType;
        
        }
    }

    private Category_Type _categoryTypeX = Category_Type.None;
    public Category_Type categoryTypeX
    {
        get
        {
            if (_categoryTypeX == Category_Type.None)
            {
                switch (this.CategoryType.ToUpper())
                {
                    case "CTOSCATEGORY":
                        _categoryTypeX = Category_Type.CTOS;
                        break;
                    case "ACCESSORIAL":
                    case "STANDARDCATEGORY":
                        _categoryTypeX = Category_Type.Standard;
                        break;
                    case "APPLICATION":
                        _categoryTypeX = Category_Type.Application;
                        break;
                    case "USTORECATEGORY":
                        _categoryTypeX = Category_Type.uStoreCategory;
                        break;
                    case "PROMOTIONCATEGORY":
                        _categoryTypeX = Category_Type.Promotion;
                        break;
                    case "DELIVERYCATEGORY": 
                        _categoryTypeX = Category_Type.Delivery;
                        break;
                    case "APPLICATIONCATEGORY":
                        _categoryTypeX = Category_Type.ApplicationCategory;
                        break;
                    default:
                        _categoryTypeX = Category_Type.None;
                        break;
                }
            }

            return _categoryTypeX;
        }
    }

    private bool? _hasOption = null;
    public bool hasOption
    {
        get 
        {
            if (_hasOption == null)
                _hasOption = childCategoriesX.Any();
            return _hasOption.Value; 
        }
    }


    public RenderStyle DisplayTypeX
    {
        get
        {
            RenderStyle style = RenderStyle.None;
            if (!String.IsNullOrEmpty(this.DisplayType))
            {
                switch (this.DisplayType.ToLower())
                {
                    case "tabs":
                        style = RenderStyle.Tabs;
                        break;
                    case "categorylist":
                    case "productcategory":
                        style = RenderStyle.CategoryList;
                        break;

                    case "productlistwithmodel":
                        style = RenderStyle.ProductListWithModel;
                        break;
                    case "productlist":
                    case "productSub":
                        style = RenderStyle.ProductList;
                        break;
                    case "matrix":
                        style = RenderStyle.Matrix;
                        break;
                    case "customurl":
                        style = RenderStyle.CustomURL;
                        break;
                    case "promotions":
                        style = RenderStyle.Promotions;
                        break;
                    case "selectbyspec":
                        style = RenderStyle.SelectBySpec;
                        break;
                    case "none":
                    default:
                        style = RenderStyle.None;
                        break;
                }
            }

            return style;
        }
    }
    /// <summary>
    /// This property only retains published child category and will filter out non-published ones
    /// </summary>
    public virtual ICollection<ProductCategory> childCategoriesX
    {
        get
        {
            if (_childCategoryX == null)
            {
                ProductCategoryHelper helper = new ProductCategoryHelper();
                _childCategoryX = helper.getchildCategories(this);
            }
            return _childCategoryX;
        }
    }

    public void AddCategoryX(ProductCategory cate)
    {
        if (_childCategoryX == null)
            _childCategoryX = new List<ProductCategory>();
        _childCategoryX.Add(cate);
    }

    public void clearChildCategories()
    {
        _childCategoryX = new List<ProductCategory>();
    }

    private ProductCategory _parentCategoryX;
    public ProductCategory parentCategoryX
    {
        get
        {
            if (_parentCategoryX == null)
            {
                ProductCategoryHelper helper = new ProductCategoryHelper();
                _parentCategoryX = helper.getParentCategory(this);
            }
            return _parentCategoryX;
        }
    }
    private List<ProductCategory> _ancestor;
    public List<ProductCategory> ancestorX
    {
        get {
            if (_ancestor == null)
            {
                _ancestor = new List<ProductCategory>();
                if (this.parentCategoryX != null)
                {
                    _ancestor.Add(this.parentCategoryX);
                    _ancestor.AddRange(this.parentCategoryX.ancestorX);
                }
            }
            return _ancestor;
        }
    }

    private List<ProductCategory> _childCategoryX = null;

    public String descriptionX
    {
        get { return String.IsNullOrEmpty(this.Description) ? this.ExtendedDescription??"" : this.Description; }
    }

    public String extendedDescX
    {
        get { return String.IsNullOrEmpty(this.ExtendedDescription) ? this.Description ?? "" : this.ExtendedDescription; }
    }

    /// <summary>
    /// The following three prooperties are for product detail page SEO purpose
    /// </summary>
    public String pageTitle
        {
            get { return String.IsNullOrEmpty(PageTitle) ? (string.IsNullOrEmpty(this.descriptionX) ?  this.localCategoryNameX : this.descriptionX) : PageTitle; }


        }

    public String keywords
    {
        get { return String.IsNullOrEmpty(Keywords) ? CategoryName : Keywords; }
    }

    public String metaData
    {
        get { return String.IsNullOrEmpty(PageDescription) ? descriptionX : PageDescription; ; }
    }

    public string localCategoryNameX
    {
        get
        {
            return string.IsNullOrEmpty(LocalCategoryName) ? CategoryName : LocalCategoryName;
        }
    }

    private bool? _hasInitSEOPath;
    private void initSEOPath()
    {
        try
        {
            if (this.parentCategoryX == null)
            {
                seopath1 = this.localCategoryNameX;
                seopath2 = this.keywords;
            }
            else
            {
                seopath1 = categoryHierarchy.First().localCategoryNameX;
                seopath2 = string.Join("-", categoryHierarchy.Skip(1).Select(x => x.localCategoryNameX).ToArray());
            }
        }
        catch (Exception)
        {
            seopath1 = this.localCategoryNameX;
            seopath2 = this.keywords;
        }
     
        _hasInitSEOPath = true;
    }
    private string seopath1 = string.Empty;
    private string seopath2 = string.Empty;
    public string SEOPath1
    {
        get
        {
            if (_hasInitSEOPath.HasValue==false)
            {
                initSEOPath();
            }
            return seopath1; 
        }
    }
    public string SEOPath2
    {
        get
        {
            if (_hasInitSEOPath.HasValue == false)
            {
                initSEOPath();
            }
            return seopath2; 
        }
    }

    /// <summary>
    /// This property returns category direct line hierarchy in the order from its root to itself
    /// </summary>
    public List<ProductCategory> categoryHierarchy
    {
        get
        {
            if (_hierarchy == null)
            {
                Stack<ProductCategory> hierarchyStack = new Stack<ProductCategory>();
                    ProductCategory current = this;
                    while (current != null)
                    {
                        hierarchyStack.Push(current);
                        if (current.parentCategoryX == null && current.ParentCategoryID != null)
                        {
                            _hierarchy = new List<ProductCategory>();
                            return _hierarchy;
                        }
                        else
                            current = current.parentCategoryX;   //move the upper level
                    }

                    _hierarchy = hierarchyStack.ToList();
            }

            return _hierarchy;
        }
    }
    private List<ProductCategory> _hierarchy = null;

    /// <summary>
    /// This property is a temporary property used to hold weighting level for cross-sell product weight calculation
    /// </summary>
    public int weightingLevel
    {
        get;
        set;
    }
        private List<SpecMask> _SpecMasks;
        public List<SpecMask> SpecMasks
        {
            get
            {
                if (_SpecMasks == null)
                    _SpecMasks = (new SpecMaskHelper()).getSpecMaskbyCategoryPath(this.CategoryPath, this.Storeid);
                return _SpecMasks;
            }
        }

        /// <summary>
        /// get ReplicationCategoryProductsMapping buy target store
        /// </summary>
        /// <param name="targetStore"></param>
        /// <returns></returns>
        public ReplicationCategoryProductsMapping getSourceReplicateMappingCategory(string targetStore)
    {
        if (this.ReplicationCategoryProductsMappings1 == null || !this.ReplicationCategoryProductsMappings1.Any())
            return null;
        var mapping = this.ReplicationCategoryProductsMappings1.FirstOrDefault(c => c.StoreIDFrom == targetStore);
        return mapping;
    }

    public bool hasOrderProduct()
    {
        foreach (var p in this.productList)
        {
            if (p.isOrderable())
                return true;
        }
        return false;
    }

    public void reSetCategoryGlobalResource(List<CategoriesGlobalResource> resLs)
    {
        if (resLs.Any())
        {
            foreach (var c in resLs)
            {
                var obj = this.CategoriesGlobalResources.FirstOrDefault(t => t.LanguageId == c.LanguageId);
                if (obj != null)
                    obj.copyfrom(c);
                else
                    this.CategoriesGlobalResources.Add(c);
            }
        }
        foreach (var c in this.CategoriesGlobalResources.ToList())
        {
            var obj = resLs.FirstOrDefault(t => c.LanguageId == t.LanguageId);
            if (obj == null)
                this.CategoriesGlobalResources.Remove(c);
        }
    }

    //排除已经删除的category
    private List<ProductCategory> _childCategoryStatusX;
    public List<ProductCategory> childCategoryStatusX
    {
        get {
            _childCategoryStatusX = (from category in ChildCategories
                                     where category.CategoryStatus == null || (category.CategoryStatus != null && category.CategoryStatus.ToLower() != "delete")
                                     select category).ToList();
            return _childCategoryStatusX;
        }
    }

    private List<ProductCategory> AssociatedCategorys;

    /// <summary>
    /// 得到Category关联的Categories
    /// </summary>
    /// <param name="includeNonpublished">是否包括没发布的Category</param>
    /// <returns></returns>
    public List<ProductCategory> getAssociatedCategorys(Boolean includeNonpublished = false)
    {
        if (includeNonpublished || AssociatedCategorys == null)
        {
            ProductCategoryHelper pHelper = new ProductCategoryHelper();
            CategoryMappingHelper cHelper = new CategoryMappingHelper();
            List<int> categoryIDList;
            AssociatedCategorys = new List<ProductCategory>();
            ProductCategory pc;
            categoryIDList = cHelper.getCategoryAssociationID(this.CategoryID);
            if (categoryIDList.Count > 0)
            {
                foreach (var item in categoryIDList)
                {
                    pc = pHelper.getProductCategory(item, this.Storeid, includeNonpublished);
                    if (pc != null)
                        AssociatedCategorys.Add(pc);
                }
            }
            return AssociatedCategorys;
        }
        else
        {
            return AssociatedCategorys;
        }

    }

    public ProductCategory copyToTemp()
    {
        ProductCategory ppc = new ProductCategory()
        {
            Storeid = this.Storeid,
            CategoryName = this.CategoryName,
            CategoryStatus = this.CategoryStatus,
            CategoryID = this.CategoryID,
            CategoryPath = this.CategoryPath,
            DisplayType = this.DisplayType,
            Description = this.Description,
            CategoryType = this.CategoryType
        };
        ppc.clearChildCategories();
        ppc.clearSimpleProductList();
        return ppc;
    }

    public bool isSpecialCategory
    {
        get
        {
            return this.categoryTypeX == Category_Type.Promotion || this.categoryTypeX == Category_Type.Delivery;
        }
    }

    public bool isMatrixCategory()
    {
        return (!this.childCategoriesX.Any() && this.DisplayTypeX == POCOS.ProductCategory.RenderStyle.Matrix)
                    || (this.DisplayTypeX == POCOS.ProductCategory.RenderStyle.Tabs && this.childCategoriesX.Any())
                    || (this.parentCategoryX != null && this.parentCategoryX.DisplayTypeX == RenderStyle.Tabs);
    }

    /// <summary>
    /// 遍历时是否访问过
    /// </summary>
    public bool IsVist
    {
        get;
        set;
    }
        public Sort_Type SortTypeX
        {
            get
            {
                switch (SortType)
                {
                    case "PriceLowest":
                        return Sort_Type.PriceLowest;
                    case "PriceHighest":
                        return Sort_Type.PriceHighest;
                    case "LatestedAdd":
                        return Sort_Type.LatestedAdd;
                    default:
                        return Sort_Type.Seq;
                }
            }
        }
 #endregion
    } 
 }