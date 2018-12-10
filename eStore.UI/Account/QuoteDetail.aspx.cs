using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Account
{
    public partial class QuoteDetail : Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request["ReviewQuotation"]))
            {
                POCOS.Quotation currentdQuotation = (from Quotation in Presentation.eStoreContext.Current.User.actingUser.quotations
                                                     where Quotation.QuotationNumber == Request["ReviewQuotation"].ToString()
                                                     select Quotation).FirstOrDefault();

                if (currentdQuotation != null)
                {
                    Presentation.eStoreContext.Current.Quotation = currentdQuotation;
                    switch (Presentation.eStoreContext.Current.Quotation.statusX)
                    {
                        case POCOS.Quotation.QStatus.Open:
                        case POCOS.Quotation.QStatus.NeedTaxIDReview:
                        case POCOS.Quotation.QStatus.NeedFreightReview:
                        case POCOS.Quotation.QStatus.Unfinished:
                            Response.Redirect("~/Quotation/quote.aspx");
                            break;
                        case POCOS.Quotation.QStatus.Confirmed:
                        case POCOS.Quotation.QStatus.ConfirmedbutNeedTaxIDReview:
                        case POCOS.Quotation.QStatus.ConfirmedbutNeedFreightReview:
                        case POCOS.Quotation.QStatus.Expired:

                            Response.Redirect("~/Quotation/confirm.aspx");
                            break;
                        default:
                            //add error msg
                            break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(Request["QuotationRevise"]))
            {
                POCOS.Quotation currentdQuotation = (from Quotation in Presentation.eStoreContext.Current.User.actingUser.quotations
                                                     where Quotation.QuotationNumber == Request["QuotationRevise"].ToString()
                                                     select Quotation).FirstOrDefault();

                if (currentdQuotation != null)
                {
                    Presentation.eStoreContext.Current.Quotation = currentdQuotation.revise();
                    if (Presentation.eStoreContext.Current.Quotation.statusX == POCOS.Quotation.QStatus.ConfirmedbutNeedTaxIDReview)
                        Presentation.eStoreContext.Current.Quotation.statusX = POCOS.Quotation.QStatus.NeedTaxIDReview;
                    else if (Presentation.eStoreContext.Current.Quotation.statusX == POCOS.Quotation.QStatus.ConfirmedbutNeedFreightReview)
                        Presentation.eStoreContext.Current.Quotation.statusX = POCOS.Quotation.QStatus.NeedFreightReview;
                    else
                        Presentation.eStoreContext.Current.Quotation.statusX = POCOS.Quotation.QStatus.Open;
                    Response.Redirect("~/Quotation/Quote.aspx");
                }
            }

            if (!string.IsNullOrEmpty(Request["ReviewEquotation"]))
            {

            }

            Response.Redirect("~/Admin/MyAdmin.aspx");
        }
    }
}