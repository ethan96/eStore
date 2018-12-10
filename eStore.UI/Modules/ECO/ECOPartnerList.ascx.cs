using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.POCOS;
using eStore.UI.Modules.ECO;

namespace eStore.UI.Modules
{
    public partial class ECOPartnerList : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {

        private List<ECOPartner> dataSource;
        public List<ECOPartner> DataSource
        {
            get { return dataSource; }
            set { dataSource = value; }
        }

        private EcoInfor searchInfor;
        public EcoInfor SearchInfor
        {
            get { return searchInfor; }
            set { searchInfor = value; }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (this.CollectionPager1.Visible)
                bottomPager.Text = CollectionPager1.RenderedHtml;
            else
                bottomPager.Text = string.Empty;

            base.Render(writer);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                if (eStore.Presentation.eStoreContext.Current.User != null)
                {
                    txtFr_YourName.Text = eStore.Presentation.eStoreContext.Current.Store.getCultureGreetingName(eStore.Presentation.eStoreContext.Current.User.FirstName, eStore.Presentation.eStoreContext.Current.User.LastName);
                    txtFr_YourEmail.Text = eStore.Presentation.eStoreContext.Current.User.UserID;

                    txtRa_Tel.Text = eStore.Presentation.eStoreContext.Current.User.TelNo;
                    txtRa_Email.Text = eStore.Presentation.eStoreContext.Current.User.UserID;
                    txtRa_FirstName.Text = eStore.Presentation.eStoreContext.Current.User.FirstName;
                    txtRa_LastName.Text = eStore.Presentation.eStoreContext.Current.User.LastName;
                    if (eStore.Presentation.eStoreContext.Current.User.actingUser.mainContact != null)
                    {
                        var contact = eStore.Presentation.eStoreContext.Current.User.actingUser.mainContact;
                        txtRa_Address.Text = contact.Address1 + " " + contact.Address2;
                        txtRa_Zip.Text = contact.ZipCode;
                        txtRa_Tel.Text = contact.TelNo;
                    }
                }
            }
            bindResult();
        }

        public void bindResult()
        {
            if (DataSource != null)
            {
                CollectionPager1.DataSource = DataSource;
                CollectionPager1.BindToControl = rpECOSearch;
                rpECOSearch.DataSource = CollectionPager1.DataSourcePaged;
                rpECOSearch.DataBind();

                ltCount.Text = DataSource.Count.ToString();
            }
            if (SearchInfor != null)
            {
                ltCountry.Text = SearchInfor.Country;
                liSpecialty.Text = string.Join(",", SearchInfor.Specialties);
                if (!string.IsNullOrEmpty(SearchInfor.StateName))
                    ltCountry.Text += string.Format(" {0} {1} ", "<img src='App_Themes/Default/arrow_blue.jpg'>", SearchInfor.StateName);
                if (!string.IsNullOrEmpty(SearchInfor.Industry))
                    ltIndustry.Text = " " + SearchInfor.Industry;
                else
                    ltIndustry.Text = "";
            }
            else
                ecoSearchHere.Visible = false;
                        
            bindCollectionPagerFonts();
        }

        protected void bindCollectionPagerFonts() //可以考虑提升到 CollectionPager 控件中
        {
            CollectionPager1.ResultsFormat = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Displaying_results);
            CollectionPager1.NextText = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Next);
            CollectionPager1.BackText = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Previous);
            CollectionPager1.FirstText = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_First);
            CollectionPager1.LastText = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Last);

            SelectCountry1.CountryLableName = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Country) + ":";
            SelectCountry1.StateLableName = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_State_Province) + ":";
        }

        protected void btFr_Send_Click(object sender, EventArgs e)
        {
            string emailTo = txtFr_FrindsEmail.Text.Trim();
            if (string.IsNullOrEmpty(emailTo))
                return; // show error
            int partnerid = 0;
            int.TryParse(hfFr_PartnerId.Value, out partnerid);
            POCOS.ECOPartner partner = eStore.Presentation.eStoreContext.Current.Store.getECOPartnerById(partnerid);
            if (partner != null)
            {
                eStore.BusinessModules.EmailECOPartner emailHelper = new BusinessModules.EmailECOPartner(eStore.Presentation.eStoreContext.Current.Store, partner);
                var result = emailHelper.sendECORecommend2Friend(eStore.Presentation.eStoreContext.Current.User, emailTo);
                if (result.ErrCode != esUtilities.EMailReponse.ErrorCode.NoError)
                    alertMessage(result.ErrCode.ToString());
                else
                    alertMessage("Email send success!");
            }
            else
                alertMessage("get Partner Error!");

        }



        protected void btRa_Send_Click(object sender, EventArgs e)
        {
            int partnerid = 0;
            int.TryParse(hfFr_PartnerId.Value, out partnerid);
            POCOS.ECOPartner partner = eStore.Presentation.eStoreContext.Current.Store.getECOPartnerById(partnerid);
            if (partner != null)
            {
                //save to db
                POCOS.UserRequest request = new POCOS.UserRequest(eStore.Presentation.eStoreContext.Current.Store.profile, POCOS.UserRequest.ReqType.ECORequest)
                {
                    Address = txtRa_Address.Text.Trim() + " , " + txtRa_Zip.Text.Trim(),
                    Budget = txtRa_Budget.Text.Trim(),
                    Email = txtRa_Email.Text.Trim(),
                    FirstName = txtRa_FirstName.Text.Trim(),
                    LastName = txtRa_LastName.Text.Trim(),
                    Country = SelectCountry1.Country,
                    State = SelectCountry1.State,
                    Telephone = txtRa_Tel.Text.Trim(),
                    Comment = txtRa_ProjectDetail.Text.Trim(),
                    ProductName = txtRa_InterestPro.Text.Trim(),
                    ProductDesc = txtRa_Project.Text.Trim(),
                    ContactType = string.Format("{0}-{1}", ServerTime2.StartDate, ServerTime2.EndDate)
                };
                if (request.save() == 0)
                {
                    eStore.BusinessModules.EmailECOPartner emailHelper = new BusinessModules.EmailECOPartner(eStore.Presentation.eStoreContext.Current.Store, partner);
                    var result = emailHelper.sendRequestAssistance(eStore.Presentation.eStoreContext.Current.User, request);
                    if (result.ErrCode == esUtilities.EMailReponse.ErrorCode.NoError)
                        alertMessage("Email send success!");
                    else
                        alertMessage(result.ErrCode.ToString());
                }
                else
                    alertMessage("Save information fail!");
            }
        }

        protected string setLogoUrl(object imgUrl)
        {
            if (imgUrl == null)
                return "images/photounavailable.gif";
            else
            {
                var img = imgUrl.ToString();
                if (img.ToLower().StartsWith("http://"))
                    return imgUrl.ToString();
                else
                    return string.Format("{0}resource/{1}", esUtilities.CommonHelper.GetStoreLocation(), imgUrl.ToString());
            }
        }

        protected string setUrl(object linkUrl)
        {
            if (linkUrl == null)
                return "#";
            else
            {
                var link = linkUrl.ToString();
                if (link.ToLower().StartsWith("http"))
                    return link;
                else
                    return "http://" + link;
            }
        }
       
    }
}