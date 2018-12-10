using eStore.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI
{
    public partial class MyRegister : Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected string type;
        protected void Page_Load(object sender, EventArgs e)
        {
            type = Request["edit"] ?? "regist";
            if (!Page.IsPostBack)
            {                
                var logInfor = eStoreIoc.Resolve<Presentation.VModles.Member.LogInfor>();
                if (logInfor is eStore.Presentation.VModles.Member.AdvantechLogInfor)
                {
                    Response.Redirect(string.Format(logInfor.RegisterLink
                        , eStoreContext.Current.storeMembershippass
                        , eStoreContext.Current.Store.profile.StoreLangID
                        , eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_User_Login_Go_eStore)
                        , "Default.aspx"
                        , eStoreContext.Current.BusinessGroup
                        ));
                }

                if (Request["edit"] != null)
                {
                    if (eStoreContext.Current.User == null)
                    {
                        eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation("eStore_Please_login_first"));
                        return;
                    }
                    Presentation.VModles.Member.VRegiesterUser vuser = new Presentation.VModles.Member.VRegiesterUser(eStoreContext.Current.User);
                    BindBaseInfor(vuser);
                }
                else
                    BindBaseInfor();

                btsubmit.Text = eStore.Presentation.eStoreLocalization.Tanslation("eStore_Submit");
            }
        }

        protected void BindBaseInfor(Presentation.VModles.Member.VRegiesterUser vuser = null)
        {
            ddlCountry.Items.Clear();
            List<POCOS.Country> countries;
            string _CountryCode = "";
            if (eStoreContext.Current.User != null && eStoreContext.Current.User.actingUser.mainContact != null)
                _CountryCode = eStoreContext.Current.User.actingUser.mainContact.countryCodeX;
            if (string.IsNullOrEmpty(_CountryCode))
                _CountryCode = eStoreContext.Current.CurrentCountry.Shorts;

            if (eStoreContext.Current.User != null && eStoreContext.Current.User.actingRole == POCOS.User.Role.Employee)
            {
                eStore.BusinessModules.StoreSolution eSolution = eStore.BusinessModules.StoreSolution.getInstance();
                countries = eSolution.countries.OrderBy(c => c.CountryName).ToList();
            }
            else
            {
                countries = eStoreContext.Current.Store.profile.Countries.ToList<POCOS.Country>();
                string addtionalShipToCountry = eStore.Presentation.eStoreContext.Current.getStringSetting("Ship_To_Country");
                if (string.IsNullOrEmpty(addtionalShipToCountry) == false)
                {
                    if (countries == null)
                        countries = new List<POCOS.Country>();
                    eStore.BusinessModules.StoreSolution eSolution = eStore.BusinessModules.StoreSolution.getInstance();
                    string[] addtionalcountries = addtionalShipToCountry.Split(',');
                    countries.AddRange((from c in eSolution.countries
                                        where addtionalcountries.Contains(c.Shorts)
                                        select c).ToList());
                }
            }
            var isCheckVAT = Presentation.eStoreContext.Current.getBooleanSetting("EnableVATSetting");
            ddlCountry.Items.Add(new ListItem("[" + eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Select_Country) + "]", ""));
            POCOS.Country currentcountry = null;
            foreach (POCOS.Country country in countries.OrderBy(x => x.CountryName))
            {
                ListItem item = new ListItem();
                if (!String.IsNullOrEmpty(_CountryCode))
                {
                    if (_CountryCode == country.Shorts)
                    {
                        currentcountry = country;
                        item.Selected = true;
                    }                        
                }
                item.Text = country.CountryName;
                item.Value = country.Shorts;
                ddlCountry.Items.Add(item);
            }

            ddlState.Items.Clear();
            ddlState.Items.Add(new ListItem("[" + eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Select_Country) + "]", ""));
            if (currentcountry != null)
            {
                foreach (var state in currentcountry.CountryStates)
                {
                    ddlState.Items.Add(new ListItem { Text = state.StateName, Value = state.StateShorts });
                }
            }

            if (vuser != null)
            {
                tbFirst_Name.Text = vuser.FirstName;
                tbLast_Name.Text = vuser.LastName;
                tbEmail.Text = vuser.Email;
                tbCompany_Name.Text = vuser.CompanyName;
                tbJob_Title.Text = vuser.JobTitle;
                tbPhone_number.Text = vuser.Phonenumber;
                tbMobile_number.Text = vuser.Mobilenumber;
                tbAddress.Text = vuser.Address;
                tbCity.Text = vuser.City;
                tbZip.Text = vuser.Zip;
                
                var countryitem = ddlCountry.Items.FindByValue(vuser.Country);
                if (countryitem != null)
                {
                    ddlCountry.ClearSelection();
                    countryitem.Selected = true;
                }

                var functionitem = ddlJob_Function.Items.FindByValue(vuser.JobFunction);
                if (functionitem != null)
                {
                    ddlJob_Function.ClearSelection();
                    functionitem.Selected = true;
                }

                var stateitem = ddlState.Items.FindByValue(vuser.State);
                if (stateitem != null)
                {
                    ddlState.ClearSelection();
                    stateitem.Selected = true;
                }
                btsubmit.Text = "Update";
                btsubmit.CommandName = "update";
                tbEmail.Enabled = false;
            }
        }

        protected void btsubmit_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            Presentation.VModles.Member.VRegiesterUser member = new Presentation.VModles.Member.VRegiesterUser(eStoreContext.Current.User);

            member.FirstName = tbFirst_Name.Text.Trim();
            member.LastName = tbLast_Name.Text.Trim();
            member.Email = tbEmail.Text.Trim();
            member.CompanyName = tbCompany_Name.Text.Trim();
            member.Country = ddlCountry.SelectedItem.Text;
            member.JobFunction = ddlJob_Function.SelectedValue;
            member.JobTitle = tbJob_Title.Text.Trim();
            member.Phonenumber = tbPhone_number.Text.Trim();
            member.Mobilenumber = tbMobile_number.Text.Trim();
            member.Address = tbAddress.Text.Trim();
            member.City = tbCity.Text.Trim();
            member.State = string.IsNullOrEmpty(ddlState.SelectedValue) ? Request.Form["ctl00$eStoreMainContent$ddlState"] : ddlState.SelectedValue;
            member.Zip = tbZip.Text.Trim();
            member.CountryCode = ddlCountry.SelectedValue;

            if (btn.CommandName == "update")
            {
                if(!string.IsNullOrEmpty(tbPassword.Text.Trim()))
                    member.Password = tbPassword.Text.Trim();
                if (eStoreUserAccount.UpdateUser(member, eStoreContext.Current.User))
                {
                    eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation("eStore_Update_success"));
                }
                else
                    eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation("eStore_Update_fail"));
            }
            else
            {
                member.Password = tbPassword.Text.Trim();
                if (eStoreUserAccount.RegisterUser(member))
                {
                    //eStoreContext.Current.AddStoreErrorCode("Regiester success");
                    Presentation.VModles.Member.VLoginDialog LogInfor = new Presentation.VModles.Member.VLoginDialog(member.Email, member.Password, eStoreContext.Current.getUserIP());
                    if (Presentation.eStoreUserAccount.TrySignIn(LogInfor)) // 尝试登录
                    {
                        Response.Redirect("MyAccount.aspx");
                    }
                }
                else
                {
                    eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation("eStore_Regiester_fail"));
                }
            }

            
        }
    }
}