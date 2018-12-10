using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS
{
    /// <summary>
    /// Packing List decribes a list of packaging boxes which contain cart items
    /// </summary>
    public partial class PackingList
    {
    #region Attributes
        private CartContact _shipto;
        public CartContact ShipTo
        {
            get
            {
                return _shipto;
            }
        }

        private CartContact _soldto;
        public CartContact SoldTo
        {
            get
            {
                return _soldto;
            }
        }

        private CartContact _billto;
        public CartContact BillTo
        {
            get
            {
                return _billto;
            }
        }

        private CartContact _shipFrom;
        public CartContact ShipFrom
        {
            get
            {
                return _shipFrom;
            }
        }

    #endregion


    #region Methods
        public PackingList()
        {}

        /// <summary>
        /// Default construct, initial ShipFrom, ShipTo, BillTo, SoldTo
        /// </summary>
        public PackingList(Cart cart)
        {
            if (cart == null)
                throw new Exception("CartIsNull");
            //get ship from by store
            try
            {
                eStore.POCOS.Store _store = cart.storeX;
                if(_store.ShipFromAddress == null)
                    throw new Exception("StoreShipFromIsNull");
                CartContact _shipFromContact = new CartContact();
                _shipFromContact.Address1 = _store.ShipFromAddress.Address1;
                _shipFromContact.Address2 = _store.ShipFromAddress.Address2;
                _shipFromContact.City = _store.ShipFromAddress.City;
                _shipFromContact.ZipCode = _store.ShipFromAddress.ZipCode;
                _shipFromContact.State = _store.ShipFromAddress.State;
                _shipFromContact.Country = _store.ShipFromAddress.Country;
                _storeID = cart.StoreID;
                this._shipFrom = _shipFromContact;
                this._shipto = cart.ShipToContact;
                this._billto = cart.BillToContact;
                this._soldto = cart.BillToContact;
            }
            catch (Exception ex)
            {
                if (ex.Message == "CartIsNull")
                    throw ex;
                if (ex.Message == "StoreShipFromIsNull")
                {
                    eStoreLoger.Warn("Store's ship from address is null", "", "", _storeID, ex);   
                    throw ex;
                }
            }
        }

        //count boxes on the packing list
        public int getItemCount()
        {
            return (PackagingBoxes == null) ? 0 : PackagingBoxes.Count();
        }
        #endregion
    }
        
}
