using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

 namespace eStore.POCOS{ 

public partial class CartContact {
      private CartContactHelper _helper = null;

      public CartContactHelper helper
      {
          get { return _helper; }
          set { _helper = value; }
      }

      public int save()
      {
          if (_helper == null)
              _helper = new CartContactHelper();
          return _helper.save(this);
      }

     
   } 
	 
 }