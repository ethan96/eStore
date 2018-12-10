using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

 namespace eStore.POCOS{ 

public partial class Order{
      private OrderHelper _helper = null;

      public OrderHelper helper
      {
          get { return _helper; }
          set { _helper = value; }
      }

      public int save() 
        {
            if (_helper == null)
                _helper = new OrderHelper();
 		  return _helper.save(this);
        }
  
        public int delete() 
        {
            if (_helper == null)
                _helper = new OrderHelper();
            return _helper.delete(this);
        }

        public int savedirectly()
        {
            if (_helper == null)
                _helper = new OrderHelper();
            return _helper.saveDirectly(this);
        }
	} 
	 
 }