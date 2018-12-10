using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;

namespace eStore.POCOS.DAL
{
    public class SAP_EMPLOYEEHelper : Helper
    {
        public List<SalesPerson> getOrgerSalesPersons(Store store)
        {
            if (store.StoreID == "AUS")
                return store.SalesPersons.OrderBy(x => x.Name).ToList();

            string _orger = store.getStringSetting("PriceSAPOrg");
            if (_orger.Length > 2)
                _orger = _orger.Substring(0, 2);
            var ls = (from c in context.SAP_EMPLOYEE
                      where c.PERS_AREA.StartsWith(_orger)
                      orderby c.FULL_NAME
                      select new
                      {
                          EmployeeID = c.SALES_CODE,
                          Name = c.FULL_NAME,
                          StoreID = store.StoreID,
                          Email = string.IsNullOrEmpty(c.EMAIL) ? "" : c.EMAIL
                      }).ToList()
                      .Select(b => new SalesPerson()
                      {
                          EmployeeID = b.EmployeeID,
                          Name = b.Name,
                          StoreID = b.StoreID,
                          Email = b.Email
                      }).OrderBy(x => x.Name).ToList();
            return ls;
        }

        public SalesPerson getSalesPersonByCode(string code,Store store)
        {
            if (store.StoreID == "AUS")
                return store.SalesPersons.FirstOrDefault(c => c.EmployeeID == code);

            if (string.IsNullOrEmpty(code) || store == null)
                return new SalesPerson();
            string _orger = store.getStringSetting("PriceSAPOrg");
            if (_orger.Length > 2)
                _orger = _orger.Substring(0, 2);
            var item = (from c in context.SAP_EMPLOYEE
                        where c.SALES_CODE == code && c.PERS_AREA.StartsWith(_orger)
                        select new
                        {
                            EmployeeID = c.SALES_CODE,
                            Name = c.FULL_NAME,
                            StoreID = store.StoreID,
                            Email = string.IsNullOrEmpty(c.EMAIL) ? "" : c.EMAIL
                        }).ToList()
                      .Select(b => new SalesPerson()
                      {
                          EmployeeID = b.EmployeeID,
                          Name = b.Name,
                          StoreID = b.StoreID,
                          Email = b.Email
                      }).FirstOrDefault();
            return item;
        }


        public List<SalesPerson> getSalesPersonByCompany(string companyid, Store store,string partnerFunction = "")
        {
            
            if (string.IsNullOrEmpty(companyid) || store == null)
                return new List<SalesPerson>();
            string _orger = store.getStringSetting("PriceSAPOrg");
            if (_orger.Length > 2)
                _orger = _orger.Substring(0, 2);

            var item = (from p in context.SAP_COMPANY_PARTNERS
                        where p.COMPANY_ID == companyid && p.ORG_ID.StartsWith(_orger)
                            && (string.IsNullOrEmpty(partnerFunction) ? true : p.PARTNER_FUNCTION == partnerFunction)
                        select p).FirstOrDefault();
            if (item == null)
                return new List<SalesPerson>();

            if (store.StoreID == "AUS")
                return store.SalesPersons.Where(c => c.EmployeeID == item.SALES_CODE).ToList();

            var ls = (from c in context.SAP_EMPLOYEE
                      where c.PERS_AREA.StartsWith(_orger) && c.SALES_CODE == item.SALES_CODE
                      select new
                      {
                          EmployeeID = c.SALES_CODE,
                          Name = c.FULL_NAME,
                          StoreID = store.StoreID,
                          Email = string.IsNullOrEmpty(c.EMAIL) ? "" : c.EMAIL
                      }).ToList()
                      .Select(b => new SalesPerson()
                      {
                          EmployeeID = b.EmployeeID,
                          Name = b.Name,
                          StoreID = b.StoreID,
                          Email = b.Email
                      }).ToList();
            return ls;

        }

        public SalesPerson getSalesPersonByEmail(string email, Store store)
        {
            if (string.IsNullOrEmpty(email) || store == null)
                return null;
            string _orger = store.getStringSetting("PriceSAPOrg");
            if (_orger.Length > 2)
                _orger = _orger.Substring(0, 2);
            var item = (from c in context.SAP_EMPLOYEE
                        where c.EMAIL == email && c.PERS_AREA.StartsWith(_orger)
                        select new
                        {
                            EmployeeID = c.SALES_CODE,
                            Name = c.FULL_NAME,
                            StoreID = store.StoreID,
                            Email = string.IsNullOrEmpty(c.EMAIL) ? "" : c.EMAIL
                        }).ToList()
                      .Select(b => new SalesPerson()
                      {
                          EmployeeID = b.EmployeeID,
                          Name = b.Name,
                          StoreID = b.StoreID,
                          Email = b.Email
                      }).FirstOrDefault();
            return item;
        }

        public SalesPerson getSalesPersonBySalesCode(string salesCode, Store store)
        {
            if (string.IsNullOrEmpty(salesCode) || store == null)
                return null;
            string _orger = store.getStringSetting("PriceSAPOrg");
            if (_orger.Length > 2)
                _orger = _orger.Substring(0, 2);
            var item = (from c in context.SAP_EMPLOYEE
                        where c.SALES_CODE == salesCode && c.PERS_AREA.StartsWith(_orger)
                        select new
                        {
                            EmployeeID = c.SALES_CODE,
                            Name = c.FULL_NAME,
                            StoreID = store.StoreID,
                            Email = string.IsNullOrEmpty(c.EMAIL) ? "" : c.EMAIL
                        }).ToList()
                      .Select(b => new SalesPerson()
                      {
                          EmployeeID = b.EmployeeID,
                          Name = b.Name,
                          StoreID = b.StoreID,
                          Email = b.Email
                      }).FirstOrDefault();
            return item;
        }
    }
}
