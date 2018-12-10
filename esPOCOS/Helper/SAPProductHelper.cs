using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class SAPProductHelper : Helper
    {
        #region Business
        public SAPProduct getSAPProductByPartNo(string partNo,string orgId)
        {
            var _sapProduct = (from s in context.SAPProducts
                               where s.PART_NO.ToUpper().Equals(partNo.ToUpper()) && s.ORG_ID == orgId
                                select s).FirstOrDefault();

            if (_sapProduct != null)
                _sapProduct.helper = this;
            return _sapProduct;
        }
        #endregion


        #region Creat Update Delete
        public int save(SAPProduct _sapProduct)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_sapProduct == null || _sapProduct.validate() == false) return 1;
            //Try to retrieve object from DB
            SAPProduct _exist_sapProduct = getSAPProductByPartNo(_sapProduct.PART_NO, _sapProduct.ORG_ID);
            try
            {
                if (_exist_sapProduct == null)  //object not exist 
                {
                    context.SAPProducts.AddObject(_sapProduct);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    if (_sapProduct.helper != null && _sapProduct.helper.context != null)
                        context = _sapProduct.helper.context;
                    context.SAPProducts.ApplyCurrentValues(_sapProduct);
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
        #endregion

        #region Others

        private static string myclassname()
        {
            return typeof(SAPProductHelper).ToString();
        }
        #endregion
    }
}
