using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{

    public partial class EDMCampaignHelper : Helper
    {
        /// <summary>
        /// 根据id获取相应的EDMCampaign信息
        /// </summary>
        /// <param name="campaignID"></param>
        /// <returns></returns>
        public EDMCampaign getEdmCampaignByID(int campaignID)
        {
            if (string.IsNullOrEmpty(campaignID.ToString()))
                return null;
            try
            {
                var result = (from c in context.EDMCampaigns
                              where c.CampaignID==campaignID
                              select c).FirstOrDefault();
                if (result != null)
                    result.helper = this;
                return result;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        /// <summary>
        /// 获取当前store下所有的campains
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="todayTime"></param>
        /// <param name="isCanShow"></param>
        /// <returns></returns>
        public List<EDMCampaign> getAllEDMCampaignByParentID(string storeid, int parentId)
        {
            if (string.IsNullOrEmpty(storeid))
                return null;
            try
            {
                var _result = (from c in context.EDMCampaigns
                               where c.StoreID == storeid && c.ParentID == parentId
                               orderby c.CreatedDate ascending
                               select c).ToList();

                List<EDMCampaign> campaigns = null;
                if (_result != null)
                    campaigns = _result;
                else
                    return new List<EDMCampaign>();

                foreach (EDMCampaign c in campaigns)
                    c.helper = this;

                return campaigns;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return new List<EDMCampaign>();
            }

        }

        /// <summary>
        /// 获取parent lead selecttion
        /// </summary>
        /// <param name="leadSelection"></param>
        /// <returns></returns>
        public EDMCampaign getAllEDMCampaignByName(string leadSelection)
        {
            if (string.IsNullOrEmpty(leadSelection))
                return null;
            try
            {
                var _result = (from c in context.EDMCampaigns
                               where c.LeadSelection == leadSelection && c.ParentID == null
                               select c).FirstOrDefault();

                if (_result != null)
                    _result.helper = this;

                return _result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        #region CRUD

        public int save(EDMCampaign _campaign)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_campaign == null || _campaign.validate() == false) return 1;
            //Try to retrieve object from DB             

            EDMCampaign _exist_camp = getEdmCampaignByID(_campaign.CampaignID);
            try
            {
                if (_campaign.helper != null && _campaign.helper.context != null)
                    context = _campaign.helper.context;


                if (_exist_camp == null)  //object not exist 
                {
                    context.EDMCampaigns.AddObject(_campaign); //state=added.                            
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.EDMCampaigns.ApplyCurrentValues(_campaign);
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


        public int delete(EDMCampaign _edmcampaign)
        {

            if (_edmcampaign == null || _edmcampaign.validate() == false) return 1;

            //Campaign will not be permenently removed.  Instead it'll be marked as deleted

            try
            {
                context = _edmcampaign.helper.context;
                context.DeleteObject(_edmcampaign);
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
    }
}