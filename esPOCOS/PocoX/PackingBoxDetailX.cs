using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using eStore.POCOS.PocoX;


namespace eStore.POCOS
{
    public partial class PackingBoxDetail
    {
        #region Extension Properties
        private Dictionary<Part, int> _btosPart;
        public Dictionary<Part, int> BtosPart
        {
            get { return _btosPart; }
            set { _btosPart = value; }
        }
        #endregion
    }
}
