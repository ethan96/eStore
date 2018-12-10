using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public class ZTSD_106DAL
    {
        private ZTSD_106Helper _helper = null;

        public ZTSD_106Helper helper
        {
            get { return _helper; }
            set { _helper = value; }
        }

        public int Truncate106A()
        {
            if (_helper == null)
                _helper = new ZTSD_106Helper();
            return _helper.Truncate106A();
        }

        public int Truncate106C()
        {
            if (_helper == null)
                _helper = new ZTSD_106Helper();
            return _helper.Truncate106C();
        }

        public int BulkInsert106A(System.Data.DataTable dt)
        {
            if (_helper == null)
                _helper = new ZTSD_106Helper();
            return _helper.BulkInsert106A(dt);
        }

        public int BulkInsert106C(System.Data.DataTable dt)
        {
            if (_helper == null)
                _helper = new ZTSD_106Helper();
            return _helper.BulkInsert106C(dt);
        }
    }
}
