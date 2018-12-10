using eStore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.POCOS.DAL
{
    public class LocationIpHelper : Helper
    {
        public List<LocationIp> GetActiveIps(bool include = false)
        {

            var ls = (from c in context.LocationIps
                      where c.EndDate >= DateTime.Now
                        && c.StartDate <= DateTime.Now
                        && (include ? true : c.Publish == true)
                      select c).ToList();
            return ls;
        }

        internal int save(LocationIp locationIp)
        {
            if (locationIp == null || locationIp.validate() == false)
                return 1;
            try
            {
                var exitLocationIp = getLocationIpById(locationIp.Id);
                if (exitLocationIp == null)
                {
                    context.LocationIps.AddObject(locationIp);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    context = locationIp.helper.context;
                    context.LocationIps.ApplyCurrentValues(locationIp);
                    context.SaveChanges();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        private object getLocationIpById(int id)
        {
            return context.LocationIps.FirstOrDefault(c => c.Id.Equals(id));
        }

        internal int delete(LocationIp locationIp)
        {
            if (locationIp == null || locationIp.validate() == false) return 1;
            try
            {
                var exIp = getLocationIpById(locationIp.Id);
                if (exIp != null)
                {
                    context.DeleteObject(exIp);
                    context.SaveChanges();
                    return 0;
                }
                return -5000;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }
    }
}
