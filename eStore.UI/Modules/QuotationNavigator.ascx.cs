using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class QuotationNavigator : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public string NavigatorTitle;

        private readonly string inactivelink = "<li>{0}</li>";
        private readonly string activelink = "<li class='on'>{0}<span></span></li>";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                initStatus();
            }
        }

        private void initStatus()
        {
            ltNavigatorTitle.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Confirm_Quotation);

            ltQuotation.Text = string.Format(inactivelink, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Quotation));
            ltAddress.Text = string.Format(inactivelink, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Contacts));
            ltConfirm.Text = string.Format(inactivelink, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Confirm));

            switch (this.QuotationProgressStep.ToLowerInvariant())
            {
                case "quotation":
                    {
                        ltQuotation.Text = string.Format(activelink, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Quotation));
                        ltnavigator.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Quotation);
                    }
                    break;
                case "address":
                    {
                        ltQuotation.Text = string.Format(activelink, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Quotation));
                        ltAddress.Text = string.Format(activelink, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Contacts));
                        ltnavigator.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Contacts);
                    }
                    break;


                case "confirm":
                    {
                        ltQuotation.Text = string.Format(activelink, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Quotation));
                        ltAddress.Text = string.Format(activelink, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Contacts));
                        ltConfirm.Text = string.Format(activelink, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Confirm));
                        ltnavigator.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Confirm);
                    }
                    break;
                default:
                    {
                        ltQuotation.Text = string.Format(activelink, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Quotation));
                        ltnavigator.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Quotation);
                    }
                    break;
            }

           
        }
        public string QuotationProgressStep
        {
            get
            {
                object obj2 = this.ViewState["QuotationProgressStep"];
                if (obj2 != null)
                    return (string)obj2;
                else
                    return string.Empty;
            }
            set
            {
                this.ViewState["QuotationProgressStep"] = value;
            }
        }
    }
}