using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{

    public partial class PopularModelLogHelper : Helper
    {
        #region Business Read
        //根据Id 获取PopularModelLog 对象
        public PopularModelLog getPopularModelLogById(int id)
        {
            if (id == 0)
                return null;
            var popularLogItem = (from p in context.PopularModelLogs
                      where p.Id == id
                      select p).FirstOrDefault();
            if (popularLogItem != null)
                popularLogItem.helper = this;

            return popularLogItem;
        }
        //点击 +1
        public void saveModelLogByClick(PopularModelLog modelLog)
        {
            DateTime searchDate = DateTime.Now.AddDays(-15);//半个月
            PopularModelLog popularLog = (from p in context.PopularModelLogs
                                          where p.StoreID == modelLog.StoreID
                                                && (!string.IsNullOrEmpty(modelLog.UserId) ? p.UserId == modelLog.UserId : p.SessionID == modelLog.SessionID)
                                                && p.SourceProduct == modelLog.SourceProduct && p.PopulareProduct == modelLog.PopulareProduct 
                                                && p.CreatedDate >= searchDate
                                          orderby p.Id descending
                                          select p).FirstOrDefault();

            if (popularLog != null)
            {
                if (string.IsNullOrEmpty(popularLog.UserId) && !string.IsNullOrEmpty(modelLog.UserId))
                    popularLog.UserId = modelLog.UserId;

                popularLog.Click += 1;
                popularLog.save();
            }
        }
        //添加 或者 更新 推窗次数.
        public void saveModelLogByImpression(PopularModelLog modelLog)
        {
            DateTime searchDate = DateTime.Now.AddDays(-15);//半个月
            //查看半个月内推送产品
            PopularModelLog popularLog = (from p in context.PopularModelLogs
                                          where p.StoreID == modelLog.StoreID
                                                //合并userid 和sessionId 条件
                                                && (!string.IsNullOrEmpty(modelLog.UserId) ? p.UserId == modelLog.UserId : p.SessionID == modelLog.SessionID)
                                                && p.PopularModelId == modelLog.PopularModelId
                                                && p.SourceProduct == modelLog.SourceProduct && p.PopulareProduct == modelLog.PopulareProduct 
                                                && p.CreatedDate >= searchDate
                                          orderby p.Id descending
                                          select p).FirstOrDefault();

            if (popularLog == null)
                save(modelLog);
            else
            {
                popularLog.Impression += 1;
                save(popularLog);
            }
        }

        //登录时,把15天内的sessionId一样的记录,把userId更新
        public void saveModelLogByLogin(PopularModelLog modelLog)
        {
            DateTime searchDate = DateTime.Now.AddDays(-15);//半个月
            List<PopularModelLog> modelLogList = (from p in context.PopularModelLogs
                                                  where p.StoreID == modelLog.StoreID
                                                        && p.SessionID == modelLog.SessionID
                                                        && string.IsNullOrEmpty(p.UserId)
                                                        && p.CreatedDate >= searchDate
                                                  select p).ToList();
            foreach (PopularModelLog itemLog in modelLogList)
            {
                itemLog.UserId = modelLog.UserId;
                itemLog.save();
            }
        }

        public void saveModelLogByOrderQuotation(PopularModelLog modelLog,string docId,List<string> sproductList)
        {
            sproductList = sproductList.Distinct().ToList();
            DateTime searchDate = DateTime.Now.AddDays(-15);//半个月
            List<PopularModelLog> modelLogList = (from p in context.PopularModelLogs.Include("PopularModelConvertHistories") 
                                                  where p.StoreID == modelLog.StoreID
                                                        && p.UserId == modelLog.UserId 
                                                        && sproductList.Contains(p.PopulareProduct) && p.Click > 0
                                                        && p.CreatedDate >= searchDate
                                                        orderby p.Id descending //log 排序
                                                   select p).ToList();
            if (modelLogList != null && modelLogList.Count > 0)
            {
                foreach (PopularModelLog itemLog in modelLogList)
                {
                    PopularModelConvertHistory convertHistory = itemLog.PopularModelConvertHistories.FirstOrDefault(p => p.DocId == docId);
                    if (convertHistory == null)
                    {
                        convertHistory = new PopularModelConvertHistory();
                        convertHistory.ModelLogId = itemLog.Id;
                        convertHistory.DocId = docId;
                        convertHistory.save();
                    }
                }
            }
        }
        #endregion

        #region Creat Update Delete
        public int save(PopularModelLog _modelLog)
        {
            if (_modelLog == null) return 1;

            PopularModelLog _exists_modelLog = getPopularModelLogById(_modelLog.Id);
            try
            {
                if (_exists_modelLog == null)
                {
                    context.PopularModelLogs.AddObject(_modelLog);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    if (_modelLog.helper != null && _modelLog.helper.context != null)
                        context = _modelLog.helper.context;
                    context.PopularModelLogs.ApplyCurrentValues(_modelLog);
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
            return typeof(PopularModelLogHelper).ToString();
        }
        #endregion
    }
}