using eStore.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Account
{
    public partial class SiteBuilder :   Presentation.eStoreBaseControls.eStoreBasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                POCOS.SiteBuilder siteBuilder = new POCOS.SiteBuilder();// Presentation.eStoreContext.Current.User.getSiteBuilder();
                bind(siteBuilder);


            }
        }
        void bind(POCOS.SiteBuilder siteBuilder)
        {
            List<POCOS.ProductCategory> ls;
            ls = eStoreContext.Current.Store.getTopLevelStandardProductCategories(eStoreContext.Current.MiniSite);
            ls.AddRange(eStoreContext.Current.Store.getTopLevelCTOSProductCategories(eStoreContext.Current.MiniSite));

            List<int> filter = new List<int>();
            string sfilter = Presentation.eStoreContext.Current.getStringSetting("SICategoryFilters");
            if (!string.IsNullOrEmpty(sfilter))
            {
                try
                {
                    filter = sfilter.Split(',').Select(x => int.Parse(x)).ToList();
                }
                catch (Exception)
                {

                    
                }

            }
            if (filter.Count > 0)
            {
                ls = ls.Where(x => filter.Contains(x.CategoryID)).OrderBy(x => x.Sequence).ThenBy(x => x.localCategoryNameX).ToList();
            }
            else
                ls = ls.OrderBy(x => x.Sequence).ThenBy(x => x.localCategoryNameX).ToList();

            var data = (from category in ls
                        select new
                        {
                            category.localCategoryNameX
                            ,
                            category.CategoryID
                            ,
                            IsSelected = siteBuilder.SelectedCategories.Contains(category.CategoryID)
                        }
                      );

            this.rpCategories.DataSource = data;
            this.rpCategories.DataBind();


            rpThemes.DataSource =
                (
                from theme in Enum.GetNames(typeof(POCOS.SiteBuilder.StoreThemes))
                select new
                {
                    Name = theme,
                    IsSelected = siteBuilder.Theme == theme
                }
                );

            rpThemes.DataBind();

            if (!string.IsNullOrEmpty(siteBuilder.Logo))
            {
                imgLogo.ImageUrl = Presentation.eStoreFileManager.downloadURL(Presentation.eStoreFileManager.eStoreFileType.SystemIntegrator, siteBuilder.Logo);
            }
            txtCompanyName.Text = siteBuilder.CompanyName;
            txtPhoneNumber.Text = siteBuilder.PhoneNumber;

            txtStoreHours.Text = siteBuilder.StoreHours;

            rblHasSSLCertificate.SelectedIndex = (siteBuilder.HasSSLCertificate) ? 0 : 1;
            rblHaseCommerceChat.SelectedIndex = (siteBuilder.HaseCommerceChat) ? 0 : 1;

        }

        protected void lbtnSubmit_Click1(object sender, EventArgs e)
        {
            POCOS.SiteBuilder siteBuilder = new POCOS.SiteBuilder(); // Presentation.eStoreContext.Current.User.getSiteBuilder(true);
            if (Presentation.eStoreContext.Current.User != null)
                siteBuilder.UserId = Presentation.eStoreContext.Current.User.actingUser.UserID;

            string categories = Request["SelectedCategories"];
            if (!string.IsNullOrEmpty(categories))
            {
                List<int> ids = (from id in categories.Split(',')
                           select int.Parse(id)).ToList();
                siteBuilder.SelectedCategories = ids;
            }
            if (fupLogo.HasFile)
            {
                string Logo  = Presentation.eStoreFileManager.save(Presentation.eStoreFileManager.eStoreFileType.SystemIntegrator, this.fupLogo);
                siteBuilder.Logo = Logo;
            }
            siteBuilder.FirstName = txtFirstName .Text;
            siteBuilder.LastName = txtLastName.Text;
            siteBuilder.eMail = txteMail.Text;

            siteBuilder.CompanyName = txtCompanyName.Text;
            siteBuilder.PhoneNumber = txtPhoneNumber.Text;
            siteBuilder.StoreHours = txtStoreHours.Text;
            siteBuilder.HasSSLCertificate = rblHasSSLCertificate.SelectedIndex == 0;
            siteBuilder.HaseCommerceChat = rblHaseCommerceChat.SelectedIndex == 0;
            siteBuilder.Theme = Request["storeTheme"];
            siteBuilder.save();
            bind(siteBuilder);
            BindScript("Script", "savesuccessful", "popupMessagewithTitle('Thank you!', 'Thank you for your submission. An Advantech Representative will contact shortly.');");

            //send email 
            List<BusinessModules.VWidgetModel> viewmodels = new List<BusinessModules.VWidgetModel>();
            viewmodels.Add(new BusinessModules.VWidgetModel
            {
                Key = "TempId",
                Value = "SiteBuilder",
                Title = "TempId"
            });
            viewmodels.Add(new BusinessModules.VWidgetModel
            {
                Key = "Title",
                Value = "SiteBuilder",
                Title = "Title"
            });
            viewmodels.Add(new BusinessModules.VWidgetModel
            {
                Key = "Logo",
                Value = siteBuilder.Logo == null ? "" :$"<img scr='{esUtilities.CommonHelper.GetStoreHost(false)}resource/SystemIntegrator/{siteBuilder.Logo}'>" ,
                Title = "Logo"
            });

            viewmodels.Add(new BusinessModules.VWidgetModel
            {
                Key = "Email",
                Value = siteBuilder.eMail,
                Title = "Email"
            });
  
            viewmodels.Add(new BusinessModules.VWidgetModel
            {
                Key = "FirstName",
                Value = siteBuilder.FirstName,
                Title = "FirstName"
            });

            viewmodels.Add(new BusinessModules.VWidgetModel
            {
                Key = "LastName",
                Value = siteBuilder.LastName,
                Title = "LastName"
            });

      

            viewmodels.Add(new BusinessModules.VWidgetModel
            {
                Key = "CompanyName",
                Value = siteBuilder.CompanyName,
                Title = "CompanyName"
            });
            viewmodels.Add(new BusinessModules.VWidgetModel
            {
                Key = "PhoneNumber",
                Value = siteBuilder.PhoneNumber,
                Title = "PhoneNumber"
            });
            viewmodels.Add(new BusinessModules.VWidgetModel
            {
                Key = "StoreHours",
                Value = siteBuilder.StoreHours,
                Title = "StoreHours"
            });
            viewmodels.Add(new BusinessModules.VWidgetModel
            {
                Key = "HasSSLCertificate",
                Value = siteBuilder.HasSSLCertificate.ToString(),
                Title = "HasSSLCertificate"
            });
            viewmodels.Add(new BusinessModules.VWidgetModel
            {
                Key = "HaseCommerceChat",
                Value = siteBuilder.HaseCommerceChat.ToString(),
                Title = "HaseCommerceChat"
            });
            viewmodels.Add(new BusinessModules.VWidgetModel
            {
                Key = "Theme",
                Value = siteBuilder.Theme,
                Title = "Theme"
            });

            List<POCOS.ProductCategory> ls;
            ls = eStoreContext.Current.Store.getTopLevelStandardProductCategories(eStoreContext.Current.MiniSite);
            ls.AddRange(eStoreContext.Current.Store.getTopLevelCTOSProductCategories(eStoreContext.Current.MiniSite));
            ls = ls.OrderBy(x => x.Sequence).ThenBy(x => x.localCategoryNameX).ToList();
            viewmodels.Add(new BusinessModules.VWidgetModel
            {
                Key = "SelectedCategories",
                Value = string.Join(",", ls.Where(c => siteBuilder.SelectedCategories.Contains(c.CategoryID)).Select(c => c.localCategoryNameX)),
                Title = "SelectedCategories"
            });


            eStore.BusinessModules.EMailNoticeTemplate mailTemplate = new BusinessModules.EMailNoticeTemplate(Presentation.eStoreContext.Current.Store);
            var response = mailTemplate.sendWidgetRequestEmail(viewmodels
                , siteBuilder.eMail.Split('@')[0]
                , eStore.Presentation.eStoreLocalization.Tanslation("eStore_SiteBuilder_subject")
                , eStore.Presentation.eStoreContext.Current.Store
                , eStore.Presentation.eStoreContext.Current.CurrentLanguage
                , eStore.Presentation.eStoreContext.Current.MiniSite
                , "buy@Advantech.com;Gladys.Liu@Advantech.com;Russell.Barber@Advantech.com;Mike.Liu@Advantech.com");
        }
    }
}