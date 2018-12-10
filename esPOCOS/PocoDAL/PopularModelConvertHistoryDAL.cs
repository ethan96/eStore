using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS{ 

public partial class PopularModelConvertHistory
{
    private PopularModelConvertHistoryHelper _helper = null;

    public PopularModelConvertHistoryHelper helper
    {
        get { return _helper; }
        set { _helper = value; }
    }

    public int save()
    {
        if (_helper == null)
            _helper = new PopularModelConvertHistoryHelper();
        return _helper.save(this);
    }
}
}