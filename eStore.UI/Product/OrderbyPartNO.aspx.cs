using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.POCOS.DAL;
using eStore.POCOS;


namespace eStore.UI.Product
{
    public partial class OrderbyPartNO : Presentation.eStoreBaseControls.eStoreBasePage
    {
        public override bool OverwriteMasterPageFile
        {
            get
            {
                return false;
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //getMatchPartsTest();
        }

        protected override void OnPreRender(EventArgs e)
        {
            Presentation.eStoreContext.Current.keywords.Add("Keywords", "OrderByPartNo");
            base.OnPreRender(e);
        }
        public void getMatchPartsTest()
        {
            eStore.POCOS.Store profile = null; // TODO: Initialize to an appropriate value
            eStore.BusinessModules.Store target = new eStore.BusinessModules.Store(profile); // TODO: Initialize to an appropriate value
            string keyword = "AIMB-212N-S6A1E"; // TODO: Initialize to an appropriate value
            IList<Part> actual;
            actual = target.getMatchParts(keyword);
        }

    }

}