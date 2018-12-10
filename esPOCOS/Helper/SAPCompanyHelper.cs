using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public partial class SAPCompanyHelper : Helper
    {
        #region Business Read
        /// <summary>
        /// get SAP Company from DB
        /// </summary>
        /// <param name="companyid"></param>
        /// <returns></returns>

        public List<SAPCompany > getSAPCompanies()
        {

            var _company = (from company in context.SAPCompanies
                          select company);

            foreach (SAPCompany m in _company)
            {
                m.helper = this;
            }

            return _company.ToList();

        }

        public VSAPCompany  getSAPCompanybyID(string comapnyid)
        {

            var _company = (from company in context.VSAPCompanies
                               where company.CompanyID==comapnyid
                         select company).FirstOrDefault ();
            return _company;
        }

        public SAPCompany getSAPCompanybyCompanyID(string comapnyid)
        {
            var _sapCompany = (from cp in context.SAPCompanies
                         where cp.CompanyID == comapnyid
                         select cp).FirstOrDefault();

            if (_sapCompany != null)
                _sapCompany.helper = this;
            return _sapCompany;
        }
        public SAPCompany getSAPCompanybyCompanyID(string comapnyid,string orgId,string parentCompanyId)
        {
            var _sapCompany = (from cp in context.SAPCompanies
                               where cp.CompanyID == comapnyid && cp.OrgID == orgId && cp.ParentCompanyID == parentCompanyId
                               select cp).FirstOrDefault();

            if (_sapCompany != null)
                _sapCompany.helper = this;
            return _sapCompany;
        }
        #endregion

        #region Creat Update Delete
        public int save(SAPCompany _sapCompany)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_sapCompany == null || _sapCompany.validate() == false) return 1;
            //Try to retrieve object from DB

            SAPCompany _exist_Company = new SAPCompanyHelper().getSAPCompanybyCompanyID(_sapCompany.CompanyID
                                        , _sapCompany.OrgID, _sapCompany.ParentCompanyID);
            try
            {
                if (_exist_Company == null)  //object not exist 
                {
                    //Insert
                    context.SAPCompanies.AddObject(_sapCompany);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    if (_exist_Company.helper != null)
                        context = _exist_Company.helper.context;
                    context.SAPCompanies.ApplyCurrentValues(_sapCompany);
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
