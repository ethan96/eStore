using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

 namespace eStore.POCOS{ 

public partial class CTOSComponent{

    public CTOSComponentHelper helper;

  public int save() 
        {
            if (helper == null)
                helper = new CTOSComponentHelper();

            return helper.save(this);
        }
  public int delete() 
        {
            if (helper == null)
                helper = new CTOSComponentHelper();

            return helper.delete(this);
        }
	} 
 }