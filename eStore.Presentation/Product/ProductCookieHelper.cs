using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

///
/// ----- eStoreCompareProducts Cookie name
/// ----- CompareProductIds  Cookie Key
///


namespace eStore.Presentation.Product
{
    class ProductCookieHelper
    {
        private HttpCookie compareCookie = null;

        /// <summary>
        /// get eStoreCompareProducts Cookie
        /// </summary>
        public ProductCookieHelper()
        {
            compareCookie = HttpContext.Current.Request.Cookies["eStoreCompareProducts"];
            if (compareCookie == null)
            {
                compareCookie = new HttpCookie("eStoreCompareProducts");
                TimeSpan ts = new TimeSpan(10, 0, 0, 0);
                compareCookie.Expires = DateTime.Now.Add(ts);
            }
        }

        /// <summary>
        /// save eStoreCompareProducts Cookie
        /// </summary>
        /// <param name="datespanCount"></param>
        private void save(int datespanCount = 10)
        {
            compareCookie.Expires = DateTime.Now.Add(new TimeSpan(datespanCount, 0, 0, 0));
            HttpContext.Current.Response.Cookies.Set(compareCookie);
            HttpContext.Current.Response.AppendCookie(compareCookie);
        }

        /// <summary>
        /// CompareProductIds has product or not
        /// </summary>
        /// <param name="partNo"></param>
        /// <returns>true or false</returns>
        public bool isHasCompareProduct(string partNo)
        {
            bool isResult = false;
            if (compareCookie.Values["CompareProductIds"] != null)
            {
                isResult = compareCookie.Values.GetValues("CompareProductIds").Contains(partNo);
            }
            return isResult;
        }

        /// <summary>
        /// detele eStoreCompareProducts Cookie
        /// </summary>
        public void ClearCompareProducts()
        {
            if (compareCookie != null)
            {
                compareCookie.Values.Clear();
                save(-1);
            }
        }

        /// <summary>
        /// Add one product to CompareProductIds
        /// </summary>
        /// <param name="partNo"></param>
        public void AddCompareProducts(string partNo)
        {
            if (compareCookie.Values["CompareProductIds"] != null)
            {
                if (!compareCookie.Values.GetValues("CompareProductIds").Contains(partNo))
                {
                    compareCookie.Values.Add("CompareProductIds", partNo);
                }
            }
            else
            {
                compareCookie.Values.Add("CompareProductIds", partNo);
            }
            save();
        }

        /// <summary>
        /// delete one product from CompareProductIds
        /// </summary>
        /// <param name="partNo"></param>
        public void DelCompaerPoducts(string partNo)
        {
            if (compareCookie.Values["CompareProductIds"] != null)
            {
                if (compareCookie.Values.GetValues("CompareProductIds").Contains(partNo))
                {
                    List<string> ls = GetAllCompareProducts();
                    ls.Remove(partNo);
                    compareCookie.Values.Clear();
                    foreach (string str in ls)
                    {
                        compareCookie.Values.Add("CompareProductIds", str);
                    }
                }
            }
            save();
        }

        /// <summary>
        /// get all CompareProductIds products
        /// </summary>
        /// <returns>List<string> </returns>
        public List<string> GetAllCompareProducts()
        {
            List<string> ls = null;
            if (compareCookie.Values["CompareProductIds"] != null)
            {
                ls = compareCookie.Values.GetValues("CompareProductIds").ToList();
            }
            return ls;
        }
    }
}
