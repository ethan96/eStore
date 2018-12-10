using eStore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.POCOS.DAL
{
    public class KitThemeMappingHelper : Helper
    {
        internal int save(KitThemeMapping kitThemeMapping)
        {
            if (kitThemeMapping == null || kitThemeMapping.validate() == false)
                return 1;
            try
            {
                var exitAddress = getKitThemeMappingById(kitThemeMapping.Id);
                if (exitAddress == null)
                {
                    context.KitThemeMappings.AddObject(kitThemeMapping);
                    context.SaveChanges();
                }
                else
                {
                    context = kitThemeMapping.helper.context;
                    context.KitThemeMappings.ApplyCurrentValues(kitThemeMapping);
                    context.SaveChanges();
                }
                foreach (var d in kitThemeMapping.KitThemeMappingDetails)
                    d.save();
                var tm = getKitThemeMappingById(kitThemeMapping.Id);
                foreach (var td in tm.KitThemeMappingDetails.ToList())
                {
                    if (kitThemeMapping.KitThemeMappingDetails.FirstOrDefault(c => c.Id == td.Id) == null)
                        td.delete();
                }

                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        public KitThemeMapping getKitThemeMappingById(int id)
        {
            return context.KitThemeMappings.FirstOrDefault(c => c.Id == id);
        }

        internal int delete(KitThemeMapping kitThemeMapping)
        {
            if (kitThemeMapping == null || kitThemeMapping.validate() == false) return 1;
            try
            {
                var _kitThemeMapping = getKitThemeMappingById(kitThemeMapping.Id);
                if (_kitThemeMapping != null)
                {
                    context.DeleteObject(_kitThemeMapping);
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
