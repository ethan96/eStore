using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.POCOS;

namespace eStore.Presentation
{
   public class eStoreSAPCompanies
    {
        public class SAPCompanies
        {
     
            public static List<VSAPCompany> getMatchVSAPCompany(string keyword,int Count=10)
            {
                /*
                var scs = (from sc in eStoreContext.Current.Store.sapContacts
                          where sc.CompanyName.ToLower().Contains(keyword.ToLower())
                          select sc).Take(Count);
                return scs.ToList();
                 * */

                return eStoreContext.Current.Store.getMatchVSAPCompanies(keyword, Count);
            }
            public static VSAPCompany getVSAPCompany(string CompanyID)
            {
                /*
                VSAPCompany vsapcompany = (from sc in eStoreContext.Current.Store.sapContacts
                                         where sc.CompanyID==CompanyID
                                         select sc).FirstOrDefault();
                */
                VSAPCompany vsapcompany = eStoreContext.Current.Store.findSAPContact(CompanyID);

                return vsapcompany;
            }
        }
    }
}
