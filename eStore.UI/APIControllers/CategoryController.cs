using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace eStore.UI.APIControllers
{
    public class CategoryController : ApiController
    {
        [HttpGet]
        public Models.Category Get(string id, bool withMatrx = false, string filterType = "")
        {
            POCOS.ProductCategory pc = Presentation.eStoreContext.Current.Store.getProductCategory(id);
            if (pc != null)
            {
                var model = new Models.Category(pc,false,filterType);
                if (withMatrx && eStore.Presentation.eStoreContext.Current.getBooleanSetting("showMatrix"))
                {
                    model.setBaseInforAndMatrix(pc, true);
                    model.setParentCategory(pc);
                }
                return model;
            }
            else
                return null;
        }


        [HttpGet]
        public IEnumerable<Models.Product> Products(string id, int page, int pagesize, string SortType = "", string filterType = "", bool MatrixPage = false, bool paps = false)
        {
            Models.VCategoryFilterResult result = new Models.VCategoryFilterResult();
            return GetProducts(ref result, id, page, pagesize, SortType, filterType, MatrixPage, paps);
        }

        [HttpPost]
        public Models.VCategoryFilterResult ProductsPost(Models.VCategoryProSearch ty)
        {
            Models.VCategoryFilterResult result = new Models.VCategoryFilterResult();
            result.Products = GetProducts(ref result, ty.Id, ty.Page, ty.Pagesize, ty.SortType, ty.FilterType, ty.MatrixPage, ty.Paps, ty.CategoryAttrs);
            return result;
        }

        [HttpGet]
        public List<Models.Category> AssociatedProductCategories(string id)
        {
            List<POCOS.ProductCategory> pcs = Presentation.eStoreContext.Current.Store.getAssociatedProductCategoriesByProductID(id);
            List<Models.Category> apcs = new List<Models.Category>();
            if (pcs.Count > 0)
            {
                foreach (var pc in pcs)
                {
                    apcs.Add(new Models.Category(pc));
                    if (apcs.Count == 4)
                        break;
                }
                return apcs;
            }
            else
                return null;
        }

        [HttpGet]
        public List<Models.Category> ProductCategories(string id)
        {
            List<POCOS.ProductCategory> pcs = Presentation.eStoreContext.Current.Store.getAssociatedProductCategoriesByCategoryPath(id.ToUpper());
            List<Models.Category> apcs = new List<Models.Category>();
            if (pcs.Count > 0)
            {
                foreach (var pc in pcs)
                {
                    apcs.Add(new Models.Category(pc));
                    if (apcs.Count == 3)
                        break;
                }
                return apcs;
            }
            else
                return null;
        }

        [HttpGet]
        public Models.Category getCategoryWithMatrix(string id)
        { 
            POCOS.ProductCategory pc = Presentation.eStoreContext.Current.Store.getProductCategory(id);
            if (pc != null)
            {
                Models.Category category = new Models.Category();
                category.setBaseInforAndMatrix(pc);

                //System.Web.Script.Serialization.JavaScriptSerializer jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                //string formString = jsonSerializer.Serialize(category);

                return category;
            }
            return new Models.Category();
        }

        
        private IEnumerable<Models.Product> GetProducts(ref Models.VCategoryFilterResult VResult, string id, int page, int pagesize, string SortType = "", string filterType = "", bool MatrixPage = false, bool paps = false, List<Models.VCategoryAttr> attrs = null)
        {
            if (page == 0)
                page = 1;
            List<POCOS.SimpleProduct> simpProductList = new List<POCOS.SimpleProduct>();

            POCOS.ProductCategory pc = Presentation.eStoreContext.Current.Store.getProductCategory(id);
            if (pc != null)
            {
                List<Models.Product> result = new List<Models.Product>();
                if (MatrixPage && eStore.Presentation.eStoreContext.Current.getBooleanSetting("showMatrix") && pc.isMatrixCategory())
                {
                    switch (SortType)
                    {
                        case "PriceHighest":
                            result = pc.productList.OrderByDescending(x => x.getListingPrice().value).Select(x => new Models.Product(x)).ToList();
                            break;
                        case "PriceLowest":
                            result = pc.productList.OrderBy(x => x.getListingPrice().value == 0 ? 1 : 0).ThenBy(x => x.getListingPrice().value).Select(x => new Models.Product(x)).ToList();
                            break;
                        case "LatestedAdd":
                            result = pc.productList.OrderByDescending(x => x.CreatedDate).Select(x => new Models.Product(x)).ToList();
                            break;
                        default:
                            result = pc.productList.Select(x => new Models.Product(x)).ToList();
                            break;
                    }
                    result.ForEach(x => x.loadMatrix());
                    
                }
                else
                {
                    if (paps == true) //筛选状态
                    {
                        var papsMarketingStatus = new eStore.POCOS.Product.PRODUCTMARKETINGSTATUS[] { eStore.POCOS.Product.PRODUCTMARKETINGSTATUS.NEW, eStore.POCOS.Product.PRODUCTMARKETINGSTATUS.HOT, eStore.POCOS.Product.PRODUCTMARKETINGSTATUS.FEATURE };

                        foreach (var v in pc.simpleProductList)
                        {
                            foreach (var r in v.mappedToProduct().marketingstatus)
                                if (papsMarketingStatus.Contains(r))
                                    simpProductList.Add(v);
                        }
                    }
                    else
                        simpProductList = pc.simpleProductList;

                    if (!string.IsNullOrEmpty(filterType))
                        Presentation.eStoreContext.Current.Store.margerSimpleProduct(ref simpProductList, filterType);

                    if (MatrixPage && eStore.Presentation.eStoreContext.Current.getBooleanSetting("showMatrix")
                            && pc.DisplayTypeX == POCOS.ProductCategory.RenderStyle.SelectBySpec && attrs != null && attrs.Any())
                    {
                        List<POCOS.SimpleProduct> simpProductListTemp = new List<POCOS.SimpleProduct>();
                        List<Models.Product> mprolstemp = new List<Models.Product>();
                        List<Models.VCategoryAttr> attrstemp = new List<Models.VCategoryAttr>();
                        var prls = simpProductList.Select(x => new Models.Product(x)).ToList();
                        prls.ForEach(x => x.loadMatrix());

                        var attrlsTemp = attrs.GroupBy(x => new { x.CateId, x.AttrId }).Select(g => new Models.AttrGP_Result
                            { CateId = g.Key.CateId, AttrId = g.Key.AttrId, Values = g.ToList(), IsMach = false }).ToList();

                        foreach (var pro in prls)
                        {
                            attrlsTemp.ForEach(c => c.IsMach = false); // reset macth false.
                            foreach (var valg in attrlsTemp)
                            {
                                if (valg.Values.Count == 1) // only one
                                {
                                    if (pro.Matrix.Any(c => c.CatID == valg.Values[0].CateId && c.AttrId == valg.Values[0].AttrId && c.AttrValueId == valg.Values[0].ValueId))
                                        valg.IsMach = true;
                                }
                                else
                                {
                                    foreach (var item in valg.Values) // or
                                    {
                                        if (pro.Matrix.Any(c => c.CatID == item.CateId && c.AttrId == item.AttrId && c.AttrValueId == item.ValueId))
                                        {
                                            valg.IsMach = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (!attrlsTemp.Any(c => !c.IsMach)) // math success
                            {
                                if (!simpProductListTemp.Any(c => c.SProductID == pro.Id))
                                    simpProductListTemp.Add(simpProductList.FirstOrDefault(c => c.SProductID == pro.Id));
                                if (!mprolstemp.Any(c => c.Id == pro.Id))
                                    mprolstemp.Add(pro);
                            }
                        }

                        simpProductList = simpProductListTemp;
                        foreach (var sproitem in mprolstemp)
                        {
                            foreach (var spc in sproitem.Matrix)
                            {
                                if (!attrstemp.Any(c => c.Id == $"{spc.CatID}-{spc.AttrId}-{spc.AttrValueId}"))
                                    attrstemp.Add(new Models.VCategoryAttr(spc));
                            }
                        }
                        VResult.ProductAttrs = attrstemp;
                    }

                    string pagesizeStr = eStore.Presentation.eStoreContext.Current.getStringSetting("eStore_PageSizes", "9");
                    var availablepagesize = pagesizeStr.Split(',').Select(c => int.Parse(c)).ToList();

                    if (availablepagesize.Contains(pagesize) == false && simpProductList.Count != pagesize)
                        pagesize = availablepagesize.Min();
                    if (paps == true)
                        pagesize = availablepagesize.Max();

                    VResult.Count = simpProductList.Count; // reset prouct count.
                    if (string.IsNullOrEmpty(SortType))
                        SortType = pc.SortType;
                    VResult.SortType = SortType;

                    if (simpProductList.Count > (page - 1) * pagesize)
                    {
                        switch (SortType)
                        {
                            case "PriceHighest":
                                result = simpProductList.OrderByDescending(x => x.mappedToProduct().getListingPrice().value).Skip((page - 1) * pagesize).Take(pagesize).Select(x => new Models.Product(x)).ToList();
                                break;
                            case "PriceLowest":
                                result = simpProductList.OrderBy(x => x.mappedToProduct().getListingPrice().value == 0 ? 1 : 0).ThenBy(x => x.mappedToProduct().getListingPrice().value).Skip((page - 1) * pagesize).Take(pagesize).Select(x => new Models.Product(x)).ToList();
                                break;
                            case "LatestedAdd":
                                result = simpProductList.OrderByDescending(x => x.CreatedDate).Skip((page - 1) * pagesize).Take(pagesize).Select(x => new Models.Product(x)).ToList();
                                break;
                            default:
                                result = simpProductList.OrderBy(x => x.Sequnce).Skip((page - 1) * pagesize).Take(pagesize).Select(x => new Models.Product(x)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (pc.SortTypeX)
                        {
                            case POCOS.ProductCategory.Sort_Type.PriceHighest:
                                result = simpProductList.OrderByDescending(x => x.mappedToProduct().getListingPrice().value).Select(x => new Models.Product(x)).ToList();
                                break;
                            case POCOS.ProductCategory.Sort_Type.PriceLowest:
                                result = simpProductList.OrderBy(x => x.mappedToProduct().getListingPrice().value == 0 ? 1 : 0).ThenBy(x => x.mappedToProduct().getListingPrice().value).Select(x => new Models.Product(x)).ToList();
                                break;
                            case POCOS.ProductCategory.Sort_Type.LatestedAdd:
                                result = simpProductList.OrderByDescending(x => x.CreatedDate).Select(x => new Models.Product(x)).ToList();
                                break;
                            default:
                                result = simpProductList.OrderBy(x => x.Sequnce).Select(x => new Models.Product(x)).ToList();
                                break;
                        }                            
                    }
                }
                return result;

            }
            else
                return null;
        }
    }
}