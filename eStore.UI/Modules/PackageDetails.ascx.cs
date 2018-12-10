using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class PackageDetails : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
       
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        POCOS.Cart cart;
        public POCOS.Cart Cart { get { return cart; } set { cart = value; } }
        public void bindPackageDetails()
        {
            this.rpPackingBox.DataSource = getPackingList();
            this.rpPackingBox.DataBind();
            this.rpPackingBox.Visible = true;
            BindScript("script", "showContactDialog", "$(function() { showpackingdetailsDialog(); });");
        }

        List<POCOS. PackagingBox> getPackingList()
        {
            List<POCOS.PackagingBox> boxes = new List<POCOS.PackagingBox>();
            try
            {
                eStore.POCOS.PackingList packinglist = new POCOS.PackingList();
                eStore.BusinessModules.PackingManager packingManager = new BusinessModules.PackingManager(Presentation.eStoreContext.Current.Store.profile);
                packinglist = packingManager.getPackingList(Cart, 0m);
                boxes = packinglist.PackagingBoxes.ToList();
            }
            catch (Exception)
            {


            }
            return boxes;
        }
    }
}