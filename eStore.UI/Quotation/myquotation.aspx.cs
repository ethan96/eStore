using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Quotation
{
    public partial class myquotation : Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindDefaultQuote();
                bindFonts();
            }

        }

        protected void BindDefaultQuote()
        {
            this.gvmyquotation.DataSource = (from q in Presentation.eStoreContext.Current.User.actingUser.quotations
                                             where q.cartX.UserID == q.UserID
                                             orderby q.statusX, q.ConfirmedDate descending
                                             select q);
            this.gvmyquotation.DataBind();
            this.gvTransfered.DataSource = (from q in Presentation.eStoreContext.Current.User.actingUser.quotations
                                            where q.cartX.UserID != q.UserID
                                            orderby q.statusX, q.ConfirmedDate descending
                                            select q);
            this.gvTransfered.DataBind();
            this.mytransferedHeader.Visible = this.gvTransfered.Rows.Count > 0;
        }

        protected void gvmyquotation_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "reviewQuotation")
            {
                POCOS.Quotation currentdQuotation = (from Quotation in Presentation.eStoreContext.Current.User.actingUser.quotations
                                                     where Quotation.QuotationNumber == e.CommandArgument.ToString()
                                                     select Quotation).FirstOrDefault();

                if (currentdQuotation != null)
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
            if (e.CommandName == "QuotationRevise")
            {
                POCOS.Quotation currentdQuotation = (from Quotation in Presentation.eStoreContext.Current.User.actingUser.quotations
                                                     where Quotation.QuotationNumber == e.CommandArgument.ToString()
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
                else
                { }

            }
        }

        protected void bt_searchQuote_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tb_searchQuoteNo.Text.Trim()))
            {
                this.gvmyquotation.DataSource = (from q in Presentation.eStoreContext.Current.User.actingUser.quotations
                                                 where q.cartX.UserID == q.UserID && q.QuotationNumber.Contains(tb_searchQuoteNo.Text.Trim())
                                                 orderby q.statusX, q.ConfirmedDate descending
                                                 select q);
                this.gvmyquotation.DataBind();
                this.gvTransfered.DataSource = (from q in Presentation.eStoreContext.Current.User.actingUser.quotations
                                                where q.cartX.UserID != q.UserID && q.QuotationNumber.Contains(tb_searchQuoteNo.Text.Trim())
                                                orderby q.statusX, q.ConfirmedDate descending
                                                select q);
                this.gvTransfered.DataBind();
                this.mytransferedHeader.Visible = this.gvTransfered.Rows.Count > 0;
            }
            else
            {
                BindDefaultQuote();
            }
        }

        protected void gvmyquotation_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Presentation.eStoreControls.Repeater rp = (Presentation.eStoreControls.Repeater)e.Row.FindControl("rpRelatedOrders");
                POCOS.Quotation quote = (POCOS.Quotation)e.Row.DataItem;
                rp.DataSource = (from o in Presentation.eStoreContext.Current.User.actingUser.ordersX
                                 where o.Source == quote.QuotationNumber
                                 select o);
                rp.DataBind();
                Button bt_QuoteRevise = e.Row.FindControl("bt_QuoteRevise") as Button;
                if(bt_QuoteRevise != null)
                    bt_QuoteRevise.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Revise);
                Button bt_TQuotate = e.Row.FindControl("bt_TQuotate") as Button;
                if (bt_TQuotate != null)
                    bt_TQuotate.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Revise);
            }
        }

        protected void bindFonts()
        {
            bt_searchQuote.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Search);
        }

    }
}