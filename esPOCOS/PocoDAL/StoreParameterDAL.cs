using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

 namespace eStore.POCOS{ 

public partial class StoreParameter{ 
  public int save() 
        { 
 		    return StoreParameterHelper.save(this);
        }
  public int delete() 
        { 
 		    return StoreParameterHelper.delete(this);
        }
	} 
 }