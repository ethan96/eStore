using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Utilities;
using eStore.BusinessModules.SSO.Advantech;

namespace eStore.UI
{
    public partial class RegisterForm : Presentation.eStoreBaseControls.eStoreBasePage
    {
        /// <summary>
        /// interest object mapping simple to member db
        /// 中文进simple from ,英文进membership
        /// </summary>
        public static List<InterestObj> lsInterest = new List<InterestObj>() 
        {
            new InterestObj(){ Text="智能POS结帐终端",Value="Point of Service Terminals"},
            new InterestObj(){ Text="互动多媒体",Value="Interactive Digital Signage, Digital Signage Players"},
            new InterestObj(){ Text="智能影像分析解决方案",Value="Intelligent Video Platforms"},
            new InterestObj(){ Text="顾客自助终端",Value="Interactive Digital Signage"},
            new InterestObj(){ Text="多功能触控一体机",Value="Interactive Digital Signage"},
            new InterestObj(){ Text="智能云端管理平台",Value="Interactive Digital Signage", DisplayName="智能云端管理平台（如: 优店联网UShop+）"},
            new InterestObj(){ Text="物流终端解决方案",Value="Industrial Mobile Computers, Industrial Portable Computers"},
            new InterestObj(){ Text="其他",Value="Interactive Digital Signage"}
        };


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pnStep1.Visible = true;
                pnStep2.Visible = pnStep3.Visible = false;
            }
            ltMessage.Text = "";
        }

        protected void btnStep1Next_Click(object sender, EventArgs e)
        {
            //pnStep1.Visible = false;
            //pnStep2.Visible = true;
            //return;

            string userEmail = esUtilities.StringUtility.striphtml(tbUserEmail.Text);
            string password = tbUserPassWord.Text;
            try
            {
                SSOUSER user = new SSOUSER()
                {
                    email_addr = userEmail,
                    login_password = password,
                    primary_org_id = eStore.Presentation.eStoreContext.Current.getStringSetting("SAPSales_Org"),
                    source = "ESTORE_CN_USHOP"
                };

                pnStep1.Visible = false;
                pnStep2.Visible = true;
                Session["RegisterUserTemp"] = user;
                bindBaseInfor();
            }
            catch (Exception ex)
            {
                ltMessage.Text = "发生异常注册失败,请联系我们!";
            }
        }

        protected void btnSetp2_Click(object sender, EventArgs e)
        {
            //pnStep2.Visible = false;
            //pnStep3.Visible = true;
            //return;            
            if (Session["RegisterUserTemp"] == null)
            {
                ltMessage.Text = "超时，信息丢失!";
                return;
            }

            SSOUSER user = Session["RegisterUserTemp"] as SSOUSER;

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
                //user.business_application_area = ddlBaa.SelectedValue;
                user.source = "ESTORE_CN_USHOP";
                user.primary_org_id = eStore.Presentation.eStoreContext.Current.getStringSetting("SAPSales_Org");

                Session["RegisterUserTemp"] = user;
            }
            try
            {
                pnStep2.Visible = false;
                pnStep3.Visible = true;
            }
            catch (Exception ex)
            {
                ltMessage.Text = "发生异常注册失败,请联系我们!";
            }
        }



        protected void tbStep3_Click(object sender, EventArgs e)
        {
            //pnSuccess.Visible = true;
            //tbStep3.Visible = false;
            //this.ClientScript.RegisterStartupScript(this.GetType(), "fbShowSuccessDialog", "$(document).ready(function(){$('#linkRegisterSuccess').click()});", true);
            //return;

            if (Session["RegisterUserTemp"] == null)
            {
                ltMessage.Text = "超时，信息丢失!";
                return;
            }

            string xingzhi = ddlCompanyProperty.SelectedValue;
            string interest = (Request.Form["cblInterest"] ?? "").Replace("其他", "其他:" + esUtilities.StringUtility.striphtml(tbInterest.Text));
            string caigou = ddlCaigou.SelectedValue;
            string zixunfuwu = (Request.Form["cklzixunfuwu"] ?? "").Replace("其他", "其他:" + esUtilities.StringUtility.striphtml(tbzixunfuwu.Text));
            string CompanyYt = (Request.Form["CompanyYt"] ?? "").Replace("其他", "其他:" + esUtilities.StringUtility.striphtml(tbCompanyYt.Text));
            string liansuo = ddlLianSuo.SelectedValue;
            string simpleid = "186d08f3-1f87-4b21-9e7e-efdadd73dcb7";
            try
            {
                SSOUSER userTemp = Session["RegisterUserTemp"] as SSOUSER;
                MEMBER_SIMPLE record = new MEMBER_SIMPLE();
                record.SimpleFromID = Guid.Parse(simpleid);
                record.FirstName = userTemp.first_name; //required.
                record.LastName = userTemp.last_name; //required.
                record.Email = userTemp.email_addr; //required.
                record.CompanyName = userTemp.company_name; //required.
                record.Country = userTemp.country; //required.

                List<MemberSimpleMultiQuestionObject> answers = new List<MemberSimpleMultiQuestionObject>();

                answers.Add(new MemberSimpleMultiQuestionObject() { question = "公司性质", answer = xingzhi });
                answers.Add(new MemberSimpleMultiQuestionObject() { question = "您感兴趣的智能零售产品与解决方案？（可复选）", answer = interest });
                answers.Add(new MemberSimpleMultiQuestionObject() { question = "近期有采购智能零售相关解决产品的需求吗？", answer = caigou });
                answers.Add(new MemberSimpleMultiQuestionObject() { question = "您希望获得的资讯与服务？（可复选）", answer = zixunfuwu });
                answers.Add(new MemberSimpleMultiQuestionObject() { question = "若您为零售业者，您所属公司业态：", answer = CompanyYt });
                answers.Add(new MemberSimpleMultiQuestionObject() { question = "呈上题，您的连锁店数：", answer = liansuo });


                var inls = interest.Split(',');
                List<string> ls = new List<string>();
                foreach (var item in inls)
                {
                    InterestObj obj;
                    if (item.StartsWith("其他"))
                        obj = lsInterest.FirstOrDefault(c => c.Text == "其他");
                    else
                        obj = lsInterest.FirstOrDefault(c => c.Text == item);
                    if (obj != null)
                    {
                        if (!ls.Contains(obj.Value))
                            ls.Add(obj.Value);
                    }
                }
                userTemp.in_product_values = string.Join(",", ls);
                var regiestResult = eStore.Presentation.eStoreContext.Current.Store.registeredUsertoSSO(userTemp, "ESTORE_CN_USHOP"); //storeid will use Store Parameter
                if (regiestResult)
                {
                    if (Presentation.eStoreContext.Current.Store.sso.AddSimpleFormRecord(record, answers.ToArray()))
                    {
                        pnSuccess.Visible = true;
                        tbStep3.Visible = false;
                        this.ClientScript.RegisterStartupScript(this.GetType(), "fbShowSuccessDialog", "$(document).ready(function(){$('#linkRegisterSuccess').click()});", true);
                        // try login
                        String authKey = Presentation.eStoreContext.Current.Store.SSOAuthenticate(userTemp.email_addr, userTemp.login_password, Presentation.eStoreContext.Current.getUserIP());
                        var cc = Presentation.eStoreUserAccount.TrySSOSignIn(Presentation.eStoreContext.Current.getUserIP(), authKey, userTemp.email_addr);
                    }
                    else
                        ltMessage.Text = "发生异常注册失败,请联系我们!";
                }
                else
                {
                    ltMessage.Text = "发生异常注册失败,请联系我们!";
                }
            }
            catch (Exception ex)
            {
                ltMessage.Text = "发生异常注册失败,请联系我们!";
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

            //ddlBaa.Items.Clear();
            //ddlBaa.Items.Add(new ListItem() { Text = "-- 请选择 --", Value = "" });
            //string baaJsonStr = Presentation.eStoreContext.Current.Store.sso.GetBAAListInJson(Presentation.eStoreContext.Current.Store.profile.StoreLangID);
            //var baals = esUtilities.JsonHelper.JsonDeserialize<List<MemberBaa>>(baaJsonStr);
            //foreach (var item in baals)
            //    ddlBaa.Items.Add(new ListItem() { Text = item.displayName, Value = item.value });

        }

        /// <summary>
        /// interest objec from simple form
        /// </summary>
        public class InterestObj
        {
            private string _text;

            public string Text
            {
                get { return _text; }
                set { _text = value; }
            }

            private string _value;

            public string Value
            {
                get { return _value; }
                set { _value = value; }
            }

            private string _displayName;

            public string DisplayName
            {
                get
                {
                    if (string.IsNullOrEmpty(_displayName))
                        _displayName = Text;
                    return _displayName;
                }
                set { _displayName = value; }
            }


        }
    }

    /// <summary>
    /// country object from membership
    /// </summary>
    public class MemberCountry
    {
        private string _country;

        public string country
        {
            get { return _country; }
            set { _country = value; }
        }

        private string _phoneCode;

        public string phoneCode
        {
            get { return _phoneCode; }
            set { _phoneCode = value; }
        }

        private string _countryCode;

        public string countryCode
        {
            get { return _countryCode; }
            set { _countryCode = value; }
        }


    }

    /// <summary>
    /// baa from membership
    /// </summary>
    public class MemberBaa
    {
        private string _displayName;

        public string displayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }


        private string _value;

        public string value
        {
            get { return _value; }
            set { _value = value; }
        }

    }
}