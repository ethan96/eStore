using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class BundelItemHelper : Helper
    {
        #region Business Read
        public BundleItem getBundleByID(int bundleItemId)
        {
            try
            {
                var _Bundle = (from b in context.BundleItems
                                where b.BundleItemID == bundleItemId
                                select b).FirstOrDefault();
                if (_Bundle != null)
                    _Bundle.helper = this;

                return _Bundle;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }
        #endregion

        #region Creat Update Delete
        public int delete(int bundleItemId)
        {
            BundleItem _bundelItem = getBundleByID(bundleItemId);
            if (_bundelItem == null || _bundelItem.validate() == false) return 1;
            try
            {
                context = _bundelItem.helper.context;
                //context.DeleteObject(_bundelItem);
                context.BundleItems.DeleteObject(_bundelItem);
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
        /// 
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="sproductid"></param>
        /// <returns></returns>
        public List<ProductBundleItem> getBundleItems(string storeid, string sproductid)
        {
            if (string.IsNullOrEmpty(storeid) || string.IsNullOrEmpty(sproductid))
                return new List<ProductBundleItem>();

            var ls = (from item in context.ProductBundleItems
                      where item.SProductID == sproductid && item.StoreID == storeid
                      orderby item.Sequence
                      select item).ToList();
            return ls;
        }

        #endregion

        #region Others

        private static string myclassname()
        {
            return typeof(BundelItemHelper).ToString();
        }
        #endregion
    }
}