using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Product
{
    public partial class SubCategory : Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                if (Request["category"] == null || string.IsNullOrEmpty(Request["category"]))
                {
                    Presentation.eStoreContext.Current.AddStoreErrorCode("Product category is not available", null, true);
                }
                else
                {
                    string _category = Request["category"].ToString();
                    //List<POCOS.Product> products = new List<POCOS.Product>();
                    //POCOS.ProductCategory productCategory = Presentation.eStoreContext.Current.Store.getProducts(_category, out products, false);
                    POCOS.ProductCategory productCategory = Presentation.eStoreContext.Current.Store.getProductCategory(_category);
                    if (productCategory != null)
                    {
                        //set user activit log
                        this.UserActivitLog.CategoryID = productCategory.CategoryID.ToString();
                        this.UserActivitLog.CategoryName = productCategory.CategoryName;
                        this.UserActivitLog.CategoryType = productCategory.CategoryType;

                        this.lCategoryName.Text = eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(productCategory);
                        this.lCategoryDescription.Text = eStore.Presentation.eStoreGlobalResource.getLocalCategoryExtDescription(productCategory);
                        this.YouAreHere1.productCategory = productCategory;
                        Presentation.eStoreContext.Current.BusinessGroup = productCategory.businessGroup;
                        this.isExistsPageMeta = setPageMeta(productCategory.pageTitle, productCategory.metaData, productCategory.keywords);
                        this.ProductCategory1.productCategory = productCategory;

                        eStoreLiquidSlider1.Advertisements = Presentation.eStoreContext.Current.Store.sliderBanner(productCategory);
                        //if root category and have slider banners, then hide you are here
                        if (productCategory.parentCategoryX == null)
                            YouAreHere1.Visible = eStoreLiquidSlider1.Advertisements.Any() == false;
                        if (productCategory.parentCategoryX == null)//fist level
                        {
                            if (productCategory.childCategoriesX.Count > 2)
                                this.dlRootCategorylist.RepeatColumns = 3;
                            else
                            {
                                this.dlRootCategorylist.RepeatColumns = 2;
                                this.dlRootCategorylist.ItemStyle.CssClass = "SubCategoryTowColumn";
                            }
                            this.dlRootCategorylist.DataSource = productCategory.childCategoriesX;
                            this.dlRootCategorylist.DataBind();
                            this.dlCategory.Visible = false;
                            this.singlecategories.Visible = false;
                        
                        }
                        else if (productCategory.childCategoriesX.Count == 0)//end level
                        {
                            Response.Redirect(Presentation.UrlRewriting.MappingUrl.getMappingUrl(productCategory));
                        }
                        else if (productCategory.childCategoriesX.TakeWhile(c => c.childCategoriesX.Count() == 0).Count() > 0)//single mutli level
                        {
                            this.dlCategory.Visible = false;
                            this.CategoryWithSubCategoryAndProducts1.productCategory = productCategory;
                            this.CategoryWithSubCategoryAndProducts1.MaxProducts = 4;
                        }
                        else
                        {
                            this.dlCategory.Visible = false;
                            this.singlecategories.Visible = false;
                            this.CategoryWithSubCategoryAndProductsV21.category = productCategory;
                            this.CategoryWithSubCategoryAndProductsV21.Visible = true;

                        
                        }
                        //else if (productCategory.childCategoriesX.Count == 1)
                        //{
                        //    this.CategoryWithSubCategoryAndProducts1.productCategory = productCategory.childCategoriesX.First();
                        //    this.CategoryWithSubCategoryAndProducts1.MaxProducts = 4;
                        //    this.dlCategory.Visible = false;
                        //}
                        //else if (productCategory.childCategoriesX.Count % 2 == 0)
                        //{
                        //    this.dlCategory.DataSource = productCategory.childCategoriesX;
                        //    this.dlCategory.DataBind();
                        //    this.CategoryWithSubCategoryAndProducts1.Visible = false;
                        //}
                        //else
                        //{
                        //    this.dlCategory.DataSource = productCategory.childCategoriesX.Take(productCategory.childCategoriesX.Count - 1);
                        //    this.dlCategory.DataBind();
                        //    this.CategoryWithSubCategoryAndProducts1.productCategory = productCategory.childCategoriesX.Last();
                        //    this.CategoryWithSubCategoryAndProducts1.MaxProducts = 4;
                        //}


                    }
                    else
                    {
                        this.UserActivitLog.CategoryName = _category;
                        this.UserActivitLog.CategoryType = "ErrorCategory";
                        Presentation.eStoreContext.Current.AddStoreErrorCode("Product category is not available", null, true);
                    }
                    Presentation.eStoreContext.Current.keywords.Add("CategoryID", _category);
                }


            }
        }
        protected void dlCategory_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                POCOS.ProductCategory category = (POCOS.ProductCategory)e.Item.DataItem;
                PlaceHolder phCategory = (PlaceHolder)e.Item.FindControl("phCategory");

                Modules.CategoryWithSubCategoryAndProducts categoryItem = (Modules.CategoryWithSubCategoryAndProducts)LoadControl("~/Modules/CategoryWithSubCategoryAndProducts.ascx");
                categoryItem.productCategory = category;
                categoryItem.MaxProducts = 2;
                phCategory.Controls.Add(categoryItem);

            }
        }
        protected void dlRootCategorylist_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                POCOS.ProductCategory category = (POCOS.ProductCategory)e.Item.DataItem;
                PlaceHolder phCategory = (PlaceHolder)e.Item.FindControl("phCategory");
                if (category.childCategoriesX == null || category.childCategoriesX.Count == 0 || category.parentCategoryX.parentCategoryX == null)
                {
                    Modules.CategoryWithoutSubCategory categoryItem = (Modules.CategoryWithoutSubCategory)LoadControl("~/Modules/CategoryWithoutSubCategory.ascx");
                    categoryItem.productCategory = category;
                    phCategory.Controls.Add(categoryItem);
                }
                else
                {
                    Modules.CategoryWithSubCategory categoryItem = (Modules.CategoryWithSubCategory)LoadControl("~/Modules/CategoryWithSubCategory.ascx");
                    categoryItem.productCategory = category;
                    phCategory.Controls.Add(categoryItem);
                }
            }
        }
    }
}