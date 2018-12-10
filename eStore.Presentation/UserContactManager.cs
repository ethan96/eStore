using System;
using System.Collections.Generic;
using eStore.POCOS;

namespace eStore.Presentation
{
    /// <summary>
    ///     create by andy.
    ///     for user contact
    /// </summary>
    public class UserContactManager
    {
        public static List<eStoreContact> GeteStoreContacts(POCOS.Cart cart)
        {
            var eStoreContacts = new List<eStoreContact>();

            foreach (var contact in eStoreContext.Current.User.actingUser.Contacts)
            {
                var userContact = eStoreContacts.Find(x => x.AddressID == contact.AddressID);
                if (userContact == null)
                {
                    if (contact.AddressID == eStoreContext.Current.User.actingUser.CompanyID)
                    {
                        var tmpContact = contact;
                        tmpContact.CreatedDate = DateTime.Now;
                        userContact = new eStoreContact(tmpContact);
                    }
                    else
                    {
                        userContact = new eStoreContact(contact);
                    }

                    eStoreContacts.Add(userContact);
                }
            }
            if (cart.ShipToContact != null)
            {
                var shipToContact = eStoreContacts.Find(x => x.AddressID == cart.ShipToContact.AddressID);
                if (shipToContact == null)
                {
                    shipToContact = new eStoreContact(cart.ShipToContact) { isShipTo = true };
                    eStoreContacts.Add(shipToContact);
                }
                else
                {
                    shipToContact.isShipTo = true;
                }
            }
            if (cart.BillToContact != null)
            {
                var billToContact = eStoreContacts.Find(x => x.AddressID == cart.BillToContact.AddressID);
                if (billToContact == null)
                {
                    billToContact = new eStoreContact(cart.BillToContact) { isBillTo = true };
                    eStoreContacts.Add(billToContact);
                }
                else
                {
                    billToContact.isBillTo = true;
                }
            }
            if (cart.SoldToContact != null) //&& ContactList1.ShowBillTo)
            {
                var soldToContact = eStoreContacts.Find(x => x.AddressID == cart.SoldToContact.AddressID);
                if (soldToContact == null)
                {
                    soldToContact = new eStoreContact(cart.SoldToContact) { isSoldTo = true };
                    eStoreContacts.Add(soldToContact);
                }
                else
                {
                    soldToContact.isSoldTo = true;
                }
            }

            return eStoreContacts;
        }
    }
}