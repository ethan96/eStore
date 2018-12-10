namespace eStore.POCOS.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using eStore.POCOS.DAL;
    using Utilities;

    public partial class CampaignMailLogByEDM_Helper: Helper
    {
        public CampaignMailLogByEDM getCampaignMailLogByEDMByID(string storeid, int id)
        {
            var _campaignMailLogByEDM = (from campaignMailLogByEDM in context.CampaignMailLogByEDMs
                                         where campaignMailLogByEDM.Storeid == storeid && campaignMailLogByEDM.ID == id
                                         select campaignMailLogByEDM).FirstOrDefault();
            if (_campaignMailLogByEDM != null)
            {
                _campaignMailLogByEDM.helper = this;
            }

            return _campaignMailLogByEDM;
        }

        public CampaignMailLogByEDM getCampaignMailLogByEDMByMailLogId(string storeid, string mailLogId)
        {
            var _campaignMailLogByEDM = (from campaignMailLogByEDM in context.CampaignMailLogByEDMs
                                         where campaignMailLogByEDM.Storeid == storeid && campaignMailLogByEDM.MailLogId == mailLogId
                                         select campaignMailLogByEDM).FirstOrDefault();
            if (_campaignMailLogByEDM != null)
            {
                _campaignMailLogByEDM.helper = this;
            }

            return _campaignMailLogByEDM;
        }

        public List<CampaignMailLogByEDM> getCampaignMailLogByEDMByStoreID(string storeid, DateTime startDate, DateTime endDate, int?[] isCheck)
        {
            var _campaignMailLogByEDM = (from campaignMailLogByEDM in context.CampaignMailLogByEDMs
                                         where campaignMailLogByEDM.Storeid == storeid 
                                         && campaignMailLogByEDM.CreateDate >= startDate
                                         && campaignMailLogByEDM.CreateDate <= endDate
                                         && (isCheck.Contains(campaignMailLogByEDM.IsCheck))
                                         select campaignMailLogByEDM);

            foreach (CampaignMailLogByEDM m in _campaignMailLogByEDM)
            {
                m.helper = this;
            }
            return _campaignMailLogByEDM.ToList();
        }

        public int save(CampaignMailLogByEDM _campaignMailLogByEDM)
        {
            int errorCode = -5000;
            CampaignMailLogByEDM _exist_CampaignMailLogByEDM = new CampaignMailLogByEDM_Helper()
                                .getCampaignMailLogByEDMByID(_campaignMailLogByEDM.Storeid, _campaignMailLogByEDM.ID);
            try
            {
               
                if (_exist_CampaignMailLogByEDM == null)
                {
                    context.CampaignMailLogByEDMs.AddObject(_campaignMailLogByEDM);
                    context.SaveChanges();
                    int id = _campaignMailLogByEDM.ID;
                    errorCode = id;
                }
                else
                {
                    context.CampaignMailLogByEDMs.ApplyCurrentValues(_campaignMailLogByEDM);
                    context.SaveChanges();
                    errorCode = 0;
                }

                return errorCode;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return errorCode;
            }
        }

        public int delete(CampaignMailLogByEDM _campaignMailLogByEDM)
        {
            int errorCode = -5000;

            try
            {
                _campaignMailLogByEDM.helper.context.DeleteObject(_campaignMailLogByEDM);
                _campaignMailLogByEDM.helper.context.SaveChanges();
                errorCode = 0;
                return errorCode;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return errorCode;
            }
        }
    }
}
