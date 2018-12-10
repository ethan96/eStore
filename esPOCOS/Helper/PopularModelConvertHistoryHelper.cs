using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{

    public partial class PopularModelConvertHistoryHelper : Helper
    {
        #region Business Read
        //根据Id 获取PopularModelLog 对象
        public PopularModelConvertHistory getPopularModelConvertHistoryById(int id)
        {
            if (id == 0)
                return null;
            var popularItem = (from p in context.PopularModelConvertHistories
                      where p.Id == id
                      select p).FirstOrDefault();
            if (popularItem != null)
                popularItem.helper = this;

            return popularItem;
        }
        
        #endregion

        #region Creat Update Delete
        public int save(PopularModelConvertHistory _modelHistory)
        {
            if (_modelHistory == null) return 1;

            PopularModelConvertHistory _exists_modelHistory = getPopularModelConvertHistoryById(_modelHistory.Id);
            try
            {
                if (_exists_modelHistory == null)
                {
                    context.PopularModelConvertHistories.AddObject(_modelHistory);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    if (_modelHistory.helper != null && _modelHistory.helper.context != null)
                        context = _modelHistory.helper.context;
                    context.PopularModelConvertHistories.ApplyCurrentValues(_modelHistory);
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
            return typeof(PopularModelConvertHistoryHelper).ToString();
        }
        #endregion
    }
}