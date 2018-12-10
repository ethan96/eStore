using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;


namespace eStore.POCOS.Sync
{
    class OrderImport
    {
        AdvStoreEntities acontext = new AdvStoreEntities();
        eStore3Entities6 context = new eStore3Entities6();
        Store store;
        public void syncOrder(string storeid)
        {
            var _oldorders = from o in acontext.EAPRO_ORDER_MASTER
                             where o.Store_Id == storeid && o.ORDER_NO.StartsWith("OUS")
                             orderby o.ORDER_NO descending
                             //&& o.ORDER_NO=="OUS008072"  CTOS order test.
                             select o;

            store = new StoreHelper().getStorebyStoreid(storeid);

            foreach (EAPRO_ORDER_MASTER o in _oldorders.ToList())
            {

                //Create Order
                Order neworder = new OrderHelper().getOrderbyOrderno(o.ORDER_NO);

                if (neworder == null)
                    neworder = new Order();

                neworder.ChannelID = o.ChannelID;
                neworder.ChannelName = o.ChannelName;
                neworder.StoreID = o.Store_Id;
                neworder.Courier = o.Courier;
                neworder.CourierAccount = o.Courier_account;
                neworder.CourierPhone = o.Courier_phone;

                neworder.CustomerComment = o.ORDER_NOTE;
                neworder.OrderNo = o.ORDER_NO;
                neworder.OrderDate = o.ORDER_DATE;
                neworder.OrderStatus = o.ORDER_STATUS;
                neworder.TotalAmount = o.TOTAL_AMOUNT;
                neworder.UserID = o.User_ID;
                neworder.ShippingMethod = o.Shippingmethod;
                neworder.ShipmentTerm = o.SHIPMENT_TERM;
                neworder.EarlyShipFlag = o.EARLY_SHIP_FLAG == "Y" ? true : false;


                //create Cart   //Create cartContact
                User owner = new UserHelper().getUserbyID(o.User_ID);
                Cart cart = neworder.Cart;

                if (cart == null)
                    cart = new Cart(store, owner);

                cart.StoreID = o.Store_Id;

                CartContact soldto = createCartContact(o.SOLDTO_ID, neworder, store);
                if (soldto != null)
                {
                    soldto.save();
                    cart.SoldtoID = soldto.ContactID;
                }

                CartContact billto = createCartContact(o.BILLTO_ID, neworder, store);
                if (billto != null)
                {
                    billto.save();
                    cart.BilltoID = billto.ContactID;
                }


                CartContact shipto = createCartContact(o.SHIPTO_ID, neworder, store);
                if (shipto != null)
                {
                    shipto.save();
                    cart.ShiptoID = shipto.ContactID;
                }




                if (cart.ShiptoID != null && cart.BilltoID != null && cart.SoldtoID != null)
                {

                    createCartitem(cart, o);
                    neworder.Cart = cart;
                    //neworder.savedirectly();
                }
                else
                {

                    Console.WriteLine(o.ORDER_NO);
                }

            }

        }

        public void createCartitem(Cart cart, EAPRO_ORDER_MASTER oldorder)
        {
            var _items = from x in acontext.EAPRO_ORDER_DETAIL
                         where x.ORDER_NO == oldorder.ORDER_NO && x.ORDER_ID == oldorder.ORDER_ID && x.Store_Id == oldorder.Store_Id
                         select x;

            foreach (EAPRO_ORDER_DETAIL odetail in _items.ToList())
            {
                Part p = constructPart(odetail.PART_NO, store);

                if (odetail.ORDER_LINE_TYPE == "FG")
                {
                    cart.addItem(p, odetail.QTY.GetValueOrDefault());

                }
                else if (odetail.ORDER_LINE_TYPE == "BTO")
                {
                    Product_Ctos bto = (Product_Ctos)new PartHelper().getPart("SBC-BTO", store);
                    BTOSystem newbtos = bto.getDefaultBTOS();


                    newbtos.BTONo = odetail.PART_NO;

                    var _childs = from c in acontext.EAPRO_ORDER_DETAIL
                                  where c.ORDER_NO == odetail.ORDER_NO && c.ORDER_ID == odetail.ORDER_ID && c.PARENT_LINE_NO == odetail.LINE_NO
                                  && c.Store_Id == odetail.Store_Id
                                  select c;

                    foreach (EAPRO_ORDER_DETAIL child in _childs)
                    {
                        Part ctospart = constructPart(child.PART_NO, store);
                        newbtos.addNoneCTOSItem(ctospart, child.QTY.GetValueOrDefault());
                    }

                    cart.addItem(p, odetail.QTY.GetValueOrDefault(), newbtos);
                }


            }

            //          cart.save();


        }

        public Part constructPart(string partno, Store store)
        {
            PartHelper phelper = new PartHelper();

            Part ctospart = phelper.getPart(partno, store.StoreID, true);
            if (ctospart == null)
                ctospart = phelper.AddtoStore(partno, store); // Save to Part , if the part is not exist

            return ctospart;
        }

        public CartContact createCartContact(string addressid, Order order, Store store)
        {
            var _existContact = (from ec in context.CartContacts
                                 where ec.AddressID == addressid
                                 select ec).FirstOrDefault();

            if (_existContact == null)
            {

                var _cartcontact = (from c in acontext.EBIZ_ADDRESS
                                    where c.Address_ID.ToUpper().Equals(addressid.ToUpper())
                                    select c).FirstOrDefault();

                CartContact newcontact = new CartContact();
                if (_cartcontact != null)
                {
                    newcontact.AddressID = _cartcontact.Address_ID;
                    newcontact.Address1 = _cartcontact.ADDRESS1;
                    newcontact.Address2 = _cartcontact.ADDRESS2;
                    newcontact.City = _cartcontact.CITY;
                    newcontact.State = _cartcontact.STATE;
                    newcontact.Country = _cartcontact.COUNTRY;
                    newcontact.CountryCode = getCountryCode(store,_cartcontact.COUNTRY);
                    newcontact.TelNo = _cartcontact.TEL_NO;
                    newcontact.FirstName = _cartcontact.ATTENTION;
                    newcontact.AttCompanyName = _cartcontact.ATTENTION_Company_Name;
                    newcontact.CreatedBy = _cartcontact.CREATED_BY;
                    newcontact.CreatedDate = _cartcontact.CREATED_DATE.HasValue == true ? _cartcontact.CREATED_DATE.Value : DateTime.Now;
                    newcontact.FaxNo = _cartcontact.FAX_NO;
                    newcontact.TelExt = _cartcontact.TEL_EXT;
                    newcontact.UserID = order.UserID;

                    return newcontact;
                }
                else
                {
                    return null;
                }

            }
            else
            {
                return _existContact;

            }
        }


        private string getCountryCode(Store store, string name)
        {
            var code = (from country in store.Countries
                        where country.CountryName.ToUpper().Equals(name) || country.Shorts.ToUpper().Equals(name)
                        select country.Shorts).FirstOrDefault();

            return code;

        }
    }
}