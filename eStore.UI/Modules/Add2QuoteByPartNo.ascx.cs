using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation;

namespace eStore.UI.Modules
{
    public partial class Add2QuoteByPartNo : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                bindFonts();
        }

        protected void bt_ByPartNoAddtoQuote_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tb_ByPartNoAddtoQuote.Text.Trim()))
            {
                POCOS.Part part=null;
                try
                {
                  part = Presentation.eStoreContext.Current.Store.getPart(tb_ByPartNoAddtoQuote.Text.Trim());
                    if (part == null)
                        part = Presentation.eStoreContext.Current.Store.addVendorPartToPart(tb_ByPartNoAddtoQuote.Text.Trim());
                }
                catch (Exception)
                {  
                }

                if (part != null && part.getListingPrice().value>0)
                {
                    POCOS.Quotation quotation = Presentation.eStoreContext.Current.Quotation;
                    //quotation.cartX.addItem(part, 1);
                    quotation.cartX.addSeparatedItem(part, 1);
                    quotation.save();
                    Response.Redirect("~/Quotation/Quote.aspx");

                }
                else
                {
                    object[] args = new object[1];
                    args[0] = tb_ByPartNoAddtoQuote.Text.Trim();
                    Presentation.eStoreContext.Current.AddStoreErrorCode("Product {0} is not currently available.", args);
                }
            }
            else
            { Presentation.eStoreContext.Current.AddStoreErrorCode("Product is not currently available."); }
            
        }

        protected void bindFonts()
        {
            bt_ByPartNoAddtoQuote.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoQuotation);
        }

    }
}