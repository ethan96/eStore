using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using eStore.POCOS;
using Newtonsoft.Json;

namespace eStore.Presentation.AJAX.Function
{
    class getSAPCompany : IAJAX
    {
        public string ProcessRequest(System.Web.HttpContext context)
        {
            //this function shall only be available for internal sales since SAP is a critical company assets
            User currentUser = Presentation.eStoreContext.Current.User;
            if (currentUser == null || currentUser.actingUser.hasRight(User.ACToken.AccessSAPContact) == false)
                return "";

            int maxRows = 10;
            string keyword = context.Request["keyword"].ToString();
            int.TryParse(context.Request["maxRows"].ToString(), out maxRows);

            if (maxRows > 20)
                maxRows = 20;   //limited searching result

            /*
            var rlt = (from company in Presentation.eStoreSAPCompanies.SAPCompanies.getMatchVSAPCompany(keyword,maxRows)
                       where company.CompanyName.ToLower().Contains(keyword)
                       orderby company.Address
                       select new JObject {
                        new JProperty("value", company.CompanyID),
                         new JProperty("label", company.CompanyName) 
                       })
                       .Take(maxRows);
             * */
            var rlt = (from company in Presentation.eStoreSAPCompanies.SAPCompanies.getMatchVSAPCompany(keyword, maxRows)
                       orderby company.CompanyName
                       select new JObject {
                        new JProperty("value", company.CompanyID),
                         new JProperty("label", string.Format("{0}({1})", company.companyNameX,company.CompanyID)) 
                         //new JProperty("label", string.Format("{0}({1})", company.CompanyName,company.CompanyID)) 
                       })
                       .Take(maxRows);
            return JsonConvert.SerializeObject(rlt);

        }
    }
}
