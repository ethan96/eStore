using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public partial class USPSPackageHelper : Helper
    {
        #region Business Read

        public USPSPackage getUSPSPackageById(int id)
        {
            var package = context.USPSPackages.FirstOrDefault(c => c.ID == id);
            if (package != null)
                package.helper = this;
            return package;
        }

        public List<USPSPackage> getUSPSPackages()
        {
            return context.USPSPackages.ToList();
        }
        #endregion

        #region Creat Update Delete

        #endregion

        #region Others

        private static string myclassname()
        {
            return typeof(USPSPackageHelper).ToString();
        }

        #endregion

        public int save(USPSPackage uspsPackage)
        {
            if (uspsPackage == null || uspsPackage.validate() == false)
                return 1;
            try
            {
                var exitAddress = getUSPSPackageById(uspsPackage.ID);
                if (exitAddress == null)
                {
                    context.USPSPackages.AddObject(uspsPackage);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    context = uspsPackage.helper.context;
                    context.USPSPackages.ApplyCurrentValues(uspsPackage);
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

        internal int delete(USPSPackage uspsPackage)
        {
            throw new NotImplementedException();
        }
    }
}
