
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
    public partial class CTOSAttribute
    {
        public CTOSComparisionHelper helper;

        public int save()
        {
            if (helper == null)
                helper = new CTOSComparisionHelper();

            return helper.save(this);
        }
        public int delete()
        {
            if (helper == null)
                helper = new CTOSComparisionHelper();

            return helper.delete(this);
        }
    }
}
