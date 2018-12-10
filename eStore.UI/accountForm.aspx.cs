using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.BusinessModules.SSO.Advantech;

namespace eStore.UI
{
    public partial class accountForm : Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //重置密码
            if (Request.QueryString["email"] != null && Request.QueryString["tempid"] != null)
            {
                pnaccount.Enabled = false;
                string email = Request.QueryString["email"];
                string tempid = Request.QueryString["tempid"];
                var result = eStore.Presentation.eStoreContext.Current.Store.sso.ValidateForgetPasswordID(email, tempid);
                if (result)
                {
                    if (!IsPostBack)
                    {
                        this.ClientScript.RegisterStartupScript(this.GetType(), "showResetpsw", "$(document).ready(function(){$('#editpsd').click()});", true);
                        troldpws.Visible = false;
                    }
                }
                else
                    Response.Redirect("/");
            }
            else
            {
                if (eStore.Presentation.eStoreContext.Current.User == null)
                    Response.Redirect("/");
                if (!IsPostBack)
                    bindBaseInfor();
            }
        }


        protected void bindBaseInfor()
        {
            string countryJsonStr = Presentation.eStoreContext.Current.Store.sso.GetCountryListInJson();
            var countryls = esUtilities.JsonHelper.JsonDeserialize<List<MemberCountry>>(countryJsonStr);
            ddlCountries.Items.Clear();
            foreach (var item in countryls)
            {
                var c = new ListItem() { Text = item.country, Value = item.country };
                if (c.Text.Equals("China", StringComparison.OrdinalIgnoreCase))
                    c.Selected = true;
                c.Attributes.Add("phonecode", item.phoneCode);
                ddlCountries.Items.Add(c);
            }

            SSOUSER user = eStore.Presentation.eStoreContext.Current.Store.SSOGetUserProfile(eStore.Presentation.eStoreContext.Current.User.UserID);
            if (user != null)
            {
                ltEmail.Text = user.email_addr;
                lbNa.Text = tbUserNa.Text = user.first_name;
                lbMe.Text = tbUserMe.Text = user.last_name;
                lbBumen.Text = user.department;
                var selectitem = ddlBumen.Items.FindByValue(user.department);
                if (selectitem != null)
                {
                    ddlBumen.ClearSelection();
                    selectitem.Selected = true;
                }
                hfareacode.Value = lbTelArea.Text = spTelArea.Text = user.tel_areacode;
                lbTel.Text = tbTel.Text = user.tel_no;
                lbExt.Text = tbExt.Text = user.tel_ext;
                lbmobile.Text = tbmobile.Text = user.mobile;
                lbcompany.Text = tbcompany.Text = user.company_name;
                lbcountry.Text = user.country;
                hfstate.Value = user.state;
                selectitem = ddlCountries.Items.FindByValue(user.country);
                if (selectitem != null)
                {
                    ddlCountries.ClearSelection();
                    selectitem.Selected = true;
                    if (!string.IsNullOrEmpty(user.state))
                    {
                        var statestr = Presentation.eStoreContext.Current.Store.sso.GetStateListByCountryInJson(user.country);
                        if (!string.IsNullOrEmpty(statestr))
                        {
                            var statels = esUtilities.JsonHelper.JsonDeserialize<List<MState>>(statestr);
                            if (statels != null && statels.Any())
                            {
                                var selectState = statels.FirstOrDefault(c => c.abbr == user.state);
                                if (selectState != null)
                                    lbstate.Text = selectState.state;
                            }
                        }
                    }
                }

                lbCity.Text = user.city;
                selectitem = ddlStates.Items.FindByValue(user.state);
                if (selectitem != null)
                {
                    ddlStates.ClearSelection();
                    selectitem.Selected = true;
                }
                tbcity.Text = user.city;
                lbaddress.Text = tbaddress.Text = user.address;

                MEMBER_SIMPLE record = eStore.Presentation.eStoreContext.Current.Store.sso.getSimpleFormLogByEmail(user.email_addr, "186d08f3-1f87-4b21-9e7e-efdadd73dcb7");
                if (record != null)
                {
                    string answerStr = record.Answer;
                    var answers = esUtilities.JsonHelper.JsonDeserialize<List<SimpleQuo>>(answerStr) ?? new List<SimpleQuo>();
                    foreach (var item in answers)
                    {
                        switch (item.question)
                        {
                            case "公司性质":
                                lbcompanyproperty.Text = item.answer;
                                selectitem = ddlCompanyProperty.Items.FindByValue(item.answer);
                                if (selectitem != null)
                                {
                                    ddlCompanyProperty.ClearSelection();
                                    selectitem.Selected = true;
                                }
                                break;
                            case "您感兴趣的智能零售产品与解决方案？（可复选）":
                                lbinterest.Text = item.answer;
                                break;
                            case "近期有采购智能零售相关解决产品的需求吗？":
                                lbcaigou.Text = item.answer;
                                selectitem = ddlCaigou.Items.FindByValue(item.answer);
                                if (selectitem != null)
                                {
                                    ddlCaigou.ClearSelection();
                                    selectitem.Selected = true;
                                }
                                break;
                            case "您希望获得的资讯与服务？（可复选）":
                                lbzixunfuwq.Text = item.answer;
                                break;
                            case "若您为零售业者，您所属公司业态：":
                                lbcompanyyt.Text = item.answer;
                                break;
                            case "呈上题，您的连锁店数：":
                                lbliansuo.Text = item.answer;
                                selectitem = ddlLianSuo.Items.FindByValue(item.answer);
                                if (selectitem != null)
                                {
                                    ddlLianSuo.ClearSelection();
                                    selectitem.Selected = true;
                                }
                                break;
                        }
                    }
                }
            }


        }

        protected void btUpdateUserInfor_Click(object sender, EventArgs e)
        {
            SSOUSER user = eStore.Presentation.eStoreContext.Current.Store.SSOGetUserProfile(eStore.Presentation.eStoreContext.Current.User.UserID);
            if (user != null)
            {
                user.first_name = esUtilities.StringUtility.striphtml(tbUserNa.Text);
                user.last_name = esUtilities.StringUtility.striphtml(tbUserMe.Text);
                user.department = ddlBumen.SelectedValue;
                user.tel_areacode = hfareacode.Value;
                user.tel_no = esUtilities.StringUtility.striphtml(tbTel.Text);
                user.tel_ext = esUtilities.StringUtility.striphtml(tbExt.Text);
                user.mobile = esUtilities.StringUtility.striphtml(tbmobile.Text);
                user.company_name = esUtilities.StringUtility.striphtml(tbcompany.Text);
                user.country = ddlCountries.SelectedValue;
                user.state = hfstate.Value;
                user.city = tbcity.Text;
                user.address = esUtilities.StringUtility.striphtml(tbaddress.Text);


                var inls = (Request.Form["cblInterest"] ?? "").Split(',');
                List<string> ls = new List<string>();
                foreach (var item in inls)
                {
                    eStore.UI.RegisterForm.InterestObj obj;
                    if (item.StartsWith("其他"))
                        obj = eStore.UI.RegisterForm.lsInterest.FirstOrDefault(c => c.Text == "其他");
                    else
                        obj = eStore.UI.RegisterForm.lsInterest.FirstOrDefault(c => c.Text == item);
                    if (obj != null)
                    {
                        if (!ls.Contains(obj.Value))
                            ls.Add(obj.Value);
                    }
                }
                user.in_product_values = string.Join(",", ls);

                var result = eStore.Presentation.eStoreContext.Current.Store.sso.updProfile(user, eStore.Presentation.eStoreContext.Current.Store.profile.StoreMembershippass);

                string xingzhi = ddlCompanyProperty.SelectedValue;
                string interest = (Request.Form["cblInterest"] ?? "").Replace("其他", "其他:" + esUtilities.StringUtility.striphtml(tbInterest.Text));
                string caigou = ddlCaigou.SelectedValue;
                string zixunfuwu = (Request.Form["cklzixunfuwu"] ?? "").Replace("其他", "其他:" + esUtilities.StringUtility.striphtml(tbzixunfuwu.Text));
                string CompanyYt = (Request.Form["CompanyYt"] ?? "").Replace("其他", "其他:" + esUtilities.StringUtility.striphtml(tbCompanyYt.Text));
                string liansuo = ddlLianSuo.SelectedValue;
                try
                {
                    List<MemberSimpleMultiQuestionObject> answers = new List<MemberSimpleMultiQuestionObject>();

                    answers.Add(new MemberSimpleMultiQuestionObject() { question = "公司性质", answer = xingzhi });
                    answers.Add(new MemberSimpleMultiQuestionObject() { question = "您感兴趣的智能零售产品与解决方案？（可复选）", answer = interest });
                    answers.Add(new MemberSimpleMultiQuestionObject() { question = "近期有采购智能零售相关解决产品的需求吗？", answer = caigou });
                    answers.Add(new MemberSimpleMultiQuestionObject() { question = "您希望获得的资讯与服务？（可复选）", answer = zixunfuwu });
                    answers.Add(new MemberSimpleMultiQuestionObject() { question = "若您为零售业者，您所属公司业态：", answer = CompanyYt });
                    answers.Add(new MemberSimpleMultiQuestionObject() { question = "呈上题，您的连锁店数：", answer = liansuo });

                    bool updateSimple = true;

                    MEMBER_SIMPLE record = eStore.Presentation.eStoreContext.Current.Store.sso.getSimpleFormLogByEmail(user.email_addr, "186d08f3-1f87-4b21-9e7e-efdadd73dcb7");
                    if (record == null)
                    {
                        updateSimple = false;
                        record = new MEMBER_SIMPLE();
                        record.SimpleFromID = Guid.Parse("186d08f3-1f87-4b21-9e7e-efdadd73dcb7");
                        record.FirstName = user.first_name; //required.
                        record.LastName = user.last_name; //required.
                        record.Email = user.email_addr; //required.
                    }
                    record.CompanyName = user.company_name; //required.
                    record.Country = user.country; //required.
                    record.State = user.state;
                    bool resultSmple = true;
                    if (updateSimple)
                        resultSmple = Presentation.eStoreContext.Current.Store.sso.UpdateSimpleFormLog(record, answers.ToArray());
                    else
                        resultSmple = Presentation.eStoreContext.Current.Store.sso.AddSimpleFormRecord(record, answers.ToArray());

                    if (!resultSmple)
                        ltMessage.Text = "更新信息失败,请联系我们!";
                    else
                        bindBaseInfor();
                }
                catch (Exception ex)
                {
                    ltMessage.Text = "更新信息失败,请联系我们!";
                }
            }
        }

        protected void btupdatepsd_Click(object sender, EventArgs e)
        {
            string oldpsd = tbuserOldPSD.Text;
            string password = tbUserPassWord.Text;

            if (Request.QueryString["email"] != null && Request.QueryString["tempid"] != null)
            {
                string email = Request.QueryString["email"];
                string tempid = Request.QueryString["tempid"];

                var result = Presentation.eStoreContext.Current.Store.sso.UpdateUserPassword(email, tempid, "ESTORE_CN_USHOP", "", password);
                if (result)
                {
                    this.ClientScript.RegisterStartupScript(this.GetType(), "showcahgepswSuccess", "$(document).ready(function(){$('#ito_ushop_chagepswSuccess').click()});", true);
                }
            }
            else
            {
                String authKey = Presentation.eStoreContext.Current.Store.SSOAuthenticate(eStore.Presentation.eStoreContext.Current.User.UserID, oldpsd, eStore.Presentation.eStoreContext.Current.getUserIP());
                if (!string.IsNullOrEmpty(authKey) && !string.IsNullOrEmpty(oldpsd) && !string.IsNullOrEmpty(password))
                {
                    var result = Presentation.eStoreContext.Current.Store.sso.UpdateUserPassword(eStore.Presentation.eStoreContext.Current.User.UserID, "", "ESTORE_CN_USHOP", oldpsd, password);
                    if (result)
                        ltdialogmessage.Text = "密码修改成功。";
                    else
                        ltdialogmessage.Text = "密码修改失败，请重试。";
                    this.ClientScript.RegisterStartupScript(this.GetType(), "showdialog", "$(document).ready(function(){$('#pshopdialog').click()});", true);
                }
                else
                {
                    ltdialogmessage.Text = "旧密码不正确，请重试。";
                    this.ClientScript.RegisterStartupScript(this.GetType(), "showdialog", "$(document).ready(function(){$('#pshopdialog').click()});", true);
                }
            }
        }


    }

    /// <summary>
    /// simple form 问答
    /// </summary>
    public class SimpleQuo
    {
        private string _question;

        public string question
        {
            get { return _question; }
            set { _question = value; }
        }

        private string _answerOptions;

        public string answerOptions
        {
            get { return _answerOptions; }
            set { _answerOptions = value; }
        }

        private string _answerType;

        public string answerType
        {
            get { return _answerType; }
            set { _answerType = value; }
        }

        private string _answer;

        public string answer
        {
            get { return _answer; }
            set { _answer = value; }
        }

        private bool _answerIsMandatory;

        public bool answerIsMandatory
        {
            get { return _answerIsMandatory; }
            set { _answerIsMandatory = value; }
        }

    }

    /// <summary>
    /// membership state object
    /// </summary>
    public class MState
    {
        private string _state;

        public string state
        {
            get { return _state; }
            set { _state = value; }
        }

        private string _abbr;

        public string abbr
        {
            get { return _abbr.Trim(); }
            set { _abbr = value; }
        }


    }
}