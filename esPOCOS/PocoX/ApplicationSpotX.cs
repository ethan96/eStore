using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using eStore.POCOS.PocoX;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class ApplicationSpot
    {
        //标记 对应的 sub application catggory
        private string _pointScenario;
        public string PointScenario
        {
            get {
                if (string.IsNullOrEmpty(_pointScenario))
                {
                    if (this.ScenarioSpotMappings != null)
                        _pointScenario = string.Join(",", this.ScenarioSpotMappings.Select(c => c.ScenarioID));
                }
                return _pointScenario;
            }
            set { _pointScenario = value; }
        }
        //删除point 和 下面的 scenaio 关系
        public int deleteAll()
        {
            if (this.ScenarioSpotMappings != null && this.ScenarioSpotMappings.Count > 0)
            {
                foreach (POCOS.ScenarioSpotMapping ssmItem in ScenarioSpotMappings)
                {
                    if (ssmItem.delete(ssmItem.ScenarioSpotID) != 0)
                        return -1;
                }
            }
            return this.delete(ApplicationSpotID);
        }
    }
}
