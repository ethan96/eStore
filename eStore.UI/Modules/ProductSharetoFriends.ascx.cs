using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using esUtilities;

namespace eStore.UI.Modules
{
    public partial class ProductSharetoFriends : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            POCOS.Product _product = getProduct();
            if (_product != null)
            {
                RenderProduct(_product);
            }

        }

        //Get Product
        private POCOS.Product getProduct()
        {
            string ProductId = esUtilities.CommonHelper.QueryString("ProductID");
            POCOS.Product _product = Presentation.eStoreContext.Current.Store.getProduct(ProductId);
            return _product;
        }

        //Set Data
        private void RenderProduct(POCOS.Product product)
        {
            lbl_model_name.Text = product.DisplayPartno;

            if (Presentation.eStoreContext.Current.User != null)
            {
                txt_first_name.Text = Presentation.eStoreContext.Current.User.FirstName;
                txt_last_name.Text = Presentation.eStoreContext.Current.User.LastName;
                txt_your_email.Text = Presentation.eStoreContext.Current.User.UserID;
            }
        }

        //Submit
        protected void btn_submit_Click(object sender, EventArgs e)
        {
            eStore.BusinessModules.EmailProductPage mailTemplate = new BusinessModules.EmailProductPage();
            EMailReponse response = mailTemplate.getEmailProductPage(Presentation.eStoreContext.Current.Store, getProduct(), txt_first_name.Text, txt_last_name.Text, this.txt_comments.Text, this.txt_friends_email.Text, this.txt_your_email.Text
                , Presentation.eStoreContext.Current.MiniSite);
        }

    }
}