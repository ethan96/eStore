using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

 namespace eStore.POCOS{ 

public partial class Currency { 
 
	 #region Extension Methods 

    public Currency()
    { }

    public Currency(string currencyId)
    {
        if (!string.IsNullOrEmpty(currencyId))
        { 
            var temp = CurrencyHelper.getCurrencybyID(currencyId);
            this.CurrencyID = temp.CurrencyID;
            this.CurrencySign = temp.CurrencySign;
            this.ToUSDRate = temp.ToUSDRate;
            this.ID = temp.ID;
        }
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        if (!(obj is Currency))
            return false;
        var cc = obj as Currency;
        return cc.ID == this.ID;
    }

 #endregion 
	} 
 }