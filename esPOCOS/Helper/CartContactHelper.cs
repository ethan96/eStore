using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{

    public partial class CartContactHelper : Helper
    {
        #region Business Read
        public CartContact getCartContactID(int contactid)
        {

            try
            {
               
                var _cartcontact= (from cm in context.CartContacts
                                   where cm.ContactID == contactid
                                   select cm).FirstOrDefault();


                return _cartcontact;

            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

 


 

        #endregion

        #region Creat Update Delete


        public int save(CartContact _contact)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_contact == null || _contact.validate() == false) return 1;
            //Try to retrieve object from DB             
            _contact.Address2 = _contact.Address2 ?? ""; //确保Address2不为null
            CartContact _exist_CartContact = getCartContactID(_contact.ContactID);
            try
            {

                if (_exist_CartContact == null)  //object not exist 
                {

                    context.CartContacts.AddObject(_contact); //state=added.
                    context.SaveChanges();
              
                    return 0;
                }
                else
                {

                    context.CartContacts.ApplyCurrentValues(_contact); //Even applycurrent value, cartmaster state still in unchanged.                        
                    context.SaveChanges(); // No changes make in DB
              
                    return 0;
                }

            }

            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
                throw ex;
                
            }

        }

        public int delete(Cart _cartmaster)
        {

            if (_cartmaster == null || _cartmaster.validate() == false) return 1;

            try
            {
                context.DeleteObject(_cartmaster);
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
            return typeof(CartHelper).ToString();
        }
        #endregion
    }
}