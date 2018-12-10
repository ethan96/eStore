using eStore.POCOS.PocoX;
using eStore.Presentation;
using eStore.Utilities;
using esUtilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.SubscribeUs
{
    public partial class SubscribeUs :  Presentation.eStoreBaseControls.eStoreBasePage
    {

        private string _testing = ConfigurationManager.AppSettings.Get("TestingMode");
        private string testingOrderDeptEmail = ConfigurationManager.AppSettings.Get("eStoreItEmailGroup");

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var BAAList = Presentation.eStoreContext.Current.Store.getAllStandardSiebelContactBAA();
                foreach (var baa in BAAList)
                {
                    ListItem li1 = new ListItem(baa.TEXT, baa.VALUE, true);
                    BAACbl.Items.Add(li1);
                }

                var countries = Presentation.eStoreContext.Current.Store.getAllCountries().Select(c => c.CountryName);
                var defaultcountry = Presentation.eStoreContext.Current.Store.profile.DefaultCountry;
                CountryDrp.DataSource = countries;
                CountryDrp.DataBind();
                var currentUser = Presentation.eStoreContext.Current.User;
                if (currentUser != null)
                {
                    FirstNameTexBox.Text = currentUser.FirstName;
                    LastNameTexBox.Text = currentUser.LastName;
                    EmailTextBox.Text = currentUser.UserID;
                    CompanyNameTextBox.Text = currentUser.CompanyName;
                    JobTitleTextBox.Text = currentUser.JobTitle;
                    CountryDrp.SelectedValue = currentUser.Country;
                }
                else
                    CountryDrp.SelectedValue = defaultcountry;
            }

        }


        protected void submitSubscribe_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    BusinessModules.tw.com.advantech.unica.ExtApi unicaExtAPI = new BusinessModules.tw.com.advantech.unica.ExtApi();

                    var objImportedActivity = new BusinessModules.tw.com.advantech.unica.ObjImportedActivity();

                    objImportedActivity.Country = CountryDrp.SelectedValue;
                    objImportedActivity.FirstName = FirstNameTexBox.Text;
                    objImportedActivity.LastName = LastNameTexBox.Text;
                    List<String> BAAStrList = new List<string>();
                    foreach (ListItem item in BAACbl.Items)
                    {
                        if (item.Selected)
                            BAAStrList.Add(item.Value);
                    }
                    objImportedActivity.BAA = BAAStrList.ToArray();
                    objImportedActivity.Email = EmailTextBox.Text;
                    objImportedActivity.JobTitle = JobTitleTextBox.Text;
                    objImportedActivity.AccountName = CompanyNameTextBox.Text;
                    objImportedActivity.Comments = "Consent to receive electronic messages from Advantech:" + rblReceiveMessage.SelectedValue;

                    var result = unicaExtAPI.ImportEStoreActivity(objImportedActivity);


                    if (result.ReturnValue == true)
                    {
                        string mailSubject = "Thank you for Subscribing to Advantech";


                        eStore.BusinessModules.EMailNoticeTemplate mailTemplate = new BusinessModules.EMailNoticeTemplate(Presentation.eStoreContext.Current.Store);

                        var htmlStringTable = GetMyTable(BAAStrList, x => x);
                        var subscribeDetail = "<b>Contact Email:</b> '" + EmailTextBox.Text + "'<br><br>" + "<b>BAA List:</b>" + htmlStringTable;

                        EMailReponse response = mailTemplate.SendSubscribeUsEmail(LastNameTexBox.Text, EmailTextBox.Text, mailSubject, subscribeDetail,Presentation.eStoreContext.Current.Store, eStore.Presentation.eStoreContext.Current.CurrentLanguage);


                        if (response != null && response.ErrCode != EMailReponse.ErrorCode.NoError)
                        {
                            Utilities.eStoreLoger.Error(string.Format("Sent mail for User Subscribe failed. {0}",  response.ErrCode.ToString())
                                , EmailTextBox.Text, Presentation.eStoreContext.Current.getUserIP(), Presentation.eStoreContext.Current.Store.storeID, null);
                        }

                        POCOS.UserRequest requestSubscribeUs = new POCOS.UserRequest(Presentation.eStoreContext.Current.Store.profile, POCOS.UserRequest.ReqType.Subscription);
                        requestSubscribeUs.Email = EmailTextBox.Text;
                        requestSubscribeUs.Country = CountryDrp.SelectedValue;
                        requestSubscribeUs.FirstName = FirstNameTexBox.Text;
                        requestSubscribeUs.LastName = LastNameTexBox.Text;
                        requestSubscribeUs.Comment = string.Join("|", BAAStrList.ToArray()); ;
                        requestSubscribeUs.save();
                    }
                    else
                    {
                        eStoreLoger.Error(string.Format("Import eStore Activity error: {0}", result.ErrorMessage), "", Presentation.eStoreContext.Current.getUserIP(), Presentation.eStoreContext.Current.Store.storeID, null);
                    }
                }
                catch (Exception ex)
                {
                    eStoreLoger.Error("Import eStore Activity error", "", "", Presentation.eStoreContext.Current.Store.storeID, ex);
                }

                Response.Redirect("~/SubscribeUs/SubscribeThankyou.aspx");
            }

        }

        public static string GetMyTable<T>(IEnumerable<T> list, params Func<T, object>[] fxns)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("<TABLE>\n");
            foreach (var item in list)
            {
                sb.Append("<TR>\n");
                foreach (var fxn in fxns)
                {
                    sb.Append("<TD>");
                    sb.Append(fxn(item));
                    sb.Append("</TD>");
                }
                sb.Append("</TR>\n");
            }
            sb.Append("</TABLE>");

            return sb.ToString();
        }


    }
}