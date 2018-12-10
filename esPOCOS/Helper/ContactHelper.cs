using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class ContactHelper : Helper
    {
        public Contact getContactbyID(int id)
        {
            var _c = (from c in context.Contacts
                      where c.ContactID == id
                      select c).FirstOrDefault();

            return _c;

        }

        #region Creat Update Delete
        public int save(Contact _contact)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_contact == null || _contact.validate() == false) return 1;
            //Try to retrieve object from DB
            Contact _exist_contact = getContactbyID(_contact.ContactID);
            try
            {
                if (_exist_contact == null)  //object not exist 
                {
                    //Insert
                    context.Contacts.AddObject(_contact);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.Contacts.ApplyCurrentValues(_contact);
                    context.SaveChanges();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        public int delete(Contact _contact)
        {
            if (_contact == null || _contact.validate() == false) return 1;
            try
            {
                context.DeleteObject(_contact);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }
        #endregion

        #region Others

        private static string myclassname()
        {
            return typeof(ContactHelper).ToString();
        }
        #endregion
    }
}