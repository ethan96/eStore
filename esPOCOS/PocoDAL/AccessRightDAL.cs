using eStore.POCOS.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.POCOS
{
    
    public class AccessRight
    {

        

        private AccessRightHelper _helper;

        public AccessRightHelper helper
        {
            get { return _helper; }
            set { _helper = value; }
        }

        public int bulkupdate(AdminRole role, List<AdminAuth> selectedAuthList)
        {
            if(_helper == null)
                _helper = new AccessRightHelper();
            return _helper.bulkupdate(role, selectedAuthList);
        }

        //private static void AssociateAndSave(int oId, int cId)
        //{
        //    using (var context = new ModelEntities())
        //    {
        //        var owner = (from o in context.Owners
        //                     select o).FirstOrDefault(o => o.ID == oId);
        //        var child = (from o in context.Children
        //                     select o).FirstOrDefault(c => c.ID == cId);

        //        owner.Children.Add(child);
        //        context.Attach(owner);
        //        context.SaveChanges();
        //    }
        //}
    }
}
