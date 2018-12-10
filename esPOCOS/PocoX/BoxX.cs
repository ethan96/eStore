using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using eStore.POCOS.PocoX;

namespace eStore.POCOS
{
    public partial class Box
    {
        //properties

        //Max numbers of items in the box
        public virtual int Capacity
        {
            get;
            set;
        }

    }
}
