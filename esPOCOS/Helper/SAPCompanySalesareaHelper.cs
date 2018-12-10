using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public partial class SAPCompanySalesareaHelper : Helper
    {
        #region Business Read
        public SAPCompanySalesarea getSAPCompanySalebyCompanyID(string comapnyid,string orgId,string channel,string divsion)
        {
            var _sapCompanySale = (from cp in context.SAPCompanySalesareas
                         where cp.COMPANY_ID == comapnyid && cp.ORG_ID == orgId && cp.CHANNEL == channel && cp.DIVISION == divsion
                         select cp).FirstOrDefault();

            if (_sapCompanySale != null)
                _sapCompanySale.helper = this;
            return _sapCompanySale;
        }

        public SAPCompanySalesarea getSAPCompanySalebyCompanyID(string companyId)
        {
            var _sapCompanySale = (from cp in context.SAPCompanySalesareas
                                   where cp.COMPANY_ID == companyId 
                                   select cp).FirstOrDefault();

            if (_sapCompanySale != null)
                _sapCompanySale.helper = this;
            return _sapCompanySale;
        }
        #endregion

        #region Creat Update Delete
        public int save(SAPCompanySalesarea _sapCompanySale)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_sapCompanySale == null || _sapCompanySale.validate() == false) return 1;
            //Try to retrieve object from DB

            SAPCompanySalesarea _exist_Company = new SAPCompanySalesareaHelper().getSAPCompanySalebyCompanyID(_sapCompanySale.COMPANY_ID,
                                                    _sapCompanySale.ORG_ID, _sapCompanySale.CHANNEL, _sapCompanySale.DIVISION);
            try
            {
                if (_exist_Company == null)  //object not exist 
                {
                    //Insert
                    context.SAPCompanySalesareas.AddObject(_sapCompanySale);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    if (_exist_Company.helper != null)
                        context = _exist_Company.helper.context;
                    context.SAPCompanySalesareas.ApplyCurrentValues(_sapCompanySale);
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
    }
}
