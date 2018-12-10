using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{

    public partial class CampaignHelper : Helper
    {

        #region Business Read
        public  List<Campaign> getCampaigns(string storeid)
        {
            if (String.IsNullOrEmpty(storeid)) return null;

            try
            {
                List<Campaign> campaigns = new List<Campaign>();

                using (eStore3Entities6 context = new eStore3Entities6())
                {
                    DateTime today = DateTime.Now.Date;
                    var _campaign = (from cmp in context.Campaigns.Include("CampaignStrategies")
                                     where (cmp.StoreID == storeid &&
                                      (String.IsNullOrEmpty(cmp.Status) || cmp.Status.ToUpper().Equals("DELETED") == false))
                                      && cmp.ExpiredDate>=today
                                      && cmp.EffectiveDate<=today
                                     select cmp);

                    if (_campaign != null)
                    {
                        campaigns = _campaign.ToList();
                        foreach (Campaign c in campaigns)
                            c.helper = this;
                    }
                }

                return campaigns;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return new List<Campaign>();
                //return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public List<Campaign> getCampaignByType(string type, string storeid, Boolean isExpiredDate = false)
        {
            if (String.IsNullOrEmpty(storeid)) return null;

            try
            {
                List<Campaign> campaigns = new List<Campaign>();
                DateTime todayNow = DateTime.Now;
                using (eStore3Entities6 context = new eStore3Entities6())
                {
                    var _campaign = (from cmp in context.Campaigns.Include("CampaignStrategies").Include("Store")
                                     where (cmp.StoreID == storeid && cmp.PromotionType.ToUpper().Equals(type.ToUpper()) &&
                                      (String.IsNullOrEmpty(cmp.Status) || cmp.Status.ToUpper().Equals("DELETED") == false)
                                      && (isExpiredDate ? cmp.ExpiredDate > todayNow : true))
                                     select cmp);

                    if (_campaign != null)
                    {
                        campaigns = _campaign.ToList();
                        foreach (Campaign c in campaigns)
                            c.helper = this;
                    }
                }

                return campaigns;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return new List<Campaign>();
                //return null;
            }
        }


        /// <summary>
        /// To retrieve store campaigns within specified period
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public List<Campaign> getCampaigns(string storeid, DateTime startDate, DateTime endDate, Boolean includeNonPublished = false, string promotionCode = "")
        {
            if (String.IsNullOrEmpty(storeid)) return null;

            try
            {
                using (eStore3Entities6 context = new eStore3Entities6())
                {
                    //There is no publishment control at campaign yet.  Will add campaign publish status check up later
                    var _resultSet = (from cmp in context.Campaigns.Include("CampaignStrategies")
                                    where (cmp.StoreID == storeid &&
                                            (
                                                (startDate <= cmp.EffectiveDate && endDate >= cmp.EffectiveDate) ||
                                                (startDate <= cmp.ExpiredDate && endDate >= cmp.ExpiredDate) || 
                                                (startDate >= cmp.EffectiveDate && endDate <= cmp.ExpiredDate  )
                                            )
                                                 && (String.IsNullOrEmpty(cmp.Status) || cmp.Status.ToUpper().Equals("DELETED") == false))
                                                 && (!String.IsNullOrEmpty(promotionCode) ? cmp.PromotionCode.ToLower().Contains(promotionCode) : true)
                                      orderby cmp.CreatedDATE  descending 
                                      select cmp);

                    List<Campaign> campaigns = null;
                    if (_resultSet != null)
                        campaigns = _resultSet.ToList<Campaign>();
                    else
                        campaigns = new List<Campaign>();

                    foreach (Campaign c in campaigns)
                        c.helper = this;

                    return campaigns;
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return new List<Campaign>();
            }
        }

        /// <summary>
        /// get Campaign by datetime
        /// </summary>
        /// <param name="storeid">store di</param>
        /// <param name="todayTime">today time</param>
        /// <returns></returns>
        public List<Campaign> getCampaigns(string storeid, DateTime todayTime,bool isCanShow = true)
        {
            if (String.IsNullOrEmpty(storeid)) return null;
            try
            {
                using (eStore3Entities6 context = new eStore3Entities6())
                {
                    var _resultSet = from cmp in context.Campaigns.Include("CampaignStrategies")
                                     where cmp.StoreID == storeid && cmp.EffectiveDate <= todayTime && cmp.ExpiredDate >= todayTime
                                            && (String.IsNullOrEmpty(cmp.Status) || cmp.Status.ToUpper().Equals("DELETED") == false)
                                            && cmp.Show == isCanShow
                                     select cmp;

                    List<Campaign> campaigns = null;
                    if (_resultSet != null)
                        campaigns = _resultSet.ToList<Campaign>();
                    else
                        campaigns = new List<Campaign>();

                    foreach (Campaign c in campaigns)
                        c.helper = this;
                    return campaigns;
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return new List<Campaign>();
            }
        }
        //根据CampaignCodes 获取Campaign
        public Campaign getCampaignsByCodes(string storeid, string campaignCode)
        {
            if (String.IsNullOrEmpty(storeid)) return null;
            DateTime currentTime = DateTime.Now.Date;

            var _campaign = (from cmp in context.Campaigns.Include("CampaignStrategies")
                              join cc in context.CampaignCodes on cmp.CampaignID equals cc.CampaignID
                             where cmp.StoreID == storeid && cmp.PromotionType == "System"
                                    && cmp.EffectiveDate <= currentTime && cmp.ExpiredDate >= currentTime
                                    && (String.IsNullOrEmpty(cmp.Status) || cmp.Status.ToUpper().Equals("DELETED") == false)
                                    && cc.StoreId == storeid && cc.PromotionCode == campaignCode
                                    && cc.EffectiveDate <= currentTime && currentTime <= cc.ExpiredDate 
                             select cmp).FirstOrDefault();

            if (_campaign != null)
                _campaign.helper = this;
            return _campaign;
        }

        public Campaign getCampaignByID(Campaign camp) 
        {
            Campaign _campaign = getCampaignByID(camp.StoreID, camp.CampaignID);

            if (_campaign != null)
                _campaign.helper = this;

            return _campaign;
        }

        public Campaign getCampaignByID(String storeID, String campaignID)
        {
            try
            {
                int campID = Convert.ToInt32(campaignID);
                return getCampaignByID(storeID, campID);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Campaign getCampaignByID(string storeid, int campid)
        {
            var _campaign = (from cmp in context.Campaigns
                             where (cmp.StoreID == storeid && cmp.CampaignID == campid)
                             select cmp).FirstOrDefault();

            if (_campaign != null)
                _campaign.helper = this;
            
            return _campaign;
        }

        /// <summary>
        /// This method will return how many times the specified campaign strategy has been applied by the specified user
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="storeid"></param>
        /// <param name="strategyID"></param>
        /// <returns></returns>
        public int getUserCampaignUsageCount(string userid, string storeid, int strategyID){

            var _aptimes = (from promotionapp in context.PromotionAppliedLogs
                          where promotionapp.UserID.ToLower() == userid.ToLower() && promotionapp.StoreID == storeid
                          && promotionapp.CampaignStrategyID == strategyID
                          && promotionapp.Type == "Order"
                          select promotionapp).Count();

            return _aptimes;
        }

        /// <summary>
        /// This method returns how many time the specified campaign strategy has been applied in current store.
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="promotioncode"></param>
        /// <returns></returns>
        public int getStoreCampaignUsageCount( string storeid, string promotioncode)
        {

            var _aptimes = (from promotionapp in context.PromotionAppliedLogs
                            where  promotionapp.StoreID == storeid
                            && promotionapp.PromotionCode == promotioncode
                            && promotionapp.Type == "Order"
                            select promotionapp.Qty).Count();

            return _aptimes;
        }

        public List<PromotionAppliedLog> getPromotionAppliedByQuotationNumber(string storeid, string quotationNumber)
        {

            var _aptimes = (from promotionapp in context.PromotionAppliedLogs
                            where  promotionapp.StoreID == storeid
                            && promotionapp.QuoteOrderNo == quotationNumber
                            && promotionapp.Type == "Quotation"
                            select promotionapp).ToList();

            return _aptimes;
        }

        #endregion

        #region CRUD

        public int save(Campaign  _campaign)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_campaign == null || _campaign.validate() == false) return 1;
            //Try to retrieve object from DB             

            Campaign _exist_camp = getCampaignByID(_campaign);
            try
            {
                if (_campaign.helper != null && _campaign.helper.context != null)
                    context = _campaign.helper.context;


                if (_exist_camp == null)  //object not exist 
                {
                    context.Campaigns .AddObject(_campaign); //state=added.                            
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                   context.Campaigns.ApplyCurrentValues(_campaign);
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



        public int delete(Campaign _campaign)
        {

            if (_campaign == null || _campaign.validate() == false) return 1;

            //Campaign will not be permenently removed.  Instead it'll be marked as deleted
            /*
            try
            {
                context = _campaign.helper.context;
                context.DeleteObject(_campaign);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
             * */
            _campaign.Status = "DELETED";
            foreach (CampaignStrategy strategy in _campaign.CampaignStrategies)
                strategy.Status = "DELETED";

            return save(_campaign);
        }

        #endregion

        #region Others

        private static string myclassname()
        {
            return typeof(CampaignHelper).ToString();
        }
        #endregion
    }
}