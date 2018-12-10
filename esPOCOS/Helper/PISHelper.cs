using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
using eStore.POCOS.Sync;
namespace eStore.POCOS.DAL
{

    public partial class PISHelper : Helper
    {
        public PISEntities context;

        public PISHelper()
        {
            context = new PISEntities ();
        }

        #region Business Read
        public Dictionary<string,DateTime> getPublishedModelByDateFromPIS(DateTime startdate, DateTime enddate)
        {

            Dictionary<string, DateTime> modelpublish = new Dictionary<string, DateTime>();
            if (startdate==null || enddate==null) return null;

            try
            {
                enddate = enddate.AddHours(24);
                List<getPublishedModelByDate_Result> _publishedmodels = context.getPublishedModelByDate(startdate, enddate).ToList();
                if (_publishedmodels != null && _publishedmodels.Count > 0)
                    _publishedmodels = _publishedmodels.OrderByDescending(p => p.publisheddate).ToList();

                foreach (getPublishedModelByDate_Result a in _publishedmodels)
                {
                    if (!modelpublish.ContainsKey(a.modelname))
                        modelpublish.Add(a.modelname, a.publisheddate);
                }

                return modelpublish;
            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        

        #endregion
        //根据keyword去pis查询数据.
        public List<Sync.PRODUCT> getProductListInPISByKeyWord(string keyWord)
        {
            List<Sync.PRODUCT> productList = (from p in context.PRODUCTs 
                         where p.PART_NO.Contains(keyWord)
                         select p).ToList();
            if (productList == null)
                productList = new List<PRODUCT>();

            return productList;
        }
        //根据keyword查询 productNo
        public Dictionary<string, string> getProductListInPISByHint(string keyWord)
        {
            Dictionary<string, string> matchProducts = new Dictionary<string, string>();
            List<eStore.POCOS.Sync.PRODUCT> productList = getProductListInPISByKeyWord(keyWord);
            if (productList != null)
            {
                foreach (var item in productList)
                {
                    if (!matchProducts.ContainsKey(item.PART_NO))
                        matchProducts.Add(item.PART_NO, item.PRODUCT_DESC);
                }
                matchProducts.Distinct();
            }
            return matchProducts;
        }
        //根據part No查询 SAP Product
        public SAP_PRODUCT getSAPProductInPISByPartNo(string partNo)
        {
            var sp = (from p in context.SAP_PRODUCT
                      where p.PART_NO.Equals(partNo.ToUpper())
                      select p).FirstOrDefault();
            return sp;
        }

        #region Others

        private static string myclassname()
        {
            return typeof(PISHelper).ToString();
        }
        #endregion
    }
}