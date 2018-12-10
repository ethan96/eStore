using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

 namespace eStore.POCOS{ 

public partial class CTOSComponentDetail{

    public CTOSComponentDetailHelper helper;

  public int save() 
        {
            if (helper == null)
                helper = new CTOSComponentDetailHelper();

            return helper.save(this);
        }
  public int delete() 
        {
            if (helper == null)
                helper = new CTOSComponentDetailHelper();

            return helper.delete(this);
        }
	} 
 }