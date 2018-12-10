using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation.Product;
using System.Text;
namespace eStore.UI
{
    public partial class Compare : Presentation.eStoreBaseControls.eStoreBasePage
    {
        public override bool OverwriteMasterPageFile
        {
            get
            {
                return false;
            }
        }
        public override bool isMobileFriendly
        {
            get
            {
                return true;
            }
            set
            {
                base.isMobileFriendly = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string pagetitle = "Comparison Results";

                if (!string.IsNullOrEmpty(Request["fp"]))
                {
                    pagetitle = $"Sorry, {Request["fp"]} is not available,<br /> but we have similar one for you !";
                    ProductsComparison1.Title = pagetitle;
                }
                

                string comparisionproducts=string.Empty;

                if (!string.IsNullOrEmpty(Request["parts"]))
                {
                    List<POCOS.Part> parts = Presentation.eStoreContext.Current.Store.getPartList(Request["parts"]);
                    ProductsComparison1.parts = parts.Where(x => x is POCOS.Product && x.isOrderable()).Select(x => (POCOS.Product)x).ToList();
                }
                else if (!string.IsNullOrEmpty(Request["action"]) && !string.IsNullOrEmpty(Request["type"]))
                {
                    string model=null , pn = null, key=string.Empty;
                    switch (Request["type"])
                    {
                        case "model":
                            model = Request["modelno"];
                            key = model;
                            break;
                        case "pn":
                            pn  = Request["ProductID"];
                            
                            key = pn;
                          
                            break;
                        default:
                            break;
                    }
                    POCOS.DAL.PartHelper helper = new POCOS.DAL.PartHelper();
                    List<POCOS.Product> products = new List<POCOS.Product>();
                    var raw = helper.link2eStoreRaw(model, pn, Presentation.eStoreContext.Current.Store.storeID);
                    switch (Request["action"])
                    {
                        case "buyonline":
                            pagetitle = string.Format("Select your product for {0}", key.ToUpper());
                            products = helper.prefetchProductList(Presentation.eStoreContext.Current.Store.storeID, raw.Where(x=>!string.IsNullOrEmpty(x.SProductID)).Select(x => x.SProductID).Distinct().ToList());
                            break;
                        case "configure":

                            pagetitle = string.Format("Configure Your System for {0}", key.ToUpper());
                            products = helper.prefetchProductList(Presentation.eStoreContext.Current.Store.storeID, raw.Where(x => !string.IsNullOrEmpty(x.systemSProductID)).Select(x => x.systemSProductID).Distinct().ToList());
                            break;
                        default:
                            break;
                    }

                    ProductsComparison1.parts = products;
                    ProductsComparison1.Title = pagetitle;
                }
                else
                {
                    List<POCOS.Product> parts = getProducts();
                    if (parts != null && parts.Any())
                    {
                        ProductsComparison1.parts = parts;
                    }
                }
               // ScriptManager.RegisterArrayDeclaration(this, "comparisionproducts", comparisionproducts);
                bindFonts();
                this.isExistsPageMeta = setPageMeta(
         $"{pagetitle} - {Presentation.eStoreContext.Current.Store.profile.StoreName}", $"{Presentation.eStoreContext.Current.Store.profile.StoreName} {pagetitle}", "");

            }
        }
        protected void lClearCompareItems_Click(object sender, EventArgs e)
        {
            ProductCompareManagement.ClearCompareProducts();
            Page.Response.Redirect("~/");
        }

        protected void bindFonts()
        {

        }
        private void generateCompareUI(List<POCOS.Product> parts)
        {
         
            StringBuilder sbCompare = new StringBuilder();
            StringBuilder sbCompareTop = new StringBuilder();
            StringBuilder sbCompareBottom = new StringBuilder();
            sbCompareTop.AppendFormat("<div class=\"eStore_compare_contentTop\"><div class=\"eStore_compare_left\"><div class=\"eStore_productBlock_name\">{0}</div><div class=\"eStore_productBlock_action\">{1}</div><div class=\"eStore_productBlock_link\">{2}</div></div><!--left--><div class=\"eStore_compare_right\"><div class=\"carouselBannerSingle\" id=\"comparingProducts\"><ul>"
                , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Name)
                , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Price)
                 , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Datasheet));
            foreach(var product in parts)
            {
                sbCompareTop.AppendFormat("<li><div class=\"eStore_productBlock\"><div class=\"eStore_productBlock_pic\"><a href=\"#\" class=\"close\"><img src=\"images/orderlistTable_close.png\" alt=\"remove this product\" /></a><img src=\"{0}\" alt=\"{7}\" title=\"{1}\" /></div><div class=\"eStore_productBlock_name\">{1}</div><div class=\"eStore_productBlock_action\"><div class=\"priceOrange\">{2}</div><a href=\"#\" class=\"eStore_btn borderBlue\">{3}</a><a href=\"#\" class=\"eStore_btn\">{4}</a></div><div class=\"eStore_productBlock_link\"><a href=\"{5}\" class=\"eStore_linkDataSheet\">{6}</a></div></div></li>"
                    , product.thumbnailImageX
                    ,product.name
                    ,Presentation.Product.ProductPrice.getPrice(product)
                    ,eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoCart)
                    , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoQuotation)
                    ,product.dataSheetX
                    , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Datasheet)
                    , product.productDescX
                    );
            }
            sbCompareTop.Append("</ul><div class=\"clearfix\"></div><div class=\"carousel-control\"><a id=\"prev\" class=\"prev\" href=\"#\"></a><a id=\"next\" class=\"next\" href=\"#\"></a></div></div></div></div>");

            var specificationAttributes = from spec in parts[0].specs
                                          select new
                                          {
                                              CatID = spec.CatID,
                                              Category=spec.LocalCatName,
                                              ID = spec.AttrID,
                                              Name = spec.LocalAttributeName
                                          };

            for (int i = 1; i < parts.Count; i++)
            {
                specificationAttributes = specificationAttributes.Union(
                     from spec in parts[i].specs
                     orderby spec.seq, spec.LocalAttributeName
                     select new
                     {
                         CatID = spec.CatID,
                         Category = spec.LocalCatName,
                         ID = spec.AttrID,
                         Name = spec.LocalAttributeName
                     });
            }

            var specCategories = (from s in specificationAttributes
                                  group s by new { s.CatID, s.Category } into g
                                  select new
                                  {
                                      g.Key.CatID,
                                      g.Key.Category,
                                      Attributes = (from a in g
                                                    group a by new { a.ID } into ag
                                                    select new
                                                    {
                                                        ag.Key.ID,
                                                        Name = ag.Select(x => x.Name).FirstOrDefault()
                                                    })
                                  });

            sbCompareBottom.Append("<div class=\"eStore_compare_contentBottom\">");
            foreach (var sc in specCategories)
            {
                sbCompareBottom.Append("<div class=\"eStore_compare_contentCategory\">");
                sbCompareBottom.AppendFormat("<div class=\"eStore_compare_contentCategory_title eStore_openBox\">{0}</div>",sc.Category);
                sbCompareBottom.Append("<div class=\"eStore_compare_table\">");

                StringBuilder sbCompareBottomRight = new StringBuilder();
                sbCompareBottom.Append("<ul class=\"eStore_compare_left\">");
                foreach (var sa in sc.Attributes)
                {
                    sbCompareBottom.AppendFormat("<li>{0}</li>", sa.Name);
                    sbCompareBottomRight.Append("<ul>");
                    foreach (var p in parts)
                    {
                        var spec = p.specs.FirstOrDefault(x => x.CatID == sc.CatID && x.AttrID == sa.ID);
                        sbCompareBottomRight.AppendFormat("<li>{0}</li>", spec == null ? "-" : spec.LocalValueName);
                    }
                    sbCompareBottomRight.Append("</ul>");
                }
                sbCompareBottom.Append("</ul>");

                sbCompareBottom.Append("<div class=\"eStore_compare_right\">");
                sbCompareBottom.Append(sbCompareBottomRight.ToString());
                sbCompareBottom.Append("</div>");

                sbCompareBottom.Append("</div>");
                sbCompareBottom.Append("</div>");
            }
            sbCompareBottom.Append("</div>");
            sbCompareTop.Append(sbCompareBottom.ToString());
           
        }

        private List<POCOS.Product> getProducts()
        {
            IList<POCOS.Product> compareProducts = new List<POCOS.Product>();

            if (!string.IsNullOrEmpty(Request["category"]))
            {
                compareProducts = ProductCompareManagement.GetCompareProductsByCategoryID();


            }
            else if (!string.IsNullOrEmpty(Request["parts"]))
            {
                compareProducts = ProductCompareManagement.GetCompareProductsFromQueryString();

            }
            else if (!string.IsNullOrEmpty(Request["partno"]))
            {
                compareProducts = ProductCompareManagement.GetCompareSystemsByComponent();

            }
            else
            {
                compareProducts = ProductCompareManagement.GetCompareProducts();

            }
            return compareProducts.ToList();
        }

    }
}