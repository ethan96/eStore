using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class TaxConfigHelper : Helper
    {
            public decimal? getFixTaxRate(string storeid)
            {
                if (String.IsNullOrEmpty(storeid)) return null;

                try
                {
                    //.Include("Cartitems").Include("PackingLists").Include("Creator").Include("Store").Include("Orders").Include("Quotations")    
                    var _tx = (from tx in context.TaxConfigs
                               where (tx.StoreID == storeid)
                               select tx).FirstOrDefault();

                    if (_tx != null)
                    {
                        return _tx.TaxRate;
                    }
                    else
                    {
                        return null;
                    }

                }
                catch (Exception ex)
                {

                    eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                    return null;
                }
            }

            public decimal? getFixTaxRate(string storeid,string countrycode,string state)
            {
                if (String.IsNullOrEmpty(storeid)) return null;

                try
                {
                    //.Include("Cartitems").Include("PackingLists").Include("Creator").Include("Store").Include("Orders").Include("Quotations")    
                    var _tx = (from tx in context.TaxConfigs
                               where (tx.StoreID == storeid)
                               && (tx.CountryName == null || tx.CountryName ==""|| tx.CountryName == countrycode)
                               && (tx.State == null || tx.State=="" || tx.State == state)
                               select tx).FirstOrDefault();

                    if (_tx != null)
                    {
                        return _tx.TaxRate;
                    }
                    else
                    {
                        return null;
                    }

                }
                catch (Exception ex)
                {

                    eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                    return null;
                }
            }

            public POCOS.TaxConfig getFixTaxRateConfig(string storeid, string countrycode, string state)
            {
                if (String.IsNullOrEmpty(storeid)) return null;

                try
                {
                    //.Include("Cartitems").Include("PackingLists").Include("Creator").Include("Store").Include("Orders").Include("Quotations")    
                    POCOS.TaxConfig _tx = (from tx in context.TaxConfigs
                                           where (tx.StoreID == storeid)
                                           && (tx.CountryName == null || tx.CountryName == "" || tx.CountryName == countrycode)
                                           && (tx.State == null || tx.State == "" || tx.State == state)
                                           orderby tx.CountryName descending, tx.State descending
                                           select tx
                               ).FirstOrDefault();

                    return _tx;

                }
                catch (Exception ex)
                {

                    eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                    return null;
                }
            }
            private static string myclassname()
            {
                return typeof(TaxConfigHelper).ToString();
            }
        
        }
    }
 
 