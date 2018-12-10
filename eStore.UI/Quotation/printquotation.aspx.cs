using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Quotation
{
    public partial class printquotation : Presentation.eStoreBaseControls.eStoreBasePagePrint
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Presentation.eStoreContext.Current.Quotation == null)
            {
                Presentation.eStoreContext.Current.AddStoreErrorCode("Timeout", null, true);
                return;
            }
            if (Presentation.eStoreContext.Current.Quotation.statusX != POCOS.Quotation.QStatus.Confirmed
                    && Presentation.eStoreContext.Current.Quotation.statusX != POCOS.Quotation.QStatus.ConfirmedbutNeedTaxIDReview
                    && Presentation.eStoreContext.Current.Quotation.statusX != POCOS.Quotation.QStatus.ConfirmedbutNeedFreightReview)
            {
                Presentation.eStoreContext.Current.AddStoreErrorCode(Presentation.eStoreContext.Current.Quotation.statusX.ToString() + " quotation can't be printed.", null, true);
                return;
            }
            if (!IsPostBack)
            {
                if(Presentation.eStoreContext.Current.Quotation!=null)
                
                this.QuotationContentPreview1.quotation = Presentation.eStoreContext.Current.Quotation;
                this.lQuotationNumber.Text = Presentation.eStoreContext.Current.Quotation.QuotationNumber;
                this.lQuoteDate.Text = Presentation.eStoreLocalization.DateTime((DateTime)Presentation.eStoreContext.Current.Quotation.QuoteDate);
                this.lQuoteExpiredDate.Text = Presentation.eStoreLocalization.DateTime((DateTime)Presentation.eStoreContext.Current.Quotation.QuoteExpiredDate);
            }
        }
    }
}