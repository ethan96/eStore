using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

 namespace eStore.POCOS{ 

public partial class Store
    {
        private int save()
        {
            if (this.helper == null)
                helper = new StoreHelper();
            return helper.save(this);
        }

        public int delete()
        {
            if (helper == null)
                helper = new StoreHelper();
            return helper.delete(this);
        }
	} 
 }