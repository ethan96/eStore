using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

 namespace eStore.POCOS{ 

public partial class Menu{

    private MenuHelper _helper = null;

    public MenuHelper helper
    {
        get { return _helper; }
        set { _helper = value; }
    }

  public int save() 
        {
            if (_helper == null)
                _helper = new MenuHelper ();
            return _helper.save(this);
        }
  public int delete() 
        {
            if (_helper == null)
                _helper = new MenuHelper();
            return _helper.delete(this);
        }
	} 
 }