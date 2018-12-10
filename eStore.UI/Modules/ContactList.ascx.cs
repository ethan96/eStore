using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.POCOS;

namespace eStore.UI.Modules
{
    public partial class ContactList : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public event GridViewCommandEventHandler RowCommand;

        public List<Presentation.eStoreContact> eStoreContacts
        {
            set {
                if (value != null)
                {
                    rpContactInfor.DataSource = value.OrderByDescending(c => c.CreatedDate).Take(5).OrderBy(c => c.AddressID).ToList();
                    rpContactInfor.DataBind();
                }
            }
        }
        private bool? _showBillTo;
        public bool ShowBillTo
        {
            get
            {

                if (_showBillTo == null)
                {
                    _showBillTo = Presentation.eStoreContext.Current.User != null && Presentation.eStoreContext.Current.User.actingUser.actingRole == User.Role.Employee;
                }
                return _showBillTo ?? false;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (RowCommand != null)
            //    this.gvContacts.RowCommand += RowCommand;
            //if (ShowBillTo)
            //    this.gvContacts.Columns[2].Visible = true;
            //else
            //    this.gvContacts.Columns[2].Visible = false;
        }

        protected void rpContactInfor_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Presentation.eStoreContact ec = e.Item.DataItem as Presentation.eStoreContact;

                if (ec.AddressID == Presentation.eStoreContext.Current.User.actingUser.mainContact.AddressID || ec.contactType == Presentation.ContactType.Customer)
                {
                    //e.Item.FindControl("btnDel").Visible = false;

                }

                Literal ltAddressInfor = e.Item.FindControl("ltAddressInfor") as Literal;
                ltAddressInfor.Text = Presentation.eStoreContext.Current.Store.profile.formatContactAddress(ec.toCartContact());
                Literal ltPhoneInfor = e.Item.FindControl("ltPhoneInfor") as Literal;
                ltPhoneInfor.Text = Presentation.eStoreContext.Current.Store.profile.formatContactPhone(ec.toCartContact());
            }
        }



    }
}