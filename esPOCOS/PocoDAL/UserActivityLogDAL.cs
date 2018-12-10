using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using eStore.POCOS.PocoX;
using eStore.POCOS.DAL;


namespace eStore.POCOS
{
    public partial class UserActivityLog
    {

        private UserActivityHelper _helper = null;

        public UserActivityHelper helper
    {
        get { return _helper; }
        set { _helper = value; }
    }

    public int save()
    {
        if (_helper == null)
            _helper = new UserActivityHelper();
        return _helper.save(this);
    }

    public int delete()
    {
        if (_helper == null)
            _helper = new UserActivityHelper();
        return _helper.delete(this);
    }
	} 
 }