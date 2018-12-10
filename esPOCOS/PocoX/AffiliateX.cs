using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using System.Collections.Specialized;
using System.Web;

namespace eStore.POCOS
{
    public partial class Affiliate
    {
        public bool isExistInStore()
        {
            if (string.IsNullOrEmpty(this.SiteID))
                return false;
            AffiliateHelper helper = new AffiliateHelper();
            var aff = helper.getAffiliateBySiteID(this.SiteID);
            if (aff != null)
            {
                this.ID = aff.ID;
                this.Name = aff.Name;
                this.SiteID = aff.SiteID;
                this.Description = aff.Description;
                this.TrackingURL = aff.TrackingURL;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// return url for 1 pixel image tracking
        /// </summary>
        /// <param name="order"></param>
        /// <param name="TrackingID"></param>
        /// <returns></returns>
        public string TrackingOrder(POCOS.Order order, string TrackingID)
        {
            if (order == null)
                return string.Empty;
            AffiliateTransaction afftran = this.AffiliateTransactions.FirstOrDefault(c => c.OrderNO == order.OrderNo);
            if (afftran == null)
            {
                afftran = new AffiliateTransaction()
                {
                    OrderAmount = order.totalAmountX,
                    SiteID = this.SiteID,
                    AffiliateID = this.ID,
                    OrderNO = order.OrderNo,
                    TrackingID = TrackingID,
                    CreateDate = DateTime.Now
                };
                if (afftran.save() == 0)
                {
                    this.AffiliateTransactions.Add(afftran);

                    Dictionary<string, string> q = new Dictionary<string, string>();
                    q.Add("site", this.SiteID);
                    q.Add("trackingID", TrackingID);
                    q.Add("orderNumber", order.OrderNo);
                    q.Add("amount",order.totalAmountX.ToString());
                    return replaceQueryString(this.TrackingURL, q);
                }
                else
                    return string.Empty; 
            }
            return string.Empty; 
        }


        public string TrackingQuotation(POCOS.Quotation quotation, string TrackingID)
        {
            if (quotation == null)
                return string.Empty;
            AffiliateTransaction afftran = this.AffiliateTransactions.FirstOrDefault(c => c.OrderNO == quotation.QuotationNumber);
            if (afftran == null)
            {
                afftran = new AffiliateTransaction()
                {
                    OrderAmount = quotation.totalAmountX,
                    SiteID = this.SiteID,
                    AffiliateID = this.ID,
                    OrderNO = quotation.QuotationNumber,
                    TrackingID = TrackingID,
                    CreateDate = DateTime.Now
                };
                if (afftran.save() == 0)
                {
                    this.AffiliateTransactions.Add(afftran);

                    Dictionary<string, string> q = new Dictionary<string, string>();
                    q.Add("site", this.SiteID);
                    q.Add("trackingID", TrackingID);
                    q.Add("quoteNumber", quotation.QuotationNumber);
                    q.Add("amount", quotation.totalAmountX.ToString());
                    return replaceQueryString(this.TrackingURL, q);
                }
                else
                    return string.Empty;
            }
            return string.Empty;
        }


        public string TrackingUser(POCOS.User user, string TrackingID)
        {
            if (user == null || user.newUser==false)
                return string.Empty;
            AffiliateTransaction afftran = this.AffiliateTransactions.FirstOrDefault(c => c.OrderNO == user.UserID);
            if (afftran == null)
            {
                afftran = new AffiliateTransaction()
                {
                    SiteID = this.SiteID,
                    AffiliateID = this.ID,
                    OrderNO = user.UserID,
                    TrackingID = TrackingID,
                    CreateDate = DateTime.Now
                };
                if (afftran.save() == 0)
                {
                    this.AffiliateTransactions.Add(afftran);

                    Dictionary<string, string> q = new Dictionary<string, string>();
                    q.Add("site", this.SiteID);
                    q.Add("trackingID", TrackingID);
                    q.Add("registration", user.UserID);
                    return replaceQueryString(this.TrackingURL, q);
                }
                else
                    return string.Empty;
            }
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trackingurl">http://intelligentsolutionsfinder.com/buy.php?advantech-tracking&trackingID={trackingID}&registration={registration}&quoteNumber={quoteNumber}&orderNumber={orderNumber}&amount={amount}</param>
        /// <param name="trackingvalue">valid key: registration -email,  quoteNumber, orderNumber, amount-quotation/order total amount </param>
        /// <returns></returns>
        protected string replaceQueryString(string trackingurl,Dictionary<string, string> trackingvalue)
        {
            Uri trackinguri = new Uri(trackingurl);

            NameValueCollection objNewValueCollection = HttpUtility.ParseQueryString(trackinguri.Query);

            string[] keys = objNewValueCollection.AllKeys;
            foreach (string qk in keys)
            {

                if (objNewValueCollection[qk].StartsWith("{") && objNewValueCollection[qk].EndsWith("}"))
                {
                    if (!trackingvalue.Keys.Any(x => string.Format("{{{0}}}", x) == objNewValueCollection[qk]))
                        objNewValueCollection.Remove(qk);
                    else
                    {
                        objNewValueCollection.Set(qk, trackingvalue[trackingvalue.Keys.First(x => string.Format("{{{0}}}", x) == objNewValueCollection[qk])]);
                    }
                }
            }
            UriBuilder ub = new UriBuilder(trackinguri);
            ub.Query = objNewValueCollection.ToString();

            return ub.Uri.ToString();


        }
    }
}
