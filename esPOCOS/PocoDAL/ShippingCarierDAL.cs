using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

 namespace eStore.POCOS{ 

public partial class ShippingCarier{ 
  public int save() 
        { 
 		    return ShippingCarierHelper.save(this);
        }
  public int delete() 
        { 
 		    return ShippingCarierHelper.delete(this);
        }
	} 
 }