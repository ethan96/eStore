using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class CultureFullName : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        private bool? _eastCulture;
        public string currentCss = "title";
        bool EastCulture
        {
            get
            {
                if (_eastCulture == null)
                {
                    switch (Presentation.eStoreContext.Current.Store.storeID)
                    {
                        case "ACN":
                        case "ATW":
                        case "AMY":
                        case "AKR":
                        case "AJP":
                            _eastCulture = true;
                            break;
                        default:
                            _eastCulture = false;
                            break;
                    }
                }

                return _eastCulture ?? false;

            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            bindFonts();
        }
        public string FirstName
        {

            get
            {

                if (EastCulture)
                    return txtright.Text;
                else
                    return txtlfet.Text;
            }

            set
            {
                if (EastCulture)
                    txtright.Text = value;
                else
                    txtlfet.Text = value;
            }
        }

        public string LastName
        {
            get
            {

                if (EastCulture)
                    return txtlfet.Text;
                else
                    return txtright.Text;
            }

            set
            {
                if (EastCulture)
                    txtlfet.Text = value;
                else
                    txtright.Text = value;
            }
        }

        protected void bindFonts()
        {
            if (EastCulture)
            {
                lright.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_First_Name);
                txtright.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_First_Name);
                lleft.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Last_Name);
                txtlfet.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Last_Name);

            }
            else
            {
                lleft.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_First_Name);
                txtlfet.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_First_Name);
                lright.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Last_Name);
                txtright.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Last_Name);
            }
        }
    }
}