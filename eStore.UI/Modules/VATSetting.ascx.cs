using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace eStore.UI.Modules
{
    public partial class VATSetting : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {

        public eStore.POCOS.Contact.VATValidResult VATValidStatus
        {
            get
            {
                eStore.POCOS.Contact.VATValidResult  status =POCOS.Contact.VATValidResult.UNKNOW;
                Enum.TryParse(this.hVATValidStatus.Value, out status);
                return status;
            }
            set
            {
                this.hVATValidStatus.Value = ((int)value).ToString();
            }
        }
        public string VATNumber
        {
            get
            {
                if (!string.IsNullOrEmpty(this.txtCode.Text)
                    && this.txtCode.Text != this.txtCode.ToolTip
                    && !string.IsNullOrEmpty(this.txtVATNumber.Text)
                    && this.txtVATNumber.Text != this.txtVATNumber.ToolTip)
                    return this.txtCode.Text + this.txtVATNumber.Text;
                else
                    return string.Empty;
            }
            set
            {

                try
                {
                    Regex regexObj = new Regex(@"(\w{2})(\d+)");
                    Match matchResult = regexObj.Match(value);
                    if (matchResult.Success)
                    {
                        txtCode.Text = matchResult.Groups[1].Value;
                        txtVATNumber.Text = matchResult.Groups[2].Value;

                    }
                }
                catch (ArgumentException)
                {
                    txtCode.Text = string.Empty;
                    txtVATNumber.Text = string.Empty;
                }
            }
        }

        /// <summary>
        /// for eu store , Registration Number is not empty
        /// </summary>
        public string RegistrationNumber
        {
            get
            {
                return tbRegistNumber.Text.Trim();
            }
            set
            {
                tbRegistNumber.Text = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.BindScript("url", "jsvat", "jsvat.js");
            txtVATNumber.TabIndex = (short)((int)txtCode.TabIndex + 1);
        }
    }
}