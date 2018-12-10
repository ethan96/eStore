using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{

    public partial class QuotationHelper : Helper
    {
        #region Business Read
        public Quotation getQuoteByQuoteno(string Quoteno)
        {
            if (String.IsNullOrEmpty(Quoteno)) return null;

            try
            {
             
                var _quote = (from _quo in context.Quotations
                              where (_quo.QuotationNumber == Quoteno)
                              select _quo).FirstOrDefault();

                if(_quote!=null)
                _quote.helper = this;


                return _quote;

            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }
        public List<Quotation> getMatchQuoteByQuoteno(string Quoteno, string followUpStatus = "")
        {
            if (String.IsNullOrEmpty(Quoteno)) return null;

            try
            {

                var _quote = (from _quo in context.Quotations
                              let t = context.TrackingLogs.Where(p => p.TrackingNo == _quo.QuotationNumber).OrderByDescending(x => x.LogId).FirstOrDefault()
                              where (_quo.QuotationNumber.Contains (Quoteno))
                              && (string.IsNullOrEmpty(followUpStatus) ? true : (
                                    followUpStatus.ToUpper() == "OPEN" ? (t.FollowUpStatus == null || eStore.POCOS.PocoX.FollowUpable.OrderFollowUpStatues.Contains(t.FollowUpStatus))
                                    : (t.FollowUpStatus != null && !eStore.POCOS.PocoX.FollowUpable.OrderFollowUpStatues.Contains(t.FollowUpStatus))
                                ))
                              select _quo).ToList ();

                foreach (Quotation q in _quote)
                {
                    q.helper = this;
                }

                return _quote;

            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }
        //查询Quotation 具体no的 的open count
        public int[] getQuotationByNoOpenCount(string Quoteno)
        {
            int[] openClose = new int[] { 0, 0 };
            if (String.IsNullOrEmpty(Quoteno)) return null;

            try
            {
                var recordeCount = (from _quo in context.Quotations
                                    let t = context.TrackingLogs.Where(p => p.TrackingNo == _quo.QuotationNumber).OrderByDescending(x => x.LogId).FirstOrDefault()
                                    where (_quo.QuotationNumber.Contains(Quoteno))
                                    select t.FollowUpStatus).ToList();
            openClose[1] = (from p in recordeCount where
                            !string.IsNullOrEmpty(p) && !eStore.POCOS.PocoX.FollowUpable.OrderFollowUpStatues.Contains(p) 
                            select p).Count();
            openClose[0] = recordeCount.Count - openClose[1];
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return openClose;
            }
            return openClose;
        }

        /// <summary>
        /// Returns the latest quote of the user
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>

        public Quotation getLastestOpenQuote(string userid , String storeid)
        {
            if (String.IsNullOrEmpty(userid)) return null;

            try
            {
                var _quote = (from _quo in context.Quotations
                              where _quo.UserID == userid && _quo.Status.ToLower()=="open" && _quo.StoreID==storeid orderby _quo.QuoteDate descending
                              select _quo).FirstOrDefault();

                if(_quote!=null)
                _quote.helper = this;


                return _quote;

            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }


        /// <summary>
        ///  Return quotation of the user
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>

        public List<Quotation> getUserQuotes(string userid, string storeid,  bool includeAbandon=false)
        {
            if (String.IsNullOrEmpty(userid)) return null;
    
            try
            {
                if (includeAbandon == true)
                {
                     var _quote = (from q in context.Quotations.Include("Cart")
                                   where (q.UserID == userid || q.Cart.CreatedBy == userid) && q.StoreID == storeid
                                   orderby q.QuoteDate descending
                                  select q);

                     List<Quotation> quotes = _quote.ToList();
                     foreach (Quotation q in quotes)
                     {
                         q.helper = this;
                     }

                     return quotes;
                }

                else
                {
                     var _quoteall = (from q in context.Quotations.Include("Cart")
                                      where (q.UserID == userid || q.Cart.CreatedBy == userid) && q.StoreID == storeid && (q.Status.ToLower() == "confirmed" || q.Status.ToLower() == "confirmedbutneedTaxidreview")
                                      orderby q.QuoteDate descending
                                  select q);

                    List<Quotation> quotes = _quoteall.ToList();
                     foreach (Quotation q in quotes) {
                         q.helper = this;
                     }


                     return quotes;
                }

                

            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        /// <summary>
        /// Set the ordernoX property of Quoation if it was converted to an order.
        /// </summary>
        /// <param name="quotes"></param>
        /// <returns></returns>

        public List<Quotation> setSource(List<Quotation> quotes) {
            var _q = from q in quotes
                     from o in context.Orders
                     where q.QuotationNumber == o.Source
                     select new { q.QuotationNumber, o.OrderNo };

            foreach (Quotation qx in quotes)
            {
                foreach (var q in _q) {
                    if (qx.QuotationNumber == q.QuotationNumber)
                        qx.ordernoX = q.OrderNo;
                }

            }

            return quotes;
        }


        /// <summary>
        /// For OM use, get Quotations by DMF and date range
        /// </summary>
        /// <param name="store"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        public List<Quotation> getQuotations(DMF dmf, DateTime startdate, DateTime enddate, string Company = null,
                                        string email = null, bool isAdvantech = false, string rbu = "", string status = "-- ALL --", string followUpStatus = "", int minisiteId = 0, string errorMsg = "")
        {
            //Add 24 hours to include the orders in the enddate
            StringBuilder condition = new StringBuilder();
            condition.Append("dmf: " + dmf.ToString() + " startdate: " + startdate.ToString() + " enddate: " + enddate.ToString() + " Company: " + Company 
                + " email: " + email + " isAdvantech: " + isAdvantech.ToString() + " rbu: " + rbu + " status: " + status + " followUpStatus: " + followUpStatus + " minisiteId: " + minisiteId.ToString());
            try
            {
                enddate = enddate.Date.AddHours(24);

                context.CommandTimeout = 180;
                var _quotations = (from o in context.Quotations
                                   from c in context.Carts
                                   from contact in context.CartContacts
                                   from country in context.Countries
                                   let t = context.TrackingLogs.Where(p => p.TrackingNo == o.QuotationNumber).OrderByDescending(x => x.LogId).FirstOrDefault()
                                   where ((contact.CountryCode == country.Shorts || contact.CountryCode == country.CountryName)
                                         || (string.IsNullOrEmpty(rbu) ? o.StoreID == dmf.StoreID : false))
                                   && ((string.IsNullOrEmpty(Company) && string.IsNullOrEmpty(email)) ? o.QuoteDate >= startdate.Date && o.QuoteDate <= enddate : true)
                                   && (!string.IsNullOrEmpty(Company) ? o.User.CompanyName.Contains(Company) : true)
                                   && (!string.IsNullOrEmpty(email) ? o.UserID.Contains(email) : true)
                                   && (minisiteId == 0 ? true : c.MiniSiteID == minisiteId)
                                   && (isAdvantech ? true : !o.UserID.ToLower().Contains("@advantech."))
                                   && (status.ToUpper() != "-- ALL --" ? o.Status.ToUpper() == status : true)
                                   && c.CartID == o.CartID
                                   && (c.BilltoID == null ? c.SoldtoID == contact.ContactID : c.BilltoID == contact.ContactID)
                                   && country.DMF == dmf.DMFID
                                   && (string.IsNullOrEmpty(rbu) ? true : country.RbuOM == rbu)
                                   && (string.IsNullOrEmpty(followUpStatus) ? true : (
                                         followUpStatus.ToUpper() == "OPEN" ? (t.FollowUpStatus == null || eStore.POCOS.PocoX.FollowUpable.OrderFollowUpStatues.Contains(t.FollowUpStatus))
                                         : (t.FollowUpStatus != null && !eStore.POCOS.PocoX.FollowUpable.OrderFollowUpStatues.Contains(t.FollowUpStatus))
                                     ))
                                   orderby o.QuoteDate descending
                                   select o).Distinct().ToList();
                if (_quotations != null && _quotations.Any())
                {
                    foreach (var q in _quotations)
                        q.helper = this;
                }
                return _quotations;
            }
            catch (Exception ex)
            {
                eStoreLoger.Error(condition.ToString(),"" ,"" ,"" ,  ex);
                List<Quotation> quoation = new List<Quotation>();
                return quoation;
            }
        }

        //查询Quotation 的open count
        public int[] getQuotationOpenCount(DMF dmf, DateTime startdate, DateTime enddate, string Company = null,
                                        string email = null, bool isAdvantech = false, string rbu = "", string status = "")
        {
            int[] openClose = new int[] { 0, 0 };
            //Add 24 hours to include the orders in the enddate
            enddate = enddate.Date.AddHours(24);
            var recordeCount = (from o in context.Quotations
                                from c in context.Carts
                                from contact in context.CartContacts
                                from country in context.Countries
                                let t = context.TrackingLogs.Where(p => p.TrackingNo == o.QuotationNumber).OrderByDescending(x => x.LogId).FirstOrDefault()
                                where ((contact.CountryCode == country.Shorts || contact.CountryCode == country.CountryName)
                                    || (string.IsNullOrEmpty(rbu) ? o.StoreID == dmf.StoreID : false))
                                && ((string.IsNullOrEmpty(Company) && string.IsNullOrEmpty(email)) ? o.QuoteDate >= startdate.Date && o.QuoteDate <= enddate : true)
                                && (!string.IsNullOrEmpty(Company) ? o.User.CompanyName.Contains(Company) : true)
                                && (!string.IsNullOrEmpty(email) ? o.UserID.Contains(email) : true)
                                && (isAdvantech ? true : !o.UserID.ToLower().Contains("@advantech."))
                                && (!string.IsNullOrEmpty(status) ? o.Status.ToUpper() == status : true)
                                && c.CartID == o.CartID
                                && c.BilltoID == contact.ContactID && country.DMF == dmf.DMFID && (string.IsNullOrEmpty(rbu) ? true : country.RbuOM == rbu)
                                select new
                                {
                                    o.QuotationNumber,
                                    t.FollowUpStatus
                                }).Distinct().ToList();
            openClose[1] = (from p in recordeCount where
                            !string.IsNullOrEmpty(p.FollowUpStatus) && !eStore.POCOS.PocoX.FollowUpable.OrderFollowUpStatues.Contains(p.FollowUpStatus) 
                            select p).Count();
            openClose[0] = recordeCount.Count - openClose[1];
            return openClose;
        }
        #endregion

        #region Creat Update Delete


        public int save(Quotation _quote)
        {

          //  _quote.QuotationNumber  = "ous00001";
            //if parameter is null or validation is false, then return  -1 
            if (_quote == null || _quote.validate() == false) return 1;
            //Try to retrieve object from DB             

            Quotation _exist_order = getQuoteByQuoteno(_quote.QuotationNumber);
            try
            {

                if (_exist_order == null)  //object not exist 
                {
                    context.Quotations.AddObject(_quote); //state=added.
                    context.SaveChanges();                 
                    return 0;
                }
                else
                {
                    //Update   
                    context.Quotations.ApplyCurrentValues(_quote); //Even applycurrent value, cartmaster state still in unchanged.                        
                    context.SaveChanges(); // No changes make in DB                  
                    return 0;
                }

            }

            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);               
                return -5000;
            }

        }

        public int delete(Quotation _quote)
        {

            if (_quote == null) return 1;

            try
            {
                context.DeleteObject(_quote);
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