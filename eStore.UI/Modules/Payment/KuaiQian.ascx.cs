using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace eStore.UI.Modules.Payment
{
    public partial class KuaiQian : Presentation.eStoreBaseControls.eStoreBaseUserControl, Presentation.Payment.IPaymentMethodModule
    {

        protected override void OnPreRender(EventArgs e)
        {

        }

        public POCOS.Payment GetPaymentInfo()
        {
            POCOS.Payment paymentInfo = new POCOS.Payment();
            return paymentInfo;
        }

        public bool ValidateForm()
        {
            return true;
        }

        public string ClientValidataionFunction()
        {
            return string.Empty;
        }

        public bool PreLoad()
        {
            return true;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
                bindImage();

        }

        protected void bindImage()
        {
            bank_abc.ImageUrl = ResolveUrl("~/images/ACN/banklogo/bank_abc.gif");
            bank_abc.Attributes.Add("bankid", "ABC");

            bank_bcom.ImageUrl = ResolveUrl("~/images/ACN/banklogo/bank_bcom.gif");
            bank_bcom.Attributes.Add("bankid", "BCOM");

            bank_bea.ImageUrl = ResolveUrl("~/images/ACN/banklogo/bank_bea.gif");
            bank_bea.Attributes.Add("bankid", "BEA");

            bank_bjrcb.ImageUrl = ResolveUrl("~/images/ACN/banklogo/bank_bjrcb.gif");
            bank_bjrcb.Attributes.Add("bankid", "BJRCB");

            bank_bob.ImageUrl = ResolveUrl("~/images/ACN/banklogo/bank_bob.gif");
            bank_bob.Attributes.Add("bankid", "BOB");

            bank_boc.ImageUrl = ResolveUrl("~/images/ACN/banklogo/bank_boc.gif");
            bank_boc.Attributes.Add("bankid", "BOC");

            bank_cbhb.ImageUrl = ResolveUrl("~/images/ACN/banklogo/bank_cbhb.gif");
            bank_cbhb.Attributes.Add("bankid", "CBHB");

            bank_ccb.ImageUrl = ResolveUrl("~/images/ACN/banklogo/bank_ccb.gif");
            bank_ccb.Attributes.Add("bankid", "CCB");
            bank_ccb.CssClass = "bankselectImg";

            bank_ceb.ImageUrl = ResolveUrl("~/images/ACN/banklogo/bank_ceb.gif");
            bank_ceb.Attributes.Add("bankid", "CEB");

            bank_cib.ImageUrl = ResolveUrl("~/images/ACN/banklogo/bank_cib.gif");
            bank_cib.Attributes.Add("bankid", "CIB");

            bank_citic.ImageUrl = ResolveUrl("~/images/ACN/banklogo/bank_citic.gif");
            bank_citic.Attributes.Add("bankid", "CITIC");

            bank_cmb.ImageUrl = ResolveUrl("~/images/ACN/banklogo/bank_cmb.gif");
            bank_cmb.Attributes.Add("bankid", "CMB");

            bank_cmbc.ImageUrl = ResolveUrl("~/images/ACN/banklogo/bank_cmbc.gif");
            bank_cmbc.Attributes.Add("bankid", "CMBC");

            bank_czb.ImageUrl = ResolveUrl("~/images/ACN/banklogo/bank_czb.gif");
            bank_czb.Attributes.Add("bankid", "CZB");

            bank_gdb.ImageUrl = ResolveUrl("~/images/ACN/banklogo/bank_gdb.gif");
            bank_gdb.Attributes.Add("bankid", "GDB");

            bank_gzcb.ImageUrl = ResolveUrl("~/images/ACN/banklogo/bank_gzcb.gif");
            bank_gzcb.Attributes.Add("bankid", "GZCB");

            bank_hsb.ImageUrl = ResolveUrl("~/images/ACN/banklogo/bank_hsb.gif");
            bank_hsb.Attributes.Add("bankid", "HSB");

            bank_hxb.ImageUrl = ResolveUrl("~/images/ACN/banklogo/bank_hxb.gif");
            bank_hxb.Attributes.Add("bankid", "HXB");

            bank_hzb.ImageUrl = ResolveUrl("~/images/ACN/banklogo/bank_hzb.gif");
            bank_hzb.Attributes.Add("bankid", "HZB");

            bank_icbc.ImageUrl = ResolveUrl("~/images/ACN/banklogo/bank_icbc.gif");
            bank_icbc.Attributes.Add("bankid", "ICBC");

            bank_nbcb.ImageUrl = ResolveUrl("~/images/ACN/banklogo/bank_nbcb.gif");
            bank_nbcb.Attributes.Add("bankid", "NBCB");

            bank_njcb.ImageUrl = ResolveUrl("~/images/ACN/banklogo/bank_njcb.gif");
            bank_njcb.Attributes.Add("bankid", "NJCB");

            bank_pab.ImageUrl = ResolveUrl("~/images/ACN/banklogo/bank_pab.gif");
            bank_pab.Attributes.Add("bankid", "PAB");

            bank_post.ImageUrl = ResolveUrl("~/images/ACN/banklogo/bank_post.gif");
            bank_post.Attributes.Add("bankid", "POST");

            bank_sdb.ImageUrl = ResolveUrl("~/images/ACN/banklogo/bank_sdb.gif");
            bank_sdb.Attributes.Add("bankid", "SDB");

            bank_shb.ImageUrl = ResolveUrl("~/images/ACN/banklogo/bank_shb.gif");
            bank_shb.Attributes.Add("bankid", "SHB");

            bank_shrcc.ImageUrl = ResolveUrl("~/images/ACN/banklogo/bank_shrcc.gif");
            bank_shrcc.Attributes.Add("bankid", "SHRCC");

            bank_spdb.ImageUrl = ResolveUrl("~/images/ACN/banklogo/bank_spdb.gif");
            bank_spdb.Attributes.Add("bankid", "SPDB");
        }

    }
}