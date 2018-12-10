using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{

    public partial class CartHelper : Helper
    {
        #region Business Read
        public Cart getCartMasterbyCartID(string storeid,string cartid)  
        {
            if (String.IsNullOrEmpty(cartid)) return null;

            try
            {
                //.Include("Cartitems").Include("PackingLists").Include("Creator").Include("Store").Include("Orders").Include("Quotations")    
                var _cartmaster = (from cm in context.Carts.Include("ShipToContact").Include("BillToContact").Include("Store").Include("SoldToContact")
                                   where (cm.CartID == cartid && cm.StoreID==storeid)
                                   select cm).FirstOrDefault();

                if (_cartmaster != null)
                    _cartmaster.helper = this;
                return _cartmaster;

            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        /// <summary>
        ///  Return Single cart belongs to this user.
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public Cart getCartMastersbyUserID(string userid, string storeid)
        {
            if (String.IsNullOrEmpty(userid)) return null;

            try
            {

                var _cart = (from cm in context.Carts.Include("Cartitems").Include("PackingLists").Include("Creator").Include("Orders").Include("Quotations")
                              where cm.CartID  == userid && cm.StoreID==storeid
                              select cm).FirstOrDefault();

                if (_cart != null)
                {
                    foreach (CartItem ci in _cart.CartItems.ToList())
                    {
                        context.LoadProperty(ci, "Part");
                        context.LoadProperty(ci, "BTOSystem");
                        if (ci.BTOSystem != null)
                            ci.BTOSystem.storeID = _cart.StoreID;
                    }

                    _cart.helper = this;

                }
              
                return _cart;

            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        /// <summary>
        /// For OM, return inactiveCarts.
        /// </summary>
        /// <param name="store"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>

        public List<Cart> getInactiveCarts(Store store, DateTime startdate, DateTime enddate, bool isAdvantech = false,
                    string company = "", string email = "", string FollowUpStatus = "")
        {
            //enddate = enddate.Date.AddHours(72);
            //startdate = startdate.AddDays(2);
            var _carts = from c in context.Carts
                         //let lastupdateitem = c.CartItems.OrderByDescending(x => x.DueDate).FirstOrDefault()
                         let t = context.TrackingLogs.Where(p => p.TrackingNo == c.CartID && p.TrackingType == "CART" && p.StoreID == store.StoreID).OrderByDescending(x => x.LogId).FirstOrDefault() 
                         where c.StoreID == store.StoreID && c.Status.ToUpper() == "OPEN"
                         //&& lastupdateitem.DueDate >= startdate.Date && lastupdateitem.DueDate <= enddate
                         && c.LastUpdateDate >= startdate.Date && c.LastUpdateDate <= enddate
                         && c.CartID==c.UserID // 
                         && (isAdvantech ? true : !c.UserID.ToLower().Contains("@advantech.")) 
                         && (!string.IsNullOrEmpty(company) ? c.User.CompanyName.ToUpper().Contains(company) : true)
                         && (!string.IsNullOrEmpty(email) ? c.UserID.ToUpper().Contains(email) : true)
                         && (c.TotalAmount>0  && c.CartItems.Count>0) // 已经排除 结帐完成对 cart，
                         && (string.IsNullOrEmpty(FollowUpStatus) ? true : (
                            FollowUpStatus.ToUpper() == "OPEN" ? (t.FollowUpStatus == null || eStore.POCOS.PocoX.FollowUpable.ShippingCartFollowUpStatues.Contains(t.FollowUpStatus))
                            : (t.FollowUpStatus != null && !eStore.POCOS.PocoX.FollowUpable.ShippingCartFollowUpStatues.Contains(t.FollowUpStatus))
                         ))
                         //orderby lastupdateitem.DueDate descending 
                         orderby c.LastUpdateDate descending 
                         select c;

            if (_carts != null)
            {
                foreach (var cart in _carts)
                    cart.helper = this;
                return _carts.ToList();
            }
            else
                return new List<Cart>();
        
        }

        public int[] getInactiveCartsOpenCount(Store store, DateTime startdate, DateTime enddate, bool isAdvantech = false,
                            string company = "", string email = "")
        {
            enddate = enddate.Date.AddHours(72);
            startdate = startdate.AddDays(2);
            int[] openClose = new int[] { 0, 0 };
            var recordeCount = (from c in context.Carts
                         //let lastupdateitem = c.CartItems.OrderByDescending(x => x.DueDate).FirstOrDefault()
                                let t = context.TrackingLogs.Where(p => p.TrackingNo == c.CartID && p.TrackingType == "CART" && p.StoreID == store.StoreID).OrderByDescending(x => x.LogId).FirstOrDefault()
                         where c.StoreID == store.StoreID && c.Status.ToUpper() == "OPEN"
                         //&& lastupdateitem.DueDate >= startdate.Date && lastupdateitem.DueDate <= enddate
                         && c.LastUpdateDate >= startdate.Date && c.LastUpdateDate <= enddate
                         && c.CartID == c.UserID
                         && (isAdvantech ? true : !c.UserID.ToLower().Contains("@advantech."))
                         && (!string.IsNullOrEmpty(company) ? c.User.CompanyName.ToUpper().Contains(company) : true)
                         && (!string.IsNullOrEmpty(email) ? c.UserID.ToUpper().Contains(email) : true)
                         && (c.TotalAmount > 0 && c.CartItems.Count > 0)
                         select t.FollowUpStatus).ToList();
            openClose[1] = (from p in recordeCount
                            where
                                !string.IsNullOrEmpty(p) && !eStore.POCOS.PocoX.FollowUpable.ShippingCartFollowUpStatues.Contains(p)
                            select p).Count();
            openClose[0] = recordeCount.Count - openClose[1];
            return openClose;
        }

        public Dictionary<string, int> getCustomersAlsoBought(Cart cart)
        {
            try
            {
                string[] buyingitems=cart.CartItems.Select(x=>x.SProductID).ToArray();
                var result = (from buy in context.CartItems
                              from alsobuy in context.CartItems
                              from buying in buyingitems
                              from order in context.Orders
                              where buy.StoreID == cart.StoreID
                              && alsobuy.StoreID == cart.StoreID
                              && order.StoreID==cart.StoreID
                              && order.CartID == buy.CartID
                              && order.OrderStatus.ToLower()=="confirmed"
                              && buy.CartID==alsobuy.CartID
                              && buy.SProductID == buying
                              && alsobuy.SProductID != buying
                              group alsobuy by alsobuy.SProductID into g
                              orderby g.Sum(x => x.Qty) descending
                              select new
                              {
                                  SProductID = g.Key,
                                  count = g.Sum(x => x.Qty)

                              })
                              //.Take(30)
                              .ToDictionary(x => x.SProductID, x => x.count);

                return result;
        
            }
            catch (Exception)
            {

                return new Dictionary<string, int>();
            }
            
        }

        public Dictionary<string, int> getCustomersAlsoBoughtX(List<POCOS.Product> productList)
        {
            try
            {
                string storeid = productList.Select(p => p.StoreID).ToString();
                string[] buyingitems = productList.Select(x => x.SProductID).ToArray();
                var result = (from buy in context.CartItems
                              from alsobuy in context.CartItems
                              from buying in buyingitems
                              from order in context.Orders
                              where buy.StoreID == storeid
                              && alsobuy.StoreID == storeid
                              && order.StoreID == storeid
                              && order.CartID == buy.CartID
                              && order.OrderStatus.ToLower() == "confirmed"
                              && buy.CartID == alsobuy.CartID
                              && buy.SProductID == buying
                              && alsobuy.SProductID != buying
                              group alsobuy by alsobuy.SProductID into g
                              orderby g.Sum(x => x.Qty) descending
                              select new
                              {
                                  SProductID = g.Key,
                                  count = g.Sum(x => x.Qty)

                              })
                    //.Take(30)
                              .ToDictionary(x => x.SProductID, x => x.count);

                return result;

            }
            catch (Exception)
            {

                return new Dictionary<string, int>();
            }

        }

        /// <summary>
        /// This method will return all inactive carts globally.  It's used by AbandonedCart event publisher. 
        /// </summary>
        /// <param name="store"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="isAdvantech"></param>
        /// <param name="company"></param>
        /// <param name="email"></param>
        /// <param name="FollowUpStatus"></param>
        /// <returns></returns>
        public List<Cart> getInactiveCarts(DateTime startdate, DateTime enddate, bool isAdvantech = false)
        {
            //temporary solution
            startdate = startdate.AddDays(2);
            enddate = enddate.AddDays(2);  

            var _carts = from c in context.Carts
                         let lastupdateitem = c.CartItems.OrderByDescending(x => x.DueDate).FirstOrDefault()
                         let t = context.TrackingLogs.Where(p => p.TrackingNo == c.CartID && p.TrackingType == "CART").OrderByDescending(x => x.LogId).FirstOrDefault()
                         where c.Status.ToUpper() == "OPEN"
                         && c.CartID == c.UserID
                         && lastupdateitem.DueDate >= startdate.Date && lastupdateitem.DueDate <= enddate
                         && (isAdvantech ? true : !c.UserID.ToLower().Contains("@advantech."))
                         && (c.TotalAmount > 0 && c.CartItems.Count > 0)
                         orderby lastupdateitem.DueDate descending
                         select c;

            if (_carts != null)
                return _carts.ToList();
            else
                return new List<Cart>();
        }

        #endregion

        #region Creat Update Delete


        public int save2(Cart _cartmaster)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_cartmaster == null || _cartmaster.validate() == false) return 1;

            //Try to retrieve object from DB  
            Cart _exist_cartmaster = getCartMasterbyCartID(_cartmaster.StoreID,_cartmaster.CartID);
            try
            {

                if (_exist_cartmaster == null)  //Add 
                {
                    foreach (CartItem ci in _cartmaster.CartItems.ToList())
                    {
                        if (ci.Part != null)
                        context.ObjectStateManager.ChangeObjectState(ci.Part, System.Data.EntityState.Unchanged);
                    }               

                    context.Carts.AddObject(_cartmaster); //state=added.
                    context.SaveChanges();                                
              
                    return 0;
                }
                else //Update 
                {
                    context = _cartmaster.helper.context;                      
                    foreach (CartItem ci in _cartmaster.CartItems.ToList())
                    {
                        try
                        {
                            if (ci.Part != null && context.ObjectStateManager.GetObjectStateEntry(ci.Part) != null)
                                context.ObjectStateManager.ChangeObjectState(ci.Part, System.Data.EntityState.Unchanged);

                            if (ci.BTOSystem != null)
                            {
                                context.BTOSystems.ApplyCurrentValues(ci.BTOSystem);
                                context.SaveChanges();
                            }

                        }catch(Exception){
                        
                        }
                    }

                    context.Carts.ApplyCurrentValues(_cartmaster); //Even applycurrent value, cartmaster state still in unchanged.                 
                    context.SaveChanges();            

                    }

                    return 0;               

            }

            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);                
                return -5000;
            }

        }



        public int save(Cart _cartmaster)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_cartmaster == null || _cartmaster.validate() == false) return 1;

            //Try to retrieve object from DB  
            Cart _exist_cartmaster = getCartMasterbyCartID(_cartmaster.StoreID,_cartmaster.CartID);
            try
            {

                if (_exist_cartmaster == null)  //Add 
                {
                    foreach (CartItem ci in _cartmaster.CartItems.ToList())
                    {
                        if (ci.Part != null)
                            context.ObjectStateManager.ChangeObjectState(ci.Part, System.Data.EntityState.Unchanged);
                    }

                    context.Carts.AddObject(_cartmaster); //state=added.
                    context.SaveChanges();

                    return 0;
                }
                else //Update 
                {
                    context = _cartmaster.helper.context;
                    foreach (CartItem ci in _cartmaster.CartItems.ToList())
                    {
                        try
                        {
                            if (ci.Part != null && context.ObjectStateManager.GetObjectStateEntry(ci.Part) != null)
                                context.ObjectStateManager.ChangeObjectState(ci.Part, System.Data.EntityState.Unchanged);

                            if (ci.BTOSystem != null)
                            {
                                context.BTOSystems.ApplyCurrentValues(ci.BTOSystem);
                                context.SaveChanges();
                            }

                        }
                        catch (Exception)
                        {

                        }
                    }

                    context.Carts.ApplyCurrentValues(_cartmaster); //Even applycurrent value, cartmaster state still in unchanged.                 
                    context.SaveChanges();

                }

                return 0;

            }

            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }

        }



        public void remove(CartItem ci , Cart cart) {

            context.CartItems.DeleteObject(ci);
           
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