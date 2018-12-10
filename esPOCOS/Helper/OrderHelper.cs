using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{

    public partial class OrderHelper : Helper
    {
        #region Business Read
        public Order getOrderbyOrderno(string orderno)
        {
            if (String.IsNullOrEmpty(orderno)) return null;

            Order _order = null;//CachePool.getInstance().getOrder(orderno);
            try
            {
                if (_order == null)
                {
                    //.Include("Cartitems").Include("PackingLists").Include("Creator").Include("Store").Include("Orders").Include("Quotations")    
                    _order = (from _or in context.Orders
                              where (_or.OrderNo == orderno)
                              select _or).FirstOrDefault();

                    if (_order != null)
                    {
                        _order.helper = this;
                        //cache order
                        CachePool.getInstance().cacheOrder(_order);
                    }
                }

                return _order;

            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }
        public List<Order> getOrdersbyOrderno(string orderno)
        {
            if (String.IsNullOrEmpty(orderno)) return null;

            List<Order> _order = null;//CachePool.getInstance().getOrder(orderno);
            try
            {
                if (_order == null)
                {
                    //.Include("Cartitems").Include("PackingLists").Include("Creator").Include("Store").Include("Orders").Include("Quotations")    
                    _order = (from _or in context.Orders
                              where (_or.OrderNo.Contains(orderno))
                              select _or).ToList() ;

                    if (_order != null)
                    {
                        foreach (Order o in _order.ToList())
                            o.helper = this;
                    }
                }

                return _order;

            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }


        public List<Order> getUserOrders(string userid, string storeid, bool includeAbandon = false)
        {
            if (String.IsNullOrEmpty(userid)) return null;

            try
            {
                if (includeAbandon == true)
                {
                    var _order = (from q in context.Orders
                                  where q.UserID == userid && q.StoreID == storeid
                                  select q);
                    if (_order != null)
                    {
                        foreach (var item in _order)
                        {
                            item.helper = this;
                        }
                    }
                    return _order.ToList();
                }

                else
                {
                    var _orderall = (from q in context.Orders
                                     where q.UserID == userid && q.StoreID == storeid && q.OrderStatus.ToLower() != "open"
                                     select q);
                    if (_orderall != null)
                    {
                        foreach (var item in _orderall)
                        {
                            item.helper = this;
                        }
                    }
                    return _orderall.ToList();
                }

            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }


        //查询order 的open count
        public int[] getOrderOpenCount(List<Order> orders, bool isCartAbandone = false)
        {
            List<String> orderStatuList = new List<string> { "Closed_Converted", "Closed", "Closed_Complete", "Confirmed", 
                                                               "ConfirmdButNeedFreightReview", "ConfirmdButNeedTaxIDReview" };

            string[] OrderFollowUpStatues;
            if (isCartAbandone)//Cart Abandone
                OrderFollowUpStatues = eStore.POCOS.PocoX.FollowUpable.ShippingCartFollowUpStatues;
            else
                OrderFollowUpStatues = eStore.POCOS.PocoX.FollowUpable.OrderFollowUpStatues;

            int[] openClose = new int[] { 0, 0 };
            var recordeCount = (from o in orders
                                let t = context.TrackingLogs.Where(p => p.TrackingNo!=null && p.TrackingNo == o.OrderNo).OrderByDescending(x => x.LogId).FirstOrDefault()
                                select new
                                {
                                    o.OrderNo,
                                    FollowUpStatus = (t == null) ? "N/A" : t.FollowUpStatus
                                }).Distinct().ToList();

            openClose[1] = (from p in recordeCount
                            where
                                !string.IsNullOrEmpty(p.FollowUpStatus) && !OrderFollowUpStatues.Contains(p.FollowUpStatus)
                            select p).Count();
            openClose[0] = recordeCount.Count - openClose[1];

            return openClose;
        }
        
        
        /// <summary>
        /// For OM use, get Orders by DMF and date range
        /// </summary>
        /// <param name="store"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        public List<Order> getOrders(DMF dmf, DateTime startdate, DateTime enddate, ref int[] openCloseCount, string Company = null, string email = null,string orderno = null,
                                    List<string> orderStatuList = null, bool isAdvantech = false, string rbu = "", string FollowUpStatus = "", bool isCartAbandone = false, int minisiteId = 0)
        {
            string[] OrderFollowUpStatues;
            if (isCartAbandone)//Cart Abandone
            {
                OrderFollowUpStatues = eStore.POCOS.PocoX.FollowUpable.ShippingCartFollowUpStatues;
                //Cart Abandone的时候,如果不是open/close. 就只查这一statu
                if (!string.IsNullOrEmpty(FollowUpStatus) && FollowUpStatus.ToUpper() != "OPEN" && FollowUpStatus.ToUpper() != "CLOSE")
                {
                    OrderFollowUpStatues = new string[] { FollowUpStatus };
                    FollowUpStatus = "OPENCLOSE";
                }
            }
            else
                OrderFollowUpStatues = eStore.POCOS.PocoX.FollowUpable.OrderFollowUpStatues;

            //Add 24 hours to include the orders in the enddate
            enddate = enddate.Date.AddHours(24);
            List<Order> orderList = new List<Order>();
            int openCount = 0; int closeCount = 0;

            if (String.IsNullOrEmpty(orderno))
            {
                #region 删除
                //最开始的版本
                //var _orders = (from o in context.Orders.Include("Cart")
                //               from c in context.Carts
                //               from contact in context.CartContacts
                //               from country in context.Countries
                //               let t = context.TrackingLogs.Where(p => p.TrackingNo == o.OrderNo).OrderByDescending(x => x.LogId).FirstOrDefault()
                //               where ((contact.CountryCode == country.Shorts || contact.CountryCode == country.CountryName)
                //                        || (string.IsNullOrEmpty(rbu) ? o.StoreID == dmf.StoreID : false))
                //               && ((string.IsNullOrEmpty(Company) && string.IsNullOrEmpty(email)) ? o.OrderDate >= startdate.Date && o.OrderDate <= enddate : true)
                //               && (!string.IsNullOrEmpty(Company) ? o.User.CompanyName.ToUpper().Contains(Company.ToUpper()) : true)
                //               && (!string.IsNullOrEmpty(email) ? o.UserID.ToUpper().Contains(email.ToUpper()) : true)
                //               && (isAdvantech ? true : !o.UserID.ToLower().Contains("@advantech."))
                //               && o.CartID == c.CartID && orderStatuList.Contains(o.OrderStatus)

                //               && c.BilltoID == contact.ContactID && country.DMF == dmf.DMFID && (string.IsNullOrEmpty(rbu) ? true : country.RbuOM == rbu)

                //               && (string.IsNullOrEmpty(FollowUpStatus) ? true : (
                //                FollowUpStatus.ToUpper() == "OPEN" ? (t.FollowUpStatus == null || OrderFollowUpStatues.Contains(t.FollowUpStatus))
                //                : (t.FollowUpStatus != null && !OrderFollowUpStatues.Contains(t.FollowUpStatus))
                //                ))
                //              orderby o.OrderDate descending
                //              select o).Distinct().ToList();

                //不带FollowUpStatus
                //var _orders = (from o in context.Orders.Include("Cart")
                //               join c in context.Carts on o.CartID equals c.CartID
                //               join contact in context.CartContacts on c.BilltoID equals contact.ContactID
                //               join country in context.Countries on contact.CountryCode equals country.Shorts
                //               let t = context.TrackingLogs.Where(p => p.TrackingNo == o.OrderNo).OrderByDescending(x => x.LogId).FirstOrDefault()
                //               where (country.DMF == dmf.DMFID || (string.IsNullOrEmpty(rbu) ? o.StoreID == dmf.StoreID : false))

                //               && ((string.IsNullOrEmpty(Company) && string.IsNullOrEmpty(email)) ? o.OrderDate >= startdate.Date && o.OrderDate <= enddate : true)
                //               && (!string.IsNullOrEmpty(Company) ? o.User.CompanyName.ToUpper().Contains(Company.ToUpper()) : true)
                //               && (!string.IsNullOrEmpty(email) ? o.UserID.ToUpper().Contains(email.ToUpper()) : true)
                //               && (isAdvantech ? true : !o.UserID.ToLower().Contains("@advantech."))

                //               && orderStatuList.Contains(o.OrderStatus)
                //               && (string.IsNullOrEmpty(rbu) ? true : country.RbuOM == rbu)

                //               && (string.IsNullOrEmpty(FollowUpStatus) ? true : (
                //                 FollowUpStatus.ToUpper() == "OPEN" ? (t.FollowUpStatus == null || OrderFollowUpStatues.Contains(t.FollowUpStatus))
                //                 : (t.FollowUpStatus != null && !OrderFollowUpStatues.Contains(t.FollowUpStatus))
                //                 ))
                //               orderby o.OrderDate descending
                //               select o).ToList();
                #endregion
                
                var sourceOrderList = (from o in context.Orders.Include("Cart")
                                 join c in context.Carts on o.CartID equals c.CartID 
                                 join contact in context.CartContacts on c.BilltoID equals contact.ContactID
                                 join country in context.Countries on contact.CountryCode equals country.Shorts
                                 let t = context.TrackingLogs.Where(p => p.TrackingNo == o.OrderNo).OrderByDescending(x => x.LogId).FirstOrDefault()
                                 where (country.DMF == dmf.DMFID || (string.IsNullOrEmpty(rbu) ? o.StoreID == dmf.StoreID : false))

                                 && ((string.IsNullOrEmpty(Company) && string.IsNullOrEmpty(email)) ? o.OrderDate >= startdate.Date && o.OrderDate <= enddate : true)
                                 && (!string.IsNullOrEmpty(Company) ? o.User.CompanyName.ToUpper().Contains(Company.ToUpper()) : true)
                                 && (!string.IsNullOrEmpty(email) ? o.UserID.ToUpper().Contains(email.ToUpper()) : true)
                                 && (isAdvantech ? true : !o.UserID.ToLower().Contains("@advantech."))

                                 && (minisiteId == 0 ? true : c.MiniSiteID == minisiteId) 

                                 && orderStatuList.Contains(o.OrderStatus)
                                 && (string.IsNullOrEmpty(rbu) ? true : country.RbuOM == rbu) 

                                 && ((!string.IsNullOrEmpty(FollowUpStatus) && FollowUpStatus == "OPENCLOSE") ? 
                                                    (t.FollowUpStatus != null && OrderFollowUpStatues.Contains(t.FollowUpStatus)) : true) 

                                 && ((string.IsNullOrEmpty(FollowUpStatus) || FollowUpStatus == "OPENCLOSE") ? true : (
                                   FollowUpStatus.ToUpper() == "OPEN" ? (t.FollowUpStatus == null || OrderFollowUpStatues.Contains(t.FollowUpStatus))
                                   : (t.FollowUpStatus != null && !OrderFollowUpStatues.Contains(t.FollowUpStatus))
                                   ))
                                 orderby o.OrderDate descending
                                 select new
                                 {
                                     OrderObj = o,
                                     CurrentFollowupStatus = (t == null) ? "" : t.FollowUpStatus
                                 }).ToList();

                foreach (var item in sourceOrderList)
                {
                    orderList.Add(item.OrderObj);
                    if (OrderFollowUpStatues.Contains(item.CurrentFollowupStatus))
                        openCount++;
                    else
                        closeCount++;
                }
                
            }
            else  //search by order no
            {
                #region 删除
                //var _orders = (from o in context.Orders
                //              from c in context.Carts
                //              from contact in context.CartContacts
                //              from country in context.Countries
                //              let t = context.TrackingLogs.Where(p => p.TrackingNo == o.OrderNo).OrderByDescending(x => x.LogId).FirstOrDefault()
                //                where ((contact.CountryCode == country.Shorts || contact.CountryCode == country.CountryName)
                //                     || (string.IsNullOrEmpty(rbu) ? o.StoreID == dmf.StoreID : false))
                //              && o.OrderNo.ToUpper().Contains(orderno.ToUpper())
                //              && o.CartID == c.CartID
                //              && (isAdvantech ? true : !o.UserID.ToLower().Contains("@advantech."))
                //              && c.BilltoID == contact.ContactID && country.DMF == dmf.DMFID && (string.IsNullOrEmpty(rbu) ? true : country.RbuOM == rbu)
                //              && (string.IsNullOrEmpty(FollowUpStatus) ? true : (
                //                FollowUpStatus.ToUpper() == "OPEN" ? (t.FollowUpStatus == null || OrderFollowUpStatues.Contains(t.FollowUpStatus))
                //                : (t.FollowUpStatus != null && !OrderFollowUpStatues.Contains(t.FollowUpStatus))
                //                ))
                //              orderby o.OrderDate descending 
                //              select o).Distinct().ToList();
                #endregion

                var sourceOrderList = (from o in context.Orders.Include("Cart")
                                 join c in context.Carts on o.CartID equals c.CartID
                                 join contact in context.CartContacts on c.BilltoID equals contact.ContactID
                                 join country in context.Countries on contact.CountryCode equals country.Shorts
                                 let t = context.TrackingLogs.Where(p => p.TrackingNo == o.OrderNo).OrderByDescending(x => x.LogId).FirstOrDefault()
                                 where (country.DMF == dmf.DMFID || (string.IsNullOrEmpty(rbu) ? o.StoreID == dmf.StoreID : false))

                                 && o.OrderNo.ToUpper().Contains(orderno.ToUpper())

                                 && (minisiteId == 0 ? true : c.MiniSiteID == minisiteId) 

                                 && orderStatuList.Contains(o.OrderStatus)
                                 && (string.IsNullOrEmpty(rbu) ? true : country.RbuOM == rbu)

                                 && ((!string.IsNullOrEmpty(FollowUpStatus) && FollowUpStatus == "OPENCLOSE") ?
                                                    (t.FollowUpStatus != null && OrderFollowUpStatues.Contains(t.FollowUpStatus)) : true)

                                 && ((string.IsNullOrEmpty(FollowUpStatus) || FollowUpStatus == "OPENCLOSE") ? true : (
                                   FollowUpStatus.ToUpper() == "OPEN" ? (t.FollowUpStatus == null || OrderFollowUpStatues.Contains(t.FollowUpStatus))
                                   : (t.FollowUpStatus != null && !OrderFollowUpStatues.Contains(t.FollowUpStatus))
                                   ))
                                 orderby o.OrderDate descending
                                 select new
                                 {
                                    OrderObj = o,
                                    CurrentFollowupStatus = (t == null) ? "" : t.FollowUpStatus
                                 }).ToList();

                foreach (var item in sourceOrderList)
                {
                    orderList.Add(item.OrderObj);
                    if (OrderFollowUpStatues.Contains(item.CurrentFollowupStatus))
                        openCount++;
                    else
                        closeCount++;
                }
            }

            openCloseCount = new int[] { openCount, closeCount };

            foreach (Order order in orderList)
                order.helper = this;
            return orderList;
        }

        public List<Order> getOrdersByPage(DMF dmf, DateTime startdate, DateTime enddate, string Company = null, string email = null, string orderno = null,
                                    List<string> orderStatuList = null, bool isAdvantech = false, string rbu = "", string FollowUpStatus = "", bool isCartAbandone = false,
                                    int skipOrderCount = 0, int pageOrderSize = 30)
        {
            string[] OrderFollowUpStatues;
            if (isCartAbandone)//Cart Abandone
            {
                OrderFollowUpStatues = eStore.POCOS.PocoX.FollowUpable.ShippingCartFollowUpStatues;
                //Cart Abandone的时候,如果
                if (!string.IsNullOrEmpty(FollowUpStatus) && FollowUpStatus != "OPEN" && FollowUpStatus != "CLOSE")
                {
                    OrderFollowUpStatues = new string[] { FollowUpStatus };
                    FollowUpStatus = "CLOSE";
                }
            }
            else
                OrderFollowUpStatues = eStore.POCOS.PocoX.FollowUpable.OrderFollowUpStatues;

            //Add 24 hours to include the orders in the enddate
            enddate = enddate.Date.AddHours(24);

            if (String.IsNullOrEmpty(orderno))
            {
                var _orders = (from o in context.Orders.Include("Cart")
                               join c in context.Carts on o.CartID equals c.CartID
                               join contact in context.CartContacts on c.BilltoID equals contact.ContactID
                               join country in context.Countries on contact.CountryCode equals country.Shorts
                               let t = context.TrackingLogs.Where(p => p.TrackingNo == o.OrderNo).OrderByDescending(x => x.LogId).FirstOrDefault()
                               where (country.DMF == dmf.DMFID || (string.IsNullOrEmpty(rbu) ? o.StoreID == dmf.StoreID : false))

                               && ((string.IsNullOrEmpty(Company) && string.IsNullOrEmpty(email)) ? o.OrderDate >= startdate.Date && o.OrderDate <= enddate : true)
                               && (!string.IsNullOrEmpty(Company) ? o.User.CompanyName.ToUpper().Contains(Company.ToUpper()) : true)
                               && (!string.IsNullOrEmpty(email) ? o.UserID.ToUpper().Contains(email.ToUpper()) : true)
                               && (isAdvantech ? true : !o.UserID.ToLower().Contains("@advantech."))

                               && orderStatuList.Contains(o.OrderStatus)
                               && (string.IsNullOrEmpty(rbu) ? true : country.RbuOM == rbu)

                               && (string.IsNullOrEmpty(FollowUpStatus) ? true : (
                                 FollowUpStatus.ToUpper() == "OPEN" ? (t.FollowUpStatus == null || OrderFollowUpStatues.Contains(t.FollowUpStatus))
                                 : (t.FollowUpStatus != null && !OrderFollowUpStatues.Contains(t.FollowUpStatus))
                                 ))
                               orderby o.OrderDate ascending
                               select o).Skip(3).Take(5).ToList();

                foreach (Order order in _orders)
                    order.helper = this;
                return _orders;
            }
            else  //search by order no
            {
                var _orders = (from o in context.Orders.Include("Cart")
                               join c in context.Carts on o.CartID equals c.CartID
                               join contact in context.CartContacts on c.BilltoID equals contact.ContactID
                               join country in context.Countries on contact.CountryCode equals country.Shorts
                               let t = context.TrackingLogs.Where(p => p.TrackingNo == o.OrderNo).OrderByDescending(x => x.LogId).FirstOrDefault()
                               where (country.DMF == dmf.DMFID || (string.IsNullOrEmpty(rbu) ? o.StoreID == dmf.StoreID : false))

                               && o.OrderNo.ToUpper().Contains(orderno.ToUpper())

                               && orderStatuList.Contains(o.OrderStatus)
                               && (string.IsNullOrEmpty(rbu) ? true : country.RbuOM == rbu)

                               && (string.IsNullOrEmpty(FollowUpStatus) ? true : (
                                 FollowUpStatus.ToUpper() == "OPEN" ? (t.FollowUpStatus == null || OrderFollowUpStatues.Contains(t.FollowUpStatus))
                                 : (t.FollowUpStatus != null && !OrderFollowUpStatues.Contains(t.FollowUpStatus))
                                 ))
                               orderby o.OrderDate ascending
                               select o).Skip(3).Take(5).ToList();

                foreach (Order order in _orders)
                    order.helper = this;
                return _orders;
            }
        }

        /// <summary>
        /// The method is to pull all abandoned orders across all regions. It may be only useful for abadoned cart event publisher
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="isAdvantech"></param>
        /// <returns></returns>
        public List<Order> getAbandonedOrders(String storeID, DateTime startdate, DateTime enddate, bool isAdvantech = false)
        {
            var _orders = (from o in context.Orders.Include("Cart")
                            where o.StoreID == storeID
                            && o.OrderDate > startdate && o.OrderDate <= enddate 
                            && (isAdvantech ? true : !o.UserID.ToLower().Contains("@advantech."))
                            && o.OrderStatus == "Open"
                            orderby o.OrderDate
                            select o).ToList();
            foreach (Order order in _orders)
                order.helper = this;
            return _orders;
        }

        /// <summary>
        /// get Revenue info
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="isAdvantech"></param>
        /// <returns></returns>
        public List<User> getCustomerRevenue(String storeID, DateTime startdate, DateTime enddate, int revenue,string userID=null, bool isAdvantech = false)
        {
            String[] notConfirmed = {"Open", "Cancel", "Deleted"};
            var _customerRecords = (from uo in
                         (from o in context.Orders.Include("User")
                          where o.StoreID == storeID
                          && o.OrderDate > startdate && o.OrderDate <= enddate
                          && (isAdvantech ? true : !o.UserID.Contains("@advantech.") && !o.UserID.Contains("damez.com"))
                          && !notConfirmed.Contains(o.OrderStatus)
                          && (string.IsNullOrEmpty(userID)?true:o.UserID.ToUpper().Contains(userID.ToUpper()))
                          group o by o.User into p
                          select new { customer = p.Key, customerRevenue = p.Sum(o => (o.TotalAmount - o.TotalDiscount)) }
                          )
                     where uo.customerRevenue >= revenue
                     select new { uo.customer, uo.customerRevenue }
                     ).ToList();

            List<User> customers = new List<User>();
            foreach (var o in _customerRecords)
            {
                o.customer.revenue = o.customerRevenue.GetValueOrDefault();
                customers.Add(o.customer);
            }
            
            return customers;
        }

        /// <summary>
        /// 获取用户在设定时间内的订单
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="storeid"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="includeAbandon"></param>
        /// <returns></returns>
        public List<Order> getUserOrdersByDate(string userid, string storeid, DateTime startdate, DateTime enddate, bool includeAbandon = false)
        {
            if (String.IsNullOrEmpty(userid)) return null;

            try
            {
                if (includeAbandon == true)
                {
                    var _order = (from q in context.Orders
                                  where q.UserID == userid && q.StoreID == storeid
                                  && (q.OrderDate > startdate && q.OrderDate <= enddate)
                                  select q);
                    return _order.ToList();
                }

                else
                {
                    var _orderall = (from q in context.Orders
                                     where q.UserID == userid && q.StoreID == storeid && q.OrderStatus.ToLower() != "open"
                                     && (q.OrderDate > startdate && q.OrderDate <= enddate)
                                     select q);
                    return _orderall.ToList();
                }

            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }
        //计算用户一年的成功消费总金额
        public decimal getUserTotalAmountByYear(string storeId, string userId, DateTime? start = null, DateTime? end = null)
        {
            if (String.IsNullOrEmpty(userId)) return 0;
            DateTime endDate = end ?? DateTime.Parse(DateTime.Now.ToString("MM/dd/yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo));
            DateTime startDate = start ?? endDate.AddYears(-1);
            endDate = endDate.AddDays(1);
            
            decimal yearOrder = (from o in context.Orders
                                 where o.OrderDate >= startDate && o.OrderDate < endDate
                                 && o.StoreID == storeId && o.UserID == userId 
                                 && POCOS.Order.SuccessfullyStatus.Contains(o.OrderStatus) 
                                 && o.TotalAmount != null
                                 group o by o.OrderNo
                                     into g
                                     select new
                                     {
                                         totalAmount = g.Sum(p=>p.TotalAmount)
                                     }).ToList().Sum(p => p.totalAmount.Value);
            
            return yearOrder;
        }
        #endregion

        #region Creat Update Delete


        public int save(Order _order)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_order == null || _order.validate() == false) return 1;
            //Try to retrieve object from DB             

            Order _exist_order = getOrderbyOrderno(_order.OrderNo);

            try
            {

                if (_exist_order == null)  //order not exist 
                {

                    CartHelper ch = new CartHelper();
                    ch.setContext(context);
                    Cart excart = ch.getCartMasterbyCartID(_order.StoreID,_order.Cart.CartID);
                    string userid = _order.Cart.UserID;
                    decimal taxrate = new decimal(0);

                    if (_order.TaxRate.HasValue)
                    {
                        taxrate = _order.TaxRate.Value;
                    }

                    string courier = _order.Courier;
                    // for quotation release to orders
                    //if (_order.Cart.Quotations.Count > 0 )
                    //{
                    //    foreach (Quotation q in _order.Cart.Quotations.ToList())
                    //    {
                    //        //q.helper.context.Detach(q);
                    //        q.helper.context.Detach(_order.Cart);
                    //        break;
                    //    }
                    //    _order.UserID = userid;
                    //    _order.TaxRate = taxrate;
                    //    _order.Courier = courier;
                    //    //context.Orders.AddObject(_order); //state=added.

                    //}

                    if (excart == null)
                    {
                        _order.Cart.save();
                        context = _order.Cart.helper.getContext();
                    }

                    else if (excart != null)
                    {
                        _order.Cart = null;
                        context.Orders.AddObject(_order); //state=added.                      
                    }

                    try
                    {
                        context.SaveChanges();
                        UpdatePayment(_order.Payments.ToList());

                        foreach (CartItem ci in _order.Cart.CartItems.ToList())
                        {
                            if (ci.BTOSystem != null)
                                context.ObjectStateManager.ChangeObjectState(ci.BTOSystem, System.Data.EntityState.Unchanged);
                            //context.BTOSystems.Detach(ci.BTOSystem);

                            context.LoadProperty(ci, "BTOSystem");
                        }
                    }
                    catch (Exception ex)
                    {
                        eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                        return -5000;
                    }

                    return 0;
                }
                else
                {
                    //Update  

                    if (_order.Cart.helper != null && _order.Cart.helper.getContext() != null)
                    {
                        context = _order.Cart.helper.getContext();
                    }
                    else if (_order.helper != null && _order.helper.context != null)
                        context = _order.helper.context;

                    //_order.Cart.save();


                    //context.ObjectStateManager.ChangeObjectState(_order.Cart, System.Data.EntityState.Unchanged);


                    context.Orders.ApplyCurrentValues(_order); //Even applycurrent value, cartmaster state still in unchanged.       
                    context.SaveChanges();
                    UpdatePayment(_order.Payments.ToList());

                    //Force to retrieve again.
                    //_order = getOrderbyOrderno(_order.OrderNo);

                    return 0;
                }

            }

            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }

        }

        public int saveDirectly(Order _order)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_order == null || _order.validate() == false) return 1;
            //Try to retrieve object from DB             
            Order _exist_order = getOrderbyOrderno(_order.OrderNo);
            try
            {
                if (_exist_order == null)  //order not exist 
                {
                    try
                    {
                        context.Orders.AddObject(_order);
                        context.SaveChanges();
                        UpdatePayment(_order.Payments.ToList());
                    }
                    catch (Exception e) {
                        return -5000;
                        throw e;
                        
                    }
                    return 0;
                }
                else
                {
                    context.Orders.ApplyCurrentValues(_order); //Even applycurrent value, cartmaster state still in unchanged.                                           
                    context.SaveChanges();
                    UpdatePayment(_order.Payments.ToList());
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

        public int delete(Order _order)
        {
            if (_order == null) return 1;

            try
            {
                context.DeleteObject(_order);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        /// <summary>
        /// Call Store procedure to encrypt cardno and update the payment records.
        /// </summary>
        /// <param name="payments"></param>
        /// <returns></returns>
        public bool UpdatePayment(List<Payment> payments)
        {
            try
            {
                foreach (Payment _payment in payments)
                {
                    if (string.IsNullOrEmpty(_payment.cardNo) == false)
                    {
                        var ret = context.spUpdatePayment(_payment.cardNo, _payment.PaymentID).FirstOrDefault();
                    }

                }


                return true;
            }
            catch (Exception e)
            {
                eStoreLoger.Fatal(e.Message, "", "", "", e);
                return false;
            }

        }

        /// <summary>
        /// For OM use, this function can only be access with a complicated Passcode.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>

        public Payment decryptPayment(Payment p)
        {

            p.cardNo = context.DecryptPayment(p.PaymentID).FirstOrDefault();

            return p;
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