using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Kit
{
    public partial class Theme : Presentation.eStoreBaseControls.eStoreBasePage
    {
        private POCOS.KitTheme _theme;
        public POCOS.KitTheme theme
        {
            get
            {
                if (_theme == null)
                {
                    int id = 0;
                    if (int.TryParse(Request.QueryString["id"], out id))
                        _theme = eStore.Presentation.eStoreContext.Current.Store.getKitThemeById(id);
                    else
                    {
                        if (themType != null && themType.Id != 0)
                            _theme = eStore.Presentation.eStoreContext.Current.Store.getKitThemeById(themType.ThemeId.GetValueOrDefault());
                        else
                            _theme = new POCOS.KitTheme();
                    }
                }
                return _theme;
            }
        }

        private POCOS.KitThemeType _themeType;
        protected POCOS.KitThemeType themType
        {
            get
            {
                if (_themeType == null)
                {
                    int tid = 0;
                    if (int.TryParse(Request.QueryString["tid"], out tid))
                    {
                        if (_theme != null)
                            _themeType = theme.KitThemeTypes.FirstOrDefault(c => c.Id == tid);
                        else
                            _themeType = eStore.Presentation.eStoreContext.Current.Store.getKitThemeTypeById(tid);
                    }
                    else
                    {
                        if(theme != null)
                            _themeType = theme.KitThemeTypes.FirstOrDefault();
                    }
                }                
                return _themeType;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (theme != null)
            {
                //ltTitle.Text = theme.Title;

                rpTypes.DataSource = theme.KitThemeTypes.OrderBy(c => c.Seq).ToList();
                rpTypes.DataBind();

                if (themType != null && themType.KitThemeMappings != null)
                {
                    string rbu = eStore.Presentation.eStoreContext.Current.getStringSetting("StoreThemeRBU");
                    List<Models.VKitThemeData> datas = new List<Models.VKitThemeData>();
                    foreach (var mapp in themType.KitThemeMappings)
                    {
                        if (string.IsNullOrEmpty(mapp.Doc))
                        {
                            datas.Add(new Models.VKitThemeData { Type = mapp.Doc, Title = mapp.Name, ShowType = mapp.ShowType });
                        }
                        else
                        {
                            string types = string.Join(",", mapp.Doc.Replace("_", " ").Replace("Slash", "/"));
                            var data = BusinessModules.CMSManager.GetCmsFromApi(rbu, "", types, theme.Tags);
                            var cuitem = datas.FirstOrDefault(c => c.ShowType == mapp.ShowType);
                            if (cuitem == null)
                                datas.Add(new Models.VKitThemeData { Type = mapp.Doc, Title = mapp.Name, Data = data, ShowType = mapp.ShowType });
                            else
                            {
                                cuitem.Data.AddRange(data);
                                cuitem.Title += "&" + mapp.Name;
                            }
                        }
                    }

                    foreach (var item in datas)
                    {
                        if (item.ShowType != "Products")
                        {
                            var control = this.Page.LoadControl("~/Modules/KitTheme/" + item.ShowType + ".ascx") as Modules.KitTheme.BaseTheme;
                            control.CmsModels = item.Data;
                            control.Title = item.Title;
                            control.Theme = theme;
                            phTheme.Controls.Add(control);
                        }
                        else
                        {
                            POCOS.ProductCategory rootCategory = new POCOS.ProductCategory { ChildCategories = new List<POCOS.ProductCategory>() };
                            foreach (var c in themType.KitThemeMappings.FirstOrDefault(c => c.ShowType == "Products").KitThemeMappingDetails.OrderBy(c => c.Seq))
                            {
                                POCOS.ProductCategory _currentProductcategory = Presentation.eStoreContext.Current.Store.getProductCategory(c.Doc);
                                if (_currentProductcategory != null)
                                    rootCategory.AddCategoryX(_currentProductcategory);
                            }
                            Modules.KitTheme.Products control = this.LoadControl("~/Modules/KitTheme/" + item.ShowType + ".ascx") as Modules.KitTheme.Products;
                            control.CurrentCategory = rootCategory;
                            phTheme.Controls.Add(control);
                        }
                    }
                    eStoreCycle2Slider1.Advertisements = Presentation.eStoreContext.Current.Store.getKitThemeAdvertisement(themType);
                }

            }
        }
    }
}