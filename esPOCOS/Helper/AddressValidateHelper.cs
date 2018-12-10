using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public class AddressValidateHelper:Helper
    {
        #region Business Read
        public AddressValidate getAddressValidateById(int id)
        {
            var addressItem = (from av in context.AddressValidates
                               where av.Id == id
                               select av).FirstOrDefault();
            if (addressItem != null)
                addressItem.helper = this;
            return addressItem;
        }

        public AddressValidate getAddressValidateByErpId(string erpId,string addressType)
        {
            var addressItem = (from av in context.AddressValidates
                               where av.ERPId == erpId && av.AddressType == addressType 
                               orderby av.UpdateDate descending
                               select av).FirstOrDefault();
            if (addressItem != null)
                addressItem.helper = this;
            return addressItem;
        }

        public AddressValidate getPrevalidatedAddress(AddressValidate validateAddress, string addressType)
        {
            var addressItem = (from av in context.AddressValidates
                               where av.ERPId == validateAddress.ERPId && av.Country == validateAddress.Country
                               && av.State == validateAddress.State && av.City == validateAddress.City
                               && av.Address1 == validateAddress.Address1 && av.Address2 == validateAddress.Address2
                               && av.ZipCode == validateAddress.ZipCode && av.AddressType == addressType
                               orderby av.Id descending
                               select av).FirstOrDefault();
            if (addressItem != null)
                addressItem.helper = this;
            return addressItem;
        }
        #endregion

        

        #region Creat Update Delete
        public int save(AddressValidate _addressItem)
        {
            if (_addressItem == null) return 1;

            AddressValidate _exists_addressItem = getAddressValidateById(_addressItem.Id);
            try
            {
                if (_exists_addressItem == null)
                {
                    context.AddressValidates.AddObject(_addressItem);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    if (_addressItem.helper != null && _addressItem.helper.context != null)
                        context = _addressItem.helper.context;
                    context.AddressValidates.ApplyCurrentValues(_addressItem);
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

        public int delete(AddressValidate _addressItem)
        {
            if (_addressItem == null) return 1;

            try
            {
                if (_addressItem.helper != null && _addressItem.helper.context != null)
                    context = _addressItem.helper.context;
                context.AddressValidates.DeleteObject(_addressItem);
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
    }
}
