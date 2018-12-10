using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS.DAL
{
    public class spGetSpecByCategoryHelper : Helper
    {
        public List<int> getIdList(string tableName,string node,string keyName,int id,int level)
        {
            return context.sp_GetTreeByNode(tableName, node, keyName, id, level).Select(c => c.id).ToList();
        }
    }
}
