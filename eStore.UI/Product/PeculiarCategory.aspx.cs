using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Script.Serialization;
using eStore.POCOS;
using System.Web.UI.HtmlControls;

namespace eStore.UI.Product
{
    public partial class PeculiarCategory : Presentation.eStoreBaseControls.eStoreBasePage
    {
        public override bool OverwriteMasterPageFile
        {
            get
            {
                return false;
            }
        }

        private string _categoryName;
        public string categoryName
        {
            get 
            {
                if (string.IsNullOrEmpty(_categoryName))
                {
                    if (Request.QueryString["Category"] != null)
                        _categoryName = esUtilities.StringUtility.replaceSpecialString(Request.QueryString["Category"]).ToUpper();
                    else
                        _categoryName = "ICG";
                }
                return _categoryName; 
            }
            set { _categoryName = value; }
        }


        private Spec_Category _category;
        public Spec_Category category
        {
            get 
            {
                if (_category == null)
                    _category = allCategory.FirstOrDefault(c => c.CATEGORY_LEVEL == 1);
                return _category; 
            }
            set { _category = value; }
        }


        private List<Spec_Category> _allCategory;
        public List<Spec_Category> allCategory
        {
            get 
            {
                if (_allCategory == null)
                    _allCategory = eStore.Presentation.eStoreContext.Current.Store.getAllSpecCategoryByRootId(categoryName);
                return _allCategory; 
            }
        }

        public string RBUStyles
        {
            get
            {
                if (categoryName == "ECG")
                {
                    return "icg-addSubCate-ECG";
                }
                return "icg-addSubCate";
            }
        }
        public int appendixID
        {
            get
            {
                int _appendixID = 0;
                if (this.category != null && this.category.CATEGORY_ID == 384)
                    return 412;
                return _appendixID;
            }
        }
        


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(categoryName))
                eStore.Presentation.eStoreContext.Current.Store.getAllSpecCategoryByRootId(categoryName);
            else
            {
                Presentation.eStoreContext.Current.AddStoreErrorCode("Product category is not available", null, true);
            }

            //AddStyleSheet(ResolveUrl("~/Styles/custom-Marianna.css"));
            //AddStyleSheet(ResolveUrl("~/Styles/simplePagination.css"));
            //BindScript("url", "simplePagination.js", "jquery.simplePagination.js");
            btConpare.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Compare);
            if (!IsPostBack)
                bindBaseInfor();

            //this.AdRotatorSelect1.BannerList = eStore.Presentation.eStoreContext.Current.Store.sliderBanner(string.Format("ProductWizard-{0}",categoryName));
            //this.AdRotatorSelect1.BannerWidth = 781;
            Presentation.eStoreContext.Current.keywords.Add("KeyWords", string.Format("ProductWizard-{0}", categoryName));
            this.isExistsPageMeta = setPageMeta($"{this.categoryName + " selector"} - {Presentation.eStoreContext.Current.Store.profile.StoreName}"
                , "", "");
        }

        protected void bindBaseInfor()
        {
            rpCategories.DataSource = allCategory.Where(c => c.CATEGORY_LEVEL == 2).OrderBy(c => c.SEQUENCE).ToList();
            rpCategories.DataBind();
        }


        protected void btConpare_Click(object sender, EventArgs e)
        {
            List<string> ls = new List<string>();
            string requestproducts = Request["temperature"];
            if (!string.IsNullOrEmpty(requestproducts))
                ls = requestproducts.Split(',').ToList();
            if (!ls.Any())
            {
                eStore.Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Iot_select_product_first));
                return;
            }
            eStore.Presentation.Product.ProductCompareManagement.setProductIDs(ls);
            Response.Redirect("~/Compare.aspx");
        }

        protected void rpCategories_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Spec_Category cate = e.Item.DataItem as Spec_Category;
                Literal ltCategoryTree = e.Item.FindControl("ltCategoryTree") as Literal;
                ltCategoryTree.Text = margerCateTree(cate);

            }
        }

        protected string margerCateTree(Spec_Category cate)
        {
            string str = "";
            cate.StoreID = Presentation.eStoreContext.Current.Store.storeID;
            var ls = allCategory.Where(f => f.NodeTree.StartsWith(cate.NodeTree) && f.CATEGORY_LEVEL == cate.CATEGORY_LEVEL + 1).OrderBy(f => f.SEQUENCE).ToList();
            switch (cate.displayType)
            {
                case Spec_Category.DisType.CheckBoxList:
                case Spec_Category.DisType.TextBox:
                    foreach (var c in ls)
                        str += string.Format("<div class=\"ckbItemLine\"><input class=\"willgetSelect\" type=\"checkbox\" name=\"inp{0}\" id=\"inp{1}\" value=\"{1}\"><label for=\"inp{1}\" id='c{1}' class='category-count' oldText='{2}'>{2}</label></div>"
                                , cate.CATEGORY_ID, c.CATEGORY_ID, c.Translation_Name);
                    break;
                case Spec_Category.DisType.DropdownList:
                    str += string.Format("<select class=\"willgetSelect\" id=\"ddl{0}\" name=\"ddl{0}\"><option value=0>{1}</option>", cate.CATEGORY_ID, cate.Translation_Name);
                    foreach (var c in ls)
                        str += string.Format("<option value=\"{0}\" id='c{0}' oldText='{1}' class='category-count'>{1}</option>"
                                ,c.CATEGORY_ID,c.Translation_Name);
                    str += "</select>";
                    break;
                case Spec_Category.DisType.List:
                    int i = 1;
                    foreach (var c in ls)
                    {
                        string divddl = "<div dataid='"+ c.CATEGORY_ID +"'{1}>{0}</div>";
                        string dtitle = "<span{1}>{0}</span>";
                        switch(c.displayType)
                        {
                            case Spec_Category.DisType.List:
                                dtitle = string.Format(dtitle, c.Translation_Name, " class='icgSubList'");
                                divddl = string.Format(divddl, dtitle + margerCateTree(c), " class='icgListCate" + (i % 2 == 1 ? " icgleftline" : "") + "'");
                                break;
                            case Spec_Category.DisType.DropdownList:
                                dtitle ="";// string.Format(dtitle, c.Translation_Name, " class='icgThreeList'");
                                divddl =  string.Format(divddl, dtitle + margerCateTree(c), "");
                                break;
                            case Spec_Category.DisType.TextBox:
                                dtitle = string.Format(dtitle, c.Translation_Name, " class='paddingleft10'");
                                divddl = string.Format(divddl, dtitle + margerCateTree(c), " id='inp" + c.CATEGORY_ID.ToString() + "' class='icgPower'");
                                break;
                            case Spec_Category.DisType.CheckBoxList:
                            default:
                                divddl = string.Format(divddl, margerCateTree(c), "");
                                break;
                        }
                        
                        i++;
                        str += divddl;
                    }
                    break;
                case Spec_Category.DisType.RadioButtonList:
                    foreach (var c in ls)
                        str += string.Format("<div class=\"ckbItemLine\"><input class=\"willgetSelect\" type=\"checkbox\" name=\"inp{0}\" id=\"inp{1}\" value=\"{1}\"><label for=\"inp{1}\" id='c{1}' class='category-count' oldText='{2}'>{2}</label></div>"
                                , cate.CATEGORY_ID, c.CATEGORY_ID, c.Translation_Name);
                    break;
                case Spec_Category.DisType.DoubleList:
                    string pc = string.Empty;
                    string gc = string.Empty;
                    foreach (var c in ls)
                    {
                        if (c.children.Count > 0)
                        {
                            pc += string.Format("<option value=\"pc{0}\" class=\"willgetSelect\" name=\"{2}\">{1}</option>", c.CATEGORY_ID, c.Translation_Name, c.children.Count);

                            foreach (var gv in c.children)
                                gc += string.Format("<option value=\"{0}\" class=\"willgetSelect\" name=\"pc{2}\">{1}</option>", gv.CATEGORY_ID, gv.Translation_Name, c.CATEGORY_ID);
                        }
                    }
                    for (int j = 1; j <=4; j++)
                    {
                        str += "<div class=\"icg-addSubCate-ECG\">";
                        str += string.Format("<select class=\"doubleListSelect\" id=\"m{0}{1}\" name=\"s{0}{1}\"><option value=0>Type</option>", j, cate.CATEGORY_ID);
                        str += pc.Replace("pc", string.Format("pc{0}", j));
                        str += "</select></div><div class=\"ecg-addSubCatelayHalf\">";
                        str += string.Format("<select class=\"willgetSelect\" id=\"s{0}{1}\" ><option value=0>Qty</option>", j, cate.CATEGORY_ID);
                        str += gc.Replace("pc", string.Format("pc{0}", j));
                        str += "</select></div>";
                    }
                    pc = string.Empty;
                    gc = string.Empty;
                    break;
                case Spec_Category.DisType.DoubleRow:
                    int k = 1;
                    foreach (var c in ls)
                    {
                        string divddl = string.Format("<div class=\"ckbItemLine {3}\"><input class=\"willgetSelect\" type=\"checkbox\" name=\"inp{0}\" id=\"inp{1}\" value=\"{1}\"><label for=\"inp{1}\" id='c{1}' class='category-count' oldText='{2}'>{2}</label></div>"
                                , cate.CATEGORY_ID, c.CATEGORY_ID, c.Translation_Name, (k % 2 == 1 ? "unoCate" : ""));
                        k++;
                        str += divddl;
                    }
                    break;
                default:
                    break;
            }
            return str;
        }
    }
}