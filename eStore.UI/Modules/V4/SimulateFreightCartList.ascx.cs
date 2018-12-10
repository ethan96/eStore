using eStore.POCOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.V4
{
    public partial class SimulateFreightCartList : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public List<CartItem> CartList
        {
            set
            {
                gvCartContent.DataSource = value;
                gvCartContent.DataBind();
            }
        }

        protected void gvCartContent_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                CartItem item = e.Row.DataItem as CartItem;
                Literal ltLineNo = e.Row.FindControl("ltLineNo") as Literal;
                Literal ltSProductID = e.Row.FindControl("ltSProductID") as Literal;
                Literal ltDisplayPartNo = e.Row.FindControl("ltDisplayPartNo") as Literal;
                Literal ltProductName = e.Row.FindControl("ltProductName") as Literal;
                Literal ltDeleteBtn = e.Row.FindControl("ltDeleteBtn") as Literal;

                ltLineNo.Text = item.ItemNo.ToString();
                ltSProductID.Text = item.SProductID;
                ltDisplayPartNo.Text = item.ProductName;
                ltProductName.Text = item.productNameX;
                ltDeleteBtn.Text = string.Format("<button type=\"button\" onclick=\"DeleteItem('{0}')\">{1}</button>", item.SProductID, Presentation.eStoreLocalization.Tanslation(Store.TranslationKey.Cart_Delete));
            }
        }
    }
}