using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class liveperson : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        private string _livePersonType = "liveperson2012"; // liveperson2012 or liveperson2011
        public string LivePersonType //暂时不在webConfig做设置。
        {
            get { return _livePersonType; }
            set { _livePersonType = value; }
        }

        public bool UserLargerImage { get; set; }


        protected void Page_Load(object sender, EventArgs e)
        {
            phLivePerson.Controls.Clear();
            //if (Presentation.eStoreContext.Current.Store.storeID == "AJP")
            //    _livePersonType = "livepersonAJP";
            if (Presentation.eStoreContext.Current.Store.storeID == "ACN")
                _livePersonType = "livepersonACN";
            else if(Presentation.eStoreContext.Current.Store.storeID == "AUS"
                || Presentation.eStoreContext.Current.Store.storeID == "ALA"
                || Presentation.eStoreContext.Current.Store.storeID == "ASC"
                )
                _livePersonType = "livepersonAUS";

            livepersonBase livepersonBase = this.LoadControl("~/Modules/" + LivePersonType + ".ascx") as livepersonBase;
            livepersonBase.UserLargerImage = UserLargerImage;
            phLivePerson.Controls.Add(livepersonBase);
        }

    }

}