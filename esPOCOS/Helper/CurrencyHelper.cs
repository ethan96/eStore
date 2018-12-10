using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
 namespace eStore.POCOS.DAL{ 

public partial class CurrencyHelper : Helper {

    #region Business Read

    public static Currency getCurrencybyID(string currency) {

        if (string.IsNullOrEmpty(currency)) return null;

        try
        {
            using (eStore3Entities6 context = new eStore3Entities6())
            {

                var _cur = (from cur in context.Currencies
                                where cur.CurrencyID == currency
                                select cur).FirstOrDefault();

                return _cur;
            }

        }
        catch (Exception ex)
        {

            eStoreLoger.Fatal(ex.Message, "", "", "", ex);

            return null;

        }

    
    }

    public List<Currency> getAllCurrency()
    {
        var _currency = (from c in context.Currencies
                    select c);

        foreach (Currency c in _currency)
        {
            c.helper = this;
        }

        if (_currency != null)
            return _currency.ToList();
        else
            return new List<Currency>();        
    }

    #endregion 

    #region Creat Update Delete 
    public int save(Currency _currency)
    {
        //if parameter is null or validation is false, then return  -1 
        if (_currency == null || _currency.validate() == false) return 1;
        //Try to retrieve object from DB
        Currency _exist_currency = getCurrencybyID(_currency.CurrencyID);
        try
        {
            if (_exist_currency == null)  //object not exist 
            {
                //Insert
                context.Currencies.AddObject(_currency);
                context.SaveChanges();
                return 0;
            }
            else
            {
                //Update
                context.Currencies.ApplyCurrentValues(_currency);
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

    public int delete(Currency _currency)
    {

        if (_currency == null || _currency.validate() == false) return 1;
        try
        {
            context.DeleteObject(_currency);
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
 
    #region Others
    private static string myclassname()
    {
        return typeof(CurrencyHelper).ToString();
    } 
    #endregion 
	} 
 }