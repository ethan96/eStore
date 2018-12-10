using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.POCOS.DAL
{
    public class HierarchyTreeHelper : Helper
    {
        public List<getHierarchyTree_Result> GetHierarchyTreeByCategoryID(int ID)
        {
            try
            {
                return context.getHierarchyTree(ID).ToList();
            }
            catch
            {
                return new List<getHierarchyTree_Result>();
            }

        }
    }
}
