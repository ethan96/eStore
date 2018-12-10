using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.POCOS.Sync
{
    public class VPisInfo
    {
        IPisSync p;
        public List<string> _bbPartNo;
        public VPisInfo(Part _part, PISEntities _pisentity)
        {
            this._bbPartNo = null;
            if (_part.StoreID == "ABB" && isBBPartNo(_part.SProductID, _pisentity) == true)
                p = new BBSync();
            else if (_part.isPTradePart())
                p = new PAPS();
            else
                p = new AdvantechSync();
            p.PISEntity = _pisentity;
        }
        
        public List<spGetModelByPN_estore_Result> getModelByPartNo(string partNo)
        {
            if (p != null)
                return p.getModelByPartNo(partNo);
            else
                return new List<spGetModelByPN_estore_Result>();
        }

        public string getCategoryID(string productName)
        {
            if (p != null)
                return p.getCategoryID(productName);
            else
                return string.Empty;
        }

        private bool isBBPartNo(string pn, PISEntities _pisentity)
        {
            if (_bbPartNo == null)
            {
                _bbPartNo = (from x in _pisentity.sp_GetBBAllPartNo()
                             select x.part_no.ToUpper().Trim()).ToList();
            }
            return _bbPartNo.Contains(pn.ToUpper());
        }
    }
}
