using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
 
    public partial class SuggestingProducts : System.Web.UI.UserControl
    {
        bool? _ShowATP = null;
        public Boolean ShowATP
        {
            get
            {
                if (_ShowATP == null)
                {
                    if (Presentation.eStoreContext.Current.User == null)
                        _ShowATP = false;
                    else
                        _ShowATP = Presentation.eStoreContext.Current.User.actingUser.hasRight(POCOS.User.ACToken.ATP);
                }
                return _ShowATP ?? false;
            }
        }
        public Dictionary<string, int> SuggestingProductsList
        {
            get;
            set;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (SuggestingProductsList != null && SuggestingProductsList.Count() > 0)
            {
                rpSuggestingProducts.DataSource = Presentation.eStoreContext.Current.Store.applyAlsoBoughtProductWeight(SuggestingProductsList);
                rpSuggestingProducts.DataBind();
            }
            else
                this.Visible = false;
        }
       
       
    }
}